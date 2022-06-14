using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    public List<Entity> actors = new List<Entity>();
    public EventAction action = EventAction.Undefined;

    public int turn = 0;
    public WorldLocation location = null;
    public string cause = "";

    public List<Entity> receivers = new List<Entity>();
    public List<Entity> witnesses = new List<Entity>();
    public WorldPoint worldPoint = null;

    public EventData(List<Entity> actors, EventAction action, int turn, WorldLocation location, string cause)
    {
        this.actors = actors;
        this.action = action;
        this.turn = turn;
        this.location = location;
        this.cause = cause;
    }

    public override string ToString()
    {
        string eventText = EventHandler.GetEnumerationOfEntity(actors) + " " + action.ToString();

        string addedTarget = action switch
        {
            EventAction.Met => EventHandler.GetEnumerationOfEntity(receivers),
            EventAction.Saw => EventHandler.GetEnumerationOfEntity(receivers),
            EventAction.WasGoingTo => worldPoint.locationName,
            EventAction.Killed => EventHandler.GetEnumerationOfEntity(receivers),
            EventAction.ArrivedAt => worldPoint.locationName,
            EventAction.Left => worldPoint.locationName,
            EventAction.WasDead => "",
            EventAction.Undefined => "UNDEFINED",
            _ => "UNDEFINED",
        };

        if (addedTarget != "") eventText += " " + addedTarget;

        eventText += $" at turn {turn}.";

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
