using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public void SetLaserLength(float range)
    {
        lineRenderer.SetPosition(1, new Vector3(0,0,range));
    }
}
