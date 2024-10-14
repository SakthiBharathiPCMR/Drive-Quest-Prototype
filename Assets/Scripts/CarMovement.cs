using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    // public Transform endPos;
    public LineRender lineRender;
    public float moveSpeed = 1f;
    private Vector3[] wayPoint;
    private Vector3[] startwayPoint;

    public bool isMoved;

    private void Start()
    {
        wayPoint = lineRender.GetPosArray();
        startwayPoint = wayPoint;
    }
    private void OnMouseDown()
    {
        if (isMoved) return;

        StartCoroutine(MoveThroughWaypoints());

    }


    private IEnumerator MoveThroughWaypoints()
    {
        foreach (Vector3 waypoint in wayPoint)
        {

            yield return StartCoroutine(MovePos(waypoint));
        }
        isMoved = true;

        Debug.Log("Finished");
    }

    private IEnumerator MovePos(Vector3 nextPos)
    {

        Vector3 startPos = transform.position;
        float startTime = 0f;
        float endTime = .2f;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out CarMovement carMovement))
        {
            Invoke("StopAllCoroutines", .5f);

        }

    }





}
