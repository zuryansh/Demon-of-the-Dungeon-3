using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : Singleton<RoomManager>
{


    [SerializeField] List<Room> rooms;
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
    }



}
