using System;
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

    private void DoTick(int turn)
    {

    }
}