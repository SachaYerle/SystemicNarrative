using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventKnowledge
{
    public List<Entity> subjects = new List<Entity>();
    public EventAction action = EventAction.Undefined;

    public int turn = 0;
    public WorldLocation location = null;
    public string cause = "";

    public List<Entity> receiverEntity = new List<Entity>();
    public WorldPoint receiverWorldPoint = null;

    public EventKnowledge(List<Entity> subjects, EventAction action, int turn, WorldLocation location, string cause)
    {
        this.subjects = subjects;
        this.action = action;
        this.turn = turn;
        this.location = location;
        this.cause = cause;
    }

    public override string ToString()
    {
        string eventText = EventHandler.GetEnumerationOfEntity(subjects) + " " + action.ToString();

        string addedTarget = action switch
        {
            EventAction.Met => EventHandler.GetEnumerationOfEntity(receiverEntity),
            EventAction.Saw => EventHandler.GetEnumerationOfEntity(receiverEntity),
            EventAction.WasGoingTo => receiverWorldPoint.locationName,
            EventAction.Killed => EventHandler.GetEnumerationOfEntity(receiverEntity),
            EventAction.ArrivedAt => receiverWorldPoint.locationName,
            EventAction.Left => receiverWorldPoint.locationName,
            EventAction.WasDead => "",
            EventAction.Undefined => "UNDEFINED",
            _ => "UNDEFINED",
        };

        if (addedTarget != "") eventText += " " + addedTarget;

        eventText += $" at turn {turn}";

        return eventText;
    }
}

public enum EventAction
{
    Undefined,
    Met,
    Saw,
    WasGoingTo,
    Killed,
    ArrivedAt,
    Left,
    WasDead
}
