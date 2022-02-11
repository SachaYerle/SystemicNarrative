using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilitiesF
{
    public static List<T> ShuffleList<T>(List<T> original)
    {
        List<T> shuffledList = new List<T>();
        for (int i = 0; i < original.Count; i++)
        {
            shuffledList.Add(original[i]);
        }
        for (int i = 0; i < shuffledList.Count; i++)
        {
            T temp = shuffledList[i];
            int randomIndex = Random.Range(i, shuffledList.Count);
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }

        return shuffledList;
    }
    public static List<T> InvertList<T>(List<T> original)
    {
        List<T> invertedList = new List<T>();
        for (int i = 0; i < original.Count; i++)
        {
            invertedList.Add(original[original.Count - 1 - i]);
        }
        return invertedList;
    }

    public static T[] ShuffleArray<T>(T[] original)
    {
        T[] shuffledList = new T[original.Length];
        for (int i = 0; i < original.Length; i++)
        {
            shuffledList[i] = original[i];
        }
        for (int i = 0; i < shuffledList.Length; i++)
        {
            T temp = shuffledList[i];
            int randomIndex = Random.Range(i, shuffledList.Length);
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }

        return shuffledList;
    }

    public static List<T> ShuffleListFromSeed<T>(List<T> original, List<int> seed)
    {
        List<T> shuffledList = new List<T>();
        for (int i = 0; i < original.Count; i++)
        {
            shuffledList.Add(default(T));
        }
        for (int i = 0; i < shuffledList.Count; i++)
        {
            shuffledList[i] = original[seed[i]];
        }
        return shuffledList;
    }

    static float GetFloat(float seed, float time)
    {
        return (Mathf.PerlinNoise(time + seed, time + seed) - 0.5f) * 2f;
    }

    public static Vector3 GetPerlinVectorThree(float seed, float time)
    {
        return new Vector3(GetFloat(seed, time), GetFloat(seed * 2, time), GetFloat(seed * 3, time));
    }


    const float alpha = 0.947543636291f;
    const float beta = 0.392485425092f;
    public static float distanceXZ(Vector2 A, Vector2 B)
    {
        Vector2 V = B - A;
        return magnitudeXZ(V);
    }
    public static float magnitudeXZ(Vector2 V)
    {
        if (V.x == 0 && V.y == 0) return 0.0001f;
        return alpha * Mathf.Max(Mathf.Abs(V.x), Mathf.Abs(V.y)) + beta * Mathf.Min(Mathf.Abs(V.x), Mathf.Abs(V.y));
    }

    public static float EasedInOutCubic(float x)
    {
        return x < 0.5 ? 16 * Mathf.Pow(x, 5) : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
    }

    public static float EasedInOut(float x)
    {
        return AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(x);
    }

    public static Texture2D GenerateSpriteSheet(Sprite[] sprites)
    {
        Vector2 cellSize = new Vector2();
        foreach (var sprite in sprites)
        {
            cellSize = new Vector2(Mathf.Max(cellSize.x, sprite.rect.size.x), Mathf.Max(cellSize.y, sprite.rect.size.y));
        }
        Vector2Int cellSizeInt = new Vector2Int(Mathf.CeilToInt(cellSize.x), Mathf.CeilToInt(cellSize.y));
        Texture2D tex = new Texture2D(cellSizeInt.x * sprites.Length, cellSizeInt.y);

        for (int i = 0; i < sprites.Length; i++)
        {
            int xDecal = cellSizeInt.x * i;
            Texture2D originTex = sprites[i].texture;
            for (int x = 0; x < cellSizeInt.x; x++)
            {
                for (int y = 0; y < cellSizeInt.y; y++)
                {
                    Color originPixel = originTex.GetPixel(Mathf.CeilToInt(sprites[i].rect.x) + x, Mathf.CeilToInt(sprites[i].rect.y) + y);
                    tex.SetPixel(xDecal + x, y, originPixel);
                }
            }
        }
        tex.Apply();
        return tex;
    }
}
