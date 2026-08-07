using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    float resistance;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Apply(Vector2 force)
    {
        rb.AddForce(force * (1f - resistance), ForceMode2D.Impulse);
    }
}