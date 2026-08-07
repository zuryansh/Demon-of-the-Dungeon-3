using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/Base")]
public class EnemySO : ScriptableObject
{
    [SerializeField] protected string enemyName;
    [SerializeField] protected AnimationClip idleAnim;
    [SerializeField] protected AnimationClip movementAnim;
    [SerializeReference, SubclassSelector] protected List<Effect> onHitEffects;
    [SerializeReference, SubclassSelector]protected  List<Effect> onDeathEffects;
    [SerializeField] protected int pointCost =1;

    public string EnemyName { get => enemyName;  }
    public int IdleAnim { get =>Animator.StringToHash( idleAnim.name);  }
    public int MovementAnim { get => Animator.StringToHash(movementAnim.name);  }
    public List<Effect> OnHitEffects { get => onHitEffects; }
    public List<Effect> OnDeathEffects { get => onDeathEffects; }
    public int PointCost { get => pointCost; }
}
