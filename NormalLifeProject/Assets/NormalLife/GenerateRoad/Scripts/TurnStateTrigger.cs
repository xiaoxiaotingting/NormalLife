using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStateTrigger : MonoBehaviour
{
    public int turnStateSetting=0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var c = other.GetComponent<Character>();
            if (c != null)
            {
                if (turnStateSetting == -1)
                {
                    c.SetTurnLeft(true,this);
                }

                if (turnStateSetting == 1)
                {
                    c.SetTurnRight(true,this);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var c = other.GetComponent<Character>();
            if (c != null)
            {
                c.SetTurnLeft();
                c.SetTurnRight();  
            }

        }
    }
}
