using UnityEngine;

public class Door : Teleporter
{
    public Room AttatchedRoom => attatchedRoom;
    public Room TeleportToRoom => teleportToRoom;
    [SerializeField] Room attatchedRoom;
    [SerializeField] Room teleportToRoom;

    public void Init(Room attachedRoom, Room teleportToRoom)
    {
        this.attatchedRoom = attachedRoom;
        this.teleportToRoom = teleportToRoom;

        teleportTo = teleportToRoom.transform.position;
    }

    protected override void Teleport(GameObject obj, string tag = "Player")
    {
        if (obj.CompareTag("Player"))
        {
            base.Teleport(obj, tag);
            attatchedRoom?.DeactivateRoom();
            teleportToRoom?.ActivateRoom();
        }
    }

    
}
