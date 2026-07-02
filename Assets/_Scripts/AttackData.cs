using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;

public enum AttackDataType { Player, Enemy }


// holds stuff like list of attacks and normalised time windows as well as the animation for the attack
[CreateAssetMenu(menuName ="attack")]
public class AttackData : ScriptableObject
{

    [SerializeField] AttackDataType attackType;

    [SerializeField] string attackName;
    [SerializeField] AnimationClip attackAnimation;
    [SerializeField] int animationPriority =0;


    [SerializeField, ShowField(nameof(attackType), AttackDataType.Player)] float mouseLockTime;
    [SerializeField, ShowField(nameof(attackType), AttackDataType.Player)]float nextAttackInputStartTime; //for next combo 
    [SerializeField, ShowField(nameof(attackType), AttackDataType.Player)] float cancelAttackBeforeTime; // can cancel attack before this


    [SerializeReference, SubclassSelector] List<Effect> onTargetHitEffects;
    [SerializeReference, SubclassSelector] List<Effect> onAttackStartEffects;




    public string AttackName { get => attackName;  }
    public int AttackAnimation { get => Animator.StringToHash(attackAnimation.name);  }
    public int AnimationPriority { get => animationPriority; }  
    public List<Effect> OnTargetHitEffects { get => onTargetHitEffects; }
    public List<Effect> OnAttackStartEffects { get => onAttackStartEffects; }


    public float MouseLockTime { get => mouseLockTime;  }
    public float NextAttackInputStartTime { get => nextAttackInputStartTime;  }
    public float CancelAttackBeforeTime { get => cancelAttackBeforeTime;  }



}
