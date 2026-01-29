using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{

    int currentWaveIndex; 
    int currentWaveSpawnCount = 0; 
    List<GameObject> existingSpawns = new List<GameObject>();

    public WaveData[] data;
    public Camera referenceCamera;

    [Tooltip("If there are more than this number of enemies, stop spawning any more. For performance.")]
    public int maximumEnemyCount = 300;
    float spawnTimer; 
    float currentWaveDuration = 0f;
    public bool boostedByCurse = true;

    public static SpawnManager instance;

    void Start()
    {
        if (instance) Debug.LogWarning("There is more than 1 Spawn Manager in the Scene! Please remove the extras.");
        instance = this;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;

        if (spawnTimer <= 0)
        {
            if (HasWaveEnded())
            {
                currentWaveIndex++;
                currentWaveDuration = currentWaveSpawnCount = 0;

                if (currentWaveIndex >= data.Length)
                {
                    Debug.Log("All waves have been spawned! Shutting down.", this);
                    enabled = false;
                }

                return;
            }

            if (!CanSpawn())
            {
                ActivateCooldown();
                return;
            }

            GameObject[] spawns = data[currentWaveIndex].GetSpawns(EnemyStats.count);

            foreach (GameObject prefab in spawns)
            {
                if (!CanSpawn()) continue;

                existingSpawns.Add( Instantiate(prefab, GeneratePosition(), Quaternion.identity) );
                currentWaveSpawnCount++;
            }

            ActivateCooldown();
        }
    }

    public void ActivateCooldown()
    {
        float curseBoost = boostedByCurse ? GameManager.GetCumulativeCurse() : 1;
        spawnTimer += data[currentWaveIndex].GetSpawnInterval() / curseBoost;
    }

    public bool CanSpawn()
    {
        if (HasExceededMaxEnemies()) return false;

        if (instance.currentWaveSpawnCount > instance.data[instance.currentWaveIndex].totalSpawns) return false;

        if (instance.currentWaveDuration > instance.data[instance.currentWaveIndex].duration) return false;
        return true;
    }

    public static bool HasExceededMaxEnemies()
    {
        if (!instance) return false; 
        if (EnemyStats.count > instance.maximumEnemyCount) return true;
        return false;
    }

    public bool HasWaveEnded()
    {
        WaveData currentWave = data[currentWaveIndex];

        if ((currentWave.exitConditions & WaveData.ExitCondition.waveDuration) > 0)
            if (currentWaveDuration < currentWave.duration) return false;

        if ((currentWave.exitConditions & WaveData.ExitCondition.reachedTotalSpawns) > 0)
            if (currentWaveSpawnCount < currentWave.totalSpawns) return false;

        existingSpawns.RemoveAll(item => item == null);
        if (currentWave.mustKillAll && existingSpawns.Count > 0)
            return false;

        return true;
    }

    void Reset()
    {
        referenceCamera = Camera.main;
    }

  public static Vector3 GeneratePosition()
{
    if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

    if (!instance.referenceCamera.orthographic)
        Debug.LogWarning("The reference camera is not orthographic! This will cause enemy spawns to sometimes appear within camera boundaries!");

    float x = Random.Range(0f, 1f), y = Random.Range(0f, 1f);

    Vector3 viewportPos;

    switch (Random.Range(0, 2))
    {
        case 0:
        default:
            viewportPos = new Vector3(Mathf.Round(x), y, instance.referenceCamera.nearClipPlane);
            break;
        case 1:
            viewportPos = new Vector3(x, Mathf.Round(y), instance.referenceCamera.nearClipPlane);
            break;
    }

    Vector3 worldPos = instance.referenceCamera.ViewportToWorldPoint(viewportPos);

    worldPos.z = 0;
    return worldPos;
}

    public static bool IsWithinBoundaries(Transform checkedObject)
    {
        Camera c = instance && instance.referenceCamera ? instance.referenceCamera : Camera.main;

        Vector2 viewport = c.WorldToViewportPoint(checkedObject.position);
        if (viewport.x < 0f || viewport.x > 1f) return false;
        if (viewport.y < 0f || viewport.y > 1f) return false;
        return true;
    }
}