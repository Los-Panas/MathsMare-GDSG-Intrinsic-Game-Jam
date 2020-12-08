using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChangeColor : MonoBehaviour
{
    public SpriteRenderer[] srs;
    private int lastColorId;

    private void Awake()
    {
        lastColorId = 99;
    }
    public void ChangeColor(int col)
    {
        Color[] allColors = AudioManager.instance.availableColors;

        int n_color = Random.Range(0, allColors.Length);

        while(n_color == col || n_color == lastColorId)
        {
            n_color = Random.Range(0, allColors.Length);
        }

        for (int i = 0; i < srs.Length; ++i)
        {
            srs[i].color = allColors[n_color];
        }

        lastColorId = n_color;
    }
}
