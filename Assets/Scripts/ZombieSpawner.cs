using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject zombiePrefab;
    public DayNightManager dayNightManager;

    [Header("Base Spawn Settings")]
    public int baseMaximumZombies = 10;
    public int zombiesAddedPerDay = 5;
    public float baseSpawnInterval = 5f;
    public float minimumSpawnInterval = 1f;
    public float intervalReductionPerDay = 0.25f;

    [Header("Spawn Areas")]
    public BoxCollider[] spawnAreas;

    [Header("NavMesh")]
    public float navMeshSearchDistance = 10f;
    public int spawnAttemptsPerArea = 20;

    [Header("Position")]
    public float spawnHeightOffset = 0f;

    private int aliveZombieCount;
    private int lastNightDay = -1;

    private void Start()
    {
        if (zombiePrefab == null)
        {
            Debug.LogError("ZombieSpawner has no zombie prefab assigned.");
            return;
        }

        if (dayNightManager == null)
        {
            dayNightManager = FindFirstObjectByType<DayNightManager>();
        }

        if (dayNightManager == null)
        {
            Debug.LogError("ZombieSpawner could not find a DayNightManager.");
            return;
        }

        if (spawnAreas == null || spawnAreas.Length == 0)
        {
            Debug.LogError("ZombieSpawner has no spawn areas assigned.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (dayNightManager.IsNight)
            {
                int currentDay = dayNightManager.currentDay;

                if (lastNightDay != currentDay)
                {
                    lastNightDay = currentDay;

                    Debug.Log(
                        "Night " + currentDay +
                        " started. Maximum zombies: " +
                        GetMaximumZombiesForDay(currentDay)
                    );
                }

                int maximumZombies =
                    GetMaximumZombiesForDay(currentDay);

                if (aliveZombieCount < maximumZombies)
                {
                    SpawnZombie();
                }

                yield return new WaitForSeconds(
                    GetSpawnIntervalForDay(currentDay)
                );
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private int GetMaximumZombiesForDay(int day)
    {
        return baseMaximumZombies +
               ((day - 1) * zombiesAddedPerDay);
    }

    private float GetSpawnIntervalForDay(int day)
    {
        float interval =
            baseSpawnInterval -
            ((day - 1) * intervalReductionPerDay);

        return Mathf.Max(minimumSpawnInterval, interval);
    }

    private void SpawnZombie()
    {
        if (!TryGetSpawnPosition(out Vector3 spawnPosition))
        {
            Debug.LogWarning(
                "Could not find a valid zombie spawn position."
            );

            return;
        }

        spawnPosition += Vector3.up * spawnHeightOffset;

        Quaternion randomRotation = Quaternion.Euler(
            0f,
            Random.Range(0f, 360f),
            0f
        );

        GameObject zombieObject = Instantiate(
            zombiePrefab,
            spawnPosition,
            randomRotation
        );

        ZombieAI zombieAI =
            zombieObject.GetComponent<ZombieAI>();

        if (zombieAI != null)
        {
            zombieAI.SetSpawner(this);
        }

        aliveZombieCount++;
    }

    private bool TryGetSpawnPosition(
        out Vector3 spawnPosition)
    {
        int startingAreaIndex =
            Random.Range(0, spawnAreas.Length);

        for (int areaOffset = 0;
             areaOffset < spawnAreas.Length;
             areaOffset++)
        {
            int areaIndex =
                (startingAreaIndex + areaOffset) %
                spawnAreas.Length;

            BoxCollider area = spawnAreas[areaIndex];

            if (area == null || !area.enabled)
            {
                continue;
            }

            Bounds bounds = area.bounds;

            for (int attempt = 0;
                 attempt < spawnAttemptsPerArea;
                 attempt++)
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(
                        bounds.min.x,
                        bounds.max.x
                    ),
                    bounds.max.y,
                    Random.Range(
                        bounds.min.z,
                        bounds.max.z
                    )
                );

                if (NavMesh.SamplePosition(
                        randomPoint,
                        out NavMeshHit hit,
                        navMeshSearchDistance,
                        NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;
                    return true;
                }
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    public void NotifyZombieDied()
    {
        aliveZombieCount--;

        if (aliveZombieCount < 0)
        {
            aliveZombieCount = 0;
        }
    }
}