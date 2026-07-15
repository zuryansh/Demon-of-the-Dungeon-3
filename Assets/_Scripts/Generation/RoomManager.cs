using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class RoomManager : Singleton<RoomManager>
{
    public Action<RoomManager> EOnAllRoomsCleared;

    [SerializeField] List<Room> rooms = new List<Room>();
    [SerializeField] HashSet<Room> clearedRooms = new HashSet<Room>();
    [SerializeField] Door entryDoor;
    public Room ActiveRoom=> rooms.Where(r => r.hasPlayer == true).FirstOrDefault();


    private void Start()
    {

    }


    private void OnEnable()
    {
        RoomAssembler.EOnAssemblyFinished += OnAssemblyFinish;
    }

    private void OnDisable()
    {
        RoomAssembler.EOnAssemblyFinished -= OnAssemblyFinish;

    }

    void OnAssemblyFinish(IReadOnlyList<Room> r)
    {
        rooms = r as List<Room>;
        if (entryDoor != null)
        {
            entryDoor.Init(null, rooms[0]);
        }

        Room.EonRoomClear += RoomCleared;
    }

    public void RoomCleared(Room room)
    {
        clearedRooms.Add(room);
        if (clearedRooms.Count == rooms.Count) AllRoomsCleared();
    }

    void AllRoomsCleared()
    {
        EOnAllRoomsCleared?.Invoke(this);
        UIManager.Instance.OnGameWin();
    }
}
