using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLink : MonoBehaviour
{
    public PrefabData data = null;
    public static PrefabData D { get; private set; }
    private void Awake()
    {
        D = data;
    }
}
