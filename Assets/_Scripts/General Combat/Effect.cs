using EditorAttributes;
using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class Effect
{
    public abstract void Apply(EffectContext context);
    
}
[Serializable]
public class DebugEffect: Effect
{
    [SerializeField] string message;
    public override void Apply(EffectContext context)
    {
        Debug.Log(message);
    }
}

[Serializable]
public class DamageEffect: Effect
{
    [SerializeField] float damage;
    [SerializeField] bool spawnDmgNo;

    public override void Apply(EffectContext context)
    {
        Health dmgable;
        if(context.Target.TryGetComponent<Health>(out dmgable))
        {
            dmgable.TakeDamage(context,damage);

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
    [SerializeField] float spreadAngle;

    public override void Apply(EffectContext context)
    {
        GameSceneManager.Instance.StartCoroutine(Spawn(context));
        

    }

    IEnumerator Spawn(EffectContext context)
    {
        float angle = UnityEngine.Random.Range(-spreadAngle, spreadAngle);

        Vector2 dir = Quaternion.Euler(0, 0, angle) * context.EffectDir.normalized;
        Quaternion spawnRot = Quaternion.FromToRotation(projectile.transform.right, dir);
        yield return new WaitForSeconds(delay);
        Projectile proj = MonoBehaviour.Instantiate(projectile, context.EffectPoint, spawnRot);
        proj.Sender = context.Source.transform;
        proj.Launch(dir * speed);

    }
}

[Serializable]
public class StunEffect : Effect
{
    [SerializeField] float duration;

    public override void Apply(EffectContext context)
    {
        if (context.Target == null) return; 
        if(context.Target.TryGetComponent<IStunnable>(out IStunnable stunable))
        {
            stunable.Stun(duration);
        }
    }
}

[Serializable]
public class ScreenShakeEffect : Effect
{
    [SerializeField] float force;

    public override void Apply(EffectContext context)
    {
        CameraShake.Instance.Shake(force);
    }
}
[Serializable]
public class SpawnLootItemEffect: Effect
{
    [SerializeField] LootItem item;
    [SerializeField] int count;
    [SerializeField] bool spawnWithRandomVelocity;
    [SerializeField, ShowField(nameof(spawnWithRandomVelocity))] float spawnSpeed;
    [SerializeField, Range(0f, 1f)] float spawnChance;

    public override void Apply(EffectContext context)
    {
        Spawn(context);
    }

    void Spawn(EffectContext context)
    {
        float n = UnityEngine.Random.Range(0f, 1f);
        if ((n<=spawnChance))
        {
            for (int i = 0; i < count; i++)
            {
                LootItem lootItem = MonoBehaviour.Instantiate(item, context.EffectPoint, Quaternion.identity);
                if (spawnWithRandomVelocity && lootItem.RB != null)
                {
                    lootItem.RB.AddForce(UnityEngine.Random.insideUnitCircle * spawnSpeed * 100);
                }
            }

        }


    }
}

[Serializable]
public class PlaySoundEffect : Effect
{
    [SerializeField] AudioClip[] clips;
    [SerializeField] float volumme=1f;
    [SerializeField] SoundType type;
    [SerializeField] float duration =0f;

    public override void Apply(EffectContext context)
    {
        if (clips.Length == 1)
            AudioManager.Instance.PlaySound(clips[0], volumme, type, duration);
        else if (clips.Length > 1)
        {
            AudioManager.Instance.PlayRandomSound(clips, volumme, type , duration);
        }
    }

}

[Serializable]
public class SpawnEnemyEffect : Effect
{
    [SerializeField] EnemyBrain[] enemies;
    public override void Apply(EffectContext context)
    {
       EnemyBrain enemy=  MonoBehaviour.Instantiate(enemies.Choice(), context.EffectPoint, Quaternion.identity);
       

    }
}

[Serializable]
public class UnityEventEffect : Effect
{
    [SerializeField] UnityEvent<EffectContext> onApply;
    public override void Apply(EffectContext context)
    {
        onApply?.Invoke(context);
    }
}