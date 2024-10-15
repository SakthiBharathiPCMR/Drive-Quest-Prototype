using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarMovement : MonoBehaviour
{
    // public Transform endPos;
    public LineRender lineRender;
    public float moveSpeed = 1f;
    private Vector3[] wayPoint;
    private Vector3[] startwayPoint;
    private Vector3 startPos;
    private Vector3 startRot;
    private int counter = 0;
    public bool isMoved;
    public bool isDetect;
    private CarMovement carMovement;

    private void Start()
    {
        wayPoint = lineRender.GetPosArray();
        startwayPoint = wayPoint;
        startPos = transform.position;
        startRot = transform.eulerAngles;
    }
    private void OnMouseDown()
    {
        if (isMoved) return;
        isMoved = true;

        StartCoroutine(MoveThroughWaypoints());

    }


    private IEnumerator MoveThroughWaypoints()
    {
        foreach (Vector3 waypoint in wayPoint)
        {

            yield return StartCoroutine(MovePos(waypoint));
        }

        Debug.Log("Finished");
    }

    private IEnumerator MovePos(Vector3 nextPos)
    {
        Vector3 startPos = transform.position;
        float startTime = 0f;
        float endTime = 0.2f;
        while (startTime < endTime)
        {
            float t = startTime / endTime;
            transform.position = Vector3.Lerp(startPos, nextPos, t);


            Vector2 direction = new Vector2(nextPos.z - transform.position.z, nextPos.x - transform.position.x);
            float angle = Mathf.Atan2(direction.normalized.y, direction.normalized.x);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f), t);


            startTime += Time.deltaTime;
            yield return null;

        }
        lineRender.RemoveLastPoint();
        transform.position = nextPos;
        counter++;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent(out CarMovement carMovement) && isMoved && !isDetect)
        {
            isDetect = true;
            StartCoroutine(DelayStop());
            // transform.GetComponent<BoxCollider>().enabled = false;
            //carMovement.ResetCar();
            this.carMovement = carMovement;
            
        }

    }



    private IEnumerator DelayStop()
    {
        yield return new WaitForSeconds(0.3f);
        StopAllCoroutines();
        Invoke("ResetCar", 0.5f);
    }


    public void ResetCar()
    {
        StartCoroutine(MoveReverseThroughWaypoints());
    }


    public void ResetAfterCollided()
    {
        lineRender.ResetWayPoints(startwayPoint);
        transform.position = startPos;
        transform.eulerAngles = startRot;
        isMoved = false;
        counter = 0;
        isDetect = false;

        //transform.GetComponent<BoxCollider>().enabled = true;
    }

    private IEnumerator MoveReverseThroughWaypoints()
    {
        //transform.rotation = Quaternion.Euler(0f, 0f,0f);

        for (int i = counter-1; i >= 0; i--)
        {
            yield return StartCoroutine(MoveReversePos(startwayPoint[i]));

        }
        Debug.Log("Reversed");
        ResetAfterCollided();
        carMovement.ResetAfterCollided();

    }

    private IEnumerator MoveReversePos(Vector3 nextPos)
    {
        Vector3 startPos = transform.position;
        float startTime = 0f;
        float endTime = 0.2f;


        while (startTime < endTime)
        {
            float t = startTime / endTime;
            transform.position = Vector3.Lerp(startPos, nextPos, t);

          


                Vector2 direction = new Vector2(nextPos.z - transform.position.z, nextPos.x - transform.position.x);
                float angle = Mathf.Atan2(direction.normalized.y, direction.normalized.x);
                angle += Mathf.PI;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f), t);


            startTime += Time.deltaTime;
            yield return null;

        }
        //lineRender.RemoveLastPoint();
        lineRender.AddFirstPoint(nextPos);
        transform.position = nextPos;




    }
}