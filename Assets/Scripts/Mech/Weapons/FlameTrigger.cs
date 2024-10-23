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
    public bool isOn;
    private float timer;

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
        if(!value)
        {
            //triggerSensor.Clear();
        }
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
                TargetHealth targetHealth = hit.GetComponent<TargetHealth>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(shotDamage, WeaponType.Flame);
                }
            }
            timer = 0;
        }
    }
}
