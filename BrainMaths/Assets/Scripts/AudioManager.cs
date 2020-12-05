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

    [Header("Spectrum Bars")]
    [SerializeField]
    private float barLength = 0;
    [SerializeField]
    private int refrashRate = 0;
    private int spectrumCount = 0;
    [SerializeField]
    private MeshRenderer[] bars;

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
        ++spectrumCount;
        if (spectrumCount >= refrashRate)
        {
            spectrumCount = 0;
            int min = Mathf.Min(spectrum.Length, bars.Length);
            for (int i = 0; i < min; ++i)
            {
                // bars[i].material.color = Color.green; // TODO: change color
                bars[i].material.SetFloat("_Fill", Mathf.Min(1, spectrum[i] * barLength));
            }
        }
    }
}
