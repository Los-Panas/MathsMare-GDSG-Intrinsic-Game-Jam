using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.2f;
    [SerializeField]
    private bool constantSpeed = false;
    [SerializeField]
    private bool fastSpeed = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y > 0)
            GetComponent<SpriteRenderer>().material.SetFloat("time", Time.fixedTime);
        else
            GetComponent<SpriteRenderer>().material.SetFloat("time", -Time.fixedTime);

        if(!constantSpeed)
            speed = AudioManager.instance.DtBeats();
        if (fastSpeed)
            speed = 1 - speed;
        GetComponent<SpriteRenderer>().material.SetFloat("speed", speed);
    }
}
