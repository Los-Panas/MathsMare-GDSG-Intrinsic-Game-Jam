using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenusManager : MonoBehaviour
{
    [SerializeField]
    GameManager GameManager;

    public GameObject MainMenu;
    public GameObject SettingsMenu;

    [SerializeField]
    Text MoveUp_Text;
    [SerializeField]
    Text MoveDown_Text;
    [SerializeField]
    Text UltiMoveUp_Text;
    [SerializeField]
    Text UltiMoveDown_Text;

    private void Start()
    {
        MoveDown_Text.text = GameManager.player.MoveDown.ToString();
        MoveUp_Text.text = GameManager.player.MoveUp.ToString();
        UltiMoveDown_Text.text = GameManager.player.MoveDown.ToString();
        UltiMoveUp_Text.text = GameManager.player.MoveUp.ToString();
    }

    public void TransitionFinished()
    {
        GameManager.ChangeState(GameManager.GameStates.Tutorial);
    }
}
