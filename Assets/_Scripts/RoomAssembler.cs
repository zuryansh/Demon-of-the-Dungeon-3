using UnityEngine;
using System.Collections.Generic;
using EditorAttributes;

public class RoomAssembler : MonoBehaviour
{
    [SerializeField] RoomGenerator generator;
    [SerializeField] List<RoomDataDebugger> rooms = new List<RoomDataDebugger>(); //TODO REPLACE DEBUGGER WITH ROOM SCRIPT
    [SerializeField] RoomDataDebugger visualiserPrefab;
    [SerializeField] Vector2Int firstRoomPos;
    [SerializeField] int spacing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generator = FindFirstObjectByType<RoomGenerator>();

    }

    RoomData GetRoom()
    {
        return generator.GetNewRoom();
    }

    [Button("Place Room")]
    void PlaceRoom()
    {
        RoomData data = GetRoom();
        Vector2Int spawnPos;

        if(rooms.Count == 0)
        {
            //place first room
            spawnPos = firstRoomPos;
        }
        else
        {
            spawnPos = GetNextRoomPos(data.BoundingBox, rooms[rooms.Count-1].transform.position.ToV2().ToV2Int());
        }
        RoomDataDebugger visualiser = Instantiate(visualiserPrefab, spawnPos.ToV3(), Quaternion.identity);
        visualiser.RoomData = data;
        
        rooms.Add(visualiser);

    }

    //CHECK IF BOUNDS ARE ACC WITH POSITIONS
    Vector2Int GetNextRoomPos(Bounds bounds, Vector2Int prevPos)
    {
        int iterations = 100;
        for (int i = 0; i < iterations; i++)
        {
            Vector2 pos = Random.insideUnitCircle * spacing + prevPos;

            bool intersects = false;

            foreach (RoomDataDebugger room in rooms)
            {
                if (room.RoomData.BoundingBox
                        .LocalToGlobalBound(room.transform.position)
                        .Intersects(bounds.LocalToGlobalBound(pos)))
                {
                    intersects = true;
                    break;
                }
            }

            if (!intersects)
                return pos.ToV2Int();
        }

        Debug.LogWarning("NOTHING FOUND");
        return prevPos;
    }

    [Button("CLear Generation")]
    void Clear()
    {
        foreach(RoomDataDebugger visualiser in rooms)
        {
            Destroy(visualiser.gameObject);
            
        }
        rooms.Clear();
    }

}
