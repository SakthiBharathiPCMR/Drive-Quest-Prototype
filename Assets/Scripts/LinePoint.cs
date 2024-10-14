using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePoint : MonoBehaviour
{
    public LineRender lineRender;
    void OnDrawGizmosSelected()
    {
        lineRender.UpdateLine();
    }

}
