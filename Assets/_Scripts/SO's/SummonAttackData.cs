using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack/ SummonAttack")]
public class SummonAttackData : AttackData
{
    [SerializeField] List<GameObject> objects = new();
    [SerializeField] Vector2Int minMaxSpawn;
    [SerializeField] Vector2 spawnBounds;
    [SerializeField] bool hasGlobalPosition;
    [SerializeField, ShowField(nameof(hasGlobalPosition))] Vector2 globalPosition;
    bool summonsMinions = false;

    public List<GameObject> Objects { get => objects; }
    public Vector2Int MinMaxSpawn { get => minMaxSpawn; }
    public Vector2 SpawnBounds => spawnBounds;
    public Vector2 GlobalSpawnPosition { get => globalPosition; }
    public bool HasGlobalPosition { get => hasGlobalPosition; }
    public bool SummonsMinions => summonsMinions;

    public GameObject GetRandomObj() { return objects.Choice(); }
    public int GetRandomCount()=> UnityEngine.Random.Range(minMaxSpawn.x, minMaxSpawn.y);

    public void OnValidate()
    {

        if (objects != null&& objects.Count > 0)
        {
            if (objects[0].TryGetComponent(out EnemyBrain _))
            {
                summonsMinions = true;
                return;
            }
        }
        summonsMinions = false;
    }
}
