using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    [SerializeField] Camera minimapCamera;
    [SerializeField] float padding;
    [SerializeField] LineRenderer lineRendererPrefab;
    [SerializeField] MapSprite roomMapSpritePrefab;
    [SerializeField] Color roomClearTint;

    Dictionary<Room, MapSprite> spriteLookup = new Dictionary<Room, MapSprite>();

    Bounds mapBounds = new Bounds();

    HashSet<(Room, Room)> drawnConnections = new HashSet<(Room, Room)>();


    private void OnEnable()
    {
        RoomAssembler.EOnAssemblyFinished += OnGenFinished;
    }

    void OnDisable()
    {
        RoomAssembler.EOnAssemblyFinished -= OnGenFinished;
    }

    void OnGenFinished(IReadOnlyList<Room> rooms)
    {
        StartCoroutine(GenMinimap(rooms));

    }

    IEnumerator GenMinimap(IReadOnlyList<Room> rooms)
    {

        yield return new WaitForEndOfFrame(); // wait for end of frame to let everything get populated first
        yield return new WaitForEndOfFrame(); // wait for room door to door connections to be populated
        foreach (Room room in rooms)
        {
            mapBounds.Encapsulate(room.GlobalBounds);
            room.EonPlayerExit += UpdateClearedRoomSprite;
        }

        FrameCameraToBound(minimapCamera, mapBounds, padding);
        DrawRoomBounds(rooms);
        DrawRoomConnections(rooms);
    }

    void DrawRoomBounds(IReadOnlyList<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            MapSprite mapSprite = Instantiate(roomMapSpritePrefab, room.GlobalPosition, Quaternion.identity);
            mapSprite.transform.SetParent(room.transform);
            mapSprite.ResizeToBounds(room.GlobalBounds);
            mapSprite.ChangeTint(room.MapSpriteTint);
            spriteLookup.Add(room,mapSprite);
        }
    }

    public static void FrameCameraToBound(Camera camera, Bounds bounds, float padding = 1f)
    {
        camera.transform.position = new Vector3(
            bounds.center.x,
            bounds.center.y,
            camera.transform.position.z);

        float vertical = bounds.extents.y;
        float horizontal = bounds.extents.x / camera.aspect;

        camera.orthographicSize = Mathf.Max(vertical, horizontal) + padding;
    }

    public void DrawRoomConnections(IReadOnlyList<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            foreach (Door door in room.Doors)
            {
                if(!(drawnConnections.Contains((door.TeleportToRoom, room)) || drawnConnections.Contains((room, door.TeleportToRoom))))
                {   

                    drawnConnections.Add((door.TeleportToRoom, room));
                    DrawConnection(door.transform.position, door.TeleportToteleporter.transform.position);
                }
                    
            }
        }
    }

    public void DrawConnection(Vector2 a, Vector2 b)
    {
        LineRenderer mapConnection = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
        Vector3[] points = { a, b };


        mapConnection.positionCount = points.Length;
        mapConnection.SetPositions(points);
        mapConnection.transform.SetParent(transform);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(mapBounds.center, mapBounds.size);
    }

    void UpdateClearedRoomSprite(Room room)
    {
        MapSprite mapSprite = spriteLookup[room];
        mapSprite.ChangeTint(roomClearTint);
    }
}
