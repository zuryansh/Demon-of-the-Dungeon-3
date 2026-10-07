using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AnimationHelper), typeof(Rigidbody2D))]
public class Boss : MonoBehaviour
{
    public Vector2 DirToPlayer => (player.transform.position - transform.position).normalized;
    public bool CanStartNewAttack => 
        (Time.time - lastAttackEndTime > currentPhase.TimeBetweenAttacks) && 
        (Time.time - attackRotationEndTime > currentPhase.TimeBetweenRotations);

    [SerializeField] protected BossData bossData;
    [SerializeField] protected AnimationHelper AnimHelper;
    [SerializeField] protected BossPhase currentPhase;
    [SerializeField] protected Hitbox[] hitBoxes;
    [SerializeField] protected LayerMask WallLayer;
    [SerializeField] List<AttackData> attacksForThisRotation = new List<AttackData>();

    protected event Action EWallHit;
    protected Rigidbody2D rb;

    protected Dictionary<Type, Func<AttackData, IEnumerator>> attackMap = new();
    protected float lastAttackEndTime = 0f;
    protected float attackStartTime=0f;
    protected float attackRotationEndTime = 0f;
    protected ConditionAttackRuntime currentAttack;  //Serialising this breaks it for some reason 
    protected bool isVulnerable=false;
    protected Health health;

    protected Player player;

    private void Awake()
    {

        RegisterBossAttacks();
        if(AnimHelper == null) AnimHelper = GetComponent<AnimationHelper>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        currentPhase = bossData.Phases[0];
    }

    protected virtual void RegisterBossAttacks() { }


    private void Start()
    {

        player = Player.Instance;
        //BeginNextAttack();


    }


    private void Update()
    {

        //if (currentAttack!= null)
        //{
        //    currentAttack.Tick();
        //}
        //else
        //{
        //    CheckPhaseChange(health.CurHealth, health.MaxHealth);

        //    if (CanStartNewAttack) //will be set false by phase change because it starts its own attack
        //    {
        //        //begin next attack
        //        BeginNextAttack();
        //    }
        //}
    }

    public void CheckPhaseChange(float currentHealth, float maxHealth)
    {
        float ratio = currentHealth / maxHealth;

        if(ratio<= currentPhase.PhaseEndPoint)
        {
            //Phase Change
            AnimHelper.ChangeAnimation(currentPhase.PhaseChangeAnim,priority:2);
            currentPhase = bossData.Phases[ bossData.Phases.IndexOf(currentPhase)+1];
            attacksForThisRotation.Clear();
            if (CanStartNewAttack) BeginNextAttack();
            
        }
    }

    public virtual void OnHit(EffectContext context)
    {
        foreach (Effect effect in bossData.OnHitEffects)
        {
            effect.Apply(context);
        }
    }

    AttackData GetAttack()
    {

        AttackData attack = attacksForThisRotation[0];
        attacksForThisRotation.RemoveAt(0);


        return attack;
    }

    protected void RegisterAttack<T>(Func<T, IEnumerator> attackFunc) where T : AttackData
    {
        attackMap.Add(typeof(T), data => attackFunc((T)data)); //lambda function where data is the paramenter and the rest is the func body
                                                               // eqv to Wrapper(AttackData data) { return attackFunc((T)data)} where T is the                                 generic type given 
    }

    public void BeginNextAttack()
    {
        if (attacksForThisRotation.Count == 0)
        {
            attacksForThisRotation = currentPhase.GetRandomAttacks();
            if (health.Vulnerable)
            {
                AnimHelper.ChangeAnimation(bossData.IdleAnim); //Change to restore animatiom
                health.SetVulnerability(false);
            }
            //return;
        }


        var attackData = GetAttack();
        if (!attackMap.TryGetValue(attackData.GetType(), out var attackFunc))
        {
            Debug.LogError($"No implementation registered for {attackData.GetType().Name}");
            return;
        }

        StartCoroutine( attackFunc(attackData));
    }

    public virtual ConditionAttackRuntime PrepareAttack(AttackData attackData, Func<bool> endFunc)
    {
        attackStartTime = Time.time;
        //foreach (Hitbox hitbox in hitBoxes) hitbox.ResetHitbox();
        currentAttack = new ConditionAttackRuntime(attackData, endFunc);

        currentAttack.EAttackFinish += OnAttackFinish;



        return currentAttack;
    }

    public void ExecuteAttackStart(AttackData attackData)
    {
        AnimHelper.ChangeAnimation(attackData.AttackAnimation, priority: attackData.AnimationPriority, forceReplay: true);

        Vector2 effectPos = transform.position;
        EffectContext context = new EffectContext(gameObject, player.gameObject, effectPos, (Vector3)DirToPlayer);
        foreach (var effect in attackData.OnAttackStartEffects) effect.Apply(context);



    }

    public virtual void OnAttackFinish()
    {
        currentAttack.EAttackFinish -= OnAttackFinish;

        Vector2 effectPos = transform.position;
        EffectContext context = new EffectContext(gameObject, null, effectPos, (Vector3)DirToPlayer);
        foreach (var effect in currentAttack.Data.OnAttackEndEffects) effect.Apply(context);

        lastAttackEndTime = Time.time;
        currentAttack = null;


        AnimHelper.ChangeAnimation(bossData.IdleAnim);
        if (attacksForThisRotation.Count == 0) Rest();

    }

    public void Rest()
    {
        AnimHelper.ChangeAnimation(bossData.WeakenedAnimation, priority: 1);
        attackRotationEndTime = Time.time;
        health.SetVulnerability(true);
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
        if (hitBoxes.Length > 0)
            foreach (Hitbox hitbox in hitBoxes) hitbox.EOnHitDetect += NotifyHit;
    }

    private void OnDisable()
    {
        if (hitBoxes.Length > 0)
            foreach (Hitbox hitbox in hitBoxes) hitbox.EOnHitDetect -= NotifyHit;
    }

    protected void Telegraph(AttackData attackData, Vector2 spawnPos, Vector2 direction)
    {
        Vector2 effectPos = spawnPos;
        EffectContext context = new EffectContext(gameObject, player.gameObject, effectPos, (Vector3)direction);
        foreach (TelegraphEffect effect in attackData.TelegraphEffects)
        {
            effect.Apply(context);
        }
    } 

    protected virtual IEnumerator Charge(ChargeAttackData chargeAttackData)
    {
        Vector2 chargeDir = DirToPlayer;


        float timeToWait = chargeAttackData.MaxChargeTime + chargeAttackData.TelegraphTime;
        ConditionAttackRuntime atk= PrepareAttack(chargeAttackData, () => Time.time - attackStartTime >= timeToWait);
        EWallHit += atk.SignalCompletion; //attack will complete when it hits a wall
        atk.EAttackFinish += Cleanup;

        //telegraph
        Telegraph(chargeAttackData, transform.position,DirToPlayer);
        yield return new WaitForSeconds(chargeAttackData.TelegraphTime);

        ExecuteAttackStart(chargeAttackData);

        //charge
        rb.linearVelocity=  10 * chargeAttackData.ChargeSpeed * chargeDir;



        void Cleanup()
        {
            rb.linearVelocity = Vector2.zero;
            EWallHit -= atk.SignalCompletion;
            atk.EAttackFinish -= Cleanup;
        }
        yield break;

    }

    protected virtual IEnumerator Jump(JumpAttackData jumpAttackData)
    {
        float timeToWait = jumpAttackData.JumpTime + jumpAttackData.StartLeapAfter + 0.1f + jumpAttackData.TelegraphTime;
        ConditionAttackRuntime atk = PrepareAttack(jumpAttackData, () => Time.time - attackStartTime >= timeToWait);
        atk.EAttackFinish += Cleanup;

        Vector2 endPos = player.transform.position - transform.position;
        endPos = Vector2.ClampMagnitude(endPos, jumpAttackData.MaxJumpDist) + transform.position.ToV2();

        //telegraph
        Telegraph(jumpAttackData, endPos,DirToPlayer);
        yield return new WaitForSeconds(jumpAttackData.TelegraphTime);

        ExecuteAttackStart(jumpAttackData);

        yield return new WaitForSeconds(jumpAttackData.StartLeapAfter); //wait for animation to get to the jump point 

        rb.DOJump(endPos, jumpAttackData.JumpPower, 1, jumpAttackData.JumpTime).SetEase(jumpAttackData.EaseType)
            .OnComplete(()=> AnimHelper.ChangeAnimation(jumpAttackData.LandAnim));



        void Cleanup()
        {
            print("attack cleanup");
        }

        yield break;
    }

    protected virtual IEnumerator BurstProjectile(ProjectileAttackData burstAttackData)
    {


        int spawnedProjectiles = 0;
        ConditionAttackRuntime atk = PrepareAttack(burstAttackData, ()=> spawnedProjectiles>= burstAttackData.NoOfProj);
        atk.EAttackFinish += Cleanup;
        float randomOffset = UnityEngine.Random.Range(-burstAttackData.RandomAngleOffset, burstAttackData.RandomAngleOffset);
        Vector2 dirToFire = Quaternion.Euler(0, 0, randomOffset) * DirToPlayer;

        //telegraph
        Telegraph(burstAttackData, transform.position, dirToFire);
        yield return new WaitForSeconds(burstAttackData.TelegraphTime);

        ExecuteAttackStart(burstAttackData);

        for (int i = 0; i < burstAttackData.NoOfProj; i++)
        {

            Vector2 dir = GetSpreadDir(dirToFire, i, burstAttackData.CoverAngle, burstAttackData.NoOfProj, 0);// random offset is applied to dir to fire;
            Vector2 spawnPos = transform.position;

            Quaternion spawnRot = Quaternion.FromToRotation(burstAttackData.ProjectilePrefab.transform.right, dir);
            //Quaternion spawnRot = Quaternion.identity;
            Projectile proj = MonoBehaviour.Instantiate(burstAttackData.ProjectilePrefab, spawnPos , spawnRot);
            proj.Sender = transform;
            proj.Launch(dir * burstAttackData.ProjSpeed);

            spawnedProjectiles++;
            if(burstAttackData.TimeBetweenProj >0f) yield return new WaitForSeconds(burstAttackData.TimeBetweenProj);

        }
        

        Vector3 GetSpreadDir(Vector2 forward, int index, float angle,int count, float offset)
        {
            if (count == 1) return forward.normalized;

            float angleStep = Mathf.Approximately(angle, 360f) ? angle / count : angle / (count - 1); //centers it for non 360 angles

            return Quaternion.Euler(0,0,angleStep*index + offset-angle/2) * forward;
            
        }

        void Cleanup()
        {
            atk.EAttackFinish -= Cleanup;
        }

    }

    protected void Die()
    {
        Destroy(gameObject);
    }


    Coroutine phaseRoutine;

    void StartPhase(BossPhase phase)
    {
        if (phaseRoutine != null)
        {
            StopCoroutine(phaseRoutine);
        }

        currentPhase = phase;
        phaseRoutine = StartCoroutine(RunPhase(phase));
        
    }

    IEnumerator RunPhase(BossPhase phase)
    {
        foreach (var b in phase.EnterBehaviors)
        {
            yield return b.Execute(this);
        }
    }
}
