using UnityEngine;


public  abstract class EnemyAttackModule : MonoBehaviour
{
    public EnemyBrain Brain;
    public AnimationHelper AnimHelper => Brain.AnimationHelper;

    [SerializeField] protected AttackData attackData;
    [SerializeField] protected AttackRuntime currentAttack;
    [SerializeField] protected bool isAttacking;
    [SerializeField] protected Hitbox attackHitbox;
    
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
    }


    public virtual void StartAttack()
    {
        isAttacking = true;
        currentAttack = new AttackRuntime(attackData, Time.time, AnimHelper.Anim);

        currentAttack.EAttackFinish += OnAttackFinish;
        AnimHelper.ChangeAnimation(currentAttack.Data.AttackAnimation, priority: currentAttack.Data.AnimationPriority, forceReplay:true);
    }

    public virtual void OnAttackFinish()
    {
        currentAttack.EAttackFinish -= OnAttackFinish;

        timeSinceLastAttack = Time.time;

        currentAttack = null;
        isAttacking = false;
        AnimHelper.ChangeAnimation(Brain.Data.IdleAnim);  
    }

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p, dir);

        foreach (Effect effect in currentAttack.Data.Effects)
            effect.Apply(context);
    }

    private void OnEnable()
    {
        attackHitbox.EOnHitDetect += NotifyHit;
    }
    private void OnDisable()
    {
        attackHitbox.EOnHitDetect -= NotifyHit;
    }

}
