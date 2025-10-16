using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableEnvironment : MonoBehaviour
{
    private bool hit = false;

    public void Hit()
    {
        if (hit) return;
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        var breakableObject = GetComponent<BreakableObject>();
        if (breakableObject != null)
        {
            breakableObject.transform.parent = null;
            breakableObject.transform.position = transform.position + transform.up * 1;
            breakableObject.Break();
        }
        hit = true;
    }
    

}
