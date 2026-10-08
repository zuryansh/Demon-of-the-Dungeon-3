using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack/ SummonAttack")]
public class SummonAttackData : AttackData
{
    [SerializeField] List<EnemyBrain> minions = new();
    [SerializeField] Vector2Int minMaxSpawn;
    [SerializeField] Vector2 spawnBounds;
    [SerializeField] bool hasGlobalPosition;
    [SerializeField, ShowField(nameof(hasGlobalPosition))] Vector2 globalPosition;

    public List<EnemyBrain> Minions { get => minions; }
    public Vector2Int MinMaxSpawn { get => minMaxSpawn; }
    public Vector2 SpawnBounds => spawnBounds;
    public Vector2 GlobalSpawnPosition { get => globalPosition; }
    public bool HasGlobalPosition { get => hasGlobalPosition; }

    public EnemyBrain GetRandomMinion() { return minions.Choice(); }
    public int GetRandomCount()=> UnityEngine.Random.Range(minMaxSpawn.x, minMaxSpawn.y);
}
