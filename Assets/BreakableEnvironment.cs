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
        GetComponent<BreakableObject>().Break();
        hit = true;
    }
    

}
