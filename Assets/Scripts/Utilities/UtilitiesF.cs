using System;
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
            int randomIndex = UnityEngine.Random.Range(i, shuffledList.Count);
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

    public static List<T> CopyList<T>(List<T> original)
    {
        List<T> copiedList = new List<T>();
        for (int i = 0; i < original.Count; i++)
        {
            copiedList.Add(original[i]);
        }
        return copiedList;
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
            int randomIndex = UnityEngine.Random.Range(i, shuffledList.Length);
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

    public static bool TryGetEnumFromString<T>(string key, out T value)
    {
        value = default(T);
        foreach (var testEnum in (T[])Enum.GetValues(typeof(T)))
        {
            if (key.ToUpper() == testEnum.ToString().ToUpper())
            {
                value = testEnum;
                return true;
            }
        }
        return false;
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
    public static float GetHighest(List<float> listOfFloat)
    {
        float maxValue = 0;
        foreach (var value in listOfFloat) { maxValue = Mathf.Max(maxValue, value); }
        return maxValue;
    }
    public static float GetAverage(List<float> listOfFloat)
    {
        float totalValue = 0;
        foreach (var value in listOfFloat) { totalValue += value; }
        return totalValue / (float)listOfFloat.Count;
    }
    public static float GetAverageWithValueAdded(List<float> listOfFloat, float nbOfOne)
    {
        float totalValue = 0;
        foreach (var value in listOfFloat) { totalValue += value; }
        totalValue += nbOfOne;
        return totalValue / (float)(listOfFloat.Count + nbOfOne);
    }

    public static List<T> MergeList<T>(List<T> a, List<T> b) => MergeList(new List<List<T>>() { a, b });
    public static List<T> MergeList<T>(List<T> a, List<T> b, List<T> c) => MergeList(new List<List<T>>() { a, b, c });
    public static List<T> MergeList<T>(List<T> a, List<T> b, List<T> c, List<T> d) => MergeList(new List<List<T>>() { a, b, c, d });

    public static List<T> MergeList<T>(List<List<T>> lists)
    {
        List<T> finalList = new List<T>();
        foreach (List<T> list in lists) { foreach (T item in list) { if (!finalList.Contains(item)) finalList.Add(item); } }
        return finalList;
    }

    public static Color GetHueShiftedColor(Color original, float shift)
    {
        Color.RGBToHSV(original, out float H, out float S, out float V);
        H += shift;
        return Color.HSVToRGB(H, S, V);
    }

    public static Gradient GetHueShiftedGradient(Gradient original, float shift)
    {
        Gradient newGradient = new Gradient();
        GradientColorKey[] newColorKeys = new GradientColorKey[original.colorKeys.Length];
        for (int i = 0; i < original.colorKeys.Length; i++)
        {
            GradientColorKey colorKey = original.colorKeys[i];
            newColorKeys[i] = new GradientColorKey(GetHueShiftedColor(colorKey.color, shift), colorKey.time);
        }
        newGradient.colorKeys = newColorKeys;
        newGradient.alphaKeys = original.alphaKeys;
        return newGradient;
    }

    public static void DrawSquareGizmos(Vector3 pos, float sizeX, float sizeY, Color col)
    {
        Gizmos.color = col;
        Vector3 decalUp = sizeY / 2 * Vector2.up;
        Vector3 decalRight = sizeX / 2 * Vector2.right;
        Vector3 decalBot = sizeY / 2 * Vector2.down;
        Vector3 decalLeft = sizeX / 2 * Vector2.left;
        Gizmos.DrawLine(pos + decalLeft + decalUp, pos + decalRight + decalUp);
        Gizmos.DrawLine(pos + decalLeft + decalBot, pos + decalRight + decalBot);
        Gizmos.DrawLine(pos + decalLeft + decalBot, pos + decalLeft + decalUp);
        Gizmos.DrawLine(pos + decalRight + decalBot, pos + decalRight + decalUp);
    }

    public static float NOnePOneToZeroOne(float x) => Mathf.Clamp(x, -1f, 1f) * .5f + .5f;
    public static float ZeroOneToNOnePOne(float x) => Mathf.Clamp01(x) * 2f - 1f;

    public static float GetEasedOutValueWithOffset(float offset, float value)
    {
        value = Mathf.Clamp01(value);
        return offset + (1 - Mathf.Pow(1 - value, 3)) * (1 - offset);
    }
    public static float GetEasedInValueWithOffset(float offset, float value)
    {
        value = Mathf.Clamp01(value);
        return offset + Mathf.Pow(value, 3) * (1 - offset);
    }

    public static void GetTilingAndOffsetFromSprite(Sprite sprite, out Vector2 tiling, out Vector2 offset)
    {
        offset = new Vector2(sprite.rect.x / (float)sprite.texture.width, sprite.rect.y / (float)sprite.texture.height);
        tiling = new Vector2(sprite.rect.width / (float)sprite.texture.width, sprite.rect.height / (float)sprite.texture.height);
    }

    public static void GetSpriteSizeY(Sprite sprite, out float posY, out float sizeY, out float sizeInWorldY)
    {
        posY = sprite.rect.y / (float)sprite.texture.height;
        sizeY = sprite.rect.height / (float)sprite.texture.height;
        sizeInWorldY = sprite.rect.height / (float)sprite.pixelsPerUnit;
    }

    public static string SpaceEveryXWithChar(string originalText, char character, int every)
    {
        int nbCharSinceLastSpace = 0;
        string newText = "";
        for (int i = originalText.Length - 1; i >= 0; i--)
        {
            char saveChar = originalText[i];
            newText = saveChar + newText;
            nbCharSinceLastSpace++;
            if (nbCharSinceLastSpace == every)
            {
                newText = character + newText;
                nbCharSinceLastSpace = 0;
            }
        }
        return newText;
    }

    public static T[] GetOnlyActivated<T>(T[] baseArray) where T : MonoBehaviour
    {
        List<T> validList = new List<T>();
        foreach (T item in baseArray)
        {
            if (item.gameObject.activeInHierarchy) validList.Add(item);
        }
        return validList.ToArray();
    }
    public static List<T> RemoveFromList<T>(List<T> originalList, List<T> toRemove)
    {
        List<T> returnedList = new List<T>();
        foreach (T item in originalList)
        {
            if (!toRemove.Contains(item)) returnedList.Add(item);
        }
        return returnedList;
    }
    public static List<T> RemoveFromList<T>(List<T> originalList, List<T> toRemove, List<T> toRemove2)
    {
        List<T> returnedList = new List<T>();
        foreach (T item in originalList)
        {
            if (!toRemove.Contains(item) && !toRemove2.Contains(item)) returnedList.Add(item);
        }
        return returnedList;
    }

    public static string GetEnumerationOfObjects<T>(List<T> objects)
    {
        string returnedString = "";
        for (int i = 0; i < objects.Count; i++)
        {
            object obj = objects[i];
            if (i == 0) returnedString += obj.ToString();
            else if (i == objects.Count - 1) returnedString += $" and {obj.ToString()}";
            else returnedString += $", {obj.ToString()}";
        }
        return returnedString;
    }
    public static void AddObjToListDictionary<T, U>(T key, U obj, ref Dictionary<T, List<U>> dic)
    {
        if (dic.ContainsKey(key)) dic[key].Add(obj);
        else dic.Add(key, new List<U>() { obj });
    }
}
