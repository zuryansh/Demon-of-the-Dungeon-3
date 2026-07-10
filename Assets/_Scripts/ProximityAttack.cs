using UnityEngine;
using DG.Tweening;
using UnityEditor.Tilemaps;

public class ProximityAttack : EnemyAttackModule
{
    public override bool CanAttack => !isAttacking &&
        !isStunned && 
        Brain.HasLOS &&
        timeSinceLastAttack > timeBetweenAttacks;

    [SerializeField] float maxAttackRange;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] LookAtObj HitboxLookScript;
    [SerializeField] bool jumpTowardsTarget;

    public override void Init()
    {
        base.Init();
        if(HitboxLookScript!= null) HitboxLookScript.target = Brain.Player.gameObject;
    }

    public override void Tick()
    {
        base.Tick();
        if(SqrDistToPlayer <= maxAttackRange*maxAttackRange)
        {
            if (CanAttack)
            {
                StartAttack();
            }
        }
        
    }

    public override void StartAttack()
    {
        if (!CanAttack) return;
        
        base.StartAttack();
        AnimatorStateInfo animatorStateInfo = Brain.AnimationHelper.Anim.GetCurrentAnimatorStateInfo(0);

        if(jumpTowardsTarget )transform.DOMove(transform.position + (Vector3)(DirToPlayer*maxAttackRange/2), animatorStateInfo.length);
    }



    public override void OnAttackFinish()
    {
        base.OnAttackFinish();

    }

}
