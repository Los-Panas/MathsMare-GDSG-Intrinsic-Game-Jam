using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    static public Player instance;

    enum Grade
    {
        F = 0,
        D_MINUS = 1,
        D = 2,
        D_PLUS = 3,
        C_MINUS = 4,
        C = 5,
        C_PLUS = 6,
        B_MINUS = 7,
        B = 8,
        B_PLUS = 9,
        A_MINUS = 10,
        A = 11,
        A_PLUS = 12,
        ULTRA_A = 13
    }

    enum GravityState
    {
        Normal,
        Floating
    }

    // Inspector -------------------
    [Header("Player Variables")]
    [SerializeField]
    float acceleration = 0.5f;
    [SerializeField]
    float max_velocity = 5.0f;
    [SerializeField]
    float sinus_speed = 2.0f;
    [SerializeField]
    int invulnerable_cycles = 3;
    [SerializeField]
    float invulnerable_time = 2;
    [SerializeField]
    Color invulnerable_color;
    [SerializeField]
    bool is_invulnerable = false;

    [Header("GameObjects / Components")]
    [SerializeField]
    GameObject DeathExpansionWave;
    [SerializeField]
    SpriteRenderer brain_sprite;
    [SerializeField]
    SpriteRenderer gradeSprite;
    [SerializeField]
    GameObject Camera;

    [Header("Grade Variables")]
    [SerializeField]
    Grade grade = Grade.C_MINUS;
    [SerializeField]
    Sprite[] spriteGrades;
    [SerializeField]
    float grade_perlin_noise_factor = 5.0f;
    [SerializeField]
    MeshRenderer gradeBar;
    int enemyCount = 0;
    [SerializeField]
    int[] enemyCoutToUpgrade;
    Coroutine barCoroutine = null;
    float auxCountBar = 0;
    // -----------------------------

    // Internal Variables ----------
    GravityState g_state = GravityState.Normal;
    float current_velocity = 0.0f;
    int current_gravity_direction = -1;
    Vector3 pos;
    float time_hold = 0.0f;
    float manual_time = 0;
    bool special_charged = false;
    Coroutine grade_perlin_noise_coroutine;
    // -----------------------------


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        pos = transform.position;
        AddGrade();
        special_charged = false;
        OnResetBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (barCoroutine == null && auxCountBar != 0.0F)
        {
            barCoroutine = StartCoroutine(BarUp(auxCountBar));
            auxCountBar = 0.0F;
        }

        HandleInput();

        if (g_state == GravityState.Normal)
        {
            // Gravity -> I use deltaTime 2 times because both acceleration and velocity need to be converted to per frame instead of per second.
            current_velocity += acceleration * current_gravity_direction;  // v = v0 + a*t 
            current_velocity = Mathf.Clamp(current_velocity, -max_velocity, max_velocity);
            pos.y += current_velocity * Time.deltaTime; // x = x0 + v*t
            transform.position = pos;
            // ------------------------------------------------------------------------------
        }
        else
        {
            // Sinus Movement while Holding
            manual_time += Time.deltaTime * sinus_speed;

            if (current_velocity != 0)
            {
                current_velocity -= acceleration * 0.5f * current_gravity_direction;
                pos.y += current_velocity * Time.deltaTime;

                if (current_velocity < 0.1f && current_velocity > -0.1f)
                {
                    current_velocity = 0;
                }
            }

            pos.y += Mathf.Sin(manual_time) * Time.deltaTime * 0.35f;
            transform.position = pos;
            // ------------------------------------------------------------------------------
        }

        //Analysis();
    }

    void HandleInput()
    {
        // TODO: Remove
        // Debug --------------------------------
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(InvulnerableAnim());
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            special_charged = true;
        }
        // ---------------------------------------

        if (Input.GetKey(KeyCode.LeftControl))
        {
            time_hold += Time.deltaTime;

            if (time_hold >= 0.15f && g_state != GravityState.Floating)
            {
                g_state = GravityState.Floating;

                if (current_gravity_direction < 0) 
                {
                    manual_time = Mathf.PI;
                }
                else
                {
                    manual_time = 0;
                }
            }
        } 

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ChangeGravityDirection();

            time_hold = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            if (special_charged)
            {
                UseSpecial();
            }
            else
            {
                if (grade_perlin_noise_coroutine == null)
                {
                    grade_perlin_noise_coroutine = StartCoroutine(NoSpecialChargedFeedback());
                }
            }
        }
    }

    void ChangeGravityDirection()
    {
        if (g_state == GravityState.Floating)
        {
            if (Mathf.Sin(manual_time) > 0)
            {
                current_gravity_direction = 1;
            }
            else
            {
                current_gravity_direction = -1;
            }

            g_state = GravityState.Normal;
        }
        else
        {
            current_gravity_direction = -current_gravity_direction;
        }
    }

    void Analysis()
    {
        Debug.Log("Analysis:\n" +
            "Current Velocity: " + current_velocity +
            "\n---------------------"
            );
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_invulnerable)
        {
            if (collision.tag == "Enemy")
            {
                DecreaseGrade();
                StartCoroutine(InvulnerableAnim());
            }
        }
    }

    IEnumerator InvulnerableAnim()
    {
        is_invulnerable = true;
        int current_cycle = 0;
        float time_start = Time.time;
        float time_per_cycle = invulnerable_time / (float)invulnerable_cycles;
        Color current_color = brain_sprite.color;

        while (current_cycle < invulnerable_cycles)
        {
            float t = (Time.time - time_start) / time_per_cycle;

            if (t < 0.5f)
            {
                brain_sprite.color = Color.Lerp(current_color, invulnerable_color, t * 2);
            }
            else if (t < 1)
            {
                brain_sprite.color = Color.Lerp(invulnerable_color, current_color, (t - 0.5f) * 2);
            }
            else
            {
                brain_sprite.color = current_color;
                ++current_cycle;
                time_start = Time.time;
            }

            yield return null;
        }

        is_invulnerable = false;
    }

    void OnPlayerDead()
    {
        // TODO: 
    }

    void AddGrade()
    {
        special_charged = true;

        if (grade != Grade.ULTRA_A)
        {
            ++grade;
            if (grade != Grade.ULTRA_A)
            { 
                gradeSprite.sprite = spriteGrades[(int)grade];
            }
        }

        if (grade == Grade.ULTRA_A)
        {
            // TODO: add pluses
        }
        OnResetBar();
    }

    void DecreaseGrade()
    {
        --grade;

        OnResetBar();

        if (grade == Grade.ULTRA_A)
        {
            // TODO: Decrease pluses
        }
        else
        {
            gradeSprite.sprite = spriteGrades[(int)grade];
            if (grade == Grade.F)
            {
                OnPlayerDead();
            }
        }
    }

    public void OnUpdateBarUp()
    {
        ++enemyCount;

        int target = enemyCoutToUpgrade[(int)grade - 1];
        if (barCoroutine != null)
        {
            auxCountBar = (float)((float)enemyCount / (float)target);
        }
        else
        {
            barCoroutine = StartCoroutine(BarUp((float)((float)enemyCount / (float)target)));
            auxCountBar = 0.0F;
        }
    }

    void OnResetBar()
    {
        gradeBar.material.SetFloat("_Fill", 0);
        enemyCount = 0;
    }

    IEnumerator BarUp(float value)
    {
        float current = gradeBar.material.GetFloat("_Fill");
        float init = current;
        float time = Time.time;
        while (true)
        {
            float t = (Time.time - time) / 1;
            current = Mathf.Lerp(init, value, t);
            gradeBar.material.SetFloat("_Fill", current);
            
            if (current >= value)
            {
                break;
            }
            else
            {
                yield return null;
            }
        }

        if (value >= 1)
        {
            // TODO: particulita guay
            AddGrade();
        }

        barCoroutine = null;
    }

    void UseSpecial()
    {
        special_charged = false;
        StartCoroutine(PerlinNoiseShake(Camera.gameObject, 10));
        Vector2 pos = transform.position;
        pos.x += 1;
        Instantiate(DeathExpansionWave, pos, Quaternion.identity);
    }

    IEnumerator NoSpecialChargedFeedback()
    {
        Vector3 original_pos = gradeSprite.transform.position;
        Vector3 pos = gradeSprite.transform.position;
        float time_start = Time.time;

        while (Time.time - time_start < 0.25f)
        {
            pos.x += (Mathf.PerlinNoise(0, Time.time * 10) - 0.5f) * grade_perlin_noise_factor * Time.deltaTime;
            pos.y += (Mathf.PerlinNoise(Time.time * 10, 0) - 0.5f) * grade_perlin_noise_factor * Time.deltaTime;
            gradeSprite.transform.position = pos;

            yield return null;
        }

        gradeSprite.transform.position = original_pos;
        grade_perlin_noise_coroutine = null;
    }

    IEnumerator PerlinNoiseShake(GameObject gameObject, float perlin_noise_factor)
    {
        Vector3 original_pos = gameObject.transform.position;
        Vector3 pos = gameObject.transform.position;
        float time_start = Time.time;

        while (Time.time - time_start < 0.25f)
        {
            pos.x += (Mathf.PerlinNoise(0, Time.time * 10) - 0.5f) * perlin_noise_factor * Time.deltaTime;
            pos.y += (Mathf.PerlinNoise(Time.time * 10, 0) - 0.5f) * perlin_noise_factor * Time.deltaTime;
            gameObject.transform.position = pos;

            yield return null;
        }

        gameObject.transform.position = original_pos;
    }
}
