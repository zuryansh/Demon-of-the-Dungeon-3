using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;


// holds stuff like list of attacks and normalised time windows as well as the animation for the attack
[CreateAssetMenu(menuName ="attack")]
public class AttackData : ScriptableObject
{
    [SerializeField] string attackName;
    [SerializeField] AnimationClip attackAnimation;

    [Header("Timings (as fraction of total animation time) ")]
    [SerializeField] float mouseLockTime;
    [SerializeField] float nextAttackInputStartTime; //for next combo 
    [SerializeField] float cancelAttackBeforeTime; // can cancel attack before this


    [Header("Timings (as sec to gen fractions) ")]
    [SerializeField] float mouseLockTimeS;
    [SerializeField] float nextAttackInputStartS; //for next combo 
    [SerializeField] float cancelAttackBeforeTimeS; // can cancel attack before this



    public string AttackName { get => attackName;  }
    public int AttackAnimation { get => Animator.StringToHash(attackAnimation.name);  }
    public float MouseLockTime { get => mouseLockTime;  }
    public float NextAttackInputStartTime { get => nextAttackInputStartTime;  }
    public float CancelAttackBeforeTime { get => cancelAttackBeforeTime;  }

    [Button("Gen Fractional Timings")]
    void GenFractionalTimings()
    {
        mouseLockTime = nextAttackInputStartS/attackAnimation.length;
        nextAttackInputStartTime = nextAttackInputStartS/ attackAnimation.length;
        cancelAttackBeforeTime = cancelAttackBeforeTimeS/ attackAnimation.length;
    }

}
