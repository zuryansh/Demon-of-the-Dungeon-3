using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationHelper))]
public class Weapon:MonoBehaviour
{

    [SerializeField] WeaponData data;
    [SerializeField] int comboIndex = 0;
    [SerializeField] float progress;
    [SerializeField] Hitbox weaponHitbox;
    [SerializeField] bool isAttacking;
    [SerializeField] bool hasBufferedAttack;

    AttackData currentAttack;
    AnimationHelper animHelper;

    private void Start()
    {
        animHelper = GetComponent<AnimationHelper>();

    }

    private void Update()
    {
        progress = animHelper.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;




        if(progress>0.95f)
        {
            OnAttackFinish();
        }

        
    }

    public void TryAttack()
    {

        if (isAttacking && hasBufferedAttack) return;
        if(isAttacking && !hasBufferedAttack) 
        {
            if(progress > currentAttack.NextAttackInputStartTime) hasBufferedAttack = true; 
            return; 
        }

        isAttacking=true;
        comboIndex++;
        comboIndex = comboIndex % data.Combo.Count;
        currentAttack = data.Combo[comboIndex];

        animHelper.ChangeAnimation(currentAttack.AttackAnimation);
    }


    void OnAttackFinish()
    {
        isAttacking = false;
        if (hasBufferedAttack) { hasBufferedAttack = false; TryAttack(); }
    }


    public void NotifyHit(Collider2D collider)
    {
        Debug.Log("HIT");
        //var target = collider.GetComponent<IDamageable>();

        //if (target == null)
        //    return;

        //EffectContext context = CreateContext(collider);

        //foreach (var effect in currentAttack.Effects)
        //    effect.Apply(context);
    }

    private void OnEnable()
    {
        weaponHitbox.EOnHitDetect += NotifyHit;
    }
    private void OnDisable()
    {
        weaponHitbox.EOnHitDetect -= NotifyHit;
    }

}
