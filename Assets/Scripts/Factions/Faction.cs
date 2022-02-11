using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction
{
    public string factionName = "Default_Name";

    public Dictionary<Faction, int> factionRelation = new Dictionary<Faction, int>();

    public Faction(string factionName)
    {
        this.factionName = factionName;
    }

    public void AddRelation(Faction fac, int relation)
    {
        factionRelation.Add(fac, relation);
    }
}
