using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity
{

    public static int NbEntity = 0;
    public bool MainCharacter { get; private set; }

    public EntityVisual ev = null;

    public string entityName = "Default_Name";
    public Faction faction { get; private set; }


    public WorldLocation currLocation = null;

    public Entity(string entityName, Faction faction, WorldLocation location, EntityVisual ev)
    {
        this.entityName = entityName;
        this.faction = faction;
        WorldSimulator.OnTick += DoTick;
        MoveEntity(null, location, this);
        this.ev = ev;
        ev.name = entityName;
        ev.ent = this;
        ResetWantsToLeave();
        speed = UnityEngine.Random.Range(.3f, .7f);


        if (NbEntity == 0)
        {
            MainCharacter = true;
            Debug.Log($"{entityName} is the main character");
        }
        NbEntity++;
    }

    public WorldPoint destination = null;
    public List<WorldPoint> journey = new List<WorldPoint>();
    public bool wantsToLeave = false;
    public WorldPathData currPath = null;
    public bool goingToA = true;
    public float distMadeOnPath = 0;
    public float speed;

    public bool onPath => currPath != null;
    public float pathPercentage => distMadeOnPath / currPath.pathLength;

    float timeRemainingBeforeLeave = 0;

    public static Dictionary<WorldPathData, List<PathMovement>> allPathMovements = new Dictionary<WorldPathData, List<PathMovement>>();
    public static List<WorldPathData> pathToCheck = new List<WorldPathData>();

    public Dictionary<Entity, List<EventKnowledge>> EventsPerEntity = new Dictionary<Entity, List<EventKnowledge>>();
    public List<EventKnowledge> memory = new List<EventKnowledge>();
    public List<Entity> entitiesKnown = new List<Entity>();

    private void DoTick(int turn)
    {
        if (!wantsToLeave)
        {
            timeRemainingBeforeLeave = Mathf.MoveTowards(timeRemainingBeforeLeave, 0, 1);
            wantsToLeave = timeRemainingBeforeLeave == 0;
        }
        if (wantsToLeave)
        {
            if (destination == null)
            {
                destination = WorldGenerator.GetRandomDestination(currLocation as WorldPoint);
                if (WorldGenerator.GetPath(currLocation as WorldPoint, destination, out List<WorldPoint> _journey))
                {
                    journey = _journey;
                }
            }
            if (currPath == null)
            {
                StartPath(journey[0]);
            }
            else
            {
                if (pathPercentage == 1) ArriveAtEndOfPath();
                else MoveForward();
            }
        }
    }

    private void StartPath(WorldPoint dir)
    {
        currPath = (currLocation as WorldPoint).validPaths.FirstOrDefault(path => path.pathEndA == dir || path.pathEndB == dir);
        goingToA = currPath.pathEndA == dir;
        distMadeOnPath = 0;
        MoveEntity(currLocation, currPath.wp, this);
    }

    private void MoveForward()
    {
        float pastValue = distMadeOnPath;
        distMadeOnPath = Mathf.MoveTowards(distMadeOnPath, currPath.pathLength, speed);
        PathMovement movement = new PathMovement(this, currPath, goingToA, pastValue, distMadeOnPath);
        AddMovement(movement);
    }

    private void AddMovement(PathMovement movement)
    {
        if (allPathMovements.ContainsKey(movement.path))
        {
            allPathMovements[movement.path].Add(movement);
            if (!pathToCheck.Contains(movement.path)) pathToCheck.Add(movement.path);
        }
        else
        {
            allPathMovements.Add(movement.path, new List<PathMovement>());
            allPathMovements[movement.path].Add(movement);
        }
    }

    private void ArriveAtEndOfPath()
    {
        MoveEntity(currLocation, journey[0], this);
        currPath = null;
        journey.RemoveAt(0);
        ResetWantsToLeave();
        if (currLocation == destination) ArrivedAtDestination();
    }

    private void ResetWantsToLeave()
    {
        wantsToLeave = false;
        timeRemainingBeforeLeave = UnityEngine.Random.Range(3f, 15f);
        if (MainCharacter) timeRemainingBeforeLeave = 3f;
    }

    private void ArrivedAtDestination()
    {
        destination = null;
    }

    public static void MoveEntity(WorldLocation from, WorldLocation to, Entity ent)
    {
        if (from != null) from.EntitiesIn.Remove(ent);
        ent.currLocation = to;
        if (to != null) to.EntitiesIn.Add(ent);
    }
}

public class PathMovement
{
    public Entity ent = null;
    public WorldPathData path = null;
    public bool goingToA = true;
    public float pastValue = 0;
    public float endValue = 1;
    public float moveValue => endValue - pastValue;

    public PathMovement(Entity ent, WorldPathData path, bool goingToA, float pastValue, float endValue)
    {
        this.ent = ent;
        this.path = path;
        this.goingToA = goingToA;
        this.pastValue = pastValue;
        this.endValue = endValue;
    }
}