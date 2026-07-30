using System;
using UnityEngine;

public class EnemyBrain : MonoBehaviour,IStunnable
{
    // ADD Line Of Sight
    public Transform Player;
    public EnemySO Data=> enemyData;
    public AnimationHelper AnimationHelper => animHelper;
    public event Action<EnemyBrain> EOnDeath;
    public bool HasLOS => hasLOS;
    public Vector2 LastPlayerPos;
    public Transform EffectPoint => effectPoint;
    public LayerMask ObstacleLayer => obstacleLayer;
    public EnemySpawner ParentSpawner=> parentSpawner;

    [SerializeField] EnemySO enemyData;
    [SerializeField] bool hasLOS;
    [SerializeField] float timeBetweenTicks = 0.1f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float startDelay;
    [SerializeField] Transform effectPoint;

    AnimationHelper animHelper;
    EnemyMovementModule movementModule;
    EnemyAttackModule attackModule;
    float startTime;
    bool canTick = false;
     EnemySpawner parentSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startDelay += UnityEngine.Random.Range(0f, 0.6f);
        if(Player == null) Player = FindAnyObjectByType<Player>().transform; //CHANGE LATER
        startTime = Time.time;
        animHelper = GetComponent<AnimationHelper>();
        movementModule = GetComponent<EnemyMovementModule>();    
        attackModule = GetComponent<EnemyAttackModule>();
        CheckDependecies();
        movementModule.Brain = this;
        attackModule.Brain = this;

        movementModule.Init();
        attackModule.Init();
    }

    public void SetSpawner(EnemySpawner spawner) => parentSpawner = spawner;

    void CheckDependecies()
    {
        if (movementModule == null) Debug.LogError("Movement Module Not Found!");
        if (attackModule == null) Debug.LogError("Attack Module Not Found");
        if (animHelper == null) Debug.LogError("No Animation Helper Found");
        if (enemyData == null) Debug.LogError("ENEMY DATA NOT FOUND");
    }


    private void Update()
    {
        if (!canTick && Time.time - startTime > startDelay) canTick = true;
        //timeSinceLastTick += Time.deltaTime;
            Tick();
        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //timeSinceLastFixTick += Time.fixedDeltaTime;
        //if(timeSinceLastFixTick > timeBetweenTicks)
        //{
            FixedTick();
        //}
    }

    void FixedTick()
    {
        //timeSinceLastFixTick = 0f;
        if (!canTick) return;
        movementModule.Tick();
    }


    public void OnHit(EffectContext context)
    {
        foreach (Effect effect in Data.OnHitEffects)
        {
            effect.Apply(context);
        }
    }

    bool dead=false;
    public void OnDeath(EffectContext context)
    {
        if(dead) return;
        dead = true;
        EOnDeath?.Invoke(this);

        foreach (Effect effect in Data.OnDeathEffects)
        {
            effect.Apply(context);
        }
        if(parentSpawner != null) parentSpawner.OnSpawnRemoved(gameObject);
        Destroy(gameObject,0.05f);
    }

  

    public void Stun(float duration)
    {
        if (movementModule == null || attackModule == null) return;
        movementModule.Stun(duration);
        attackModule.Stun(duration);
    }

    void Tick()
    {
        if (!canTick) return;

            //timeSinceLastTick = 0;
            hasLOS = CheckLOS();
        if (hasLOS) LastPlayerPos = Player.position;
        attackModule.Tick();

    }

    bool CheckLOS()
    {
        Vector2 origin = transform.position;
        Vector2 target = Player.transform.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer | playerLayer);

        Debug.DrawRay(origin, direction * distance, hit && hit.transform == Player.transform ? Color.green : Color.red);
        return hit && hit.transform == Player.transform;
    }

    
}
