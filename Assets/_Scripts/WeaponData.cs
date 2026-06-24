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

    public List<AttackData> Combo => attackCombo;
    public int IdleAnim => Animator.StringToHash(idleClip.name);
}
