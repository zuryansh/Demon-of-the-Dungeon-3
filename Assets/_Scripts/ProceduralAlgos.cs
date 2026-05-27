
using System;
using UnityEngine;

public static class ProceduralGenerationAlgorithims
{


    public static int[,] SimpleRandomWalk(int[,] map, int startX, int startY, int walkLenght, int iterations, int fillVal, int seed)
    {
        System.Random rng = new System.Random(seed);

        //fill the start position fully so it stays after smoothing iterations
        for (int neighbourX = startX - 1; neighbourX <= startX + 1; neighbourX++)
        {
            for (int neighbourY = startY - 1; neighbourY <= startY + 1; neighbourY++)
            {
                map[neighbourX, neighbourY] = fillVal;
            }
        }
        int prevX = startX;
        int prevY = startY;
        int xindex = 0;
        int yindex = 0;


        for (int j = 0; j < iterations; j++)
        {
            xindex = prevX;
            yindex = prevY;
            //xindex = startX;
            //yindex = startY;
            for (int i = 0; i < walkLenght; i++)
            {


                int n = rng.Next(0, 4);
                if (n == 0) xindex++; //right
                else if (n == 1) xindex--; //left
                else if (n == 2) yindex--; //down
                else if (n == 3) yindex++; //up
                if (!(xindex >= map.GetLength(0) - 5 || yindex >= map.GetLength(1) - 5 || xindex <= 0 || yindex <= 0))
                {

                    prevX = xindex;
                    prevY = yindex;
                    map[xindex, yindex] = fillVal;
                }

            }
        }

        return map;
    }

    public static int[,] RandomNoise(int[,] map, int seed, int randomFillPercent, int fillVal, int emptyVal)
    {
        System.Random rng = new System.Random(seed);

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = (rng.Next(0, 100) < randomFillPercent) ? fillVal : emptyVal;
            }
        }


        return map;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="seed"></param>
    /// <param name="cutoff"> btwn 0 and 1</param>
    /// <returns></returns>
    public static int[,] PerlinNoise(int[,] map, int seed, float cutoff, int fillVal, int emptyVal)
    {
        System.Random rng = new System.Random(seed);
        double offsetx = rng.Next(100);
        double offsety = rng.Next(100);
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                float x = (float)i / map.GetLength(0);
                float y = (float)j / map.GetLength(1);
                map[i, j] = (Mathf.PerlinNoise(x+((float)offsetx),y+((float)offsety)) < cutoff)? fillVal : emptyVal;
            }
        }


        return map;
    }

}
