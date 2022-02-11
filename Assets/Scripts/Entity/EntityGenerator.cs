using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityGenerator : MonoBehaviour
{
    static List<string> possibleNames = null;
    static Dictionary<string, Entity> allEntities = new Dictionary<string, Entity>();

    static string GetRandomName()
    {
        if (possibleNames == null)
        {
            possibleNames = PrefabLink.D.EntityNames.text.Split('\n').OfType<string>().ToList();
        }
        int selectedIndex = Random.Range(0, possibleNames.Count);
        string selectedName = possibleNames[selectedIndex];
        possibleNames.RemoveAt(selectedIndex);
        return selectedName;
    }

    public static Entity GenerateEntity(Faction faction, WorldLocation location)
    {
        Entity ent = new Entity(GetRandomName(), faction, location);
        allEntities.Add(ent.entityName, ent);
        return ent;
    }

    public static Entity GetEntity(string entName)
    {
        if (allEntities.TryGetValue(entName, out Entity ent)) return ent;
        return null;
    }
}
