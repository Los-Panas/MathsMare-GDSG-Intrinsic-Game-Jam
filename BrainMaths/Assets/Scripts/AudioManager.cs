using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Musics")]
    [SerializeField]
    private float overlapEndingSec = 4;
    [SerializeField]
    private AudioClip[] clips;

    private List<AudioClip> clipsUsed = new List<AudioClip>();
    private AudioProcessor audioProcessor;
    private AudioSource currentAudioSource;
    private AudioSource extraAudioSource;

    [Header("Spectrum Visual Settings")]
    [SerializeField]
    private float length = 0;
    [SerializeField]
    private float distanceBetween = 0;

    [Header("Background")]
    [SerializeField]
    [Space]
    private float beatsBeforeChange = 0;
    private float beatCout = 0;
    [SerializeField]
    private Color[] availableColors;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        currentAudioSource = audioSources[0];
        extraAudioSource = audioSources[1];

        audioProcessor = GetComponent<AudioProcessor>();

        OnChangeMusic();
    }

    private void Update()
    {
        if (currentAudioSource.isPlaying && currentAudioSource.clip.length - currentAudioSource.time <= overlapEndingSec)
        {
            OnChangeMusic();
        }
    }

    private void OnChangeMusic()
    {
        AudioSource auxAudioSource = currentAudioSource;

        extraAudioSource.clip = GetRandomClip();
        extraAudioSource.Play();

        currentAudioSource = extraAudioSource;
        extraAudioSource = auxAudioSource;

        audioProcessor.SetAudioSource(currentAudioSource);
    }

    private AudioClip GetRandomClip()
    {
        AudioClip clip;

        while (true)
        {
            clip = clips[Random.Range(0, clips.Length)];
            if (!clipsUsed.Contains(clip))
            {
                if (clipsUsed.Count == clips.Length - 1)
                {
                    clipsUsed.Clear();
                }

                clipsUsed.Add(clip);
                break;
            }
        }

        return clip;
    }

    public void OnBeat()
    {
        ++beatCout;
        if (beatCout >= beatsBeforeChange)
        {
            beatCout = 0;
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
