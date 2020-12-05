using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Spectrum Visual Settings")]

    [SerializeField]
    private float length = 0;
    [SerializeField]
    private float distanceBetween = 0;

    [Header("Background")]
    [SerializeField]
    [Space]
    private float timeBeforeChange = 0;
    private float timeLastChange = 0;
    [SerializeField]
    private Color[] availableColors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnBeat()
    {
        if (Time.time - timeLastChange > timeBeforeChange)
        {
            timeLastChange = Time.time;
            Camera.main.backgroundColor = availableColors[Random.Range(0, availableColors.Length)];
        }
    }

    public void OnSpectrum(float[] spectrum)
    {
        for (int i = 0; i < spectrum.Length; ++i)
        {
            Vector3 start = new Vector3(i * distanceBetween, 0, 0);
            Vector3 end = new Vector3(i * distanceBetween, spectrum[i] * length, 0);
            Debug.DrawLine(start, end);
        }
    }
}
