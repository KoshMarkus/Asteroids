using System.Collections;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{

    [SerializeField] int nextQuantityOfAsteroids;

    [SerializeField] GameObject alienPrefab;
    [SerializeField] float minAlienSpawnTime;
    [SerializeField] float maxAlienSpawnTime;

    private bool asteroidsSpawning = false;
    private int currentAsteroidsQuantity;

    private bool alienSpawning = false;
    private bool isThereAnAlien;
    private float alienSpawnRate;

    public static ObjectSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (currentAsteroidsQuantity == 0 && !asteroidsSpawning)
        {
            StartCoroutine(SpawnAsteroids());
        }

        if (!isThereAnAlien && !alienSpawning)
        {
            StartCoroutine(SpawnAlien());
        }
    }

    private Vector3 RollRandomEdgePosition(float min, float max, bool isAlien)
    {
        Vector3 spawnPosition;

        int chance;

        if (isAlien)
        {
            chance = Random.Range(1, 3);
        }
        else
        {
            chance = Random.Range(1, 5);
        }

        switch (chance)
        {
            case 1:
                spawnPosition = new Vector3(1, Random.Range(min, max), 0);
                break;
            case 2:
                spawnPosition = new Vector3(0, Random.Range(min, max), 0);
                break;
            case 3:
                spawnPosition = new Vector3(Random.Range(min, max), 1, 0);
                break;
            case 4:
                spawnPosition = new Vector3(Random.Range(min, max), 0, 0);
                break;
            default:
                spawnPosition = Vector3.zero;
                break;
        }

        return spawnPosition;
    }

    private IEnumerator SpawnAlien()
    {
        alienSpawning = true;

        alienSpawnRate = Random.Range(minAlienSpawnTime, maxAlienSpawnTime);

        yield return new WaitForSeconds(alienSpawnRate);

        Vector3 alienPosition = Camera.main.ViewportToWorldPoint(RollRandomEdgePosition(0.2f, 0.8f, true)) - Camera.main.transform.position;

        GameObject alienShip = Instantiate(alienPrefab, alienPosition , Quaternion.identity);
        alienShip.GetComponent<Alien>().SetPath(alienPosition);

        isThereAnAlien = true;

        alienSpawning = false;
    }

    private IEnumerator SpawnAsteroids()
    {
        asteroidsSpawning = true;

        yield return new WaitForSeconds(2);

        for (int i = 0; i < nextQuantityOfAsteroids; i++)
        {
            GameObject asteroid = ObjectPooler.Instance.SpawnFromPool("Asteroids", Camera.main.ViewportToWorldPoint(RollRandomEdgePosition(0.0f, 0.1f, false)) - Camera.main.transform.position, Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
            asteroid.GetComponent<Asteroid>().PushSetup(0);
        }

        nextQuantityOfAsteroids++;

        asteroidsSpawning = false;
    }

    public void ThereIsOneMoreAsteroid(bool isIt)
    {
        if (isIt)
        {
            currentAsteroidsQuantity++;
        }
        else
        {
            currentAsteroidsQuantity--;
        }
    }

    public void AlienDied()
    {
        isThereAnAlien = false;
    }
}
