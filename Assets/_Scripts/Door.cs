using UnityEngine;

public class Door : Teleporter
{

    [SerializeField] Room attatchedRoom;
    [SerializeField] Room teleportToRoom;


    public void Init(Room attachedRoom, Room teleportToRoom)
    {
        this.attatchedRoom = attachedRoom;
        this.teleportToRoom = teleportToRoom;

        teleportTo = teleportToRoom.transform.position;
    }

    protected override void Teleport(GameObject obj)
    {
        base.Teleport(obj);
        if (obj.CompareTag("Player"))
        {
            attatchedRoom?.DeactivateRoom();
            teleportToRoom?.ActivateRoom();
        }
    }
}
