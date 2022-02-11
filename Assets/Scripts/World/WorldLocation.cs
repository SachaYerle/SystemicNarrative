using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldLocation : MonoBehaviour
{
    public List<Entity> EntitiesIn = new List<Entity>();
    public List<EntityGroup> EntityGroupsIn = new List<EntityGroup>();
}