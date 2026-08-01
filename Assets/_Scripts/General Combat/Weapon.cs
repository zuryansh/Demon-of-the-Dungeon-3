
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AnimationHelper))]
public class Weapon: MonoBehaviour, ICombatHandler
{
    //public Vector2 MouseDir => (inputCam.ScreenToWorldPoint(Input.mousePosition) - visuals.position).normalized;
    public Vector2 MouseDir => Player.Instance.MouseAndJoystickDir;
    public WeaponData Data => weaponData;

    [SerializeField] Collider2D user;
    [SerializeField] Transform visuals;
    [SerializeField] WeaponData weaponData;
    [SerializeField] Hitbox weaponHitbox;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] float comboCyoteTime=1f;
    [SerializeField] AnimationAttackRuntime currentAttack;
    [SerializeField] LayerMask hitLayers;
    [SerializeField] int comboIndex = 0;
    [SerializeField] bool hasBufferedAttack;
    [SerializeField] bool comboIsFinished = true;

    bool onCooldown = false;
    AnimationHelper animHelper;
    float timeSinceLastAttackEnd=0f;
    Camera inputCam;
    ContactFilter2D contactFilter;

    [SerializeField] bool isAttacking;


    private void Start()
    {
        inputCam = Camera.main;
        animHelper = GetComponent<AnimationHelper>();
        currentAttack = null;
        contactFilter = new ContactFilter2D { layerMask = hitLayers };
    }

    private void Update()
    {
        timeSinceLastAttackEnd += Time.deltaTime;

        if(isAttacking)
        {
            currentAttack.Tick();
        }
        else
        {
            if(timeSinceLastAttackEnd > comboCyoteTime && !comboIsFinished)
            {
                OnComboFinish();
            }
        }
    }

    void StartAttack(int index)
    {
        comboIndex = index;
        AttackData data= weaponData.Combo[index];
        if (data == null) Debug.LogError("DATA NOT FOUND");
        comboIsFinished = false;
        isAttacking = true;
        currentAttack = CreateRuntimeAttack(data);
        currentAttack.EAttackFinish += OnAttackFinish;
        currentAttack.EToggleMouseLock += OnToggleMouseLook;

        // need to get dir this way because the way the mouse points and the way we attack is diff
        //mainly to make joystick work
        Vector2 dir = GetDirToRaycastHit(visuals.position,MouseDir);
        EffectContext context = new EffectContext(user.gameObject, null, visuals.position ,dir);
        foreach (var effect in data.OnAttackStartEffects) effect.Apply(context);


        animHelper.ChangeAnimation(currentAttack.Data.AttackAnimation, forceReplay :true);
    }


    Vector2 GetDirToRaycastHit(Vector2 referencePoint,Vector2 dir)
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        Physics2D.Raycast(visuals.position, dir,contactFilter, hits);
        return (hits[0].point - referencePoint).normalized;
    }

    public void TryAttack()
    {
        if (onCooldown) return;

        if (isAttacking)
        {
            if (currentAttack != null &&
                currentAttack.CanBufferNextAttack() && comboIndex < weaponData.Combo.Count-1) //only let buffer if not on the last attack in chain 
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
        EffectContext context = new EffectContext( user.gameObject, null, visuals.position, MouseDir.normalized);
        foreach (var effect in currentAttack.Data.OnAttackEndEffects) effect.Apply(context);

        currentAttack.EAttackFinish -= OnAttackFinish;
        currentAttack.EToggleMouseLock -= OnToggleMouseLook;


        timeSinceLastAttackEnd = 0f;

        currentAttack = null;
        isAttacking = false;



        animHelper.ChangeAnimation(weaponData.IdleAnim);
        if(hasBufferedAttack) { hasBufferedAttack = false; StartAttack(GetNextAttackInCombo()); } 
    }

    void OnComboFinish()
    {
        int prevIndex = comboIndex;

        comboIndex = 0;

        animHelper.ChangeAnimation(weaponData.IdleAnim);
        comboIsFinished = true;
        if (prevIndex == weaponData.Combo.Count - 1)
        { //only have cooldown for the last attack 

            if (weaponData.HasComboEndCooldown)
            {
                onCooldown = true;
                Invoke(nameof(ResetCooldown), weaponData.ComboEndCooldown);
            }
        }
    }
    void ResetCooldown() => onCooldown = false;

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(user.gameObject, collider.gameObject, p,dir);

        foreach (Effect effect in currentAttack.Data.OnTargetHitEffects)
            effect.Apply(context);
    }

    int GetNextAttackInCombo()
    {
        return (comboIndex + 1) % weaponData.Combo.Count;
    }

    AnimationAttackRuntime CreateRuntimeAttack(AttackData data)
    {
        return new AnimationAttackRuntime(data,Time.time, animHelper.Anim);
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
