using UnityEngine;

[CreateAssetMenu(menuName = "Attack/ Jump Attack")]
public class JumpAttackData : AttackData
{
    [SerializeField] float jumpPower;
    [SerializeField] float jumpTime;
    [SerializeField] AnimationClip landAnimation;
    [SerializeField] float startLeapAfter;
    [SerializeField] float maxJumpDist;

    public float JumpPower { get => jumpPower; }
    public float JumpTime { get => jumpTime; }
    public int LandAnim => Animator.StringToHash(landAnimation.name);
    public float MaxJumpDist => maxJumpDist;
    public float StartLeapAfter { get => startLeapAfter; }
}



