using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    static public BeatManager instance;
    public List<BeatData> subscribed_objs_to_beat;

    public class BeatData : MonoBehaviour
    {
        public GameObject obj;
        public Vector2 originalScale;
        public float incValue;
        public float speedMult;
        public bool beating;

        public void CreateData(GameObject obj)
        {
            this.obj = obj;
            this.originalScale = this.obj.transform.localScale;
            incValue = 0.0f;
            beating = false;
            speedMult = 11.0f;
        }

        public bool DoBeat()
        {
            if (obj == null)
                return false;

            if (!beating)
                StartCoroutine("Beating");

            return true;
        }

        IEnumerator Beating()
        {
            beating = true;

            while (beating)
            {
                BeatManager.instance.BeatLogic(this);
                yield return null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    { 
        instance = this;
        subscribed_objs_to_beat = new List<BeatData>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Beat();
        }
    }

    public void AddObjToBeat(GameObject obj)
    {
        BeatData bd = obj.AddComponent<BeatData>();
        bd.CreateData(obj);
        subscribed_objs_to_beat.Add(bd);
    }

    public void Beat()
    {
        for(int i = 0; i < subscribed_objs_to_beat.Count; ++i)
        {
            if(!subscribed_objs_to_beat[i].DoBeat())
            {
                subscribed_objs_to_beat.RemoveAt(i);
            }
        }
    }

    public void BeatLogic(BeatData bd)
    {
        bd.incValue += Time.deltaTime * bd.speedMult;

        float t = Mathf.Sin(bd.incValue);
        float value = Mathf.Abs(t);
        if (t < 0)
        {
            bd.incValue += Time.deltaTime * bd.speedMult;
        }

        Vector2 newScale = bd.originalScale;

        if (bd.incValue > Mathf.PI)
        {
            bd.incValue = 0.0f;
            bd.beating = false;
            bd.obj.transform.localScale = bd.originalScale;
        }
        else
            newScale = new Vector2(bd.originalScale.x + value, bd.originalScale.y + value);

        bd.obj.transform.localScale = newScale;
    }

}
