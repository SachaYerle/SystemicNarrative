using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabData", menuName = "ScriptableObjects/PrefabData")]
public class PrefabData : ScriptableObject
{
    public WorldPoint WorldPointPrefab = null;
    public TextAsset CityNames = null;
    public TextAsset EntityNames = null;
    public TextAsset FactionNames = null;
    public AnimationCurve FoodGainCurve = null;
}
