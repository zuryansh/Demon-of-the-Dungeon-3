using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Holds Combo Information and speeed modifier that directly affects the animator speed

[CreateAssetMenu(menuName ="Weapon")]
public class WeaponData : ScriptableObject
{
    [SerializeField] List<AttackData> attackCombo;
    [SerializeField] AnimationClip idleClip;
    [SerializeField] LayerMask enemyLayer;
    [SerializeReference, SubclassSelector] List<Effect> effects;

    public List<Effect> Effects { get => effects; }
    public List<AttackData> Combo => attackCombo;
    public int IdleAnim => Animator.StringToHash(idleClip.name);
}
