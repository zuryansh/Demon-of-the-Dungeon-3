using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class Teleporter : MonoBehaviour
{
    [SerializeField] Vector2 teleportTo;

    void Teleport(GameObject obj)
    {
        obj.transform.position = teleportTo;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Teleport(collision.gameObject);
    }

    public void SetTeleportTo(Vector2 pos) { teleportTo = pos; }
}
