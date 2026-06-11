using UnityEngine;
using System.Collections.Generic;
using EditorAttributes;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

//THINGS TO ADD
// scoring function to score possible positions

public class RoomAssembler : MonoBehaviour
{
    [FoldoutGroup("Generation Attributes", true, nameof(noOfRooms),nameof(possibleNoOfAnchorRooms),nameof(maxConnections), nameof(anchorRoomsWeight),nameof(maxAnchorDist), nameof(roomPosSearchRad),nameof(maxAttemptsToFindPos),nameof(seed),nameof(useRandomSeed))]
    [SerializeField] private EditorAttributes.Void groupHolder;

    [SerializeField, HideProperty] List<int> possibleNoOfAnchorRooms = new List<int>();
    [SerializeField, HideProperty, Range(0f,1f)] List<float> anchorRoomsWeight = new List<float>();
    [SerializeField, HideProperty] float maxAnchorDist;
    [SerializeField, HideProperty] int maxConnections;
    [SerializeField, HideProperty] int maxAttemptsToFindPos;
    [SerializeField, HideProperty] int seed;
    [SerializeField, HideProperty] bool useRandomSeed;
    [SerializeField,HideProperty] int noOfRooms;
    [SerializeField,HideProperty] int roomPosSearchRad;


    [SerializeField] RoomGenerator generator;
    [SerializeField] List<Room> placedRooms = new List<Room>(); //TODO REPLACE DEBUGGER WITH ROOM SCRIPT
    [SerializeField] Room roomPrefab;
    [SerializeField] Vector2Int firstRoomPos;

    System.Random prng;
    [SerializeField]List<RoomData> availaleRoomDatas;

    public static Action<IReadOnlyList<Room>> EOnAssemblyFinished;
    public static Action EClearGeneration;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generator = FindFirstObjectByType<RoomGenerator>();
        if (useRandomSeed) seed = UnityEngine.Random.Range(0, 10000);
        prng = new System.Random(seed);

        StartAssembly();
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
        if (!ValidateSettings())
        {
            Debug.LogError("Invalid assembler setup.");
            return;
        }

        availaleRoomDatas = GetAllRoomsFromGenerator(noOfRooms);
        PlaceLinearRoom();
        PlaceLinearRoom();

        int roomsLeft = noOfRooms - placedRooms.Count;
        for (int i = 0; i < roomsLeft; i++)
        {
            PlaceRoomWithRandomAnchors();
        }

        if (ValidateDungeon())
        {
            EOnAssemblyFinished?.Invoke(placedRooms);
        }
    }

    bool ValidateSettings()
    {
        return generator != null
            && roomPrefab != null
            && noOfRooms > 0;
    }
    bool  ValidateDungeon()
    {
        if(placedRooms.Count != noOfRooms) { Debug.LogError("Not Enough Rooms Placed!"); HandleInvalidDungeon(); return false; }
        return true;
    }

    void PlaceRoomWithRandomAnchors()
    {
        if (placedRooms.Count > noOfRooms) return;
        RoomData data = GetRandomRoomData();
        List<Room> anchors = GetAnchorRooms(Helper.WeightedChoice(possibleNoOfAnchorRooms, anchorRoomsWeight, prng));

        Vector2Int position = GetRoomPositionUsingAnchors( data.BoundingBox, anchors);

        PlaceRoomAtPosition(data, position, anchors);

    }

    Vector2Int GetRoomPositionUsingAnchors(Bounds bounds, List<Room> anchors)
    {
        //firs calc mean pos of anchor then
        if (anchors.Count == 0)
        {
            throw new InvalidOperationException("No Anchors provided to spawn room");
        }
        Vector3 meanPos = Vector3.zero;
        foreach (Room anchor in anchors)
        {
            meanPos += anchor.GlobalPosition;
        }
        meanPos /= anchors.Count;

        return GetValidRoomPosNear(bounds, meanPos.ToV2().ToV2Int());
    }

    List<Room> GetAnchorRooms(int count)
    {


        List<Room> anchors= new List<Room>();

        //pick anchor room and make sure its not too crowded
        List<Room> validRooms = placedRooms.Where(r => r.ConnectedRooms.Count < maxConnections).ToList();
        if (validRooms.Count == 0)
        {
            Debug.LogWarning("No valid anchor rooms found.");
            return anchors;
        }
        Room primary = validRooms.Choice(prng);
        anchors.Add(primary);


        List<Room> candidates = GetNearbyRoomsInRadius(primary.GlobalPosition, maxAnchorDist);
        candidates = candidates.Where(r => r.ConnectedRooms.Count < maxConnections && r!=primary).ToList() ;
        if(candidates.Count == 0) { Debug.LogWarning("No candidates found around primary anchor"); return anchors; }

        count = Math.Min(candidates.Count, count); 

        for (int i = 0; anchors.Count<count; i++)
        {
            Room choice = candidates.Choice(prng);
            anchors.Add(choice);
            candidates.Remove(choice);
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
        if (result.Count == 0) Debug.LogWarning($"No Rooms found near {center}");
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
            spawnPos = GetValidRoomPosNear(data.BoundingBox, placedRooms[placedRooms.Count - 1].transform.position.ToV2().ToV2Int());
        }
        List<Room> connections = (placedRooms.Count == 0) ? new() : new() { placedRooms[^1] }; //rooms[^1] = last element
        PlaceRoomAtPosition(data,spawnPos, connections);

    }

    RoomData GetRandomRoomData()
    {
        if(availaleRoomDatas.Count == 0) throw new InvalidOperationException("All RoomDatas have been used");
        RoomData data = availaleRoomDatas.Choice(prng);
        availaleRoomDatas.Remove(data);
        return data;
    } 

    Vector2Int GetValidRoomPosNear(Bounds bounds, Vector2Int center, [CallerMemberName] string caller = "")
    {
        for (int i = 0; i < maxAttemptsToFindPos; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * roomPosSearchRad + center;

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

        Debug.LogWarning($"Nothing Found when trying to find room Pos near {center} by {caller}");
        HandleInvalidDungeon();
        return center;
    }

    public void HandleInvalidDungeon()
    {
        throw new Exception("DUNGEON GENERATION INVALID");
    }

    [Button("CLear Generation")]
    void Clear()
    {
        foreach(Room room in placedRooms)
        {
            Destroy(room.gameObject);
            
        }
        placedRooms.Clear();
        EClearGeneration?.Invoke();
    }

}
