using EditorAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface ISpawner
{
    void Spawn();
    public void OnSpawnRemoved(GameObject obj);
    public void AddObjToList(GameObject obj);
}

public class EnemySpawner : MonoBehaviour , ISpawner
{
    enum SpawnType { InCircle, InRoom }
    public event Action OnAllWavesDefeated;
    public bool Defeated => (wavesDefeated >= noOfWaves);

    [SerializeField] SpawnType spawnType;
    [SerializeField, HideField(nameof(spawnType), SpawnType.InCircle)] Room parentRoom;
    [SerializeField, ShowField(nameof(spawnType), SpawnType.InCircle)] float radius;
    [SerializeField] int currentEnemyCount;
    [SerializeField] List<EnemyBrain> enemyPrefabList;

    [SerializeField] bool autoCalcPoints;
    [SerializeField, HideField(nameof(autoCalcPoints))] int availablePoints = 30;
    [SerializeField, HideField(nameof(autoCalcPoints))] int maxEnemyCount = 10;
    [SerializeField] bool SpawnContinously;
    [SerializeField] int noOfWaves = 0;
    [SerializeField] int wavesDefeated ;
    [SerializeField] PopupText counterText;


    List<GameObject> enemies = new List<GameObject>();
    bool isCounting;
    List<RoomTile> validTiles;


    private void Start()
    {
        if(spawnType == SpawnType.InRoom)
        {
            enemyPrefabList = parentRoom.Data.GenSettings.Enemies;
        }
    }


    private void Update()
    {
        if (currentEnemyCount == 0 && SpawnContinously && !isCounting)
        {

            StartCoroutine(Countdown());
        }
    }

    


    IEnumerator Countdown()
    {
        isCounting = true;
        Instantiate(counterText, transform.position, Quaternion.identity).Init("2",2f, 0.8f, true, 0.2f);
        yield return new WaitForSeconds(1.2f);
        Instantiate(counterText, transform.position, Quaternion.identity).Init("1",2f, 0.8f, true, 0.2f);
        yield return new WaitForSeconds(1.2f);
        isCounting = false;
        Spawn();

    }

    void SpawnEnemies(Room rooom) => Spawn();

    [Button("Spawn")]
    public void Spawn()
    {
        if (!SpawnContinously && Defeated) return;

        if(spawnType == SpawnType.InRoom) validTiles = parentRoom.Data.GetTilesOfType(TileTypes.Floor);

        if (autoCalcPoints)
        {
            CalcPointPools();
        }


        while (availablePoints >= 0 && currentEnemyCount <= maxEnemyCount)
        {
            enemies.Add(SpawnEnemy(enemyPrefabList.Choice()).gameObject);
        }
    }

    EnemyBrain SpawnEnemy(EnemyBrain enemyObj)
    {
        Vector2 pos = GetSpawnPosition();

        EnemyBrain enemy = Instantiate(enemyObj, pos, Quaternion.identity);
        //enemy.EOnDeath += OnSpawnRemoved;
        enemy.SetSpawner(this);
        availablePoints -= enemy.Data.PointCost;
        currentEnemyCount++;
        return enemy;
    }

    void CalcPointPools()
    {
        if(spawnType == SpawnType.InRoom)
        {
            availablePoints = validTiles.Count / 20 + 10;
            maxEnemyCount = validTiles.Count / 100 + 5;
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
            
            int n = UnityEngine.Random.Range(0, validTiles.Count);
            pos = parentRoom.GetGlobalTilePos(validTiles[n]);
        }
        else if (spawnType == SpawnType.InCircle)
        {
            pos = UnityEngine.Random.insideUnitCircle * radius;
            if (realtiveToSpawner) pos += (Vector2)transform.position;
        }
        if(pos == Vector2.negativeInfinity) { Debug.LogError("unable to find pos"); }
        return pos;
    }

    public void OnSpawnRemoved(GameObject obj)
    {
        enemies.Remove(obj);
        currentEnemyCount--;
        if(currentEnemyCount == 0)
        {
            wavesDefeated++;
            if(wavesDefeated>= noOfWaves)
            {
                Debug.Log("ALL ENEMY WAVES DEFEATED!!!!!!!!");
                OnAllWavesDefeated?.Invoke();
            }
        }
    }

    private void OnEnable()
    {
        if (spawnType == SpawnType.InRoom)
        {
            if (parentRoom == null) Debug.LogError("No parent room assigned");
            parentRoom.EonPlayerEnter += SpawnEnemies;
        }
    }
    private void OnDisable()
    {
        if (spawnType == SpawnType.InRoom)
        {
            if (parentRoom == null) Debug.LogError("No parent room assigned");
            parentRoom.EonPlayerEnter -= SpawnEnemies;
        }
    }
    public void AddObjToList(GameObject obj)
    {
        enemies.Add(obj);
        currentEnemyCount++;
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