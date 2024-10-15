using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class LineRender : MonoBehaviour
{
    public LineRenderer lineRenderer;


    public Transform[] controlPoints;
    public int resolution = 10;

    void Start()
    {
        // UpdateLine();
    }

    /*void Update()
    {
        Vector3[] positions = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            positions[i] = GetPointOnCurve(t);
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }*/
    public void UpdateLine()
    {
        Vector3[] positions = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            positions[i] = GetPointOnCurve(t);
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    Vector3 GetPointOnCurve(float t)
    {

        Vector3 p0 = controlPoints[0].position;
        Vector3 p1 = controlPoints[1].position;
        Vector3 p2 = controlPoints[2].position;
        Vector3 p3 = controlPoints[3].position;
        //return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
        return Mathf.Pow(1 - t, 3) * p0 +
           3 * Mathf.Pow(1 - t, 2) * t * p1 +
           3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
           Mathf.Pow(t, 3) * p3;

    }

    public Vector3[] GetPosArray()
    {
        Vector3[] posArray = new Vector3[lineRenderer.positionCount];

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            posArray[i] = lineRenderer.GetPosition(i);
        }

        return posArray;
    }

    public void RemoveLastPoint()
    {
        if (lineRenderer.positionCount > 0)
        {
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            // Shift points to the left, effectively removing the first point
            for (int i = 0; i < lineRenderer.positionCount - 1; i++)
            {
                positions[i] = positions[i + 1];
            }

            // Reduce the position count by 1
            lineRenderer.positionCount -= 1;

            // Apply the updated positions
            lineRenderer.SetPositions(positions);
        }
    }

    public void ResetWayPoints(Vector3[] wayPoints)
    {
        lineRenderer.positionCount = wayPoints.Length;
        lineRenderer.SetPositions(wayPoints);
    }

    public void AddFirstPoint(Vector3 nextpos)
    {
        if (lineRenderer.positionCount > 0)
        {
            Vector3[] positions = new Vector3[lineRenderer.positionCount+1];
            lineRenderer.GetPositions(positions);


            // Shift points to the left, effectively removing the first point
            for (int i = lineRenderer.positionCount; i >0; i--)
            {
                positions[i] = positions[i - 1];
            }

            positions[0] = nextpos;

            // Reduce the position count by 1
            lineRenderer.positionCount = positions.Length;

            // Apply the updated positions
            lineRenderer.SetPositions(positions);
        }
    }

}
