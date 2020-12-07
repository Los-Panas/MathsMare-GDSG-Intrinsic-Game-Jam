using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons_Functions : MonoBehaviour
{
    [SerializeField]
    GameManager GameManager;
    [SerializeField]
    MenusManager Canvas;

    public void PlayButton()
    {
        GameManager.ChangeState(GameManager.GameStates.Transition);
    }
}
