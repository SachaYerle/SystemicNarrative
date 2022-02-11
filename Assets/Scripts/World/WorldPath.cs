using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldPath : WorldLocation
{
    public WorldPathData wpD = null;

    private void Start()
    {
        Vector3 dif = wpD.pathEndB.transform.position - wpD.pathEndA.transform.position;
        transform.localScale = new Vector3(0.1f, dif.magnitude, 1);
        transform.up = dif;
        transform.position = Vector3.Lerp(wpD.pathEndA.transform.position, wpD.pathEndB.transform.position, .5f);
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
