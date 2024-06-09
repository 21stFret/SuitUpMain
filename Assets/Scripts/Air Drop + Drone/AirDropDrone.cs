using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDropDrone : MonoBehaviour
{
    public Transform startPos;
    public Transform targetPos;
    public Vector3 endPos;
    public float DroneSpeed;

    public Transform CratePivot;
    private bool reachedTarget;

    public GameObject droneMesh;
    public AirDropCrate airDropCrate;

    private bool active;
    private float lifeSpan;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        lifeSpan = 3;
    }

    public void Init()
    {
        GetDropOffLocation();
        ToggleActive();
        transform.position = startPos.position;
        transform.LookAt(endPos);
        reachedTarget = false;
        airDropCrate.transform.SetParent(CratePivot);
        airDropCrate.Init();
    }

    private void ToggleActive()
    {
        active = !active;
        droneMesh.SetActive(active);
    }

    private void GetDropOffLocation()
    {
        Vector3 randomPoint = RandomUtils.RandomInsideSphere(2);
        randomPoint.y = 0;
        endPos = targetPos.position + randomPoint;
        endPos.y = startPos.position.y -2;
    }

    // Update is called once per frame
    void Update()
    {
        if(!active)
        {
            return;
        }

        transform.position += transform.forward * Time.deltaTime * DroneSpeed;

        if (reachedTarget)
        {
            transform.position += transform.up * Time.deltaTime * DroneSpeed;
            lifeSpan += Time.deltaTime;
            if (lifeSpan > 5f)
            {
                ToggleActive();
                lifeSpan = 0;
            }
            return; 
        }

        transform.LookAt(endPos);

        if (Vector3.Distance(transform.position, endPos) < 1)
        {
            airDropCrate.Launch();
            reachedTarget = true;
        }
    }
}
