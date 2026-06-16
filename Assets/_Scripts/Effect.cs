using System;
using System.Diagnostics.Contracts;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    public abstract void Apply(EffectContext context);
    
}
[Serializable]

public class DamageEffect: Effect
{
    [SerializeField] float damage;
    

    public override void Apply(EffectContext context)
    {
        IDamageable dmgable;
        if(context.Target.TryGetComponent<IDamageable>(out dmgable))
        {
            dmgable.TakeDamage(damage);
        }
    }
}

[Serializable]
public class KnockBackEffect: Effect
{
    [SerializeField] float knockback=1000;

    public override void Apply(EffectContext context)
    {
        Rigidbody2D rb;
        if(context.Target.TryGetComponent<Rigidbody2D>(out rb))
        {
            Debug.Log("KNOCKBACK");
            rb.AddForce(context.EffectDir * knockback);
        }
    }



}
