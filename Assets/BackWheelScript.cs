using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackWheelScript : MonoBehaviour
{
    public KartControl Player;

    void OnTriggerEnter(Collider col)
    {
        Player.CanDrive = true;
        Debug.Log("Drive on");
    }

    void OnTriggerExit(Collider col)
    {
        Player.CanDrive = false;
        Debug.Log("Drive off");
    }
}
