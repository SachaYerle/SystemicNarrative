using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldLocation : MonoBehaviour
{
    public List<Entity> EntitiesIn = new List<Entity>();
    public string locationName { get; protected set; }
    public SpriteRenderer mainSpR = null;

    protected void UpdateColor () => mainSpR.color = Color.Lerp(mainSpR.color, EntitiesIn.Count > 0 ? Color.white : Color.black, Time.deltaTime * 2);

    protected virtual void Update()
    {
        UpdateColor();
    }
}