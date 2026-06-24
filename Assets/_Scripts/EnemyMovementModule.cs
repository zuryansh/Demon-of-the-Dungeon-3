using EditorAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyMovementModule : MonoBehaviour
{
    public EnemyBrain Brain;
    protected Rigidbody2D rb;
    [SerializeField] bool FlipSpriteAccToDir;
    [ConditionalField(ConditionType.OR, nameof(FlipSpriteAccToDir))]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public virtual void Init()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveInDir(Vector2 direction,float moveSpeed, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        Vector2 dir = direction.normalized;

        Vector2 targetSpeed = dir.normalized * moveSpeed;
        Vector2 speedDif = targetSpeed - rb.linearVelocity;
        rb.AddForce(speedDif, forceMode); // impulse feels more snappy but FORCE feels more floaty
    }

    protected void StopAllMovement()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public virtual void Tick()
    {
        if (FlipSpriteAccToDir) FlipSprite();
    }

    protected virtual void FlipSprite()
    {
        if (spriteRenderer == null) Debug.LogWarning("Sprite renderer not found");
        else
        {

            if(rb.linearVelocity.x >0) spriteRenderer.flipX = false;
            else spriteRenderer.flipX= true;
        }
    }


}
