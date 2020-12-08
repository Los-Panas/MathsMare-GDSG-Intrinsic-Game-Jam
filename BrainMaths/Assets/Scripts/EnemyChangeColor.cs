using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChangeColor : MonoBehaviour
{
    public SpriteRenderer[] srs;
    

    public void ChangeColor(Color[] allColors)
    {

        int n_color = Random.Range(0, allColors.Length);

        while(allColors[n_color] == Camera.main.backgroundColor)
        {
            n_color = Random.Range(0, allColors.Length);
        }

        for (int i = 0; i < srs.Length; ++i)
        {
            srs[i].color = allColors[n_color];
        }
    }
}
