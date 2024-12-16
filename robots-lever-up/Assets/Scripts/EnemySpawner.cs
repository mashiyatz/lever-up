using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector2Int xRange;
    [SerializeField] private Vector2Int zRange;
    [SerializeField] private int maxNumEnemies;
    [SerializeField] private float spawnInterval;

    private void OnEnable()
    {
        GameManager.OnPlay += () => InvokeRepeating(nameof(SpawnNewEnemy), 5.0f, spawnInterval); 
    }

    private void OnDisable()
    {
        GameManager.OnPlay -= () => InvokeRepeating(nameof(SpawnNewEnemy), 5.0f, spawnInterval);
    }

    void SpawnNewEnemy()
    {
        if (transform.childCount >= maxNumEnemies) return;
        Vector3 pos = new Vector3(Random.Range(xRange.x,xRange.y), 5.0f, Random.Range(zRange.x, zRange.y)); 
        Instantiate(enemyPrefab, pos, Quaternion.identity, transform);
    }
}
