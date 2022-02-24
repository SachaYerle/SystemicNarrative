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

    public static void EntityMeeting(List<Entity> subjects, WorldLocation location, string cause)
    {
        foreach (var subject in subjects)
        {
            foreach (var other in subjects)
            {
                if (other == subject) continue;

                if (!subject.entitiesKnown.Contains(other))
                    EntityFirstMetAnother(subject, other, location, cause);
                EntitySawAnother(subject, other, location, cause);
                if (other.journey.Count > 0)
                    EntityWasGoingTo(subject, other, location, cause, other.journey[0]);
            }
        }
    }

    private static void EntityWasGoingTo(Entity subject, Entity other, WorldLocation location, string cause, WorldPoint worldPoint)
    {
        var eK = GetEvent(new List<Entity>() { other }, EventAction.WasGoingTo, location, cause);
        eK.receiverWorldPoint = worldPoint;
        AddEventToEntity(subject, eK);
    }

    private static void EntityFirstMetAnother(Entity subject, Entity other, WorldLocation location, string cause)
    {
        var eK = GetEvent(new List<Entity>() { subject }, EventAction.Met, location, cause);
        eK.receiverEntity = new List<Entity>() { other };
        subject.entitiesKnown.Add(other);
        AddEventToEntity(subject, eK);
    }

    private static void EntitySawAnother(Entity subject, Entity other, WorldLocation location, string cause)
    {
        var eK = GetEvent(new List<Entity>() { subject }, EventAction.Saw, location, cause);
        eK.receiverEntity = new List<Entity>() { other };
        AddEventToEntity(subject, eK);
    }

    private static void AddEventToEntity(Entity subject, EventKnowledge eK)
    {
        subject.memory.Add(eK);

        foreach (var otherSubjects in eK.subjects)
        {
            if (subject.EventsPerEntity.ContainsKey(otherSubjects)) subject.EventsPerEntity[otherSubjects].Add(eK);
            else subject.EventsPerEntity.Add(otherSubjects, new List<EventKnowledge>() { eK });
        }

        if (subject.MainCharacter) Debug.Log(eK.ToString());
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
