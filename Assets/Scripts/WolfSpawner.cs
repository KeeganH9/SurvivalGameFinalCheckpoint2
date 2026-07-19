using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WolfSpawner : MonoBehaviour
{
    [Header("Wolf")]
    public GameObject wolfPrefab;

    [Header("Spawn Limits")]
    public int startingWolves = 10;
    public int maximumWolves = 20;
    public float spawnInterval = 5f;

    [Header("Spawn Areas")]
    public BoxCollider[] spawnAreas;

    [Header("NavMesh")]
    public float navMeshSearchDistance = 10f;
    public int spawnAttemptsPerArea = 20;

    [Header("Position")]
    public float spawnHeightOffset = 0f;

    private int aliveWolvesCount;

    private void Start()
    {   
        Debug.Log("SpawnWolf was called.");
        if (wolfPrefab == null)
        {
            Debug.LogError("WolfSpawner has no wolf prefab assigned.");
            return;
        }

        if (spawnAreas == null || spawnAreas.Length == 0)
        {
            Debug.LogError("WolfSpawner has no spawn areas assigned.");
            return;
        }

        for (int i = 0; i < startingWolves; i++)
        {
            SpawnWolf();
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (aliveWolvesCount < maximumWolves)
            {
                SpawnWolf();
            }
        }
    }

    private void SpawnWolf()
{
    Debug.Log("SpawnWolf was called.");

    if (aliveWolvesCount >= maximumWolves)
    {
        Debug.LogWarning(
            "Wolf was not spawned because the maximum wolf count was reached."
        );

        return;
    }

    if (wolfPrefab == null)
    {
        Debug.LogError("Wolf prefab is missing.");
        return;
    }

    if (!TryGetSpawnPosition(out Vector3 spawnPosition))
    {
        Debug.LogError(
            "Wolf was not spawned because no valid NavMesh position was found."
        );

        return;
    }

    Debug.Log("Wolf spawn position found: " + spawnPosition);

    spawnPosition += Vector3.up * spawnHeightOffset;

    Quaternion randomRotation = Quaternion.Euler(
        0f,
        Random.Range(0f, 360f),
        0f
    );

    GameObject wolfObject = Instantiate(
        wolfPrefab,
        spawnPosition,
        randomRotation
    );

    if (wolfObject == null)
    {
        Debug.LogError("Instantiate failed to create the wolf.");
        return;
    }

    wolfObject.SetActive(true);

    Debug.Log(
        "Wolf successfully created: " +
        wolfObject.name +
        " at " +
        wolfObject.transform.position
    );

    WolfAI wolfAI = wolfObject.GetComponent<WolfAI>();

    if (wolfAI != null)
    {
        wolfAI.SetSpawner(this);
    }
    else
    {
        Debug.LogWarning(
            "Spawned wolf does not have WolfAI on its root object."
        );
    }

    aliveWolvesCount++;
}

   private bool TryGetSpawnPosition(out Vector3 spawnPosition)
{
    int startingAreaIndex = Random.Range(0, spawnAreas.Length);

    for (int areaOffset = 0;
         areaOffset < spawnAreas.Length;
         areaOffset++)
    {
        int areaIndex =
            (startingAreaIndex + areaOffset) % spawnAreas.Length;

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
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.center.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (NavMesh.SamplePosition(
                    randomPoint,
                    out NavMeshHit hit,
                    navMeshSearchDistance,
                    NavMesh.AllAreas))
            {
                spawnPosition = hit.position;

                Debug.Log(
                    "Valid wolf spawn position found at: " +
                    spawnPosition
                );

                return true;
            }
        }
    }

    Debug.LogWarning(
        "No NavMesh was found underneath any wolf spawn area."
    );

    spawnPosition = Vector3.zero;
    return false;
}

    public void NotifyWolfDied()
    {
        aliveWolvesCount--;

        if (aliveWolvesCount < 0)
        {
            aliveWolvesCount = 0;
        }
    }
}