using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldLocation : MonoBehaviour
{
    public List<Entity> EntitiesIn = new List<Entity>();
    public string locationName { get; protected set; }
}