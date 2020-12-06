using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquareRoot : EnemyBase
{
    enum EnemyState
    {
        FirstAdvance,
        RandomPos,
        Shooting,
        Ending
    }

    EnemyState state = EnemyState.FirstAdvance;

    // Inspector
    [Header("SQRT Inspector")]
    [SerializeField]
    float trigger_point = 5.5f;
    [SerializeField]
    float max_random_bound = 4.5f;
    [SerializeField]
    float time_to_pos = 1.0f;

    // Variables
    float target_pos_point = 0.0f;
    Animator anim;

    public override void Initialize()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public override void Move()
    {
        switch (state)
        {
            case EnemyState.FirstAdvance:
                base.Move();

                if (transform.position.x <= trigger_point)
                {
                    target_pos_point = Random.Range(-4.5f, 4.5f);
                    StartCoroutine(RandomPosTransition());
                    state = EnemyState.RandomPos;
                }
                break;
            case EnemyState.RandomPos:
                break;
            case EnemyState.Shooting:
                break;
            case EnemyState.Ending:
                base.Move();
                break;
        }
    }

    IEnumerator RandomPosTransition()
    {
        float start_pos = transform.position.y;
        float time_start = Time.time;
        Vector3 pos = transform.position;

        while (transform.position.y != target_pos_point)
        {
            float t = (Time.time - time_start) / time_to_pos;

            if (t >= 1)
            {
                pos.y = target_pos_point;
            }
            else
            {
                pos.y = Mathf.Lerp(start_pos, target_pos_point, t);
            }

            transform.position = pos;

            yield return null;
        }

        state = EnemyState.Shooting;
        anim.SetTrigger("Shoot");
    }

    public void ShootingEnded()
    {
        state = EnemyState.Ending;
    }
}
