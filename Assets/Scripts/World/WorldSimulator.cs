using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldSimulator : MonoBehaviour
{
    public static int turn = 0;

    public delegate void TickEvent(int turn);
    public static event TickEvent OnTick;

    public TextMeshPro turnText = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            turn++;
            OnTick?.Invoke(turn);
            turnText.text = "Turn " + turn;
        }
    }
}
