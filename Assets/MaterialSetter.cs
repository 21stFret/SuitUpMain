using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    public Material portalBlack;

    // Start is called before the first frame update
    void Start()
    {
        var color = portalBlack.color;
        color.a = 0;
        portalBlack.color = color;
    }
}
