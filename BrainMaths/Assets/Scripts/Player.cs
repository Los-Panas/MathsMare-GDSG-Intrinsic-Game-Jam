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
    // -----------------------------

    // Internal Variables ----------
    GravityState g_state = GravityState.Normal;
    float current_velocity = 0.0f;
    int current_gravity_direction = -1;
    Vector3 pos;
    int frames_button_hold = 0;
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
        manual_time += Time.deltaTime;
        HandleInput();

        if (g_state == GravityState.Normal)
        {
            // Gravity -> I use deltaTime 2 times because both acceleration and velocity need to be converted to per frame instead of per second.
            current_velocity += acceleration * current_gravity_direction;  // v = v0 + a*t 
            current_velocity = Mathf.Clamp(current_velocity, -max_velocity, max_velocity);
            pos.y += current_velocity * Time.deltaTime; // x = x0 + v*t
            transform.position = pos;
        }
        else
        {
            if (current_velocity != 0)
            {
                current_velocity -= acceleration * 0.5f * current_gravity_direction;
                pos.y += current_velocity * Time.deltaTime;

                if (current_velocity < 0.1f && current_velocity > -0.1f)
                {
                    current_velocity = 0;
                }
            }

            pos.y += Mathf.Sin(manual_time * 3) * Time.deltaTime * 0.5f;
            transform.position = pos;
        }

        Analysis();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (frames_button_hold > 5 && g_state != GravityState.Floating)
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

            ++frames_button_hold;
        } 

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ChangeGravityDirection();

            frames_button_hold = 0;
        }
    }

    void ChangeGravityDirection()
    {
        g_state = GravityState.Normal;
        current_gravity_direction = -current_gravity_direction;
    }

    void Analysis()
    {
        Debug.Log("Analysis:\n" +
            "Current Velocity: " + current_velocity +
            "\n---------------------"
            );
    }
}
