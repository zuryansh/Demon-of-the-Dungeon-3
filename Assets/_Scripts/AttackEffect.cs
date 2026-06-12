using System;
using UnityEngine;

[Serializable]
public abstract class AttackEffect
{
    string adsa;
    
}
[Serializable]

public class DamageEffect: AttackEffect
{
    [SerializeField] int damage;
}
[Serializable]

public class KnockBackEffect: AttackEffect
{
    [SerializeField] int knockBack;
}
