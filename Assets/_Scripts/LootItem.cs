using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class LootItem : MonoBehaviour
{
    public Rigidbody2D RB => rb;
    bool InRange => (transform.position - player.transform.position).sqrMagnitude < pickupRange * pickupRange;

    [SerializeField] protected bool gravitateTowardsPlayer;
    [SerializeField, ShowField(nameof(gravitateTowardsPlayer))] float gravitateSpeed;
    [SerializeField, ShowField(nameof(gravitateTowardsPlayer))] float pickupRange;
    [SerializeReference, SubclassSelector] List<Effect> onCollectionEffects;
    [SerializeField] float delay;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected bool canBePickedUp = false;

    protected Player player;

    protected virtual IEnumerator Start()
    {
        player = Player.Instance;
        if (player == null) Debug.LogError("PLayer not found");
        if(rb ==null) rb = GetComponent<Rigidbody2D>();
        //Invoke(nameof(Delay), delay);
        yield return new WaitForSeconds(delay);
        canBePickedUp = true;
    }

    private void FixedUpdate()
    {
        if (InRange && canBePickedUp)
        {
                Gravitate(player.transform.position);
                AlignToVelocity(transform, rb, -transform.up);
            
        }
    }

    void Delay() => canBePickedUp = true;

    protected virtual void Gravitate(Vector3 position)
    {
        Vector2 dir = (position - transform.position).normalized;

        Vector2 targetSpeed = dir.normalized * gravitateSpeed;
        Vector2 speedDif = targetSpeed - rb.linearVelocity;
        rb.AddForce(speedDif, ForceMode2D.Impulse); // impulse feels more snappy but FORCE feels more floaty
    }

    public static void AlignToVelocity(Transform target , Rigidbody2D rb, Vector2 localDirection, float rotationSpeed = 600f)
    {
        if (rb.linearVelocity.sqrMagnitude < 0.0001f)
            return;

        Vector2 velocityDir = rb.linearVelocity.normalized;
        Vector2 currentDir = target.TransformDirection(localDirection);

        float angle = Vector2.SignedAngle(currentDir, velocityDir);
        float newAngle = Mathf.MoveTowardsAngle(
            target.eulerAngles.z,
            target.eulerAngles.z + angle,
            rotationSpeed * Time.deltaTime);

        target.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }


    //protected virtual void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (canBePickedUp)
    //    {
    //        if (collision.CompareTag("Player"))
    //        {
    //            Pickup(collision.GetComponent<Player>());
    //            Destroy(gameObject);
    //        }
    //    }
    //}

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (canBePickedUp)
        {
            if (collision.CompareTag("Player"))
            {
                Pickup(collision.GetComponent<Player>());
                Destroy(gameObject);
            }
        }
    }


    protected virtual void Pickup(Player player)
    {
        // nothing here for now;
        EffectContext cntxt = new EffectContext(gameObject, player.gameObject, transform.position, transform.right);
        foreach (Effect effect in onCollectionEffects)
        {
            effect.Apply(cntxt);
        }
    }

}
