using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{

    public string entityName = "Default_Name";
    public Faction faction { get; private set; }


    public WorldLocation currLocation = null;

    public Entity(string entityName, Faction faction, WorldLocation location)
    {
        this.entityName = entityName;
        this.faction = faction;
        WorldSimulator.OnTick += DoTick;
        currLocation = location;
        if (!currLocation.EntitiesIn.Contains(this))
            currLocation.EntitiesIn.Add(this);
    }

    public WorldPoint destination = null;
    public List<WorldPoint> journey = new List<WorldPoint>();

    private void DoTick(int turn)
    {
        if (destination == null)
        {
            destination = WorldGenerator.GetRandomDestination(currLocation as WorldPoint);
            if (WorldGenerator.GetPath(currLocation as WorldPoint, destination, out List<WorldPoint> _journey))
            {
                journey = _journey;
            }
        }
    }
}