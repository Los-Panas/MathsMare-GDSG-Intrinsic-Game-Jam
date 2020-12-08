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

    // Inspector -------------------
    [Header("Player Variables")]
    [SerializeField]
    float speed = 5.0f;
    [SerializeField]
    float sinus_speed = 2.0f;
    [SerializeField]
    bool is_invulnerable = false;
    [SerializeField]
    float time_to_hold_special = 0.25f;

    [Header("Audio")]
    [SerializeField]
    AudioClip onGradeUp;
    [SerializeField]
    AudioClip onDie;

    [Header("Invulnerability Animation")]
    [SerializeField]
    int invulnerable_cycles = 3;
    [SerializeField]
    float invulnerable_time = 2;
    [SerializeField]
    Color invulnerable_color;


    [Header("GameObjects / Components")]
    [SerializeField]
    GameObject DeathExpansionWave;
    [SerializeField]
    SpriteRenderer brain_sprite;
    [SerializeField]
    SpriteRenderer gradeSprite;
    [SerializeField]
    GameObject Camera;
    [SerializeField]
    GameObject deadPlayer;
    [SerializeField]
    GameObject EnergyCharged;

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
    [SerializeField]
    float max_scale_to_add = 0.25f;
    [SerializeField]
    GameObject plusGrade;
    [SerializeField]
    List<GameObject> pluses = new List<GameObject>();

    [Header("Controls")]
    public KeyCode MoveUp = KeyCode.W;
    public KeyCode MoveDown = KeyCode.S;
    // -----------------------------

    // Internal Variables ----------
    Rigidbody2D rb;
    float special_time_hold = 0.0f;
    float manual_time = 0;
    bool special_charged = false;
    Coroutine grade_perlin_noise_coroutine;
    AudioSource audioSource;
    Coroutine grade_growing_coroutine = null;

    // Stats
    [HideInInspector]
    public int enemies_avoided = 0;
    [HideInInspector]
    public int enemies_erased = 0;
    [HideInInspector]
    public int hits_received = 0;
    // --------------
    
    public struct BarInfo
    {
        public BarInfo(float value, Action action)
        {
            this.value = value;
            this.action = action;
        }
        public enum Action
        {
            Decrease,
            Increase,
            None
        }
        public float value;
        public Action action;
    }
    Queue<BarInfo> barActions = new Queue<BarInfo>();
    bool callNextAction = false;
    // -----------------------------

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        audioSource = GetComponent<AudioSource>();

        special_charged = false;
        gradeSprite.sprite = spriteGrades[(int)grade];
        gradeBar.material.SetFloat("_Fill", 0);

        EnergyCharged.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (callNextAction && barActions.Count > 0)
        {
            callNextAction = false;
            StartCoroutine(MoveBar(barActions.Peek()));
        }

        manual_time += Time.deltaTime * sinus_speed;
    }

    private void FixedUpdate()
    {
        HandleInput();
    }

    void HandleInput()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(MoveUp) && Input.GetKey(MoveDown))
        {
            special_time_hold += Time.deltaTime;

            if (special_time_hold >= time_to_hold_special)
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

                special_time_hold = 0;
            }
        }
        else if (!Input.GetKey(MoveUp) && !Input.GetKey(MoveDown))
        {
            movement.y += Mathf.Sin(manual_time) * 0.35f;
        }
        else
        {
            special_time_hold = 0;

            if (Input.GetKey(MoveUp))
            {
                movement.y += speed;
            }

            if (Input.GetKey(MoveDown))
            {
                movement.y -= speed;
            }

        }

        rb.velocity = movement;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_invulnerable)
        {
            if (collision.tag == "Enemy")
            {
                ++hits_received;
                StartCoroutine(InvulnerableAnim());
                DecreaseGrade();
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
        GameObject.Find("AudioSourceDead").GetComponent<AudioSource>().PlayOneShot(onDie);
        gradeBar.material.SetFloat("_Fill", 0);
        StopAllCoroutines();
        gameObject.SetActive(false);
        Instantiate(deadPlayer, transform.position, Quaternion.identity);
    }

    void AddGrade()
    {
        if (grade == Grade.F)
        {
            return;
        }

        special_charged = true;
        audioSource.PlayOneShot(onGradeUp, 0.15F);

        if (!EnergyCharged.activeSelf)
        {
            EnergyCharged.SetActive(true);
        }

        if (grade_growing_coroutine == null)
        {
            grade_growing_coroutine = StartCoroutine(GradeGrowing(1));
        }

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
            Debug.Log("ULTRA");
            // TODO: add pluses
            pluses.Add(GameObject.Instantiate(plusGrade));
            Vector2 newPos = Vector2.zero;
            newPos.x += 0.16f;
            if (pluses.Count-1  < 1)
                pluses[pluses.Count-1].transform.parent = gradeSprite.gameObject.transform;
            else
                pluses[pluses.Count-1].transform.parent = pluses[pluses.Count - 2].transform;                  

            pluses[pluses.Count - 1].transform.localPosition = newPos;
            pluses[pluses.Count - 1].transform.localRotation = Quaternion.identity;
        }
        OnResetBar();
    }

    IEnumerator GradeGrowing(int direction)
    {
        Vector3 start_scale = gradeSprite.transform.localScale;
        Vector3 scale_to_grow = gradeSprite.transform.localScale * (1 + direction * max_scale_to_add);
        float time_start = Time.time;
        float t = 0;

        while (t <= 1) 
        {
            t = (Time.time - time_start) / 0.25f;

            if (t < 0.5f)
            {
                gradeSprite.transform.localScale = Vector3.Lerp(start_scale, scale_to_grow, t * 2);
            }
            else if (t < 1)
            {
                gradeSprite.transform.localScale = Vector3.Lerp(scale_to_grow, start_scale, (t - 0.5f) * 2);
            }
            else
            {
                gradeSprite.transform.localScale = start_scale;
            }

            yield return null;
        }

        grade_growing_coroutine = null;
    }

    void DecreaseGrade()
    {
        if (grade == Grade.F)
        {
            return;
        }


        if (grade_growing_coroutine == null)
        {
            grade_growing_coroutine = StartCoroutine(GradeGrowing(-1));
        }

        OnResetBar();

        if (grade == Grade.ULTRA_A)
        {
            // TODO: Decrease pluses

            //foreach(GameObject plus in pluses)
            //{
            //    Debug.Log("DESTROY");
            //    Destroy(plus);
            //}
            Destroy(gradeSprite.transform.GetChild(0).gameObject);
            grade -= 2;
            gradeSprite.sprite = spriteGrades[(int)grade];
            pluses.Clear();
        }
        else
        {
            --grade;
            gradeSprite.sprite = spriteGrades[(int)grade];
            if (grade == Grade.F)
            {
                OnPlayerDead();
            }
        }
    }

    public void OnUpdateBarUp()
    {
        if (grade == Grade.F)
        {
            return;
        }

        ++enemyCount;
        ++enemies_avoided;

        int target = enemyCoutToUpgrade[(int)grade - 1];
        barActions.Enqueue(new BarInfo((float)((float)enemyCount / (float)target), BarInfo.Action.Increase));
        if (barActions.Count == 1)
        {
            callNextAction = true;
        }
    }

    void OnResetBar()
    {
        barActions.Enqueue(new BarInfo(0, BarInfo.Action.None));
        if (barActions.Count == 1)
        {
            callNextAction = true;
        }
        enemyCount = 0;

        if (grade == Grade.F)
        {
            return;
        }
    }

    IEnumerator MoveBar(BarInfo info)
    {
        if (grade != Grade.F)
        {
            float current = gradeBar.material.GetFloat("_Fill");
            float init = current;
            float time = Time.time;
            while (true)
            {
                float t = (Time.time - time) / 0.25f;
                current = Mathf.Lerp(init, info.value, t);
                gradeBar.material.SetFloat("_Fill", current);

                if ((current >= info.value && info.value != 0) || (info.value == 0.0f && current == 0.0F))
                {
                    break;
                }
                else
                {
                    yield return null;
                }
            }

            if (info.value >= 1.0F && info.action == BarInfo.Action.Increase)
            {
                AddGrade();
            }
            else if (info.value == 0.0F && info.action == BarInfo.Action.Decrease)
            {
                DecreaseGrade();
            }
        }

        barActions.Dequeue();
        if (barActions.Count > 0)
        {
            callNextAction = true;
        }
    }

    public void UseSpecial()
    {
        special_charged = false;
        StartCoroutine(PerlinNoiseShake(Camera.gameObject, 10));
        Vector2 pos = transform.position;
        pos.x += 1;
        Instantiate(DeathExpansionWave, pos, Quaternion.identity);
        EnergyCharged.SetActive(false);
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
