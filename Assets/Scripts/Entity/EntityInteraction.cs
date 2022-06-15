using System.Collections.Generic;
using UnityEngine;

public static class EntityInteraction
{
    public static void EntityMeetInPath(Entity a, Entity b, WorldPathData wpd)
    {
        float randomResult = Random.Range(0f, 1f);
        if (randomResult < .01f)
        {
            EntityFight(a, b, wpd.wp, $"{a.entityName} and {b.entityName} decided to fight");
        }
    }

    private static void EntityFight(Entity a, Entity b, WorldLocation wl, string cause)
    {
        float randomResult = Random.Range(0f, 1f);
        if (randomResult < .5f) EntityKilledOther(a, b, wl, cause);
        else EntityKilledOther(b, a, wl, cause);
    }

    private static void EntityKilledOther(Entity killer, Entity killed, WorldLocation wl, string cause)
    {
        EventHandler.EntityKillOther(new List<Entity>() { killer }, new List<Entity>() { killed }, wl, cause);
        killed.Die(); 
        EventHandler.EntitiesAreDead(new List<Entity>() { killed }, new List<Entity>() { killer }, wl, cause);
    }

    public static void EntityMeetInPoint(Entity a, Entity b, WorldPoint wp)
    {

    }
}
