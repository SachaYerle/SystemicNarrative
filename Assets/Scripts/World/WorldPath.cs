using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldPath : WorldLocation
{
    public WorldPathData wpD = null;
    public SpriteRenderer sp = null;

    public Color pathTakenColor = Color.white;
    public Color pathEmptyColor = Color.white;

    private void Start()
    {
        Vector3 dif = wpD.pathEndB.transform.position - wpD.pathEndA.transform.position;
        transform.localScale = new Vector3(0.1f, dif.magnitude, 1);
        transform.up = dif;
        transform.position = Vector3.Lerp(wpD.pathEndA.transform.position, wpD.pathEndB.transform.position, .5f);
    }
    private void Update()
    {
        sp.color = EntitiesIn.Count > 0 ? pathTakenColor : pathEmptyColor;
    }
}

public class WorldPathData
{
    public bool Available = false;
    public WorldPoint pathEndA = null;
    public WorldPoint pathEndB = null;
    public float pathLength = 0;
    public WorldPath wp = null;
}
