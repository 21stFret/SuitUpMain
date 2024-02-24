using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTrigger : MonoBehaviour
{
    public FOVCollider fovCollider;
    public TriggerSensor triggerSensor;
    public float shotSpeed;
    public float shotDamage;
    private bool isOn;

    private float timer;

    private void Awake()
    {
    }

    public void InitFlameTrigger(float damage, float speed, float range)
    {
        shotDamage = damage;
        shotSpeed = speed;
        fovCollider.Length = range;
    }

    public void SetCol(bool value)
    {
        fovCollider.enabled = value;
        isOn = value;
    }

    private void Update()
    {
        if (!isOn)
        {
            timer = 0;
            return;
        }

        if (triggerSensor.GetDetections().Count <= 0)
        { return; }

        timer += Time.deltaTime;
        if(timer > shotSpeed)
        {


            foreach (GameObject hit in triggerSensor.GetDetections())
            {
                if(hit.tag == "Tree")
                {
                    hit.GetComponent<Tree>().TriggerOnFire();
                }
                else
                {
                    hit.GetComponent<Crawler>().TakeDamage(shotDamage);
                }

            }
            timer = 0;
        }
    }
}
