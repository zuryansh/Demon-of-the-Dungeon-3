using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class RoomManager : Singleton<RoomManager>
{
    public Action<RoomManager> EOnAllRoomsCleared;

     List<Room> rooms = new List<Room>();
    [SerializeField] List<Room> clearedRooms = new List<Room>();
    [SerializeField] List<Room> unclearedRooms = new List<Room> ();
    [SerializeField] Door entryDoor;
    public Room ActiveRoom=> rooms.Where(r => r.HasPlayer == true).FirstOrDefault();


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
        unclearedRooms = r as List<Room>;
        if (entryDoor != null)
        {
            entryDoor.Init(null, rooms[0]);
        }

        Room.EonRoomClear += RoomCleared;
    }

    public void RoomCleared(Room room)
    {
        clearedRooms.Add(room);
        unclearedRooms.Remove(room);
        if (unclearedRooms.Count == 0)
        {
           print(" BUG CHECK");
            AllRoomsCleared();
        }
    }

    void AllRoomsCleared()
    {
        EOnAllRoomsCleared?.Invoke(this);
        UIManager.Instance.OnGameWin();
    }
}
