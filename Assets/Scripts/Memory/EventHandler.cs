using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static EventKnowledge GetEvent(List<Entity> subjects, EventAction eventAction, WorldLocation location, string cause)
    {
        return new EventKnowledge(subjects, eventAction, WorldSimulator.turn, location, cause);
    }


    #region Entity Meeting

    public static void EntityArrivedAtWorldPoint(List<Entity> subjects, WorldPoint worldPoint, string cause)
    {
        foreach (var habitant in worldPoint.EntitiesIn)
        {
            if (!subjects.Contains(habitant)) continue;
            EntitySawSomeoneArriveInWorldPoint(habitant, subjects, worldPoint, cause);
        }
        EntityMeeting(worldPoint.EntitiesIn, worldPoint, cause);
    }

    public static void EntityKillOther(Entity killer, Entity killed, WorldLocation wl, string cause)
    {
        var eK = GetEvent(new List<Entity>() { killer }, EventAction.Killed, wl, cause);
        eK.receiverEntity = new List<Entity>() { killed };
        AddEventToEntity(killed, eK);
        AddEventToEntity(killer, eK);

        eK = GetEvent(new List<Entity>() { killed }, EventAction.WasKilledBY, wl, cause);
        eK.receiverEntity = new List<Entity>() { killer };
        AddEventToEntity(killer, eK);
        AddEventToEntity(killed, eK);
    }

    public static void EntityLeftWorldPoint(List<Entity> subjects, WorldPoint worldPoint, string cause)
    {
        foreach (var habitant in worldPoint.EntitiesIn)
        {
            if (subjects.Contains(habitant)) continue;
            EntitySawSomeoneLeftWorldPoint(habitant, subjects, worldPoint, cause);
            foreach (var subject in subjects)
            {
                EntitySawSomeoneLeftToWorldPoint(habitant, subject, subject.journey[0], cause);
            }
        }
    }

    private static void EntitySawSomeoneArriveInWorldPoint(Entity habitant, List<Entity> subjects, WorldPoint worldPoint, string cause)
    {
        var eK = GetEvent(subjects, EventAction.ArrivedAt, worldPoint, cause);
        eK.receiverWorldPoint = worldPoint;
        AddEventToEntity(habitant, eK);
    }

    private static void EntitySawSomeoneLeftWorldPoint(Entity habitant, List<Entity> subjects, WorldPoint worldPoint, string cause)
    {
        var eK = GetEvent(subjects, EventAction.Left, worldPoint, cause);
        eK.receiverWorldPoint = worldPoint;
        AddEventToEntity(habitant, eK);
    }

    private static void EntitySawSomeoneLeftToWorldPoint(Entity habitant, Entity subject, WorldPoint worldPoint, string cause)
    {
        var eK = GetEvent(new List<Entity>() { subject }, EventAction.WasGoingTo, worldPoint, cause);
        eK.receiverWorldPoint = worldPoint;
        AddEventToEntity(habitant, eK);
    }

    public static void EntityEncounter(Entity subject, Entity other, WorldLocation location, string cause)
    {
        if (!subject.entitiesKnown.Contains(other))
            EntityFirstMetAnother(subject, other, location, cause);
        EntitySawAnother(subject, other, location, cause);
        if (other.journey != null && other.journey.Count > 0)
            EntityWasGoingTo(subject, other, location, cause, other.journey[0]);
    }

    public static void EntityMeeting(List<Entity> subjects, WorldLocation location, string cause)
    {
        foreach (var subject in subjects)
        {
            if (!subject.Alive) continue;
            foreach (var other in subjects)
            {
                if (other == subject) continue;

                if (!other.Alive)
                {
                    EntitySawADeadBody(subject, other, location, cause);
                }
                else
                {
                    EntityEncounter(subject, other, location, cause);
                }
            }
        }

        for (int i = 0; i < subjects.Count; i++)
        {
            Entity subject = subjects[i];
            if (!subject.Alive) continue;

            for (int j = i + 1; j < subjects.Count; j++)
            {
                Entity other = subjects[j];
                if (!other.Alive) continue;
                WorldPoint wp = location as WorldPoint;
                WorldPath path = location as WorldPath;
                if (wp != null) EntityInteraction.EntityMeetInPoint(subject, other, wp);
                if (path != null) EntityInteraction.EntityMeetInPath(subject, other, path.wpD);
            }
        }
    }

    private static void EntitySawADeadBody(Entity subject, Entity deadBody, WorldLocation location, string cause)
    {
        var eK = GetEvent(new List<Entity>() { deadBody }, EventAction.WasDead, location, cause);
        AddEventToEntity(subject, eK);
    }

    private static void EntityWasGoingTo(Entity subject, Entity other, WorldLocation location, string cause, WorldPoint worldPoint)
    {
        var eK = GetEvent(new List<Entity>() { other }, EventAction.WasGoingTo, location, cause);
        eK.receiverWorldPoint = worldPoint;
        AddEventToEntity(subject, eK);
    }

    private static void EntityFirstMetAnother(Entity subject, Entity other, WorldLocation location, string cause)
    {
        var eK = GetEvent(new List<Entity>() { other }, EventAction.Met, location, cause);
        eK.receiverEntity = new List<Entity>() { subject };
        subject.entitiesKnown.Add(other);
        AddEventToEntity(subject, eK);
    }

    private static void EntitySawAnother(Entity subject, Entity other, WorldLocation location, string cause)
    {
        var eK = GetEvent(new List<Entity>() { other }, EventAction.WasAt, location, cause);
        AddEventToEntity(subject, eK);
    }

    #endregion

    private static void AddEventToEntity(Entity subject, EventKnowledge eK)
    {
        subject.memory.Add(eK);

        foreach (var otherSubjects in eK.subjects)
        {
            if (subject.EventsPerEntity.ContainsKey(otherSubjects)) subject.EventsPerEntity[otherSubjects].Add(eK);
            else subject.EventsPerEntity.Add(otherSubjects, new List<EventKnowledge>() { eK });
        }
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
