
using UnityEngine;

[RequireComponent(typeof(AnimationHelper))]
public class Weapon: MonoBehaviour, ICombatHandler
{

    [SerializeField] WeaponData weaponData;
    [SerializeField] int comboIndex = 0;
    [SerializeField] Hitbox weaponHitbox;
    [SerializeField] bool hasBufferedAttack;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] float comboCyoteTime=1f;
    [SerializeField] bool comboIsFinished = true;

    [SerializeField] AttackRuntime currentAttack;

    [SerializeField ]AnimationHelper animHelper;
    float timeSinceLastAttack=0f;

    [SerializeField] bool isAttacking;

    float prevProgress;
    private void Start()
    {
        animHelper = GetComponent<AnimationHelper>();
        if (animHelper == null) { Debug.Log("SD"); }

        currentAttack = null;
    }

    private void Update()
    {
        if(isAttacking)
        {
            currentAttack.Tick();
        }
        else
        {
            if(Time.time - timeSinceLastAttack > comboCyoteTime && !comboIsFinished)
            {
                OnComboFinish();
            }
        }
    }

    void StartAttack(int index)
    {
        Debug.Log("Attack Start");
        comboIndex = index;
        AttackData data= weaponData.Combo[index];
        if (data == null) Debug.LogError("DATA NOT FOUND");
        comboIsFinished = false;
        isAttacking = true;
        currentAttack = CreateRuntimeAttack(data);
        currentAttack.EAttackFinish += OnAttackFinish;
        currentAttack.EToggleMouseLock += OnToggleMouseLook;
        currentAttack.EAttackFinish += test;

        animHelper.ChangeAnimation(currentAttack.Data.AttackAnimation);
    }

    void test() => Debug.Log("attack ended");

    public void TryAttack()
    {
        if (isAttacking)
        {
            if (currentAttack != null &&
                currentAttack.CanBufferNextAttack())
            {
                hasBufferedAttack = true;
            }

            return;
        }

        int index =
            comboIsFinished
            ? 0
            : GetNextAttackInCombo();

        StartAttack(index);
    }

    void OnAttackFinish()
    {
        currentAttack.EAttackFinish -= OnAttackFinish;
        currentAttack.EToggleMouseLock -= OnToggleMouseLook;
        currentAttack.EAttackFinish -= test;


        timeSinceLastAttack = Time.time;

        currentAttack = null;
        isAttacking = false;
        animHelper.ChangeAnimation(weaponData.IdleAnim);
        Debug.Log(animHelper.Anim.GetCurrentAnimatorStateInfo(0).nameHash) ;
        if(hasBufferedAttack) { hasBufferedAttack = false; StartAttack(GetNextAttackInCombo()); }
    }

    void OnComboFinish()
    {
        comboIndex = 0;

        animHelper.ChangeAnimation(weaponData.IdleAnim);
        comboIsFinished = true;
    }

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p,dir);

        if (currentAttack == null) Debug.Log("SAD");
        foreach (Effect effect in currentAttack.Data.Effects)
            effect.Apply(context);
    }

    int GetNextAttackInCombo()
    {
        return (comboIndex + 1) % weaponData.Combo.Count;
    }

    AttackRuntime CreateRuntimeAttack(AttackData data)
    {
        return new AttackRuntime(data,Time.time, animHelper.Anim);
    }

    void OnToggleMouseLook(bool val)=> mouseLook.Locked = val;

    private void OnEnable()
    {
        weaponHitbox.EOnHitDetect += NotifyHit;
    }
    private void OnDisable()
    {
        weaponHitbox.EOnHitDetect -= NotifyHit;
    }



}
