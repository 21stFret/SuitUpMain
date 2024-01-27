using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalDaylight : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.right, speed * Time.deltaTime);
    }
}
