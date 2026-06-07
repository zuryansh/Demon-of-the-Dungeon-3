using UnityEngine;
using System.Collections.Generic;
using EditorAttributes;
using System;
using System.Runtime.InteropServices;

//1.Place first room.
//2.Place second near first.
//3. Pick 1-3 anchor rooms. (near a random primary room)
//4. Compute anchor center. 
//5. Generate candidate positions around center.
//6. Reject overlaps.
//7. Connect to anchors.

public class RoomAssembler : MonoBehaviour
{
    [FoldoutGroup("Generation Attributes", true, nameof(possibleNoOfAnchorRooms),nameof(maxConnections), nameof(anchorRoomsWeight),nameof(maxAnchorDist), nameof(maxAttemptsToFindPos),nameof(seed),nameof(useRandomSeed))]
    [SerializeField] private EditorAttributes.Void groupHolder;



    [SerializeField, HideProperty] List<int> possibleNoOfAnchorRooms = new List<int>();
    [SerializeField, HideProperty, Range(0f,1f)] List<float> anchorRoomsWeight = new List<float>();
    [SerializeField, HideProperty] float maxAnchorDist;
    [SerializeField, HideProperty] int maxConnections;
    [SerializeField, HideProperty] int maxAttemptsToFindPos;
    [SerializeField, HideProperty] int seed;
    [SerializeField, HideProperty] bool useRandomSeed;


    [SerializeField] RoomGenerator generator;
    [SerializeField] List<Room> placedRooms = new List<Room>(); //TODO REPLACE DEBUGGER WITH ROOM SCRIPT
    [SerializeField] Room roomPrefab;
    [SerializeField] int noOfRooms;
    [SerializeField] Vector2Int firstRoomPos;
    [SerializeField] int spacing;

    System.Random prng;
    [SerializeField]List<RoomData> datas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generator = FindFirstObjectByType<RoomGenerator>();
        if (useRandomSeed) seed = UnityEngine.Random.Range(0, 10000);
        prng = new System.Random(seed);
    }

    List<RoomData> GetAllRoomsFromGenerator(int n)
    {
        List<RoomData> result = new List<RoomData>();
        for (int i = 0; i < n; i++)
        {
            result.Add(generator.GetNewRoom());
        }
        return result;
    }

    [Button("Start Assembly")]
    void StartAssembly()
    {
        datas = GetAllRoomsFromGenerator(noOfRooms);
        PlaceLinearRoom();
        PlaceLinearRoom();

        int roomsLeft = noOfRooms - placedRooms.Count;
        for (int i = 0; i < roomsLeft; i++)
        {
            PlaceRoomWithRandomAnchors();
        }
    }

    void PlaceRoomWithRandomAnchors()
    {
        if (placedRooms.Count > noOfRooms) return;
        //MAKE EXCEPTIONS FOR FIRST FEW ROOMS
        RoomData data = GetRandomRoomData();
        List<Room> anchors = GetAnchorRooms(Helper.WeightedChoice(possibleNoOfAnchorRooms, anchorRoomsWeight, prng));

        Vector2Int position = GetRoomPositionUsingAnchors( data.BoundingBox, anchors);

        PlaceRoomAtPosition(data, position, anchors);

    }

    Vector2Int GetRoomPositionUsingAnchors(Bounds bounds, List<Room> anchors)
    {
        //firs calc mean pos of anchor then
        Vector3 meanPos = Vector3.zero;
        foreach (Room anchor in anchors)
        {
            meanPos += anchor.GlobalPosition;
        }
        meanPos /= anchors.Count;

        return GetRoomPosNear(bounds, meanPos.ToV2().ToV2Int());
    }

    List<Room> GetAnchorRooms(int count)
    {
        List<Room> GetNRoomsInRadius(Vector3 center, float maxAnchorDist)
        {
            List<Room> candidates = GetNearbyRoomsInRadius(center, maxAnchorDist);
            while (count > candidates.Count) { count--; } //make sure count is not more than total possible candidates
            return candidates;
        }

        List<Room> anchors= new List<Room>();
        //pick random anchor and then search for few around them in a rad;
        Room primary = placedRooms.Choice(prng);
        while (primary.ConnectedRooms.Count > maxConnections) primary = placedRooms.Choice(prng); //dont want primary with too many connections
        anchors.Add(primary);

        List<Room> candidates = GetNearbyRoomsInRadius(primary.GlobalPosition, maxAnchorDist);
        

        for (int i = 0; i < count-1; i++)
        {
            // add checking for too many connetions in candidate like with primary but make sure it picks them if none are found
            Room choice = candidates.Choice(prng);
            anchors.Add(choice);
        }

        return anchors;

    }

    List<Room> GetNearbyRoomsInRadius(Vector3 center,float maxAnchorDist)
    {

        List<Room> result = new List<Room>();
        foreach (Room room in placedRooms)
        {
            
            if((room.GlobalBounds.ClosestPoint(center) - center).sqrMagnitude < maxAnchorDist * maxAnchorDist)
            {
                //room is close enough
                result.Add(room);
            }
        }
        return result;
    }

    public void PlaceRoomAtPosition(RoomData data,Vector2Int spawnPos, List<Room> connections)
    {


        Room spawnedRoom = Instantiate(roomPrefab, spawnPos.ToV3(), Quaternion.identity);

        spawnedRoom.Init(data, connections);

        placedRooms.Add(spawnedRoom);
    }

    [Button("Place Linear Room")]
    void PlaceLinearRoom()
    {
        if (placedRooms.Count > noOfRooms) return;  
        Vector2Int spawnPos;
        RoomData data = GetRandomRoomData();
        if (placedRooms.Count == 0)
        {
            //place first room
            spawnPos = firstRoomPos;
        }
        else
        {
            spawnPos = GetRoomPosNear(data.BoundingBox, placedRooms[placedRooms.Count - 1].transform.position.ToV2().ToV2Int());
        }
        List<Room> connections = (placedRooms.Count == 0) ? new() : new() { placedRooms[^1] }; //rooms[^1] = last element
        PlaceRoomAtPosition(data,spawnPos, connections);

    }

    RoomData GetRandomRoomData()
    {
        if(datas.Count == 0) throw new InvalidOperationException("All RoomDatas have been used");
        RoomData data = datas.Choice(prng);
        datas.Remove(data);
        return data;
    } 

    Vector2Int GetRoomPosNear(Bounds bounds, Vector2Int center)
    {
        for (int i = 0; i < maxAttemptsToFindPos; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * spacing + center;

            bool intersects = false;

            foreach (Room room in placedRooms)
            {
                if (room.Data.BoundingBox
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
        return center;
    }

    [Button("CLear Generation")]
    void Clear()
    {
        foreach(Room room in placedRooms)
        {
            Destroy(room.gameObject);
            
        }
        placedRooms.Clear();
    }

}
