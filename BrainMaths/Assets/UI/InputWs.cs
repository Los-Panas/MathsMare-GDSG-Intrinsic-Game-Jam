using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputWs : MonoBehaviour
{
    public Sprite sSprite;
    public Sprite wSprite;
    public Image sImage;
    public Image wImage;

    Sprite auxW;
    Sprite auxS;

    private void Start()
    {
        auxW = wSprite;
        auxS = sSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || (auxW == wImage.sprite && !Input.GetKeyDown(KeyCode.W) && !Input.GetKey(KeyCode.W) && !Input.GetKeyUp(KeyCode.W)))
        {
            SwapWSprites();
        }

        if (Input.GetKeyDown(KeyCode.S) || (auxS == sImage.sprite && !Input.GetKeyDown(KeyCode.S) && !Input.GetKey(KeyCode.S) && !Input.GetKeyUp(KeyCode.S)))
        {
            SwapSSprite();
        }
    }

    void SwapWSprites()
    {
        Sprite aux = wImage.sprite;
        wImage.sprite = wSprite;
        wSprite = aux;
    }

    void SwapSSprite()
    {
        Sprite aux = sImage.sprite;
        sImage.sprite = sSprite;
        sSprite = aux;
    }
}
