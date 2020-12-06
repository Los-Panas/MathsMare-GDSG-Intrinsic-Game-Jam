using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PercentageBalls : MonoBehaviour
{
    enum BallsState
    {
        Idle,
        Free
    }

    BallsState state = BallsState.Idle;
    Vector2 direction;
    float speed = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (state == BallsState.Free)
        {
            Vector2 pos = transform.position;
            pos.x += direction.x * speed * Time.deltaTime;
            pos.y += direction.y * speed * Time.deltaTime;
            transform.position = pos;

            transform.rotation *= Quaternion.Euler(0, 0, Time.time);

            if (Camera.main.WorldToScreenPoint(transform.position).x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetBallFree(Vector2 direction, float speed)
    {
        state = BallsState.Free;
        this.direction = direction.normalized;
        this.speed = -speed;
    }
}
