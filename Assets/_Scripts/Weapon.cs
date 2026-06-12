using System.Collections.Generic;
using UnityEngine;

public class Weapon:MonoBehaviour
{
    //[SerializeField] string attackName;
    //[SerializeField] float colliderRad;
    //[SerializeField] AnimationClip attackClip;
    //[SerializeField] float mouseLockDuration;
    //[SerializeField] float hitPointOffset;
    //[SerializeField] float timeBtwnNextAttack;
    //[SerializeField] AttackColliderType colliderType;
    //[SerializeField] float colliderActiveTime;

    //public AnimationClip AttackAnimClip => attackClip;
    //public int AttackAnimName => Animator.StringToHash(attackClip.name);
    //public string AttackName => attackName;
    //public float ColliderRad => colliderRad;
    //public float MouseLockDuration => mouseLockDuration;
    //public float HitPointOffset => hitPointOffset;
    //public float TimeBtwnNextAttack => timeBtwnNextAttack;
    //public AttackColliderType ColliderType => colliderType;
    //public float ColliderActiveTime => colliderActiveTime;

    [SerializeReference,SubclassSelector] List<AttackEffect> effects = new List<AttackEffect> ();
    [SerializeField] string test;
}
