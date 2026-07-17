using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Mellee, Ranged, Summoner
}

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [SerializeField] string enemyName;
    [SerializeField] EnemyType enemyType;
    [SerializeField] AnimationClip idleAnim;
    [SerializeField] AnimationClip movementAnim;
    [SerializeReference, SubclassSelector] List<Effect> onHitEffects;
    [SerializeReference, SubclassSelector] List<Effect> onDeathEffects;
    [SerializeField] int pointCost =1;

    public string EnemyName { get => enemyName;  }
    public EnemyType Type { get => enemyType; }
    public int IdleAnim { get =>Animator.StringToHash( idleAnim.name);  }
    public int MovementAnim { get => Animator.StringToHash(movementAnim.name);  }
    public List<Effect> OnHitEffects { get => onHitEffects; }
    public List<Effect> OnDeathEffects { get => onDeathEffects; }
    public int PointCost { get => pointCost; }
}
