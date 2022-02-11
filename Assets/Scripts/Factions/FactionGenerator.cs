using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionGenerator : MonoBehaviour
{
    static List<string> factionNames = null;
    static Dictionary<string, Faction> allFactions = new Dictionary<string, Faction>();

    static string GetRandomName()
    {
        if (factionNames == null)
        {
            factionNames = PrefabLink.D.FactionNames.text.Split('\n').OfType<string>().ToList();
        }
        int selectedIndex = Random.Range(0, factionNames.Count);
        string selectedName = factionNames[selectedIndex];
        factionNames.RemoveAt(selectedIndex);
        return selectedName;
    }

    public static Faction GenerateFaction()
    {
        Faction fac = new Faction(GetRandomName());
        foreach (var key in allFactions.Keys)
        {
            if (allFactions.TryGetValue(key, out Faction value))
            {
                value.AddRelation(fac, 0);
            }
        }
        allFactions.Add(fac.factionName, fac);
        return fac;
    }

    public static Faction GetFaction(string factionName)
    {
        if (allFactions.TryGetValue(factionName, out Faction fac)) return fac;
        return null;
    }
}
