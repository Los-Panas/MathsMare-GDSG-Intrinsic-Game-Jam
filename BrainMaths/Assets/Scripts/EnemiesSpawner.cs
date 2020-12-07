using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour
{
    static public EnemiesSpawner instance;

    public enum SpawnPos
    {
        Top,
        Middle,
        Bottom,
        Random
    }
    public enum EnemyType
    {
        Pi,
        Plus,
        Percentatge,
        Decrease,
        Sqrt,
        Infinity
    }
    [System.Serializable]
    public struct EnemyMap
    {
        public GameObject enemy;
        public EnemyType type;

        public EnemyMap(GameObject enemy, EnemyType type)
        {
            this.enemy = enemy;
            this.type = type;
        }
    }

    [Header("SFX")]
    [SerializeField]
    AudioClip onEnemyDead;
    [SerializeField]
    AudioClip onTickSpawn;

    [Header("Enemies")]
    public List<EnemyMap> enemies;
    public float spawnRate;

    private float timerSpawner;
    Vector3 objectSize;

    private AudioSource audioSource;

    void Start()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();

        timerSpawner = Time.time + spawnRate;

        Vector3 rect = transform.GetComponent<Collider2D>().bounds.size;
        objectSize = transform.position + new Vector3(rect.x - 1, rect.y - 1, 0);
    }

    public void PlayOnEnemyDeadSFX()
    {
        audioSource.PlayOneShot(onEnemyDead, 0.25f);
    }

    public GameObject SpawnEnemy(EnemyType enemy, SpawnPos pos)
    {
        //search enemy
        GameObject spawnEnemy = new GameObject();
        for(int i = 0; i < enemies.Count; ++i)
        {
            if(enemies[i].type == enemy)
            {
                spawnEnemy = enemies[i].enemy;
                break;
            }
        }
        Vector3 spwnPosicion = Vector3.zero;
        switch (pos)
        {
            case SpawnPos.Top:
                spwnPosicion = new Vector3(0, objectSize.y * 0.5f, transform.position.z);
                break;
            case SpawnPos.Middle:

                break;
            case SpawnPos.Bottom:
                spwnPosicion = new Vector3(0, -objectSize.y * 0.5f, transform.position.z);
                break;
            case SpawnPos.Random:
                spwnPosicion = new Vector3(0, Random.Range(-objectSize.y * 0.5f, objectSize.y * 0.5f), 0);
                break;
        }
        
        return Instantiate(spawnEnemy, transform.position + spwnPosicion, transform.rotation);

    }

    public GameObject SpawnRandomEnemy()
    {
        //search enemy
        timerSpawner = Time.time + spawnRate;
        GameObject spwn = enemies[Random.Range(0, enemies.Count)].enemy;


        Vector3 spwnPosicion = new Vector3(0, Random.Range(-objectSize.y * 0.5f, objectSize.y * 0.5f), 0);
        return Instantiate(spwn, transform.position + spwnPosicion, transform.rotation);
    }

    public void PlayTickSFX()
    {
        audioSource.PlayOneShot(onTickSpawn, 0.25f);
    }
}
