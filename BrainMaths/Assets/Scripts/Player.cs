using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Inspector -------------------
    [Header("Player Variables")]
    [SerializeField]
    int lives = 1;
    [SerializeField]
    float acceleration = 0.5f;
    [SerializeField]
    float max_velocity = 3.0f;
    // -----------------------------

    // Internal Variables ----------
    float current_velocity = 0.0f;
    int current_gravity_direction = -1;
    Vector3 pos;
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

        // Gravity -> I use deltaTime 2 times because both acceleration and velocity need to be converted to per second instead of per frame.
        current_velocity += acceleration * current_gravity_direction * Time.deltaTime;  // v = v0 + a*t 
        current_velocity = Mathf.Clamp(current_velocity, -max_velocity, max_velocity); 
        pos.y += current_velocity * Time.deltaTime; // x = x0 + v*t
        transform.position = pos;

        //Analysis();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeGravityDirection();
        }
    }

    void ChangeGravityDirection()
    {
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
