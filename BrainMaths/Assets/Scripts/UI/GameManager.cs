using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        MainMenu,
        Options,
        Credits,
        Transition,
        Tutorial,
        Game
    }

    GameStates game_state = GameStates.MainMenu;

    // Inspector
    [SerializeField]
    Animator canvas_anim;
    public GameObject PlayerBrain;
    [SerializeField]
    GameObject MainGame;

    [HideInInspector]
    public Player player;

    float time_holding_ulti = 0;

    // Start is called before the first frame update
    void Awake()
    {
        player = PlayerBrain.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (game_state == GameStates.Tutorial)
        {
            if (Input.GetKey(player.MoveUp) && Input.GetKey(player.MoveDown))
            {
                time_holding_ulti += 1;

                if (time_holding_ulti >= 10)
                {
                    ChangeState(GameStates.Game);
                }
            }

            if (Input.GetKeyUp(player.MoveUp) || Input.GetKeyUp(player.MoveDown))
            {
                time_holding_ulti = 0;
            }
        }
    }
    

    public void ChangeState(GameStates new_state)
    {
        switch (new_state)
        {
            case GameStates.MainMenu:
                break;
            case GameStates.Options:
                break;
            case GameStates.Credits:
                break;
            case GameStates.Transition:
                // Start Transition
                canvas_anim.SetTrigger("Transition");
                break;
            case GameStates.Tutorial:
                Time.timeScale = 0;
                break;
            case GameStates.Game:
                canvas_anim.gameObject.SetActive(false);
                MainGame.transform.GetChild(0).gameObject.SetActive(true);
                Time.timeScale = 1;
                player.UseSpecial();
                break;
        }

        game_state = new_state;
    }
}
