using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 startRot;
    private void Start()
    {
        startPos = transform.position;
        startRot = transform.eulerAngles;
    }

    public void ResetCargo()
    {
        transform.position = startPos;
        transform.eulerAngles = startRot;
    }
}
