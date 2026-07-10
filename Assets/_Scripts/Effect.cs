using EditorAttributes;
using System;
using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] bool spawnDmgNo;
    //[SerializeField, ShowField(nameof(spawnDmgNo))] PopupText popupTextPrefab;
    //[SerializeField, ShowField(nameof(spawnDmgNo))] float textSize;

    public override void Apply(EffectContext context)
    {
        Health dmgable;
        if(context.Target.TryGetComponent<Health>(out dmgable))
        {
            dmgable.TakeDamage(context,damage);
            //if(spawnDmgNo)
            //{
            //    PopupText txt = MonoBehaviour.Instantiate(popupTextPrefab, context.EffectPoint, Quaternion.identity);
            //    txt.Init(damage.ToString(), textSize, 0.5f,true, 0.2f);
            //}
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
            rb.AddForce(context.EffectDir * knockback* 100);
        }
    }

}

[Serializable]
public class SpawnParticlesEffect : Effect
{
    [SerializeField] ParticleSystem particlesPreab;
    [SerializeField] bool useAttackDir = true;

    public override void Apply(EffectContext context)
    {
        Quaternion spawnRot=Quaternion.identity;
        if(useAttackDir) spawnRot = Quaternion.FromToRotation(particlesPreab.transform.right, context.EffectDir);

        ParticleSystem particles = MonoBehaviour.Instantiate(particlesPreab, context.EffectPoint,spawnRot);
        particles.Play();
    }


}

[Serializable]
public class SpawnProjectile : Effect
{
    [SerializeField] Projectile projectile;
    [SerializeField] float speed;
    [SerializeField] float delay;

    public override void Apply(EffectContext context)
    {
        GameSceneManager.Instance.StartCoroutine(Spawn(context));
        

    }

    IEnumerator Spawn(EffectContext context)
    {
        Quaternion spawnRot = Quaternion.FromToRotation(projectile.transform.right, context.EffectDir);
        yield return new WaitForSeconds(delay);
        Projectile proj = MonoBehaviour.Instantiate(projectile, context.EffectPoint, spawnRot);
        proj.Sender = context.Source.transform;
        proj.Launch(context.EffectDir * speed);

    }
}

[Serializable]
public class StunEffect : Effect
{
    [SerializeField] float duration;

    public override void Apply(EffectContext context)
    {
        if(context.Target.TryGetComponent<IStunnable>(out IStunnable stunable))
        {
            stunable.Stun(duration);
        }
    }
}

