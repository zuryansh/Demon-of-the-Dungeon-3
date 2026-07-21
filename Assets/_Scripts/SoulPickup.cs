using UnityEngine;

public class SoulPickup : LootItem
{
    [SerializeField] int value;


    protected override void Pickup(Player player)
    {
        base.Pickup(player);
        player.AddPoints(value);

    }
}
