using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMemory
{
    #region Constructor and ToStringOverride
    public readonly string name;
    public EntityMemory(string name)
    {
        this.name = name;
    }
    public override string ToString()
    {
        return name +"'s memory";
    }
    #endregion

    List<Entity> entitiesKnown = new List<Entity>();

    Dictionary<Entity, List<EventKnowledge>> eventsPerActorEntity = new Dictionary<Entity, List<EventKnowledge>>();
    Dictionary<Entity, List<EventKnowledge>> eventsPerReceiverEntity = new Dictionary<Entity, List<EventKnowledge>>();
    Dictionary<EventAction, List<EventKnowledge>> eventsPerAction = new Dictionary<EventAction, List<EventKnowledge>>();


    public void AddEventToMemory(EventData eventD, HowLearnt howLearnt)
    {
        EventKnowledge ek = new EventKnowledge(eventD, howLearnt);
        foreach (Entity ent in eventD.actors)
        {
            CheckOtherEntity(ent);
            AddEventFromKeyType(ent, ek, ref eventsPerActorEntity);
        }
        foreach (Entity ent in eventD.receivers)
        {
            CheckOtherEntity(ent);
            AddEventFromKeyType(ent, ek, ref eventsPerReceiverEntity);
        }
        AddEventFromKeyType(eventD.action, ek, ref eventsPerAction);
    }

    private void CheckOtherEntity(Entity ent)
    {
        if (!entitiesKnown.Contains(ent))
        {
            entitiesKnown.Add(ent);
            Debug.Log("DO MEET EVENT");
        }
    }

    #region Research Functions
    public List<EventData> GetEventsAsActor(Entity actor)
    {
        List<EventKnowledge> finalList = new List<EventKnowledge>();
        if (eventsPerActorEntity.ContainsKey(actor)) finalList = eventsPerActorEntity[actor];
        return ConvertAndFilter(finalList);

    }
    public List<EventData> GetEventsAsReceiver(Entity receiver)
    {
        List<EventKnowledge> finalList = new List<EventKnowledge>();
        if (eventsPerReceiverEntity.ContainsKey(receiver)) finalList = eventsPerReceiverEntity[receiver];
        return ConvertAndFilter(finalList);
    }
    public List<EventData> GetEventsWithAction(EventAction action)
    {
        List<EventKnowledge> finalList = new List<EventKnowledge>();
        if (eventsPerAction.ContainsKey(action)) finalList = eventsPerAction[action];
        return ConvertAndFilter(finalList);
    }


    public List<EventData> GetEventFrom(Entity actor, Entity receiver, EventAction action)
    {
        List<List<EventKnowledge>> unfilteredLists = new List<List<EventKnowledge>>();
        if (eventsPerActorEntity.ContainsKey(actor)) unfilteredLists.Add(eventsPerActorEntity[actor]);
        if (eventsPerReceiverEntity.ContainsKey(receiver)) unfilteredLists.Add(eventsPerReceiverEntity[receiver]);
        if (eventsPerAction.ContainsKey(action)) unfilteredLists.Add(eventsPerAction[action]);
        List<EventData> finalList = FilterLists(unfilteredLists);
        return finalList;
    }
    #endregion

    #region Filters and Utility
    private static void AddEventFromKeyType<T>(T key, EventKnowledge ek, ref Dictionary<T, List<EventKnowledge>> dic)
    {
        if (dic.ContainsKey(key)) dic[key].Add(ek);
        else dic.Add(key, new List<EventKnowledge>() { ek });
    }

    private static List<EventData> ConvertAndFilter (List<EventKnowledge> originalList)
    {
        List<EventData> finalList = new List<EventData>();
        foreach (var ek in originalList)
        {
            if (!finalList.Contains(ek.eventD)) finalList.Add(ek.eventD);
        }
        return finalList;
    }

    private static List<EventData> FilterLists(List<List<EventKnowledge>> filteredLists)
    {
        List<EventData> currValidatedList = new List<EventData>();
        List<EventData> currUnvalidatedList = new List<EventData>();

        for (int i = 0; i < filteredLists.Count; i++)
        {
            List<EventKnowledge> currList = filteredLists[i];
            foreach (var ek in currList)
            {
                if (currValidatedList.Contains(ek.eventD) || currUnvalidatedList.Contains(ek.eventD)) continue;

                bool isValid = true;
                for (int j = i + 1; j < filteredLists.Count; j++)
                {
                    List<EventKnowledge> crossList = filteredLists[i];
                    if (!crossList.Contains(ek))
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid && !currValidatedList.Contains(ek.eventD)) currValidatedList.Add(ek.eventD);
                else if (!currUnvalidatedList.Contains(ek.eventD)) currUnvalidatedList.Add(ek.eventD);
            }
        }

        return currValidatedList;
    }
    #endregion

}
