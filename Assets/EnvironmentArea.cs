using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentArea : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public GameObject refreshingObjectsParent;
    public bool insideArea = false;

    public void RefreshArea()
    {
        if(refreshingObjectsParent == null)
        {
            Debug.LogError("No refreshing objects parent set on " + gameObject.name);
            return;
        }
        Prop[] props = refreshingObjectsParent.GetComponentsInChildren<Prop>();
        foreach (Prop child in props)
        {
            child.RefreshProp();

        }
        DamageArea[] damageAreas = refreshingObjectsParent.GetComponentsInChildren<DamageArea>();
        foreach (DamageArea child in damageAreas)
        {
            child.Init();
        }
    }
}
