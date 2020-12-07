using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDiag : EnemyBase
{
    [Header("EnemyDiagonal")]
    public float mySpeed = 0f;
    private Vector2 myDirection = Vector2.zero;
    

    // Start is called before the first frame update
    void Start()
    {
        GetRandomDirectionRange(true);
    }

    // Update is called once per frame
    public override void Move()
    {
        if (Camera.main.WorldToScreenPoint(transform.position).x < 0)
        {
            Destroy(gameObject);
        }

        mPosition = transform.position;

        if (Camera.main.WorldToScreenPoint(mPosition).y > Camera.main.pixelHeight)
            GetRandomDirectionRange(false);
        else if(Camera.main.WorldToScreenPoint(mPosition).y < 0)
            GetRandomDirectionRange(true);

        mPosition.x += myDirection.x * mySpeed * Time.deltaTime;
        mPosition.y += myDirection.y * mySpeed * Time.deltaTime;
        transform.position = mPosition;
    }

    void GetRandomDirectionRange(bool isUp)
    {
        float randRange;

        if (isUp)
            randRange = Random.Range(95, 175);
        else
            randRange = Random.Range(185, 265);

        float xComp = Mathf.Cos(Mathf.Deg2Rad*randRange);
        float yComp = Mathf.Sin(Mathf.Deg2Rad * randRange);
        myDirection.Set(xComp, yComp);
    }
}
