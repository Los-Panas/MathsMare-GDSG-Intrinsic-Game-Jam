using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsController : MonoBehaviour
{
    public GameObject[] buttons;
    public GameObject text;
    public float timeWaitBar = 0;
    public EventSystem eventSystem;

    private int selected = 2;
    private InputStates wStates = new InputStates(KeyCode.W);
    private InputStates sStates = new InputStates(KeyCode.S);

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
    }

    // Update is called once per frame
    void Update()
    {
        wStates.Update();
        sStates.Update();

        if (wStates.IsDown())
        {
            wStates.StartTime();
        }

        if (wStates.IsLastFrameRepeat() && wStates.IsRepeat() && wStates.TimeUp())
        {
            // TODO: start W bar
            print("Starting W Bar");
        }
        else if (wStates.IsUp())
        {
            if (wStates.TimeUp())
            {
                // TODO: Decrease Bar
                print("On Down W Bar");
            }
            else
            {
                if (selected != 4)
                {
                    ++selected;
                    SetSelectedGO(buttons[selected]);
                }
            }
        }

        if (sStates.IsDown())
        {
            sStates.StartTime();
        }

        if (sStates.IsLastFrameRepeat() && sStates.IsRepeat() && sStates.TimeUp())
        {
            // TODO: start S bar
            print("Starting S Bar");
        }
        else if (sStates.IsUp())
        {
            if (sStates.TimeUp())
            {
                // TODO: Decrease Bar
                print("On Down S Bar");
            }
            else
            {
                if (selected != 0)
                {
                    --selected;
                    SetSelectedGO(buttons[selected]);
                }
            }
        }
    }

    private void SetSelectedGO(GameObject gameObject)
    {
        eventSystem.SetSelectedGameObject(gameObject);
        text.transform.position = new Vector2(gameObject.transform.position.x,
            gameObject.transform.position.y - gameObject.GetComponent<RectTransform>().rect.height * 0.3F);
    }
}
