using UnityEngine;

public class EnemyRoom : Room
{
    public override bool RoomClear => enemySpawner.Defeated;

    [SerializeField] EnemySpawner enemySpawner;


    protected override void OnEnable()
    {
        base.OnEnable();

        if (enemySpawner != null)
        {
            enemySpawner.OnAllWavesDefeated += RoomCleared;
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (enemySpawner != null)
        {
            enemySpawner.OnAllWavesDefeated -= RoomCleared;
        }
    }
}
