using UnityEngine;

public class SoulPickup : LootItem
{
    [SerializeField] int value;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void Pickup(Player player)
    {
        base.Pickup(player);
        player.AddPoints(value);

    }
}
