using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaBezierCurve : MonoBehaviour
{
    public int numPoints = 50;

    public void Generator(LineRenderer lineRenderer , Vector3 start, Vector3 p1, Vector3 end)
    {
        lineRenderer.positionCount = numPoints;
        //DrawQuadraticBezierCurve(start, p1, end);

        StartCoroutine(TestCoroutine(lineRenderer,start, p1, end));
    }

   

    IEnumerator TestCoroutine(LineRenderer lineRenderer,Vector3 start, Vector3 p1, Vector3 end)
    {
        Vector3[] positions = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            positions[i] = CalculateQuadraticBezierPoint(t, start, p1, end);
        }
        lineRenderer.SetPositions(positions);
        yield return new WaitForSeconds(.4f);

        lineRenderer.positionCount = 0;

        lineRenderer.gameObject.SetActive(false);

    }



    //void DrawQuadraticBezierCurve(Vector3 start, Vector3 p1, Vector3 end)
    //{
    //    Vector3[] positions = new Vector3[numPoints];
    //    for (int i = 0; i < numPoints; i++)
    //    {
    //        float t = i / (float)(numPoints - 1);
    //        positions[i] = CalculateQuadraticBezierPoint(t, start, p1, end);
    //    }
    //    lineRenderer.SetPositions(positions);
    //}

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0; // (1-t)^2 * P0
        point += 2 * u * t * p1; // 2(1-t)t * P1
        point += tt * p2; // t^2 * P2

        return point;
    }

}
