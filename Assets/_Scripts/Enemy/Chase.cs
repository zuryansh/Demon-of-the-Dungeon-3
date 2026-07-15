using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chase : EnemyMovementModule
{


    [SerializeField] float sightRange;
    [SerializeField] float speed;
    [SerializeField] float stoppingRange;
    [SerializeField] float pushingRange = 1.2f;
    [SerializeField] Vector2 desiredPoint;
    [SerializeField] float seperationStrength = 2f;
    [SerializeField] bool DebugRanges = false;

    float ogSpeed;
    bool inRange;
    private readonly Collider2D[] nearbyEnemies = new Collider2D[16];
    ContactFilter2D enemyFilter;
    Vector2 desiredDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void  Init()
    {
        base.Init();
        enemyFilter = new ContactFilter2D
        {
            layerMask = gameObject.layer
        };
        ogSpeed = speed;
    }

    // Update is called once per frame
    public override void Tick()
    {
        base.Tick();

        //if (FlipSpriteAccToDir) FlipSprite();

        //desiredDir = Vector2.zero;


        //inRange = Brain.SqrDistToPlayer < sightRange * sightRange && Brain.SqrDistToPlayer >= stoppingRange * stoppingRange;

        //if (inRange)
        //{
        //    desiredDir += Brain.DirToPlayer;

        //}

        ChasePostition();

        if ((Brain.Player.position - transform.position).sqrMagnitude > stoppingRange * stoppingRange) ApplySeperation(); // dont aply if enemy super close to player
        MoveInDir(desiredDir.normalized, speed, ForceMode2D.Force);
        
        CheckAnimation();
    }
    
    void ChasePostition()
    {
        desiredDir = Vector2.zero;


        inRange = SqrDistToPlayer < sightRange * sightRange && SqrDistToPlayer >= stoppingRange * stoppingRange;

        if (Brain.HasLOS) //if has los chase normally
        {
            if (inRange) desiredDir += DirToPlayer;
        }
        else if(SqrDistToPlayer > 0.1f)desiredDir += DirToPlayer; //if dosent have Los the chase last position depsite being in range and not at that position
    }

    private void ApplySeperation()
    {
        

        int count = Physics2D.OverlapCircle(transform.position, pushingRange, enemyFilter, nearbyEnemies);

        Vector2 separation = Vector2.zero;

        for (int i = 0; i < count; i++)
        {
            Collider2D col = nearbyEnemies[i];

            if (col.gameObject == gameObject) continue;

            Vector2 away = transform.position.ToV2() - col.transform.position.ToV2();

            float sqrDist = away.sqrMagnitude;

            if (sqrDist < 0.0001f)
            {
                //chasePlayer = false;
                continue;
            }

            // Stronger repulsion when very close
            //Vector2 tangent = new Vector2(-away.y, away.x
            separation += away.normalized/sqrDist;
            //separation += tangent.normalized * 0.02f;
        }
        //if (!chasePlayer) desiredDir = (separation * seperationStrength);
        desiredDir += (separation * seperationStrength);
    }

    void CheckAnimation()
    {
        if (rb.linearVelocity.sqrMagnitude > 0) Brain.AnimationHelper.ChangeAnimation(Brain.Data.MovementAnim);
        else Brain.AnimationHelper.ChangeAnimation(Brain.Data.IdleAnim);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (DebugRanges)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stoppingRange);
        }
        if(Selection.Contains(gameObject) && Brain != null) Gizmos.DrawWireSphere(Brain.LastPlayerPos, 1f);
    }

#endif

    protected override void FlipSprite()
    {
        if (spriteRenderer == null) Debug.LogWarning("Sprite renderer not found");
        if (SqrDistToPlayer < sightRange * sightRange)
        {
            if (DirToPlayer.x > 0) spriteRenderer.flipX = false;
            else spriteRenderer.flipX = true;
        }


    }

    public override void Stun(float duration)
    {
        base.Stun(duration);
        speed = 0;
        CancelInvoke(nameof(ResetStun));
        Invoke(nameof(ResetStun), duration);
    }

    void ResetStun()
    {
        speed = ogSpeed;
    }
}
