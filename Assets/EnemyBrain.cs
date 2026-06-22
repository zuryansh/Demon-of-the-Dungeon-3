using EditorAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class EnemyBrain : MonoBehaviour
{

    public Transform Player;
    public float SqrDistToPlayer => (transform.position - Player.position).sqrMagnitude;
    public Vector2 DirToPlayer => (Player.position - transform.position).normalized;
    public EnemySO Data=> enemyData;
    public AnimationHelper AnimationHelper => animHelper;

    [SerializeField] EnemySO enemyData;
    AnimationHelper animHelper;

    EnemyMovementModule movementModule;
    EnemyAttackModule attackModule;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = FindAnyObjectByType<PlayerMovement>().transform; //CHANGE LATER
        animHelper = GetComponent<AnimationHelper>();
        movementModule = GetComponent<EnemyMovementModule>();    
        attackModule = GetComponent<EnemyAttackModule>();
        CheckDependecies();
        movementModule.Brain = this;
        //attackModule.Brain = this; 

        movementModule.Init();
    }

    void CheckDependecies()
    {
        if (movementModule == null) Debug.LogError("Movement Module Not Found!");
        //if (attackModule == null) Debug.LogError("Attack Module Not Found");
        if (animHelper == null) Debug.LogError("No Animation Helper Found");
        if (enemyData == null) Debug.LogError("ENEMY DATA NOT FOUND");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movementModule.Tick();
    }

    
}
