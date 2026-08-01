using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationHelper), typeof(Rigidbody2D))]
public class Boss : MonoBehaviour
{
    public Vector2 DirToPlayer => (player.transform.position - transform.position).normalized;

    [SerializeField] protected BossData bossData;
    [SerializeField] bool isAttacking= false;
    [SerializeField] AnimationHelper AnimHelper;
    [SerializeField] BossPhase currentPhase;
    [SerializeField] Hitbox selfHitbox;
    [SerializeField] LayerMask WallLayer;

    event Action EWallHit;
    Rigidbody2D rb;

    //protected Dictionary<AttackData, Func<IEnumerator>> attackMap = new();
    float timeSinceLastAttackEnd = 0f;
    [SerializeField]ConditionAttackRuntime currentAttack; 

    Player player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = Player.Instance;
        currentPhase = bossData.Phases[0];
        Charge(GetAttack());
    }

    AttackData GetAttack()
    {
        return currentPhase.Attacks.Choice();
    }

    private void Update()
    {
        if(currentAttack!= null)
        {
            currentAttack.Tick();
        }
    }

    public virtual ConditionAttackRuntime StartAttack(AttackData attackData, Func<bool> endFunc)
    {
        isAttacking = true;
        currentAttack = new ConditionAttackRuntime(attackData, endFunc);

        Vector2 effectPos = transform.position;
        EffectContext context = new EffectContext(gameObject, player.gameObject, effectPos, (Vector3)DirToPlayer);
        foreach (var effect in attackData.OnAttackStartEffects) effect.Apply(context);


        currentAttack.EAttackFinish += OnAttackFinish;
        AnimHelper.ChangeAnimation(attackData.AttackAnimation, priority: attackData.AnimationPriority, forceReplay: true);

        return currentAttack;
    }

    public virtual void OnAttackFinish()
    {
        currentAttack.EAttackFinish -= OnAttackFinish;

        Vector2 effectPos = transform.position;
        EffectContext context = new EffectContext(gameObject, null, effectPos, (Vector3)DirToPlayer);
        foreach (var effect in currentAttack.Data.OnAttackEndEffects) effect.Apply(context);

        timeSinceLastAttackEnd = 0f;
        currentAttack = null;
        isAttacking = false;

        AnimHelper.ChangeAnimation(bossData.IdleAnimation);


    }

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {
        if (currentAttack == null) return;
        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p, dir);

        foreach (Effect effect in currentAttack.Data.OnTargetHitEffects)
            effect.Apply(context);

        if(collider.gameObject.IsInLayerMask(WallLayer))
        {
            EWallHit?.Invoke();
        }
    }


    private void OnEnable()
    {
        if (selfHitbox != null)
            selfHitbox.EOnHitDetect += NotifyHit;
    }

    private void OnDisable()
    {
        if (selfHitbox != null)
            selfHitbox.EOnHitDetect -= NotifyHit;
    }

    protected virtual void Charge(AttackData data)
    {
        ConditionAttackRuntime atk= StartAttack(data, null);
        EWallHit += atk.SignalCompletion; //attack will complete when it hits a wall

        atk.EAttackFinish += Cleanup;




        void Cleanup()
        {
            EWallHit -= atk.SignalCompletion;
            atk.EAttackFinish -= Cleanup;
        }

    }


}
