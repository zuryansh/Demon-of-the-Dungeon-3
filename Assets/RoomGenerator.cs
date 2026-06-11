using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using static UnityEngine.UI.Image;
using static UnityEditor.PlayerSettings;
using UnityEditor.Tilemaps;

//Room Generator and editor ONLY HAS INT MAP
//all other systems use room Data after it has been generated from int map
// STEPS
//generate room
// generate a room data
// make necessary editions
//only pass data when edittions are done 




public enum GenerationAlgo { SimpleWalker, RandomNoise,PerlinNoise}
//encorprate TileTypes into the HashSet somehow

public class RoomGenerator : MonoBehaviour
{
    [FoldoutGroup("Generation Attributes", true, nameof(algoUsed), nameof(iterations), nameof(walklength), nameof(mapWidth), nameof(mapHeight), nameof(smoothing), nameof(smoothingIterations), nameof(smoothingCutoff), nameof(walkerCount), nameof(minRoomSize), nameof(useRandomSeed), nameof(seed),nameof(tilePallete))]
    [SerializeField] private Void groupHolder;

    public HashSet<Vector2Int> WalkerStartPositions => walkerStartPositions;
    public int MapWidth => mapWidth;
    public int MapHeight => mapHeight;
    public int[,] Map => map;   

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
    [SerializeField, HideProperty] RoomPalleteSO tilePallete;

    [SerializeField] TileTypes debugTileLayer;
    [SerializeField] bool ShowDebug = true;
    [SerializeField, Range(0, 100)] int fillPercent;
    [SerializeField, Range(0f, 1f)] float cutoff;
    [SerializeField] RoomData roomData;
    [SerializeField] RoomDataDebugger roomDebugger;

    int[,] map;
    System.Random prng;

    HashSet<Vector2Int> walkerStartPositions = new HashSet<Vector2Int>();

    private void Start()
    {
        StartProcess();
    }

    void StartProcess()
    {
        //generate
        GenerateRoom();
    }

    void GenerateRoom()
    {
        while (true)
        {
            if (useRandomSeed) seed = Random.Range(0, 10000);
            prng = new System.Random(seed);

            map = Helper.CreateEmpty2dArray(mapHeight, mapWidth, ((int)TileTypes.Air));
            walkerStartPositions = new HashSet<Vector2Int>();

            GenerateFloorTiles();

            Smooth(((int)TileTypes.Floor), ((int)TileTypes.Air));

            GenerateWalls();

            if (ValidateRoom()) break;
        }
        UpdateRoomData();
    }


    bool ValidateRoom()
    {
        int noOfFloorTilesInRoom=0;
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == ((int)TileTypes.Floor)) 
                noOfFloorTilesInRoom++;
            }
        }

        int floodFillCount = Helper.GetFloodFill(map, walkerStartPositions.AtIndex(0),((int)TileTypes.Floor)).Count;
        if(floodFillCount!=noOfFloorTilesInRoom) return false;
        else return true;
    }

    RoomData GenerateRoomData(Vector2Int origin, int[,] map)
    {
        List<RoomTile> shiftedTiles = new List<RoomTile>();

        //shift by origin
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == ((int)TileTypes.Air)) continue;
                Vector2Int pos = new Vector2Int(i - origin.x, j - origin.y);
                shiftedTiles.Add(new RoomTile(pos, (TileTypes)map[i, j]));
            }
        }

        return new RoomData(shiftedTiles,tilePallete);
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
                //if (useRandomSeed) seed = Random.Range(0, 10000);
                int walkerStartX = prng.Next(5, (int)(map.GetLength(0) / 1.15f));
                int walkerStartY = prng.Next(5, (int)(map.GetLength(1) / 1.15f));


                walkerStartPositions.Add(new Vector2Int(walkerStartX, walkerStartY));
                map[walkerStartX, walkerStartY] = ((int)TileTypes.Floor);
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

    public void SetTile(int x, int y, TileTypes fill)
    {
        if (x >= mapWidth || x < 0 || y >= mapWidth || y < 0) return;

        map[x, y] = ((int)fill) ;   
    }

    public RoomData GetNewRoom() 
    {
        
        GenerateRoom();
        return roomData;
    }

    [Button("Regenerate Walls")]
    void GenerateWalls()
    {
        //loop through each floor pos 
        // check each cardinal direction and if there is no floor add wall instead
        for (int i = 1; i < map.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j++)
            {
                if (map[i, j] == ((int)TileTypes.Floor))
                {
                    //loop through each direction
                    if (map[i + 1, j] == 0) map[i + 1, j] = ((int)TileTypes.Wall); //right
                    if (map[i - 1, j] == 0) map[i - 1, j] = ((int)TileTypes.Wall); //left
                    if (map[i, j + 1] == 0) map[i, j + 1] = ((int)TileTypes.Wall); //up
                    if (map[i, j - 1] == 0) map[i, j - 1] = ((int)TileTypes.Wall); //down

                    if (map[i + 1, j + 1] == 0) map[i + 1, j + 1] = ((int)TileTypes.Wall); //top right
                    if (map[i + 1, j - 1] == 0) map[i + 1, j - 1] = ((int)TileTypes.Wall); //bottom right
                    if (map[i - 1, j - 1] == 0) map[i - 1, j - 1] = ((int)TileTypes.Wall); //bottom left
                    if (map[i - 1, j + 1] == 0) map[i - 1, j + 1] = ((int)TileTypes.Wall); //top left
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (ShowDebug && map != null)
        {
            if (mapHeight != 0 && mapWidth != 0)
            {
                //DRAW BORDER
                Gizmos.color = Color.black;
                for (int i = 0; i < MapWidth; i++) { Gizmos.DrawCube(new Vector3(i+0.5f, 0.5f), Vector3.one); Gizmos.DrawCube(new Vector3(i + 0.5f, mapHeight + 0.5f), Vector3.one); }
                for (int i = 0; i < MapHeight; i++) { Gizmos.DrawCube(new Vector3(0.5f, i + 0.5f), Vector3.one); Gizmos.DrawCube(new Vector3(mapWidth + 0.5f, i+0.5f), Vector3.one); }


            }

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    int val = map[i, j];
                    if (val == ((int)TileTypes.Air)) continue;
                    else if (val == ((int)TileTypes.Floor)) Gizmos.color = Color.white;
                    else if (val == ((int)TileTypes.Wall)) Gizmos.color = Color.black;
                    Gizmos.DrawCube(new Vector3(i + 0.5f, j + 0.5f, 0), Vector3.one);
                }
            }

            //Gizmos.color = Color.white;
            //foreach(Vector2Int pos in roomFilledTiles)
            //{
            //    Gizmos.DrawCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Vector3.one);
            //}

            //Gizmos.color = Color.red;
            //foreach (Vector2Int pos in walkerStartPositions)
            //{
            //   Gizmos.DrawCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Vector3.one);
            //}
        }
    }
}

