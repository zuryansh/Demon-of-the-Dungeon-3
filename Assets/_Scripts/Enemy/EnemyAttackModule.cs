using System.Collections;
using UnityEngine;


public  abstract class EnemyAttackModule : MonoBehaviour
{
    public EnemyBrain Brain;
    public AnimationHelper AnimHelper => Brain.AnimationHelper;
    public virtual bool CanAttack => !isAttacking && !isStunned && Brain.HasLOS ;

    protected virtual float SqrDistToPlayer => (Brain.LastPlayerPos - transform.position.ToV2()).sqrMagnitude;
    protected virtual Vector2 DirToPlayer => (Brain.LastPlayerPos - transform.position.ToV2()).normalized;

    [SerializeField] protected AttackData attackData;
    [SerializeField] protected AttackRuntime currentAttack;
    [SerializeField] protected bool isAttacking;
    [SerializeField] protected Hitbox attackHitbox;
    
    //[SerializeField] protected LayerMask obstacleLayer;
    [SerializeField]protected bool isStunned = false;
    protected float timeSinceLastAttack;


    public virtual void Init()
    {
    }

    public virtual void Tick()
    {
        if (isAttacking)
        {
            currentAttack.Tick();
        }
        
        //if (isStunned) canAttack = false;
        if (!isStunned) timeSinceLastAttack += Time.deltaTime ;
        else timeSinceLastAttack = 0f;

    }



    public virtual void StartAttack()
    {
        isAttacking = true;
        currentAttack = new AttackRuntime(attackData, Time.time, AnimHelper.Anim);
        timeSinceLastAttack = 0f;

        Vector2 effectPos = (Brain.EffectPoint != null)? Brain.EffectPoint.position : transform.position;
        EffectContext context = new EffectContext(gameObject, null, effectPos, (Vector3)DirToPlayer);
        foreach (var effect in attackData.OnAttackStartEffects) effect.Apply(context);


        currentAttack.EAttackFinish += OnAttackFinish;
        AnimHelper.ChangeAnimation(currentAttack.Data.AttackAnimation, priority: currentAttack.Data.AnimationPriority, forceReplay:true);
    }

    public virtual void OnAttackFinish()
    {
        currentAttack.EAttackFinish -= OnAttackFinish;


        currentAttack = null;
        isAttacking = false;

        Vector2 effectPos = (Brain.EffectPoint != null) ? Brain.EffectPoint.position : transform.position;
        EffectContext context = new EffectContext(gameObject, null, effectPos, (Vector3)DirToPlayer);
        foreach (var effect in attackData.OnAttackEndEffects) effect.Apply(context);

        AnimHelper.ChangeAnimation(Brain.Data.IdleAnim);
        

    }

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p, dir);

        foreach (Effect effect in currentAttack.Data.OnTargetHitEffects)
            effect.Apply(context);
    }

    private void OnEnable()
    {
        if(attackHitbox!= null) 
        attackHitbox.EOnHitDetect += NotifyHit;
    }
    private void OnDisable()
    {
        if (attackHitbox != null)
        attackHitbox.EOnHitDetect -= NotifyHit;
    }



    private void OnDrawGizmos()
    {

        //Gizmos.DrawRay(new Ray(transform.position, Brain.DirToPlayer*Brain.SqrDistToPlayer));
    }

    public virtual void Stun(float duration)
    {
        print("STunned");
        isStunned = true;
        Invoke(nameof(ResetStun), duration);
    }

    void ResetStun()
    {
        Debug.Log("REset");    
        isStunned = false;
    }

}
