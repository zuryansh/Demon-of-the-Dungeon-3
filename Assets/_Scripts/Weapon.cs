using EditorAttributes;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(AnimationHelper))]
public class Weapon:MonoBehaviour
{

    [SerializeField] WeaponData weaponData;
    [SerializeField] int comboIndex = 0;
    [SerializeField] float progress;
    [SerializeField] Hitbox weaponHitbox;
    [SerializeField] bool isAttacking;
    [SerializeField] bool hasBufferedAttack;
    [SerializeField] MouseLook mouseLook;

    [SerializeField]AttackData currentAttack;
    AnimationHelper animHelper;


    float prevProgress;
    private void Start()
    {
        animHelper = GetComponent<AnimationHelper>();
        currentAttack = weaponData.Combo[comboIndex];
    }

    private void Update()
    {
        prevProgress = progress;
        progress = animHelper.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (isAttacking)
        {
            if (progress > 0.95f && progress<1.1f) OnAttackFinish();
            if (progress > currentAttack.MouseLockTime) mouseLook.Locked= true;
            else mouseLook.Locked= false;
        }
        //if (progress > 1f) isAttacking = false;

    }

    public void TryAttack()
    {

        if (isAttacking && hasBufferedAttack) return;
        if(isAttacking&& !hasBufferedAttack) 
        {
            if(progress > currentAttack.NextAttackInputStartTime) hasBufferedAttack = true; 
            return; 
        }

        isAttacking=true;
        comboIndex++;
        comboIndex = comboIndex % weaponData.Combo.Count;
        currentAttack = weaponData.Combo[comboIndex];

        animHelper.ChangeAnimation(currentAttack.AttackAnimation);
    }


    void OnAttackFinish()
    {
        isAttacking = false;
        if (hasBufferedAttack) { hasBufferedAttack = false; TryAttack(); }
        else 
        {
            OnComboFinish();
        }
    }

    void OnComboFinish()
    {
        Debug.Log("Combo Finished");
        comboIndex = 0;
        animHelper.ChangeAnimation(weaponData.IdleAnim);
    }

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {
        var target = collider.gameObject;

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p,dir);

        foreach (Effect effect in weaponData.Effects)
            effect.Apply(context);
    }

    private void OnEnable()
    {
        weaponHitbox.EOnHitDetect += NotifyHit;
    }
    private void OnDisable()
    {
        weaponHitbox.EOnHitDetect -= NotifyHit;
    }

    bool Crossed(float prev, float next, float threshold)
    {
        return (prev < threshold && next >= threshold);
    }

}
