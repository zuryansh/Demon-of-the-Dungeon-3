using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public static class Helper
{
    //by GPT
    public static List<string> SplitIntoPages(TMP_Text text, string[] paragraphs)
    {
        List<string> pages = new();

        foreach (string paragraph in paragraphs)
        {
            text.text = paragraph;
            text.pageToDisplay = 1;
            text.ForceMeshUpdate();
            for (int i = 0; i < text.textInfo.characterCount; i++)
            {
                Debug.Log($"{text.textInfo.characterInfo[i].character} : {text.textInfo.characterInfo[i].pageNumber}");
            }

            int pageCount = text.textInfo.pageCount;

            if (pageCount <= 1)
            {
                pages.Add(paragraph);
                continue;
            }

            TMP_TextInfo info = text.textInfo;

            for (int page = 0; page < pageCount; page++)
            {
                System.Text.StringBuilder sb = new();

                for (int i = 0; i < info.characterCount; i++)
                {
                    TMP_CharacterInfo character = info.characterInfo[i];

                    if (character.pageNumber == page)
                        sb.Append(character.character);
                }

                pages.Add(sb.ToString());
            }
        }

        return pages;
    }


    public static int[,] CreateEmpty2dArray(int height, int width, int defaultValue)
    {
        int[,] ar = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ar[i, j] = defaultValue;
            }
        }
        return ar;
    }

    /// <summary>
    /// Takes in a hash set of positions and returns the height and width of the boundig box of the positions
    /// </summary>
    /// <param name="hash"> The hash set of positions </param>
    /// <returns>[height, width] of bounding box of positions</returns>
    public static int[] FindDimensionsOfPositionHashSet(HashSet<Vector2Int> hash)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int pos in hash)
        {
            if (pos.x > maxX) maxX = pos.x;
            else if (pos.x < minX) minX = pos.x;
            if (pos.y > maxY) maxY = pos.y;
            else if (pos.y < minY) minY = pos.y;
        }

        int height = Math.Abs(maxY - minY) + 1;
        int width = Math.Abs(maxX - minX) + 1;
        return new int[2] { height, width };
    }



    public static Vector2Int[] GetPositionOf4Neighbours(Vector2Int startPos)
    {
        Vector2Int[] neighbours = new Vector2Int[4];
        neighbours[0] = startPos + new Vector2Int(0, 1);
        neighbours[1] = startPos + new Vector2Int(1, 0);
        neighbours[2] = startPos + new Vector2Int(0, -1);
        neighbours[3] = startPos + new Vector2Int(-1, 0);
        return neighbours;
    }

    
    //by GPT
    /// <summary>
    /// Gets flood Fill of all cells that are fill val
    /// </summary>
    /// <param name="map"></param>
    /// <param name="startPos"></param>
    /// <param name="fillVal"></param>
    /// <returns></returns>
    public static HashSet<Vector2Int> GetFloodFill( int[,] map, Vector2Int startPos, int fillVal)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();

        int width = map.GetLength(0);
        int height = map.GetLength(1);

        // bounds check
        if (startPos.x < 0 || startPos.x >= width ||
            startPos.y < 0 || startPos.y >= height)
        {
            return positions;
        }

        // starting cell invalid
        if (map[startPos.x, startPos.y] != fillVal)
        {
            return positions;
        }

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            // bounds check
            if (current.x < 0 || current.x >= width || current.y < 0 || current.y >= height) continue;
            // wrong tile type
            if (map[current.x, current.y] != fillVal) continue;

            // already visited
            if (positions.Contains(current)) continue;

            positions.Add(current);

            stack.Push(current + Vector2Int.up);
            stack.Push(current + Vector2Int.right);
            stack.Push(current + Vector2Int.down);
            stack.Push(current + Vector2Int.left);
        }

        return positions;
    }

    /// <summary>
    /// Gets Flood Fill of all cells that are not empty val
    /// </summary>
    /// <param name="map"></param>
    /// <param name="startPos"></param>
    /// <param name="emptyVal"></param>
    /// <returns></returns>
    public static HashSet<Vector2Int> GetFloodFillInverted( int[,] map, Vector2Int startPos, int emptyVal)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();

        int width = map.GetLength(0);
        int height = map.GetLength(1);

        // bounds check
        if (startPos.x < 0 || startPos.x >= width ||
            startPos.y < 0 || startPos.y >= height)
        {
            return positions;
        }

        // starting cell invalid
        if (map[startPos.x, startPos.y] == emptyVal)
        {
            return positions;
        }

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            // bounds check
            if (current.x < 0 || current.x >= width || current.y < 0 || current.y >= height) continue;
            // wrong tile type
            if (map[current.x, current.y] == emptyVal) continue;

            // already visited
            if (positions.Contains(current)) continue;

            positions.Add(current);

            stack.Push(current + Vector2Int.up);
            stack.Push(current + Vector2Int.right);
            stack.Push(current + Vector2Int.down);
            stack.Push(current + Vector2Int.left);
        }

        return positions;
    }

    public static Vector3 ToV3(this Vector2Int vector, float buffer = 0f)
    {
        return new Vector3(vector.x + buffer, vector.y + buffer, 0);
    }

    public static Vector2 ToV2(this Vector3 vector, float buffer = 0f)
    {
        return new Vector2(vector.x + buffer, vector.y + buffer);
    }

    public static Vector2Int ToV2Int(this Vector2 vector, float buffer = 0f)
    {
        return new Vector2Int(((int)(vector.x + buffer)), ((int)(vector.y + buffer)));
    }

    public static Vector3Int ToV3Int(this Vector2Int vector, int buffer = 0)
    {
        return new Vector3Int(vector.x + buffer, vector.y + buffer, 0);
    }
    public static Vector3Int ToV3Int(this Vector3 vector, int buffer = 0)
    {
        return new Vector3Int(((int)(vector.x + buffer)), ((int)(vector.y + buffer)), ((int)(vector.z + buffer)));
    }
    public static T AtIndex<T>(this HashSet<T> hashSet, int index)
    {
        int i = 0;
        foreach (T val in hashSet)
        {
            if (i == index) return val;
            i++;
        }
        return default(T);
    }

    public static Bounds LocalToGlobalBound(this Bounds local, Vector3 position)
    {
        Bounds global = local;
        global.center += position;
        return global;
    }
    public static int GetSurroundingTileCount(int[,] Map ,int gridX, int gridY, int tileVal, int MapWidth, int MapHeight)
    {
        int count = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < MapWidth && neighbourY >= 0 && neighbourY < MapHeight)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        if (Map[neighbourX, neighbourY] == tileVal) count++;
                    }
                }

            }
        }
        return count;
    }

    //By GPT
    public static T Choice<T>(this IList<T> collection)
    {
        if (collection == null)
            throw new System.ArgumentNullException(nameof(collection));

        if (collection.Count == 0)
            throw new System.InvalidOperationException(
                "Cannot choose from an empty collection.");

        return collection[UnityEngine.Random.Range(0, collection.Count)];
    }
    //By GPT

    public static T Choice<T>(this IList<T> collection, System.Random prng, [CallerMemberName] string caller = "")
    {
        if (collection == null)
            throw new System.ArgumentNullException(nameof(collection));

        if (collection.Count == 0)
            throw new System.InvalidOperationException(
                $"Cannot choose from an empty collection. {caller}");

        return collection[prng.Next(0, collection.Count)];
    }
    //By GPT

    public static T WeightedChoice<T>(IList<T> values, IList<float> weights, System.Random rng)
    {
        if (values == null)throw new ArgumentNullException(nameof(values));
        if (weights == null)throw new ArgumentNullException(nameof(weights));
        if (values.Count != weights.Count)throw new ArgumentException("Values and weights must have the same length.");
        if (values.Count == 0) throw new InvalidOperationException("Cannot choose from an empty collection.");

        float totalWeight = 0;

        foreach (float weight in weights)
        {
            if (weight < 0)
                throw new ArgumentException(
                    "Weights cannot be negative.");

            totalWeight += weight;
        }

        float roll = (float)rng.NextDouble() * totalWeight;

        for (int i = 0; i < values.Count; i++)
        {
            if (roll < weights[i])
                return values[i];

            roll -= weights[i];
        }

        return values[^1];
    }
    //By GPT

    public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
    {
        return (layerMask.value & (1 << gameObject.layer)) != 0;
    }
    //By GPT

    public static T RollLeastLikely<T>(List<T> values, List<float> probabilities)
    {
        if (values.Count != probabilities.Count)
            throw new ArgumentException("Values and probabilities must have the same length.");
        float roll = UnityEngine.Random.value;

        T result = default;
        float lowestPassingProbability = float.MaxValue;
        bool found = false;

        for (int i = 0; i < values.Count; i++)
        {
            float p = Mathf.Clamp01(probabilities[i]);

            if (roll <= p && p < lowestPassingProbability)
            {
                lowestPassingProbability = p;
                result = values[i];
                found = true;
            }
        }

        if (!found)
            throw new InvalidOperationException("No item passed the roll.");

        return result;
    }
}