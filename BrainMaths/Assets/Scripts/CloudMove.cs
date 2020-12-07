using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{
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

    }
}
