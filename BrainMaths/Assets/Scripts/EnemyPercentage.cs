﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPercentage : EnemyBase
{
    enum PercentageState
    {
        FirstHalf,
        Shooting,
        Ending
    }

    PercentageState current_state = PercentageState.FirstHalf;

    // Inspector
    [Header("Values")]
    [SerializeField]
    float mid_point = 0;
    [SerializeField]
    float balls_vertical_target = 5.0f;

    [Header("Balls")]
    [SerializeField]
    GameObject[] balls;

    // Variables 
    Animator anim;
    bool anim_started = false;

    public override void Initialize()
    {
        anim = GetComponent<Animator>();
    }

    public override void Move()
    {
        switch (current_state)
        {
            case PercentageState.FirstHalf:
                base.Move();

                if (transform.position.x <= mid_point)
                {
                    current_state = PercentageState.Shooting;
                }
                break;
            case PercentageState.Shooting:
                if(!anim_started)
                {
                    anim.SetTrigger("Spin");
                    anim_started = true;
                }
                break;
            case PercentageState.Ending:
                base.Move();
                break;
        }
    }

    public void LaunchBalls()
    {
        for (int i = 0; i < balls.Length; ++i)
        {
            balls[i].transform.SetParent(transform.parent);
            Vector2 pos = balls[i].transform.position;
            balls[i].GetComponent<PercentageBalls>().SetBallFree(new Vector2(-9, balls_vertical_target - balls_vertical_target * 2 * i) - pos, mVelocity.x);
        }
    }

    public void EndSpinAnim()
    {
        current_state = PercentageState.Ending;
        anim_started = false;
    }
}