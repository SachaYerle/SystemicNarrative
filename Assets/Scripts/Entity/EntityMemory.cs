using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMemory
{
    #region Constructor and ToStringOverride
    public readonly Entity ent;
    public EntityMemory(Entity ent)
    {
        this.ent = ent;
        entitiesKnown.Add(ent);
    }
    public override string ToString()
    {
        return ent.entityName + "'s memory";
    }
    #endregion

    #region Lists and dictionaries
    List<Entity> entitiesKnown = new List<Entity>();
    Dictionary<Entity, List<EventKnowledge>> eventsPerActorEntity = new Dictionary<Entity, List<EventKnowledge>>();
    Dictionary<Entity, List<EventKnowledge>> eventsPerReceiverEntity = new Dictionary<Entity, List<EventKnowledge>>();
    Dictionary<Entity, List<EventKnowledge>> eventsPerWitnessEntity = new Dictionary<Entity, List<EventKnowledge>>();
    Dictionary<EventAction, List<EventKnowledge>> eventsPerAction = new Dictionary<EventAction, List<EventKnowledge>>();
    #endregion

    public void AddEventToMemory(EventData eventD, HowLearnt howLearnt)
    {
        if (!ShouldStockEvent(eventD)) return;
        EventKnowledge ek = new EventKnowledge(eventD, howLearnt);
        HandleListOfActorInDictionary(eventD.actors, ek, ref eventsPerActorEntity);
        HandleListOfActorInDictionary(eventD.receivers, ek, ref eventsPerReceiverEntity);
        HandleListOfActorInDictionary(eventD.witnesses, ek, ref eventsPerWitnessEntity);
        UtilitiesF.AddObjToListDictionary(eventD.action, ek, ref eventsPerAction);
    }

    private bool ShouldStockEvent(EventData eventD)
    {
        bool isOnlyActor = eventD.actors.Count == 1 && eventD.actors[0] == ent;
        // TODO si opti
        return true;
    }

    private void HandleListOfActorInDictionary(List<Entity> entities, EventKnowledge ek, ref Dictionary<Entity, List<EventKnowledge>> dic)
    {
        foreach (Entity ent in entities)
        {
            CheckOtherEntity(ent, ek.eventD);
            UtilitiesF.AddObjToListDictionary(ent, ek, ref dic);
        }
    }

    private void CheckOtherEntity(Entity ent, EventData eventD)
    {
        if (!entitiesKnown.Contains(ent))
        {
            entitiesKnown.Add(ent);
            ent.memory.entitiesKnown.Add(this.ent);
            List<Entity> presentEntities = UtilitiesF.MergeList(eventD.actors, eventD.receivers, eventD.witnesses);
            EventHandler.EntitiesFirstMet(new List<Entity>() { this.ent, ent }, presentEntities, eventD.location, eventD.cause);
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
    public List<EventData> GetEventsAsWitness(Entity witness)
    {
        List<EventKnowledge> finalList = new List<EventKnowledge>();
        if (eventsPerWitnessEntity.ContainsKey(witness)) finalList = eventsPerWitnessEntity[witness];
        return ConvertAndFilter(finalList);
    }
    public List<EventData> GetEventsWithAction(EventAction action)
    {
        List<EventKnowledge> finalList = new List<EventKnowledge>();
        if (eventsPerAction.ContainsKey(action)) finalList = eventsPerAction[action];
        return ConvertAndFilter(finalList);
    }


    public List<EventData> GetEventFrom(List<Entity> actors, List<Entity> receivers, List<EventAction> actions)
    {
        List<List<EventKnowledge>> unfilteredLists = new List<List<EventKnowledge>>();
        foreach (Entity actor in actors) { if (eventsPerActorEntity.ContainsKey(actor)) unfilteredLists.Add(eventsPerActorEntity[actor]); }
        foreach (Entity receiver in receivers) { if (eventsPerReceiverEntity.ContainsKey(receiver)) unfilteredLists.Add(eventsPerReceiverEntity[receiver]); }
        foreach (EventAction action in actions) { if (eventsPerAction.ContainsKey(action)) unfilteredLists.Add(eventsPerAction[action]); }
        List<EventData> finalList = FilterLists(unfilteredLists);
        return finalList;
    }

    #endregion

    #region Filters and Utility

    private static List<EventData> ConvertAndFilter(List<EventKnowledge> originalList)
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
