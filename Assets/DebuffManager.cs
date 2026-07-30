using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

//public interface DebuffAcceptor
//{
//    public void ApplyDebuff(BuffDebuffEffect);
//    public void OnDebuffEnd(BuffDebuffEffect);
//}

[System.Serializable]
public class ActiveDebuff
{
    public BuffDebuffEffect Effect;
    public DebuffContext Context;
    public float EndTime;
    public float NextTickTime;

    public ParticleSystem Particles;

}

public class DebuffManager : MonoBehaviour
{
    public List<ActiveDebuff> ActiveDebuffs => activeDebuffs;

    [SerializeReference] List<ActiveDebuff> activeDebuffs;

    public void ApplyDebuff(BuffDebuffEffect effect, EffectContext context)
    {
        if (HasDebuff(effect) && !effect.CanStack) return;

        DebuffContext c = new DebuffContext(context.Source, context.Target, context.EffectPoint, context.EffectDir);
        ActiveDebuff debuff = new ActiveDebuff
        {
            Effect = effect,
            Context = c,
            EndTime = Time.time + effect.Duration,
            NextTickTime = Time.time + effect.TimeBetweenTicks
        };

        effect.OnApply(debuff);

        activeDebuffs.Add(debuff);
    }



    void Update()
    {
        float time = Time.time;

        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            var debuff = activeDebuffs[i];

            if (debuff.Effect.TimeBetweenTicks > 0 &&
                time >= debuff.NextTickTime)
            {
                debuff.Effect.OnTick(debuff);
                debuff.NextTickTime += debuff.Effect.TimeBetweenTicks;
            }

            if (time >= debuff.EndTime)
            {
                debuff.Effect.OnExpire(debuff);
                activeDebuffs.RemoveAt(i);
            }
        }
    }

    public bool HasDebuff(BuffDebuffEffect effect)
    {
        Type type = effect.GetType();

        return activeDebuffs.Any(d => d.Effect.GetType() == type);
    }

}
