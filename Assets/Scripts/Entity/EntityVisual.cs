using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisual : MonoBehaviour
{
    public SpriteRenderer sp = null;
    public Entity ent;

    private void Start()
    {
        Color.RGBToHSV(sp.color, out float H, out float S, out float V);
        H += Random.Range(0, 1f);
        sp.color = Color.HSVToRGB(H, S,V);
    }

    void Update()
    {
        bool display = ent.onPath;
        sp.enabled = display;
        if (display)
        {
            float percentageOnPath = ent.pathPercentage;
            transform.position = Vector2.Lerp(ent.currPath.pathEndA.transform.position, ent.currPath.pathEndB.transform.position, ent.goingToA ? 1 - percentageOnPath : percentageOnPath);
        }
    }
}
