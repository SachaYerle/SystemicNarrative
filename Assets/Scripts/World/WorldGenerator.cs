using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int nbLocation = 10;
    [Range(0, 8)] public int matriceSize = 6;
    public float matriceCellSizeWidth = 2;
    public float matriceCellSizeHeight = 2;
    public int nbPathMin = 2;
    Vector2 matriceTotalSize => new Vector2(matriceCellSizeWidth, matriceCellSizeHeight) * matriceSize;

    string[] cityNames = null;

    List<WorldPoint> allWorlsPoints = new List<WorldPoint>();
    List<WorldPathData> allWorlsPathDatas = new List<WorldPathData>();

    string[] GetCityNames()
    {
        if (cityNames != null) return cityNames;
        cityNames = PrefabLink.D.CityNames.text.Split('\n');
        return cityNames;
    }

    private void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        GameObject worldGo = new GameObject("World");

        float cellOffset = .2f;

        List<Vector2> allPossiblePoint = new List<Vector2>();
        for (int i = 0; i < matriceSize; i++)
        {
            for (int j = 0; j < matriceSize; j++)
            {
                Vector2 currPosAnchor = new Vector2(matriceCellSizeWidth * i, matriceCellSizeHeight * j);
                Vector2 randomPoint = currPosAnchor + new Vector2(Random.Range(cellOffset, 1f - cellOffset) * matriceCellSizeWidth, Random.Range(cellOffset, 1f - cellOffset) * matriceCellSizeHeight);
                allPossiblePoint.Add(randomPoint - matriceTotalSize / 2);
            }
        }

        allPossiblePoint = UtilitiesF.ShuffleList(allPossiblePoint);

        string[] possibleCityNames = UtilitiesF.ShuffleArray(GetCityNames());

        for (int i = 0; i < Mathf.Min(nbLocation, allPossiblePoint.Count); i++)
        {
            WorldPoint wl = Instantiate(PrefabLink.D.WorldPointPrefab, worldGo.transform);
            wl.transform.position = allPossiblePoint[i];
            wl.InitWorldLocation(possibleCityNames[i]);
            allWorlsPoints.Add(wl);
        }

        for (int i = 0; i < allWorlsPoints.Count; i++)
        {
            WorldPoint wp = allWorlsPoints[i];
            for (int j = i + 1; j < allWorlsPoints.Count; j++)
            {
                WorldPoint wpAimed = allWorlsPoints[j];
                WorldPathData path = new WorldPathData()
                {
                    pathEndA = wp,
                    pathEndB = wpAimed,
                    pathLength = Vector2.Distance(wp.transform.position, wpAimed.transform.position)
                };
                wp.paths.Add(path);
                wpAimed.paths.Add(path);
                allWorlsPathDatas.Add(path);
            }
            wp.paths.Sort(delegate(WorldPathData a, WorldPathData b)
            {
                if (a.pathLength < b.pathLength) return -1;
                if (a.pathLength > b.pathLength) return 1;
                return 0;
            });
            for (int pathIndex = 0; pathIndex < Mathf.Min(nbPathMin, wp.paths.Count); pathIndex++)
            {
                WorldPathData path = wp.paths[pathIndex];
                path.Available = true;
            }
        }

        for (int i = 0; i < allWorlsPoints.Count; i++)
        {
            WorldPoint wp = allWorlsPoints[i];
            foreach (var path in wp.paths)
            {
                if (path.Available)
                {
                    wp.validPaths.Add(path);
                }
            }
        }

        foreach (var pathData in allWorlsPathDatas)
        {
            if (pathData.Available)
            {
                pathData.wp = Instantiate(PrefabLink.D.WorldPathPrefab, worldGo.transform);
                pathData.wp.wpD = pathData;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector2 bottomLeft = new Vector2(-matriceTotalSize.x, -matriceTotalSize.y) / 2;
        Vector2 bottomRight = new Vector2(matriceTotalSize.x, -matriceTotalSize.y) / 2;
        Vector2 topLeft = new Vector2(-matriceTotalSize.x, matriceTotalSize.y) / 2;
        Vector2 topRight = new Vector2(matriceTotalSize.x, matriceTotalSize.y) / 2;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(topRight, topLeft);

        for (int i = 1; i < matriceSize; i++)
        {
            float percentage = i / (float)matriceSize;
            Vector2 topPoint = Vector2.Lerp(topLeft, topRight, percentage);
            Vector2 botPoint = Vector2.Lerp(bottomLeft, bottomRight, percentage);
            Gizmos.DrawLine(topPoint, botPoint);

            Vector2 leftPoint = Vector2.Lerp(topLeft, bottomLeft, percentage);
            Vector2 rightPoint = Vector2.Lerp(topRight, bottomRight, percentage);
            Gizmos.DrawLine(leftPoint, rightPoint);
        }
    }
}
