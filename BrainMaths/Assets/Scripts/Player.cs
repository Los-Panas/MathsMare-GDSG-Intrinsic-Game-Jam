using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    enum GravityState
    {
        Normal,
        Floating
    }

    // Inspector -------------------
    [Header("Player Variables")]
    [SerializeField]
    int lives = 1;
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

    [Header("GameObjects/Components")]
    [SerializeField]
    SpriteRenderer brain_sprite;
    // -----------------------------

    // Internal Variables ----------
    GravityState g_state = GravityState.Normal;
    float current_velocity = 0.0f;
    int current_gravity_direction = -1;
    Vector3 pos;
    float time_hold = 0.0f;
    float manual_time = 0;
    // -----------------------------


    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(InvulnerableAnim());
        }

        if (Input.GetKey(KeyCode.D))
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

        if (Input.GetKeyUp(KeyCode.D))
        {
            ChangeGravityDirection();

            time_hold = 0;
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
                // Substract one mark
                // mark -= 1;

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
}
