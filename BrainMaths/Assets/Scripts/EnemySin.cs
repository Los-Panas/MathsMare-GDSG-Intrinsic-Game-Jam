using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySin : EnemyBase
{
    [Header("EnemySin")]
    public float amplitude = 2.0f;
    public float waveSpeed = 10.0f;  // TODO: speed based on bpm?
    public float mySpeedX = 3.0f;
    private float value;

    public override void Initialize()
    {
        Debug.Log("Im enemy moUstachoe, and i dont want random speed");
        mSpeed = new Vector2(mySpeedX, waveSpeed);
    }

    public override void Move()
    {
        base.Move(); // always move on x

        mSpeed.y = waveSpeed; // for runtime mod
        value += mSpeed.y * Time.deltaTime;

        Vector2 pos_y = transform.position;
        pos_y.y = Mathf.Sin(value) * amplitude;

        transform.position = pos_y;
    }
}
