using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Base")]
    public Vector2 mPosition;
    public Vector2 mVelocity;
    [Header("Speed mult. factor")]
    public Vector2 mSpeed; // Used as multipler: stores speed mult independently for each component
    public bool randomizeStartingSpeed;
    public Vector2 minAndMax;

    public float minCoordToDestroy = 0.0f; // coords for the left side to despawn the enemy, to adjust sprite size offset

    private bool deleted = false;

    [Header("Death Particles")]
    [SerializeField]
    private GameObject deathParticlesTicks;
    [SerializeField]
    private GameObject deathParticlesExplosion;

    // Start is called before the first frame update
    void Start()
    {
        mPosition = transform.position;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (deleted)
        {
            return;
        }

        CheckOffScreenConditions();
        Move();
    }

    private void CheckOffScreenConditions()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if(screenPosition.x - 20 < minCoordToDestroy)
        {
            Player.instance.OnUpdateBarUp();
            DestroyWithTicks();
        }
    }

    public virtual void Initialize()
    {
        if (randomizeStartingSpeed)
        {
            mSpeed.x = Random.Range(minAndMax.x, minAndMax.y);
            mSpeed.y = Random.Range(minAndMax.x, minAndMax.y);
        }
        /*else
        {
            mSpeed = new Vector2(1.0f, 1.0f);
        }*/
    }

    public virtual void Move()
    {
        mPosition.x += mVelocity.x * mSpeed.x * Time.deltaTime;
        mPosition.y = transform.position.y;
        transform.position = mPosition; 
    }

    public virtual void Destroy()
    {
        Destroy(Instantiate(deathParticlesExplosion, transform.position, transform.rotation),2);

        Destroy(gameObject);

        deleted = true;
    }
    public virtual void DestroyWithTicks()
    {
        if (Player.instance.gameObject.activeSelf)
        {
            Destroy(Instantiate(deathParticlesExplosion, transform.position, transform.rotation), 2);

            Instantiate(deathParticlesTicks, transform.position, transform.rotation);
        }
        Destroy(gameObject);

        deleted = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Death")
        {
            ++Player.instance.enemies_erased;
            EnemyTouched();
        }

        if (collision.tag == "Player")
        {
            EnemyTouched();
        }
    }

    void EnemyTouched()
    {
        Destroy();
        EnemiesSpawner.instance.PlayOnEnemyDeadSFX();
    }
}
