using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebuffContext: EffectContext
{
    public override Vector3 EffectPoint => Target.transform.position;
    public override Vector3 EffectDir
    {
        get
        {
            if(Source!= null && Target!= null)
            return (Target.transform.position - Source.transform.position).normalized;
            else return effectDir;
        }
    }

    private readonly Dictionary<Type, Component> cache = new();

    public DebuffContext(GameObject source, GameObject target, Vector3 effectPoint, Vector3 effectDir) : base(source, target, effectPoint, effectDir)
    {
    }

    public T Get<T>() where T : Component // generate lookup for each lookup per instance of debuff. so that we dont call get component each tick
    {
        if (cache.TryGetValue(typeof(T), out var component))
            return (T)component;

        Target.TryGetComponent(out T result);
        cache[typeof(T)] = result;

        return result;
    }
}

[Serializable]
public abstract class BuffDebuffEffect : Effect
{
    public float TimeBetweenTicks => timeBetweenTicks;
    public float Duration => duration;
    public bool CanStack => canStack;

    [SerializeField] protected float timeBetweenTicks;
    [SerializeField] protected float duration;
    [SerializeField] protected ParticleSystem particlesPrefab;
    [SerializeField] protected Sprite effectSprite;
    [SerializeField] protected bool canStack = false;


    public override void Apply(EffectContext context)
    {
        if (context.Target == null) return;
        if(context.Target.TryGetComponent(out DebuffManager acceptor))
        {
            acceptor.ApplyDebuff(this,context);       
        }
    }

    public abstract void OnApply(ActiveDebuff debuff);
    public abstract void OnExpire(ActiveDebuff debuff);
    public abstract void OnTick(ActiveDebuff debuff);

}

[Serializable]
public class TickDamage : BuffDebuffEffect
{
    [SerializeField] float dmg;

    public override void OnApply(ActiveDebuff debuff)
    {
        
        Health health = debuff.Context.Get<Health>();
        if (health != null)
        {
            health.TakeDamage(debuff.Context, dmg);
            if (particlesPrefab != null)
            {
                debuff.Particles = MonoBehaviour.Instantiate(particlesPrefab, debuff.Context.EffectPoint, Quaternion.identity);
                debuff.Particles.transform.parent = debuff.Context.Target.transform;
                var main = debuff.Particles.main;
                main.duration = duration;
                debuff.Particles.Play();
            }
        }
    }

    public override void OnExpire(ActiveDebuff debuff)
    {
       if(debuff.Particles != null) MonoBehaviour.Destroy(debuff.Particles.gameObject);
    }

    public override void OnTick(ActiveDebuff debuff)
    {

        Health health = debuff.Context.Get<Health>();
        if (health != null)
        {
            health.TakeDamage(debuff.Context, dmg);
        }
    }
}