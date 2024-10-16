using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 startRot;
    private Transform parentTransform;
    private void Start()
    {
        startPos = transform.position;
        startRot = transform.eulerAngles;
        parentTransform = transform.parent;
    }

    public void ResetContainer()
    {
        transform.parent = parentTransform;
        transform.position = startPos;
        transform.eulerAngles = startRot;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
