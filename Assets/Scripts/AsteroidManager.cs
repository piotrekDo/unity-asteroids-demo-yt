using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AsteroidManager : MonoBehaviour {

    private static int ASTEROID_BIG = 3;
    private static int ASTEROID_MID = 2;
    private static int ASTEROID_MIN = 1;

    [SerializeField] private int m_startingAsteroids;
    [SerializeField] private int m_maximumAsteroids;
    [SerializeField] private float m_spawnDeley;
    [SerializeField] private List<GameObject> m_asteroidPrefarbs;
    [SerializeField] private Rect m_spawnArea;

    private int m_current_AsteroidCount;

    private void Start() {
        StartCoroutine(SpawnInitialAsteroids());
    }

    private IEnumerator AsteroidSpawner() {
        while (m_current_AsteroidCount >= m_maximumAsteroids) {
            yield return new WaitForEndOfFrame();
        }

        SpawnRandomAsteroid(3, GetSpawnPointOutside());
        yield return new WaitForSeconds(m_spawnDeley);
        StartCoroutine(AsteroidSpawner());
    }

    private IEnumerator SpawnInitialAsteroids() {
        for (int i = 0; i < m_startingAsteroids -1; ++i) {
            yield return new WaitForSeconds(0.1f);

            SpawnRandomAsteroid(ASTEROID_BIG, GetSpawnPointOutside());
        }

        StartCoroutine(AsteroidSpawner());
    }

    private void SpawnRandomAsteroid(int size, Vector2 spawnPoint) {
        IEnumerable<GameObject> sizePrefabs = m_asteroidPrefarbs.Where(x => {
            AsteroidController controller = x.GetComponent<AsteroidController>();
            return controller != null && controller.Size == size;
        });

        if (sizePrefabs == null || !sizePrefabs.Any()) {
            return;
        }

        int index = Random.Range(0, sizePrefabs.Count());
        GameObject asteroidToSpawn = Instantiate(sizePrefabs.ElementAt(index), transform);

        asteroidToSpawn.transform.position = spawnPoint;
        AsteroidController spawnedController = asteroidToSpawn.GetComponent<AsteroidController>();
        spawnedController.onAsteroidDie += onAsteroidDie;

        if (size == ASTEROID_BIG)
            m_current_AsteroidCount++;
    }


    private void onAsteroidDie(AsteroidController asteroid) {
        int newAsteroidSize = asteroid.Size - 1;
        Vector2 asteroidPoint = asteroid.transform.position;
        Destroy(asteroid.gameObject);
        GameEvents.Instance.AddToScore(1);
        if (asteroid.Size == ASTEROID_BIG)
            m_current_AsteroidCount--;

        int newAsteroidsCount = 0;
        if (newAsteroidSize == ASTEROID_MID) {
            newAsteroidsCount = Random.Range(2, 4);
        } else if (newAsteroidSize == ASTEROID_MIN) {
            newAsteroidsCount = Random.Range(2, 6);
        }

        for (int i = 0; i < newAsteroidsCount; i++) {
            SpawnRandomAsteroid(newAsteroidSize, (Random.insideUnitCircle * 5f) + asteroidPoint);
        }
    }

    private Vector2 GetSpawnPointOutside() {
        float offset = 2f;

        Camera cam = Camera.main;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        Vector2 center = cam.transform.position;

        float xMin = center.x - width;
        float xMax = center.x + width;
        float yMin = center.y - height;
        float yMax = center.y + height;

        int side = Random.Range(0, 4);

   
        switch (side) {
            case 0: // top
                return new Vector2(Random.Range(xMin, xMax), yMax + offset);

            case 1: // bottom
                return new Vector2(Random.Range(xMin, xMax), yMin - offset);

            case 2: // left
                return new Vector2(xMin - offset, Random.Range(yMin, yMax));

            default: // right
                return new Vector2(xMax + offset, Random.Range(yMin, yMax));
        }
    }
}
