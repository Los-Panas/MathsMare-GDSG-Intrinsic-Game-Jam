using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons_Functions : MonoBehaviour
{
    static public UIButtons_Functions instance;

    [SerializeField]
    GameManager GameManager;
    [SerializeField]
    MenusManager Canvas;

    private void Start()
    {
        instance = this;
    }

    public void PlayButton()
    {
        GameManager.ChangeState(GameManager.GameStates.Transition);
    }

    public void SettingsButton()
    {
        GameManager.ChangeState(GameManager.GameStates.Options);
    }
}
