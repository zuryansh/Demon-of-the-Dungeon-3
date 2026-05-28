using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using Microsoft.Win32.SafeHandles;

public enum TileTypes { Air = 0, Floor = 1, Wall = 2, Border = 3 , StartPos=4}
public enum GenerationAlgo { SimpleWalker, RandomNoise,PerlinNoise}
//encorprate TileTypes into the HashSet somehow

public class RoomGenerator : MonoBehaviour
{
    [FoldoutGroup("Generation Attributes", true, nameof(algoUsed), nameof(iterations), nameof(walklength), nameof(mapWidth), nameof(mapHeight), nameof(smoothing), nameof(smoothingIterations), nameof(smoothingCutoff), nameof(walkerCount), nameof(minRoomSize), nameof(useRandomSeed), nameof(seed))]
    [SerializeField] private EditorAttributes.Void _;

    public HashSet<Vector2Int> WalkerStartPositions => walkerStartPositions;
    public int MapWidth => mapWidth;
    public int MapHeight => mapHeight;
    public HashSet<Vector2Int> RoomFloorTiles => roomFloorTiles;

    [SerializeField, HideProperty] int iterations = 10;
    [SerializeField, HideProperty] int walklength = 10;
    [SerializeField, HideProperty] int mapWidth;
    [SerializeField, HideProperty] int mapHeight;
    [SerializeField, HideProperty] bool smoothing;
    [SerializeField, HideProperty] int smoothingIterations;
    [SerializeField, HideProperty] private int walkerCount;
    [SerializeField, HideProperty] int minRoomSize;
    [SerializeField, HideProperty] int seed;
    [SerializeField, HideProperty] GenerationAlgo algoUsed;
    [SerializeField, HideProperty] bool useRandomSeed;
    [SerializeField, HideProperty] int smoothingCutoff;

    [SerializeField] TileTypes debugTileLayer;
    [SerializeField] bool ShowDebug = true;
    [SerializeField, Range(0, 100)] int fillPercent;
    [SerializeField, Range(0f, 1f)] float cutoff;
    [SerializeField] RoomData roomData;
    [SerializeField] RoomDataDebugger roomDebugger;

    int[,] map;
    HashSet<Vector2Int> walkerStartPositions = new HashSet<Vector2Int>();
    HashSet<Vector2Int> roomFloorTiles = new HashSet<Vector2Int>();

    private void Start()
    {
        GenerateRoom();
    }

    void GenerateRoom()
    {
        if (useRandomSeed) seed = Random.Range(0, 10000);

        map = Helper.CreateEmpty2dArray(mapHeight, mapWidth, ((int)TileTypes.Air));
        walkerStartPositions = new HashSet<Vector2Int>();

        GenerateFloorTiles();

        Smooth(((int)TileTypes.Floor), ((int)TileTypes.Air));

        UpdateRoomData();
    }




    RoomData GenerateRoomData(Vector2Int origin, int[,] map)
    {
        HashSet<Vector2Int> shiftedTiles = new HashSet<Vector2Int>();
        roomFloorTiles = Helper.GetFloodFill(map, origin, ((int)TileTypes.Floor)); 

        //shift by origin
        foreach (Vector2Int tile in roomFloorTiles)
        {
            Vector2Int pos = new Vector2Int(tile.x - origin.x, tile.y - origin.y);
            shiftedTiles.Add(pos);
        }
        return new RoomData(origin, shiftedTiles);
    }



    void Smooth(int fillValue, int emptyVal)
    {
        if (smoothing)
        {
            for (int _ = 0; _ < smoothingIterations; _++)
            {
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        int tileCount = Helper.GetSurroundingTileCount(map, i, j, fillValue, mapWidth, mapHeight);

                        if (tileCount > smoothingCutoff) map[i, j] = fillValue;
                        else if (tileCount < smoothingCutoff) map[i, j] = emptyVal;
                    }
                }
            }
        }

    }

    private void GenerateFloorTiles()
    {
        if (algoUsed == GenerationAlgo.SimpleWalker)
        {

            for (int i = 0; i < walkerCount; i++)
            {
                if (useRandomSeed) seed = Random.Range(0, 10000);
                int walkerStartX = (int)UnityEngine.Random.Range(5, map.GetLength(0) / 1.15f);
                int walkerStartY = (int)UnityEngine.Random.Range(5, map.GetLength(1) / 1.15f);


                walkerStartPositions.Add(new Vector2Int(walkerStartX, walkerStartY));
                map[walkerStartX, walkerStartY] = ((int)TileTypes.StartPos);
                map = ProceduralGenerationAlgorithims.SimpleRandomWalk(map, walkerStartX, walkerStartY, walklength, iterations, ((int)TileTypes.Floor), seed);
            }

        }
        else if (algoUsed == GenerationAlgo.RandomNoise)
        {
            map = ProceduralGenerationAlgorithims.RandomNoise(map, seed, fillPercent, ((int)TileTypes.Floor), ((int)TileTypes.Air));
        }
        else if (algoUsed == GenerationAlgo.PerlinNoise)
        {
            map = ProceduralGenerationAlgorithims.PerlinNoise(map, seed, cutoff, ((int)TileTypes.Floor), ((int)TileTypes.Air));
        }
    }

    [Button("Generate")]
    public void RestartGeneration()
    {
        GenerateRoom();
    }

    [Button("Update Room Data")]
    void UpdateRoomData()
    {
        roomData = GenerateRoomData(walkerStartPositions.AtIndex(0), map);
        roomDebugger.RoomData = roomData;
    }

    public void SetTile(int x, int y, bool filled, TileTypes layer)
    {
        if (x >= mapWidth || x < 0 || y >= mapWidth || y < 0) return;

        if (layer == TileTypes.Floor)
        {
            if (filled) 
            { 
                roomFloorTiles.Add(new Vector2Int(x, y));
                map[x, y] = ((int)layer);
            }
            else 
            {
                if (roomFloorTiles.Contains(new Vector2Int(x, y))) { roomFloorTiles.Remove(new Vector2Int(x, y)); map[x, y] = ((int)TileTypes.Air); }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (ShowDebug && map != null)
        {
            if (mapHeight != 0 && mapWidth != 0)
            {
                //for (int i = 0; i < map.GetLength(0); i++)
                //{
                //    for (int j = 0; j < map.GetLength(1); j++)
                //    {
                //        Gizmos.color = Color.white;
                //        if (map[i, j] == ((int)debugTileLayer)) Gizmos.DrawCube(new Vector3(i + 0.5f, j + 0.5f, 0), Vector3.one);
                //    }
                //}
                //DRAW BORDER
                Gizmos.color = Color.black;
                for (int i = 0; i < MapWidth; i++) { Gizmos.DrawCube(new Vector3(i+0.5f, 0.5f), Vector3.one); Gizmos.DrawCube(new Vector3(i + 0.5f, mapHeight + 0.5f), Vector3.one); }
                for (int i = 0; i < MapHeight; i++) { Gizmos.DrawCube(new Vector3(0.5f, i + 0.5f), Vector3.one); Gizmos.DrawCube(new Vector3(mapWidth + 0.5f, i+0.5f), Vector3.one); }


            }
            Gizmos.color = Color.white;
            foreach(Vector2Int pos in roomFloorTiles)
            {
                Gizmos.DrawCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Vector3.one);
            }

            Gizmos.color = Color.red;
            foreach (Vector2Int pos in walkerStartPositions)
            {
               Gizmos.DrawCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Vector3.one);
            }
        }
    }
}
