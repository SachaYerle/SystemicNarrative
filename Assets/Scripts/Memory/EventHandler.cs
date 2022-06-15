using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{



    #region Entity Meetings


    public static void EntityArrivedAtWorldPoint(List<Entity> actors, WorldPoint worldPoint, string cause)
    {
        CreateEvent(actors, null, worldPoint.EntitiesIn, EventAction.ArrivedAt, worldPoint, cause, worldPoint);
        EntityMeeting(worldPoint.EntitiesIn, worldPoint, cause);
    }

    public static void EntityLeftWorldPoint(List<Entity> actors, WorldPoint worldPoint, string cause)
    {
        CreateEvent(actors, null, worldPoint.EntitiesIn, EventAction.Left, worldPoint, cause, worldPoint);
        EntitiesGoToWorldPoint(actors, worldPoint.EntitiesIn, actors[0].journey[0], cause);
    }

    public static void EntitiesEncounter(List<Entity> actors, WorldLocation wl, string cause)
    {
        if (actors.Count <= 1) return;
        CreateEvent(actors, null, null, EventAction.SawEachOther, wl, cause);
        foreach (Entity actor in actors)
        {
            if (actor.journey != null && actor.journey.Count > 0) EntitiesGoToWorldPoint(new List<Entity>() { actor }, actors, actor.journey[0], cause);
        }
    }

    public static void EntityMeeting(List<Entity> entities, WorldLocation wl, string cause)
    {
        List<Entity> aliveEntities = new List<Entity>();
        List<Entity> deadEntities = new List<Entity>();

        foreach (Entity actor in entities)
        {
            if (actor.Alive) aliveEntities.Add(actor);
            else deadEntities.Add(actor);
        }

        EntitiesAreDead(deadEntities, aliveEntities, wl, cause);
        EntitiesEncounter(aliveEntities, wl, cause);


        for (int i = 0; i < entities.Count; i++)
        {
            Entity actor = entities[i];
            if (!actor.Alive) continue;

            for (int j = i + 1; j < entities.Count; j++)
            {
                Entity other = entities[j];
                if (!other.Alive) continue;
                // TODO : Rework pour pas avoir à cast
                WorldPoint wp = wl as WorldPoint;
                WorldPath path = wl as WorldPath;
                if (wp != null) EntityInteraction.EntityMeetInPoint(actor, other, wp);
                if (path != null) EntityInteraction.EntityMeetInPath(actor, other, path.wpD);
            }
        }
    }



    #endregion

    #region Simple Events

    public static EventData EntityKillOther(List<Entity> actors, List<Entity> receivers, WorldLocation wl, string cause) => CreateEvent(actors, receivers, null, EventAction.Killed, wl, cause);
    private static EventData EntitiesGoToWorldPoint(List<Entity> actors, List<Entity> presentEntities, WorldPoint wp, string cause) => CreateEvent(actors, null, presentEntities, EventAction.WasGoingTo, wp, cause, wp);
    public static EventData EntitiesAreDead(List<Entity> actors, List<Entity> presentEntities, WorldLocation wl, string cause) => CreateEvent(actors, null, presentEntities, EventAction.WasDead, wl, cause);
    public static EventData EntitiesFirstMet(List<Entity> actors, List<Entity> presentEntities, WorldLocation wl, string cause) => CreateEvent(actors, null, presentEntities, EventAction.Met, wl, cause);


    #endregion


    #region Utilties

    public static EventData CreateEvent(List<Entity> actors, List<Entity> receivers, List<Entity> presentEntities, EventAction eventAction, WorldLocation location, string cause, WorldPoint overrideWorldPoint = null)
    {
        if (actors == null || actors.Count == 0) return null;
        if (receivers == null) receivers = new List<Entity>();
        if (presentEntities == null) presentEntities = new List<Entity>();
        EventData eD = new EventData(actors, receivers, UtilitiesF.RemoveFromList(presentEntities, actors, receivers), eventAction, WorldSimulator.turn, location, cause);
        if (overrideWorldPoint != null) eD.worldPoint = overrideWorldPoint;
        ProcessEvent(eD);
        return eD;
    }

    private static void ProcessEvent(EventData eD)
    {
        foreach (Entity actor in eD.actors) { if (actor.Alive) actor.memory.AddEventToMemory(eD, HowLearnt.WasThere); }
        foreach (Entity receiver in eD.receivers) { if (receiver.Alive) receiver.memory.AddEventToMemory(eD, HowLearnt.WasThere); }
        foreach (Entity witness in eD.witnesses) { if (witness.Alive) witness.memory.AddEventToMemory(eD, HowLearnt.WasThere); }
    }

    #endregion
}
