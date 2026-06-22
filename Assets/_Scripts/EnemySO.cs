using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [SerializeField] string enemyName;
    [SerializeField] string enemyType;
    [SerializeField] AnimationClip idleAnim;
    [SerializeField] AnimationClip movementAnim;

    public string EnemyName { get => enemyName;  }
    public string EnemyType { get => enemyType; }
    public int IdleAnim { get =>Animator.StringToHash( idleAnim.name);  }
    public int MovementAnim { get => Animator.StringToHash(movementAnim.name);  }
}
