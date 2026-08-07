using EditorAttributes;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public enum AttackDataType { Player, Enemy }


// holds stuff like list of attacks and normalised time windows as well as the animation for the attack
[CreateAssetMenu(menuName ="Attack/BaseAttack")]
public class AttackData : ScriptableObject
{

    [SerializeField] protected AttackDataType attackType;

    [SerializeField] protected string attackName;
    [SerializeField] protected AnimationClip attackAnimation;
    [SerializeField] protected int animationPriority =0;

    [SerializeField, ShowField(nameof(attackType), AttackDataType.Player)] protected float mouseLockTime;
    [SerializeField, ShowField(nameof(attackType), AttackDataType.Player)]protected float nextAttackInputStartTime; //for next combo 
    [SerializeField, ShowField(nameof(attackType), AttackDataType.Player)] protected float cancelAttackBeforeTime; // can cancel attack before this


    [SerializeReference, SubclassSelector] protected List<Effect> onTargetHitEffects;
    [SerializeReference, SubclassSelector] protected List<Effect> onAttackStartEffects;
    [SerializeReference, SubclassSelector] protected List<Effect> onAttackEndEffects;




    public string AttackName { get => attackName;  }
    public int AttackAnimation { get => Animator.StringToHash(attackAnimation.name);  }
    public int AnimationPriority { get => animationPriority; }  
    public List<Effect> OnTargetHitEffects { get => onTargetHitEffects; }
    public List<Effect> OnAttackStartEffects { get => onAttackStartEffects; }
    public List<Effect> OnAttackEndEffects { get => onAttackEndEffects; }


    public float MouseLockTime { get => mouseLockTime;  }
    public float NextAttackInputStartTime { get => nextAttackInputStartTime;  }
    public float CancelAttackBeforeTime { get => cancelAttackBeforeTime;  }

}

