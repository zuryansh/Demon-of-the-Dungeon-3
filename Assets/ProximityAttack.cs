using UnityEngine;
using DG.Tweening;

public class ProximityAttack : EnemyAttackModule
{
    [SerializeField] float attackRange;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] bool canAttack = true;


    public override void Init()
    {
        base.Init();

    }

    public override void Tick()
    {
        base.Tick();
        if(Brain.SqrDistToPlayer <= attackRange*attackRange)
        {
            if (!isAttacking && canAttack)
            {
                StartAttack();
            }
        }
    }

    public override void StartAttack()
    {
        base.StartAttack();
        canAttack = false;
        AnimatorStateInfo animatorStateInfo = Brain.AnimationHelper.Anim.GetCurrentAnimatorStateInfo(0);

        transform.DOMove(transform.position + (Vector3)(Brain.DirToPlayer*attackRange/2), animatorStateInfo.length);
    }

    public override void OnAttackFinish()
    {
        base.OnAttackFinish();
        Invoke(nameof(ResetAttack), timeBetweenAttacks);

    }

    void ResetAttack() => canAttack = true;
}
