using EditorAttributes;
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
    [SerializeField] bool hasComboEndCooldown;
    [SerializeField, ShowField(nameof(hasComboEndCooldown))] float comboEndCooldown;
    [SerializeField] Sprite displayImage;

    public List<AttackData> Combo => attackCombo;
    public int IdleAnim => Animator.StringToHash(idleClip.name);
    public float ComboEndCooldown => comboEndCooldown;
    public bool HasComboEndCooldown => hasComboEndCooldown;
    public Sprite Icon => displayImage;
}
