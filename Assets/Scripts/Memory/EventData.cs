using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    public readonly List<Entity> actors;
    public readonly List<Entity> receivers;
    public readonly List<Entity> witnesses;
    public readonly EventAction action;
    public readonly WorldLocation location = null;
    public readonly string cause = "";
    public readonly int turn = 0;

    public WorldPoint worldPoint = null;

    public EventData(List<Entity> actors, List<Entity> receivers, List<Entity> witnesses, EventAction action, int turn, WorldLocation location, string cause)
    {
        this.actors = actors;
        this.receivers = receivers;
        this.witnesses = witnesses;
        this.action = action;
        this.turn = turn;
        this.location = location;
        this.cause = cause;
    }

    public override string ToString()
    {
        string eventText = UtilitiesF.GetEnumerationOfObjects(actors) + " " + action.ToString();

        string addedTarget = action switch
        {
            EventAction.Met => UtilitiesF.GetEnumerationOfObjects(receivers),
            EventAction.SawEachOther => UtilitiesF.GetEnumerationOfObjects(receivers),
            EventAction.WasGoingTo => worldPoint.locationName,
            EventAction.Killed => UtilitiesF.GetEnumerationOfObjects(receivers),
            EventAction.ArrivedAt => worldPoint.locationName,
            EventAction.Left => worldPoint.locationName,
            EventAction.WasDead => "",
            EventAction.Undefined => "UNDEFINED",
            _ => "UNDEFINED",
        };

        if (addedTarget != "") eventText += " " + addedTarget;

        eventText += $" at turn {turn}.";

        if (cause != "") eventText += " Because " + cause;

        return eventText;
    }
}

public enum EventAction
{
    Undefined,
    Met,
    SawEachOther,
    WasGoingTo,
    Killed,
    ArrivedAt,
    Left,
    WasDead
}
