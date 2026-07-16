using UnityEngine;

public class TreasureRoom : Room
{
    [SerializeField] Interactable treasurePrefab;


    Interactable treasure;

    public override void ActivateRoom()
    {
        base.ActivateRoom();
        SpawnTreasure();
    }

    void SpawnTreasure()
    {
        Vector2 spawnPos = GetGlobalTilePos(GetRandomRoomTile(true));
        treasure = Instantiate(treasurePrefab, spawnPos, Quaternion.identity);
        treasure.EOnInteract += RoomCleared;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(treasure!=null) treasure.EOnInteract -= RoomCleared;
    }
}
