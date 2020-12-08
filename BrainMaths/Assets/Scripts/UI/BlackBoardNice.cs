using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoardNice : MonoBehaviour
{
    float manual_time = 0;
    float sinus_speed = 2.0f;
    [SerializeField]
    float arc = 10.0f;
    RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        manual_time += Time.deltaTime * sinus_speed;
    }

    private void FixedUpdate()
    {
        Vector2 movement = rt.anchoredPosition;
        movement.y += Mathf.Sin(manual_time) * arc;
        rt.anchoredPosition = movement;
    }
}
