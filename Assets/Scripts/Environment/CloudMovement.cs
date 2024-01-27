using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Update()
    {
        var newPos = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
        transform.position = newPos;
    }
}
