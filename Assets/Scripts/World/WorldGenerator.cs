using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    static List<WorldPoint> allWorlsPoints = new List<WorldPoint>();
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

    public static WorldPoint GetRandomDestination(WorldPoint currLocation)
    {
        List<WorldPoint> possiblePoints = new List<WorldPoint>(allWorlsPoints);
        possiblePoints.Remove(currLocation);
        return possiblePoints[Random.Range(0, possiblePoints.Count)];
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
                WorldPathData path = new WorldPathData(wp, wpAimed, Vector2.Distance(wp.transform.position, wpAimed.transform.position));
                wp.paths.Add(path);
                wpAimed.paths.Add(path);
                allWorlsPathDatas.Add(path);
            }
            wp.paths.Sort(delegate (WorldPathData a, WorldPathData b)
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

    static Dictionary<WorldPoint, WorldPathfindData> nodesPaths = new Dictionary<WorldPoint, WorldPathfindData>();
    static List<WorldPathfindData> sortedDatas = new List<WorldPathfindData>();
    public static bool GetPath(WorldPoint start, WorldPoint destination, out List<WorldPoint> journey)
    {
        nodesPaths = new Dictionary<WorldPoint, WorldPathfindData>();
        sortedDatas = new List<WorldPathfindData>();
        journey = new List<WorldPoint>();
        WorldPoint currPoint = start;
        WorldPathfindData startData = new WorldPathfindData(currPoint, null, 0, (start.transform.position - destination.transform.position).sqrMagnitude);
        nodesPaths.Add(currPoint, startData);
        sortedDatas.Add(startData);
        int iteration = 0;
        while (true)
        {
            WorldPathfindData data = nodesPaths[currPoint];
            data.locked = true;
            foreach (WorldPathData path in currPoint.validPaths)
            {
                WorldPoint aimed = currPoint == path.pathEndA ? path.pathEndB : path.pathEndA;
                float distWithCurr = (aimed.transform.position - currPoint.transform.position).sqrMagnitude;
                float currPathDistToAimed = data.distFromPath + distWithCurr;
                if (nodesPaths.ContainsKey(aimed))
                {
                    WorldPathfindData existingData = nodesPaths[aimed];
                    if (existingData.locked) continue;
                    if (currPathDistToAimed < existingData.distFromPath)
                    {
                        existingData.camesFrom = currPoint;
                        existingData.distFromPath = currPathDistToAimed;
                    }
                }
                else
                {
                    WorldPathfindData newData = new WorldPathfindData(aimed, currPoint, currPathDistToAimed, (aimed.transform.position - destination.transform.position).sqrMagnitude);
                    nodesPaths.Add(aimed, newData);
                    sortedDatas.Add(newData);
                }
                if (aimed == destination)
                {
                    // DO PATH
                    WorldPoint currFinalPathPoint = aimed;
                    while (true)
                    {
                        journey.Add(currFinalPathPoint);
                        WorldPathfindData pathData = nodesPaths[currFinalPathPoint];
                        currFinalPathPoint = pathData.camesFrom;
                        if (currFinalPathPoint == null) break;
                    }
                    journey.Reverse();
                    journey.RemoveAt(0);
                    return true;
                }
            }
            sortedDatas.Sort(delegate (WorldPathfindData a, WorldPathfindData b)
            {
                if (a.locked) return 1;
                if (b.locked) return -1;
                if (a.AddedValue < b.AddedValue) return -1;
                if (a.AddedValue > b.AddedValue) return 1;
                return 0;
            });
            if (sortedDatas[0].locked) return false;
            else currPoint = sortedDatas[0].point;
            iteration++;
        }
    }


    public class WorldPathfindData
    {
        public WorldPoint point;
        public WorldPoint camesFrom;
        public float distFromPath;
        public float distToTheEnd;
        public float addedFromPastPath;
        public bool locked = false;

        public WorldPathfindData(WorldPoint point, WorldPoint camesFrom, float distFromPath, float distToTheEnd)
        {
            this.point = point;
            this.camesFrom = camesFrom;
            this.distFromPath = distFromPath;
            this.distToTheEnd = distToTheEnd;
        }

        public float AddedValue => distFromPath + distToTheEnd;
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
