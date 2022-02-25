using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldSimulator : MonoBehaviour
{
    public static int turn = 0;

    public delegate void TickEvent(int turn);
    public static event TickEvent OnTickEncounter;
    public static event TickEvent OnTick;
    public static event TickEvent OnTickReset;

    public TextMeshPro turnText = null;

    public float automaticTurnTick = 1;
    float timeSinceLast;

    public bool fastMod = false;

    void Update()
    {
        timeSinceLast = Mathf.MoveTowards(timeSinceLast, 0, Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space) || timeSinceLast == 0 || fastMod)
        {
            turn++;
            OnTickReset?.Invoke(turn);
            OnTick?.Invoke(turn);
            OnTickEncounter?.Invoke(turn);
            turnText.text = "Turn " + turn;
            timeSinceLast = automaticTurnTick;
        }
    }
}
