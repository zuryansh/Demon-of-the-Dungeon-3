using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData Data => data;

    [SerializeField] RoomTile originTile = new RoomTile(Vector2Int.zero, TileTypes.StartPos);
    [SerializeField] RoomDataDebugger debugger;
    [SerializeField] List<Room> connectedRooms= new List<Room>();



    RoomData data;

    public void Init(RoomData data, List<Room> connectedRooms)
    {
        this.data = data;
        this.connectedRooms = connectedRooms;

        foreach (Room room in connectedRooms)
        {
            if(!room.connectedRooms.Contains(this)) room.connectedRooms.Add(this);
        }
    }

    private void Start()
    {
        if(debugger != null && data != null) { debugger.RoomData = data; }
    }
    public void SetRoomData(RoomData data) { this.data = data; debugger.RoomData = data; }


    private void OnDrawGizmos()
    {
        if(connectedRooms.Count > 0)
        {
            Gizmos.color = Color.red;
            foreach(Room room in connectedRooms)
            {
                Gizmos.DrawLine(transform.position, room.transform.position);
            }
        }
    }



}
