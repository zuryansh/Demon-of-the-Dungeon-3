using UnityEngine;

public class Door : Teleporter
{
    public Room AttatchedRoom => attatchedRoom;
    public Room TeleportToRoom => teleportToRoom;
    public Teleporter TeleportToteleporter => teleportToTeleporter;
    [SerializeField] Room attatchedRoom;
    [SerializeField] Room teleportToRoom;
    [SerializeField] Teleporter teleportToTeleporter;

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

    public void SetTeleportToTeleporter(Teleporter teleportToTeleporter) 
    {
        this.teleportToTeleporter = teleportToTeleporter;
    }


}
