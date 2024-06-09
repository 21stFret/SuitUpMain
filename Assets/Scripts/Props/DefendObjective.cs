using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendObjective : Prop
{
    public bool objectiveDestroyed;
    public ParticleSystem damaged;
    public ParticleSystem destroyed;
    public GameObject _base;
    public float timer;
    public float timerMax;

    private void Update()
    {
        if (!objectiveDestroyed)
        {
            timer += Time.deltaTime;
            if(timer >= timerMax)
            {
                GameManager.instance.ObjectiveComplete();
                gameObject.SetActive(false);
            }
        }
        GameUI.instance.objectiveUI.UpdateBar(health / healthMax);
        if (health < healthMax/2)
        {
            if(!damaged.isPlaying)
            {
                damaged.Play();
                GameUI.instance.objectiveUI.UpdateObjective("Defend the base!");
            }
        }
    
    }

    public override void Init()
    {
        base.Init();
        objectiveDestroyed = false;
        _base.SetActive(true);
        SetLocation();
    }

    private void SetLocation()
    {
        Vector3 pos = Random.insideUnitSphere * 50;
        pos.y = 1;
        transform.position = pos;
    }

    public override void Die()
    {
        base.Die();
        gameObject.layer = 1;
        TargetHealth targetHealth = GetComponent<TargetHealth>();
        targetHealth.alive = false;
        objectiveDestroyed = true;
        destroyed.Play();
        _base.SetActive(false);
    }
}
