using System;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    public abstract void Apply();
    
}
[Serializable]

public class DamageEffect: Effect
{
    [SerializeField] int damage;

    public override void Apply()
    {
        throw new NotImplementedException();
    }
}
[Serializable]

public class KnockBackEffect: Effect
{
    [SerializeField] int knockBack;

    public override void Apply()
    {
        throw new NotImplementedException();
    }
}
