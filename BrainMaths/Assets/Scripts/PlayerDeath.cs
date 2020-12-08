using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField]
    private float liveTimeSeconds;
    public GameObject particleExplosionPrefab;
    public GameObject particleSufferingPrefab;
    [SerializeField]
    private float noiseInc;
    [SerializeField]
    private float noiseIncMultiplier;
    [SerializeField]
    private float offsetMax;
    [SerializeField]
    private float offsetInc; // %
    [SerializeField]
    private float scaleInc; // more time, more scale
    private float scaleNoiseInc;
    [SerializeField]
    private float scaleIncMultiplier;
    public float maxAngleDeg;

    private Vector2 realPos;

    // Start is called before the first frame update
    void Start()
    {
        realPos = transform.position;
        scaleInc = transform.localScale.x;

        StartCoroutine("ParticleExplosion", liveTimeSeconds);
        StartCoroutine("Suffer");
    }

    // Update is called once per frame
    void Update()
    {
        AttitudeShake();
        ScaleBumping();
    }

    void ScaleBumping()
    {
        // always grow
        scaleInc += Time.deltaTime * scaleIncMultiplier;

        scaleNoiseInc += Time.deltaTime * scaleIncMultiplier;

        // but we want to make some ping pong effect
        float newScale = scaleInc;

        newScale += (scaleInc * 0.5f) * scaleIncMultiplier * (Mathf.PerlinNoise(scaleNoiseInc, 0) - 0.5f);

        transform.localScale = new Vector2(newScale, newScale);
    }

    void AttitudeShake()
    {
        noiseIncMultiplier += Time.deltaTime * (noiseIncMultiplier * 0.1f);
        noiseInc += Time.deltaTime * noiseIncMultiplier;
        Vector2 shakedPos = realPos;

        float newAngle = maxAngleDeg * (1 - Mathf.PerlinNoise(noiseInc, noiseInc + maxAngleDeg) * 2);
        shakedPos.x += offsetMax * offsetInc * (1 - Mathf.PerlinNoise(noiseInc, 0) * 2);
        shakedPos.y += offsetMax * offsetInc * (1 - Mathf.PerlinNoise(0, noiseInc) * 2);

        transform.position = shakedPos;
        transform.rotation = Quaternion.Euler(Vector3.forward * newAngle);
    }

    IEnumerator ParticleExplosion(float liveTimeSeconds)
    {
        yield return new WaitForSeconds(liveTimeSeconds);

        Instantiate(particleExplosionPrefab, transform.position, Quaternion.identity);

        StartCoroutine(FadeMusics(GameObject.Find("Main Camera").GetComponents<AudioSource>()));

        Invoke("ExplosionEnded", 2.0f);

        GetComponent<SpriteRenderer>().enabled = false;
    }

    void ExplosionEnded()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().ChangeState(GameManager.GameStates.Dead);
        Destroy(gameObject);
    }

    IEnumerator Suffer()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 1.0f));

            Instantiate(particleSufferingPrefab, transform.position, Quaternion.identity);
        }
    }

    IEnumerator FadeMusics(AudioSource[] sources)
    {
        float[] volumes = new float[sources.Length];

        for (int i = 0; i < sources.Length; ++i)
        {
            volumes[i] = sources[i].volume;
        }

        float time_start = Time.time;
        GameObject.Find("AudioSourceDieMenu").GetComponent<AudioSource>().Play();
        while (Time.time - time_start < 2.0f)
        {
            float t = (Time.time - time_start) / 2.0f;

            for (int i = 0; i < sources.Length; ++i)
            {
                sources[i].volume = volumes[i] - t;
            }

            yield return null;
        }
    }

}
