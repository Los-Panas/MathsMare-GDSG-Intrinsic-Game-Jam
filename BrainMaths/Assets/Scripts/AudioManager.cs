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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnBeat()
    {
        Debug.Log("Beat!!!");
    }

    public void OnSpectrum(float[] spectrum)
    {
        for (int i = 0; i < spectrum.Length; ++i)
        {
            print(spectrum[i]);
            Vector3 start = new Vector3(i, 0, 0);
            Vector3 end = new Vector3(i, spectrum[i] * length, 0);
            Debug.DrawLine(start, end);
        }
    }
}
