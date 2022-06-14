using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventKnowledge
{
    public readonly EventData eventD;
    public readonly HowLearnt howLearnt;

    public EventKnowledge(EventData eventD, HowLearnt howLearnt)
    {
        this.eventD = eventD;
        this.howLearnt = howLearnt;
    }
}

public enum HowLearnt
{
    Undefined,
    WasThere,
    Heard
}
