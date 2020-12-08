using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour
{
    public GameObject[] buttons;
    public GameObject text;
    public float timeWaitBar = 0;
    public EventSystem eventSystem;
    public Image wBar;
    public Image sBar;
    public float timeHolding = 1;

    private int selected = 2;
    private InputStates wStates = new InputStates(KeyCode.W);
    private InputStates sStates = new InputStates(KeyCode.S);

    Coroutine wUpCoroutine = null;
    Coroutine sUpCoroutine = null;
    Coroutine wDownCoroutine = null;
    Coroutine sDownCoroutine = null;

    bool wBarFinished = false;
    bool sBarFinished = false;

    bool sIngore = false;
    bool wIgnore = false;

    struct InputStates
    {
        static public float timeWaitingBeforeBar = 0;

        public InputStates(KeyCode code)
        {
            this.code = code;

            lastFrameStates = new bool[3];
            lastFrameStates[0] = false; // down
            lastFrameStates[1] = false; // repeat
            lastFrameStates[2] = false; // up

            states = new bool[3];
            states[0] = false; // down
            states[1] = false; // repeat
            states[2] = false; // up

            time = 0;
        }

        private bool[] lastFrameStates;
        private bool[] states;
        private KeyCode code;
        private float time;

        public bool IsLastFrameDown() { return lastFrameStates[0]; }
        public bool IsLastFrameRepeat() { return lastFrameStates[1]; }
        public bool IsLastFrameUp() { return lastFrameStates[2]; }

        public bool IsDown() { return states[0]; }
        public bool IsRepeat() { return states[1]; }
        public bool IsUp() { return states[2]; }

        public bool TimeUp() { return Time.time - time > timeWaitingBeforeBar; }

        public void StartTime() { time = Time.time; }

        public bool Released()
        {
            return !Input.GetKey(code) && !Input.GetKeyDown(code) && !Input.GetKeyUp(code);
        }

        public void Update()
        {
            lastFrameStates[0] = states[0];
            lastFrameStates[1] = states[1];
            lastFrameStates[2] = states[2];

            states[0] = Input.GetKeyDown(code);
            states[1] = Input.GetKey(code);
            states[2] = Input.GetKeyUp(code);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InputStates.timeWaitingBeforeBar = timeWaitBar;

        sBar.material = new Material(sBar.material);
        sBar.material.SetFloat("_Fill", 0);

        wBar.material = new Material(wBar.material);
        wBar.material.SetFloat("_Fill", 0);
    }

    // Update is called once per frame
    void Update()
    {
        wStates.Update();
        sStates.Update();

        if (wIgnore && wStates.Released())
        {
            wIgnore = false;
        }

        if (sIngore && sStates.Released())
        {
            sIngore = false;
        }

        if (!wIgnore)
        {
            if (wStates.IsDown())
            {
                wStates.StartTime();
            }

            if (wStates.IsLastFrameRepeat() && wStates.IsRepeat() && wStates.TimeUp())
            {
                if (wDownCoroutine != null)
                {
                    StopCoroutine(wDownCoroutine);
                    wDownCoroutine = null;
                }
                if (wUpCoroutine == null)
                {
                    wUpCoroutine = StartCoroutine(MoveBar(wBar, 1));
                }
            }
            else if (wStates.IsUp())
            {
                if (wStates.TimeUp())
                {
                    if (wUpCoroutine != null)
                    {
                        StopCoroutine(wUpCoroutine);
                        wUpCoroutine = null;
                    }
                    if (wDownCoroutine == null)
                    {
                        wDownCoroutine = StartCoroutine(MoveBar(wBar, 0));
                        wBarFinished = false;
                    }
                }
                else
                {
                    if (selected != 4)
                    {
                        ++selected;
                    }
                    else
                    {
                        selected = 0;
                    }

                    SetSelectedGO(buttons[selected]);
                }
            }
        }

        if (!sIngore)
        {
            if (sStates.IsDown())
            {
                sStates.StartTime();
            }

            if (sStates.IsLastFrameRepeat() && sStates.IsRepeat() && sStates.TimeUp())
            {
                if (sDownCoroutine != null)
                {
                    StopCoroutine(sDownCoroutine);
                    sDownCoroutine = null;
                }
                if (sUpCoroutine == null)
                {
                    sUpCoroutine = StartCoroutine(MoveBar(sBar, 1));
                }
            }
            else if (sStates.IsUp())
            {
                if (sStates.TimeUp())
                {
                    if (sUpCoroutine != null)
                    {
                        StopCoroutine(sUpCoroutine);
                        sUpCoroutine = null;
                    }
                    if (sDownCoroutine == null)
                    {
                        sDownCoroutine = StartCoroutine(MoveBar(sBar, 0));
                        sBarFinished = false;
                    }
                }
                else
                {
                    if (selected != 0)
                    {
                        --selected;   
                    }
                    else
                    {
                        selected = 4;
                    }
                    SetSelectedGO(buttons[selected]);
                }
            }
        }

       
        if (wBarFinished && sBarFinished)
        {
            wBarFinished = false;
            sBarFinished = false;
            sBar.material.SetFloat("_Fill", 0);
            wBar.material.SetFloat("_Fill", 0);

            sIngore = true;
            wIgnore = true;

            switch (selected)
            {
                case 0: // Github
                {
                    Application.OpenURL("https://github.com/Los-Panas/MathsMare-GDSG-Intrinsic-Game-Jam");
                    break;
                }
                case 1: // Settings
                {
                    UIButtons_Functions.instance.SettingsButton();
                    break;
                }
                case 2: // Play
                {
                    UIButtons_Functions.instance.PlayButton();
                    break;
                }
                case 3: // Credits
                {
                    UIButtons_Functions.instance.CreditsButton();
                    break;
                }
                case 4: // ItchIO
                {
                    Application.OpenURL("https://victorgg-11.itch.io/mathsmare");
                    break;
                }
            }
        }
    }

    private void SetSelectedGO(GameObject gameObject)
    {
        eventSystem.SetSelectedGameObject(gameObject);
        text.transform.position = new Vector2(gameObject.transform.position.x,
            gameObject.transform.position.y - gameObject.GetComponent<RectTransform>().rect.height * 0.31F);

        StopAllCoroutines();
        sUpCoroutine = null;
        wUpCoroutine = null;
        sDownCoroutine = null;
        wDownCoroutine = null;
        sBar.material.SetFloat("_Fill", 0);
        wBar.material.SetFloat("_Fill", 0);
    }

    IEnumerator MoveBar(Image image, float value)
    {
        image.gameObject.SetActive(true);

        float current = image.material.GetFloat("_Fill");
        float init = current;
        float time = Time.time;

        while (true)
        {
            float t = (Time.time - time) / timeHolding;
            current = Mathf.Lerp(init, value, t);
            image.material.SetFloat("_Fill", current);
            
            if (current == value)
            {
                if (image == sBar)
                {
                    if (value != 0)
                    {
                        sBarFinished = true;
                        sUpCoroutine = null;
                    }
                    else
                    {
                        sDownCoroutine = null;
                    }
                    break;
                }
                else
                {
                    if (value != 0)
                    {
                        wBarFinished = true;
                        wUpCoroutine = null;
                    }
                    else
                    {
                        wDownCoroutine = null;
                    }
                    break;
                }
            }
            yield return null;
        }
    }
}
