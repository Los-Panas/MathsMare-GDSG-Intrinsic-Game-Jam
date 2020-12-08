using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlus : EnemyBase
{
    enum CurrentDirection
    {
        Horizontal,
        Vertical
    }

    CurrentDirection dir = CurrentDirection.Horizontal;
    int vertical_dir = -1;

    float time_start = 0;

    public override void Initialize()
    {
        if (Camera.main.WorldToScreenPoint(transform.position).y >= 0.5f)
        {
            vertical_dir = -1;
        }
    }

    public override void Move()
    {
        time_start += Time.deltaTime;

        if (time_start >= 1.0f)
        {
            time_start = 0;
            switch (dir)
            {
                case CurrentDirection.Horizontal:
                    vertical_dir = -vertical_dir;
                    dir = CurrentDirection.Vertical;
                    break;
                case CurrentDirection.Vertical:
                    dir = CurrentDirection.Horizontal;
                    break;
            }
        }

        switch (dir)
        {
            case CurrentDirection.Horizontal:
                base.Move();
                break;
            case CurrentDirection.Vertical:
                mPosition.y += mVelocity.y * mSpeed.y * Time.deltaTime * vertical_dir;
                mPosition.x = transform.position.x;
                transform.position = mPosition;
                break;
        }
    }
}
