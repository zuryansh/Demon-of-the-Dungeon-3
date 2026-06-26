using EditorAttributes;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    enum SpawnType { InCircle, InRoom }
    bool spawnInCircle => spawnType == SpawnType.InCircle;

    [SerializeField] SpawnType spawnType;
    [SerializeField, HideField(nameof(spawnInCircle))] Room parentRoom;
    [SerializeField, ShowField(nameof(spawnInCircle))] float radius;
    [SerializeField] int currentEnemyCount;
    [SerializeField] List<EnemyBrain> enemyPrefabList;

    [SerializeField] bool autoCalcPoints;
    [SerializeField, HideField(nameof(autoCalcPoints))] int availablePoints = 30;
    [SerializeField, HideField(nameof(autoCalcPoints))] int maxEnemyCount = 10;
    [SerializeField] bool SpawnContinously;

    List<EnemyBrain> enemies = new List<EnemyBrain>();


    private void Update()
    {
        if(currentEnemyCount ==0 && SpawnContinously) SpawnEnemies();
    }

    [Button("Spawn")]
    public void SpawnEnemies()
    {
        if (autoCalcPoints)
        {
            CalcPointPools();
        }


        while (availablePoints >= 0 && currentEnemyCount <= maxEnemyCount)
        {
            enemies.Add(SpawnEnemy(enemyPrefabList[Random.Range(0, enemyPrefabList.Count - 1)]));
        }
    }

    EnemyBrain SpawnEnemy(EnemyBrain enemyObj)
    {
        Vector2 pos = GetSpawnPosition();

        EnemyBrain enemy = Instantiate(enemyObj, pos, Quaternion.identity);
        enemy.EOnDeath += OnEnemyDeath;
        availablePoints -= enemy.Data.PointCost;
        currentEnemyCount++;
        return enemy;
    }

    void CalcPointPools()
    {
        if(spawnType == SpawnType.InRoom)
        {
            //availablePoints = parentRoom.RoomTiles.Count / 20 + 10;
            //maxEnemyCount = parentRoom.RoomTiles.Count / 100 + 5;
            Debug.LogError("NOT IMPLEMETED YET");
        }
        else if(spawnType == SpawnType.InCircle)
        {
            availablePoints = (int)(radius * radius * 3.14f / 20) + 10;
            maxEnemyCount = (int)(radius * radius * 3.14f / 100) + 5;
        }
    }

    private Vector2 GetSpawnPosition(bool realtiveToSpawner = true)
    {
        Vector2 pos = Vector2.negativeInfinity;
        if (spawnType == SpawnType.InRoom)
        {
            //int n = Random.Range(0, parentRoom.RoomTiles.Count);
            // pos = parentRoom.RoomTiles.AtIndex<Vector2Int>(n);
        }
        else if (spawnType == SpawnType.InCircle)
        {
            pos = Random.insideUnitCircle * radius;
            if (realtiveToSpawner) pos += (Vector2)transform.position;
        }
        if(pos == Vector2.negativeInfinity) { Debug.LogError("unable to find pos"); }
        return pos;
    }

    void OnEnemyDeath(EnemyBrain enemy)
    {
        enemies.Remove(enemy);
        currentEnemyCount--;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(Selection.Contains(gameObject))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
#endif
}