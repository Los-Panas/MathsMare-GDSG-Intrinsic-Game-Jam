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
    [SerializeField]
    private float changeBarsColor = 0;
    private float beatcountbybars = 0;
    [SerializeField]
    private bool repeatColors = false;


    [Header("Background")]
    [SerializeField]
    [Space]
    private float beatsBeforeChange = 0;
    private float beatCout = 0;
    [SerializeField]
    private Color[] availableColors;

    [Header("Enemy Spawner")]
    [SerializeField]
    [Space]
    private float beatsBeforeSpawn = 0;
    private float spawnCout = 0;
    [SerializeField]
    private EnemiesSpawner spawner;

    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        currentAudioSource = audioSources[0];
        extraAudioSource = audioSources[1];

        audioProcessor = GetComponent<AudioProcessor>();

        OnChangeMusic();

        beatcountbybars = changeBarsColor;
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
            clip = clips[Random.Range(0, clips.Length - 1)];
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
            
            float mult = 0.5f;
            if (!repeatColors)
                mult = 1;
            else
                mult = 0.5f;

            Color col = Camera.main.backgroundColor;
            Color colorbefore = col;
            for (int i = 0; i <= (bars.Length - 1) *mult; ++i)
            {
                while (col == Camera.main.backgroundColor || col == colorbefore)
                {
                    col = availableColors[Random.Range(0, availableColors.Length)];
                }
                colorbefore = col;
                bars[i].material.color = col;
                if(repeatColors)
                    bars[(bars.Length - 1) - i].material.color = col;
            }
            
        }
        ++spawnCout;
        if(spawnCout >= beatsBeforeSpawn)
        {
            spawnCout = 0;
            spawner.SpawnRandomEnemy();
        }
        ++beatcountbybars;
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
                bars[i].material.SetFloat("_Fill", Mathf.Min(1, spectrum[i] * barLength));
            }
        }

        if (changeBarsColor <= beatcountbybars)
        {
            spectrumCount = 0;
            int min = Mathf.Min(spectrum.Length, bars.Length);
            
            float mult = 0.5f;
            if (!repeatColors)
                mult = 1;
            else
                mult = 0.5f;

            Color colorbefore = Camera.main.backgroundColor;

            for (int i = 0; i <= min - 1; ++i)
            {
                beatcountbybars = 0;

                if (i <= min * mult)
                {
                    Color col = Camera.main.backgroundColor;
                    while (col == Camera.main.backgroundColor || col == colorbefore)
                    {
                        col = availableColors[Random.Range(0, availableColors.Length)];
                    }
                    colorbefore = col;
                    bars[i].material.color = col; // TODO: change color

                    if(repeatColors)
                        bars[min - 1 - i].material.color = col;
                }
            }

        }
    }
}
