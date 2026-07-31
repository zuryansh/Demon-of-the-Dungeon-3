using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnAttack : EnemyAttackModule
{
    public enum AttackTime { Start, End}
    public override bool CanAttack => !isAttacking &&
    !isStunned &&
    timeSinceLastAttack > timeBetweenAttacks
        && spawnedEnemies.Count<= maxEnemyCount;




    [SerializeField] float timeBetweenAttacks;
    [SerializeField] LookAtObj HitboxLookScript;
    [SerializeField] bool jumpTowardsTarget;
    [SerializeField] AttackTime attackTime;
    [SerializeField] EnemyBrain[] spawnableEnemies;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform rotationAnchor;
    [SerializeField] float spawnCheckRad=1f;
    [SerializeField] List<EnemyBrain> spawnedEnemies = new List<EnemyBrain>();
    [SerializeField] int maxEnemyCount;
    [SerializeField] bool spawnOnDeath;

    ContactFilter2D contactFilter;

    public override void Init()
    {
        base.Init();
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = Brain.ObstacleLayer;
        contactFilter.useLayerMask = true;
        Brain.EOnDeath += OnDeath;
    }

    public override void Tick()
    {
        base.Tick();
        
        
            if (CanAttack)
            {
                StartAttack();
            }

    }

    public override void StartAttack()
    {
        if (!CanAttack) return;
        if (CheckSpawnPos())
        {
            base.StartAttack();
            if (attackTime == AttackTime.Start)
            {
                Spawn();
            }
        }
        else Brain.Stun(0.1f);

    }



    bool CheckSpawnPos()
    {
        RaycastHit2D[] results = new RaycastHit2D[1];
        Vector2 dir = (spawnPoint.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, spawnPoint.position);

        if (Physics2D.CircleCast(transform.position, spawnCheckRad, dir ,this.contactFilter, results,distance:distance) > 0)
        {
            if (results.Length >0)
            {
                FlipY();
                return false;
            }
        }
        return true;
    }

    public override void OnAttackFinish()
    {
        base.OnAttackFinish();
        if (attackTime == AttackTime.End)
        {
            Spawn();
        }
    }

    

    public void Spawn(bool listenForDeath = true)
    {


        EnemyBrain enemy = Instantiate(spawnableEnemies.Choice(), spawnPoint.position, Quaternion.identity);
        
        enemy.SetSpawner(Brain.ParentSpawner);
        if (Brain.ParentSpawner != null) { Brain.ParentSpawner.AddObjToList(enemy.gameObject); print("assigned"); }

        spawnedEnemies.Add(enemy);
        if(listenForDeath) enemy.EOnDeath += OnSpawnRemoved;
        
    }

    public void OnSpawnRemoved(EnemyBrain enemy)
    {
        spawnedEnemies.Remove(enemy);
    }

    public void AddObjToList(GameObject obj)
    {
        Brain.ParentSpawner.AddObjToList(obj);
    }

    public void FlipY()
    {
        rotationAnchor.Rotate(0f, 180f, 0f, Space.Self);
    }

    void OnDeath(EnemyBrain enemy)
    {
        if (spawnOnDeath)
        {
            for (int i = 0; i < 3; i++)
            {
                Spawn(false);
            }
        }
    }

    private void OnDisable()
    {
        foreach (EnemyBrain enemy in spawnedEnemies)
        {
            enemy.EOnDeath -= OnSpawnRemoved;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnCheckRad);
        Gizmos.DrawWireSphere(spawnPoint.position, spawnCheckRad);
    }
}
