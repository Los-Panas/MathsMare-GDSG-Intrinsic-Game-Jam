using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoardNice : MonoBehaviour
{
    float manual_time = 0;
    float sinus_speed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        manual_time += Time.deltaTime * sinus_speed;
    }
    private void FixedUpdate()
    {
        Vector2 movement = Vector2.zero;
        movement.y += Mathf.Sin(manual_time) * 10f;
        GetComponent<RectTransform>().anchoredPosition = movement;
    }
}
