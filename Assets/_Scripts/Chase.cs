using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    }

    // Update is called once per frame
    public override void Tick()
    {
        base.Tick();

        desiredDir = Vector2.zero;

        


        if (Brain.SqrDistToPlayer < sightRange * sightRange && Brain.SqrDistToPlayer >= stoppingRange*stoppingRange)
        {
            desiredDir += Brain.DirToPlayer;
        }

        if (Brain.SqrDistToPlayer > stoppingRange * stoppingRange) ApplySeperation(); // dont aply if enemy super close to player
        MoveInDir(desiredDir.normalized, speed);
        
        CheckAnimation();
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

    private void OnDrawGizmos()
    {
        if (Selection.Contains(gameObject))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stoppingRange);
        }
    }

}
