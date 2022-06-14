using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{

    public static EventData GetEvent(List<Entity> actors, EventAction eventAction, WorldLocation location, string cause)
    {
        return new EventData(actors, eventAction, WorldSimulator.turn, location, cause);
    }


    #region Entity Meeting

    public static void EntityArrivedAtWorldPoint(List<Entity> actors, WorldPoint worldPoint, string cause)
    {
        foreach (Entity habitant in worldPoint.EntitiesIn)
        {
            if (!actors.Contains(habitant)) continue;
            EntitySawSomeoneArriveInWorldPoint(habitant, actors, worldPoint, cause);
        }
        EntityMeeting(worldPoint.EntitiesIn, worldPoint, cause);
    }

    public static void EntityKillOther(Entity killer, Entity killed, WorldLocation wl, string cause)
    {
        EventData eD = GetEvent(new List<Entity>() { killer }, EventAction.Killed, wl, cause);
        eD.receivers = new List<Entity>() { killed };
        ProcessEvent(eD);
    }

    public static void EntityLeftWorldPoint(List<Entity> actors, WorldPoint worldPoint, string cause)
    {
        foreach (Entity habitant in worldPoint.EntitiesIn)
        {
            if (actors.Contains(habitant)) continue;
            EntitySawSomeoneLeftWorldPoint(habitant, actors, worldPoint, cause);
            foreach (Entity actor in actors)
            {
                EntitySawSomeoneLeftToWorldPoint(habitant, actor, actor.journey[0], cause);
            }
        }
    }

    private static void EntitySawSomeoneArriveInWorldPoint(Entity habitant, List<Entity> actors, WorldPoint worldPoint, string cause)
    {
        EventData eD = GetEvent(actors, EventAction.ArrivedAt, worldPoint, cause);
        eD.worldPoint = worldPoint;
        ProcessEvent(eD);
    }

    private static void EntitySawSomeoneLeftWorldPoint(Entity habitant, List<Entity> actors, WorldPoint worldPoint, string cause)
    {
        EventData eD = GetEvent(actors, EventAction.Left, worldPoint, cause);
        eD.worldPoint = worldPoint;
        ProcessEvent(eD);
    }

    private static void EntitySawSomeoneLeftToWorldPoint(Entity habitant, Entity actor, WorldPoint worldPoint, string cause)
    {
        EventData eD = GetEvent(new List<Entity>() { actor }, EventAction.WasGoingTo, worldPoint, cause);
        eD.worldPoint = worldPoint;
        ProcessEvent(eD);
    }

    public static void EntityEncounter(Entity actor, Entity other, WorldLocation location, string cause)
    {
        EntitySawAnother(actor, other, location, cause);
        if (other.journey != null && other.journey.Count > 0)
            EntityWasGoingTo(actor, other, location, cause, other.journey[0]);
    }

    public static void EntityMeeting(List<Entity> entities, WorldLocation location, string cause)
    {
        List<Entity> aliveEntities = new List<Entity>();
        List<Entity> deadEntities = new List<Entity>();

        foreach (Entity actor in entities)
        {
            if (actor.Alive) aliveEntities.Add(actor);
            else deadEntities.Add(actor);
        }

        foreach (Entity actor in entities)
        {
            if (!actor.Alive) continue;
            foreach (Entity other in entities)
            {
                if (other == actor) continue;

                if (!other.Alive)
                {
                    EntitySawADeadBody(actor, other, location, cause);
                }
                else
                {
                    EntityEncounter(actor, other, location, cause);
                }
            }
        }

        for (int i = 0; i < entities.Count; i++)
        {
            Entity actor = entities[i];
            if (!actor.Alive) continue;

            for (int j = i + 1; j < entities.Count; j++)
            {
                Entity other = entities[j];
                if (!other.Alive) continue;
                WorldPoint wp = location as WorldPoint;
                WorldPath path = location as WorldPath;
                if (wp != null) EntityInteraction.EntityMeetInPoint(actor, other, wp);
                if (path != null) EntityInteraction.EntityMeetInPath(actor, other, path.wpD);
            }
        }
    }

    private static void EntitySawADeadBody(Entity actor, Entity deadBody, WorldLocation location, string cause)
    {
        EventData eD = GetEvent(new List<Entity>() { deadBody }, EventAction.WasDead, location, cause);
        ProcessEvent(eD);
    }

    private static void EntityWasGoingTo(Entity actor, Entity other, WorldLocation location, string cause, WorldPoint worldPoint)
    {
        EventData eD = GetEvent(new List<Entity>() { other }, EventAction.WasGoingTo, location, cause);
        eD.worldPoint = worldPoint;
        ProcessEvent(eD);
    }

    private static void EntityFirstMetAnother(Entity actor, Entity other, WorldLocation location, string cause)
    {
        EventData eD = GetEvent(new List<Entity>() { actor }, EventAction.Met, location, cause);
        eD.receivers = new List<Entity>() { other };
        ProcessEvent(eD);
    }

    private static void EntitySawAnother(Entity actor, Entity other, WorldLocation location, string cause)
    {
        EventData eD = GetEvent(new List<Entity>() { actor }, EventAction.Saw, location, cause);
        eD.receivers = new List<Entity>() { other };
        ProcessEvent(eD);
    }

    #endregion



    private static void ProcessEvent(EventData eD)
    {
        foreach (Entity actor in eD.actors) { actor.memory.AddEventToMemory(eD, HowLearnt.WasThere); }
        foreach (Entity receiver in eD.receivers) { receiver.memory.AddEventToMemory(eD, HowLearnt.WasThere); }
    }

    public static string GetEnumerationOfEntity(List<Entity> entitiesIn)
    {
        string returnedString = "";
        for (int i = 0; i < entitiesIn.Count; i++)
        {
            Entity ent = entitiesIn[i];
            if (i == 0) returnedString += ent.entityName;
            else if (i == entitiesIn.Count - 1) returnedString += $" and {ent.entityName}";
            else returnedString += $", {ent.entityName}";
        }
        return returnedString;
    }
}
