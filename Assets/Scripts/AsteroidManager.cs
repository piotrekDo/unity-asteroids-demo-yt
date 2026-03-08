using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AsteroidManager : MonoBehaviour {

    [SerializeField] private int m_startingAsteroids;
    [SerializeField] private int m_maximumAsteroids;
    [SerializeField] private float m_spawnDeley;
    [SerializeField] private List<GameObject> m_asteroidPrefarbs;
    [SerializeField] private Rect m_spawnArea;

    private int m_current_AsteroidCount;

    private IEnumerator AsteroidSpawner() {
        while (m_current_AsteroidCount > m_maximumAsteroids) {
            yield return new WaitForEndOfFrame();
        }

        SpawnRandomAsteroid(3, GetSpawnPointRandom());
        yield return new WaitForSeconds(0.1f);
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

        if (size == 3)
            m_current_AsteroidCount++;
    }

    private void onAsteroidDie(AsteroidController asteroid) {
        int size = asteroid.Size - 1;
        Vector2 asteroidPoint = asteroid.transform.position;
        Destroy(asteroid.gameObject);
        if(size == 3) m_current_AsteroidCount--;

        if (size > 0) {
            SpawnRandomAsteroid(size, (Random.insideUnitCircle * 5f) + asteroidPoint);
            SpawnRandomAsteroid(size, (Random.insideUnitCircle * 5f) + asteroidPoint);
        }
    }

    private IEnumerator SpawnInitialAsteroids() {
        for (int i = 0; i < m_startingAsteroids; ++i) {
            yield return new WaitForSeconds(m_spawnDeley);

            SpawnRandomAsteroid(3, GetSpawnPointRandom());
        }

        StartCoroutine(AsteroidSpawner());
    }

    private void Start() {
        StartCoroutine(SpawnInitialAsteroids());
    }


    private Vector2 GetSpawnPointRandom() {
        return new Vector2(
        Random.Range(m_spawnArea.xMin, m_spawnArea.xMax),
        Random.Range(m_spawnArea.yMin, m_spawnArea.yMax));
    }
}
