using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        MainMenu,
        Options,
        Credits,
        Transition,
        Tutorial,
        Game,
        Dead
    }

    GameStates game_state = GameStates.MainMenu;

    // Inspector
    [SerializeField]
    Animator canvas_anim;
    public GameObject PlayerBrain;
    [SerializeField]
    GameObject MainGame;
    [SerializeField]
    GameObject Gameplay;
    [SerializeField]
    MenusManager MenuManager;
    [SerializeField]
    GameObject MenuCamera;
    [SerializeField]
    RectTransform LoseMenu;
    [SerializeField]
    CloudMove[] clouds;

    [SerializeField]
    Text enemiesErased;
    [SerializeField]
    Text enemiesDodged;
    [SerializeField]
    Text hitsRecieved;

    [HideInInspector]
    public Player player;

    float time_holding_options = 0;
    float time_holding_ulti = 0;
    bool accept_input = false;

    // Start is called before the first frame update
    void Awake()
    {
        player = PlayerBrain.GetComponent<Player>();
        Gameplay.SetActive(false);

        for (int i = 0; i < 2; ++i)
        {
            clouds[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (game_state)
        {
            case GameStates.MainMenu:
                break;
            case GameStates.Options:
                if (Input.GetKey(player.MoveUp) && Input.GetKey(player.MoveDown))
                {
                    time_holding_options += 1;

                    if (time_holding_options >= 120)
                    {
                        time_holding_options = 0;
                        ChangeState(GameStates.MainMenu);
                    }
                }

                if (Input.GetKeyUp(player.MoveUp) || Input.GetKeyUp(player.MoveDown))
                {
                    time_holding_options = 0;
                }
                break;
            case GameStates.Credits:
                break;
            case GameStates.Transition:
                break;
            case GameStates.Tutorial:
                if (Input.GetKey(player.MoveUp) && Input.GetKey(player.MoveDown))
                {
                    time_holding_ulti += 1;

                    if (time_holding_ulti >= 10)
                    {
                        time_holding_ulti = 0;
                        ChangeState(GameStates.Game);
                    }
                }

                if (Input.GetKeyUp(player.MoveUp) || Input.GetKeyUp(player.MoveDown))
                {
                    time_holding_ulti = 0;
                }
                break;
            case GameStates.Game:
                break;
            case GameStates.Dead:
                if (accept_input)
                {
                    if (Input.GetKey(player.MoveUp) && Input.GetKey(player.MoveDown))
                    {
                        time_holding_ulti += 1;

                        if (time_holding_ulti >= 10)
                        {
                            time_holding_ulti = 0;
                            RestartGame();
                        }
                    }

                    if (Input.GetKeyUp(player.MoveUp) || Input.GetKeyUp(player.MoveDown))
                    {
                        time_holding_ulti = 0;
                    }
                }
                break;
            default:
                break;
        }
    }
    

    public void ChangeState(GameStates new_state)
    {
        switch (new_state)
        {
            case GameStates.MainMenu:
                MenuManager.MainMenu.SetActive(true);
                MenuManager.SettingsMenu.SetActive(false);
                //MenuManager.Credits.SetActive(false);
                break;
            case GameStates.Options:
                MenuManager.MainMenu.SetActive(false);
                MenuManager.SettingsMenu.SetActive(true);
                //MenuManager.Credits.SetActive(false);
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
                {
                    canvas_anim.gameObject.SetActive(false);
                    Gameplay.SetActive(true);
                    Time.timeScale = 1;
                    player.UseSpecial();
                    MenuCamera.SetActive(false);
                    for (int i = 0; i < 2; ++i)
                    {
                        clouds[i].enabled = true;
                    }
                }
                break;
            case GameStates.Dead:
                {
                    MenuCamera.SetActive(true);
                    Gameplay.SetActive(false);
                    StartCoroutine(GetUpLoseMenu(0));
                    UpdateGameStats();
                    for (int i = 0; i < 2; ++i)
                    {
                        clouds[i].enabled = false;
                    }
                }
                break;
        }

        game_state = new_state;
    }

    void UpdateGameStats()
    {
        enemiesErased.text = player.enemies_erased.ToString();
        enemiesDodged.text = player.enemies_avoided.ToString();
        hitsRecieved.text = player.hits_received.ToString();
    }

    IEnumerator GetUpLoseMenu(float pos_y, bool restart = false)
    {
        Vector2 start_pos = LoseMenu.anchoredPosition;
        Vector2 goal_pos = start_pos;
        goal_pos.y = pos_y;
        float time_start = Time.time;

        while (LoseMenu.anchoredPosition != goal_pos)
        {
            float t = (Time.time - time_start);

            if (t < 1)
            {
                LoseMenu.anchoredPosition = Vector2.Lerp(start_pos, goal_pos, t);
            }
            else
            {
                LoseMenu.anchoredPosition = goal_pos;
            }

            yield return null;
        }

        LoseMenu.GetComponent<BlackBoardNice>().enabled = true;

        if (restart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        accept_input = true;
    }

    void RestartGame()
    {
        accept_input = false;
        StartCoroutine(GetUpLoseMenu(-1020, true));
    }
}
