using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradeBar : MonoBehaviour
{
    public List<Sprite> bar = new List<Sprite>();
    SpriteRenderer renderer;
    public int currentGrade = 0; //TODO: this variable should be the amount of dodge points that the player currently has.
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentGrade <= 16)
            renderer.sprite = bar[0];
        else if(currentGrade <= 32)
            renderer.sprite = bar[1];
        else if (currentGrade <= 48)
            renderer.sprite = bar[2];
        else if (currentGrade <= 64)
            renderer.sprite = bar[3];
        else if (currentGrade <= 80)
            renderer.sprite = bar[4];
        else if (currentGrade <= 100)
            renderer.sprite = bar[5];
    }
}
