using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossPhase
{
    [SerializeField] List<AttackData> attacks;

    public List<AttackData> Attacks { get => attacks; }
}

[CreateAssetMenu(menuName ="Boss Data")]
public class BossData : ScriptableObject
{
    [SerializeField] float health;
    [SerializeField] List<BossPhase> phases;
    [SerializeField] AnimationClip idleAnimation;
    [SerializeField] float timeBetweenAttacks;

    public float Health { get => health; }
    public List<BossPhase> Phases { get => phases; }
    public int IdleAnimation { get => Animator.StringToHash(idleAnimation.name); }
    public float TimeBetweenAttacks { get => timeBetweenAttacks; }
}
