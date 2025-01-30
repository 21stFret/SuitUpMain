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
    private WeaponType weaponType;

    public void InitFlameTrigger(float damage, float speed, float range, WeaponType type = WeaponType.Flame)
    {
        shotDamage = damage;
        shotSpeed = speed;
        fovCollider.Length = range;
        isOn = false;
        weaponType = type;
        fovCollider.CreateCollider();
    }

    public void SetCol(bool value)
    {
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

            triggerSensor.Pulse();
            List<GameObject> list = new List<GameObject>();
            list = triggerSensor.GetDetections();
            foreach (GameObject hit in list)
            {
                TargetHealth targetHealth = hit.GetComponent<TargetHealth>();
                if (targetHealth != null)
                {
                    if(!targetHealth.alive)
                    {
                        continue;
                    }
                    targetHealth.TakeDamage(shotDamage, weaponType);
                }
            }
            timer = 0;
        }
    }
}
