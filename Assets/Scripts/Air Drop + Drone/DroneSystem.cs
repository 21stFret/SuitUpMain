using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneSystem : MonoBehaviour
{
    public Transform startPos;
    public float yPos;
    public Transform target;
    public float DroneSpeed;
    public float slewSpeed;
    public float stoppingDistance;
    public float pingTime;
    private float pingTimeT;
    public Vector3 hoverOffest;

    public Transform CratePivot;
    public bool finishedAbility;

    public GameObject droneMesh;
    public AirDropCrate airDropCrate;
    public bool hasCreate;
    public Crawler crawler;
    public LayerMask layerMask;
    public AudioClip droneStart;
    public AudioClip droneLoop;
    private AudioSource _audioSource;
    public AudioSource _incomingAudioSource;

    public bool active;
    private bool hovering;
    private bool triggerAbility;

    private GameObject player;

    public OrbitalStrike orbitalStrike;
    public Minigun minigun;
    public float companionTime;
    public float companionDamage;
    private DroneType currentType;
    public List<AudioClip> droneIncomingSounds;

    [InspectorButton("Init")]
    public bool init;

    public void Init(DroneType droneType)
    {
        _audioSource = GetComponent<AudioSource>();
        player = BattleMech.instance.gameObject;
        transform.position = startPos.position;
        minigun.gameObject.SetActive(false);
        minigun.autoTurret = false;
        triggerAbility = false;
        finishedAbility = false;
        hasCreate = false;
        currentType = droneType;

        switch (droneType)
        {
            case DroneType.Repair:
                InitCrate();
                break;
            case DroneType.Shield:
                InitCrate();
                break;
            case DroneType.Airstrike:
                BattleMech.instance.droneController.MissileStrike();
                active = true;
                break;
            case DroneType.ElementBomb:
                BattleMech.instance.droneController.FatManLaunch();
                active = true;
                break;
            case DroneType.Orbital:
                PingClosestEnemy();
                stoppingDistance = 0.5f;
                break;
            case DroneType.Companion:
                PingClosestEnemy();
                minigun.gameObject.SetActive(true);
                stoppingDistance = 5;
                break;
        }
        int droneTypeInt = (int)droneType;
        if(droneTypeInt < droneIncomingSounds.Count)
        {
            _incomingAudioSource.PlayOneShot(droneIncomingSounds[droneTypeInt]);
        }

        ToggleActive();
    }

    private void InitCrate()
    {
        stoppingDistance = 0.5f;
        target = BattleMech.instance.transform;
        hasCreate = true;
        airDropCrate.transform.SetParent(CratePivot);
        airDropCrate.crateType = currentType;
        airDropCrate.Init();
    }

    private void ToggleActive()
    {
        active = !active;
        droneMesh.SetActive(active);
        if (active)
        {
            _audioSource.clip = droneStart;
            _audioSource.loop = false;
            _audioSource.Play();
            StartCoroutine(DelayLoopSFX());
        }
        else
        {
            _audioSource.loop = false;
            _audioSource.Stop();
        }
    }

    private IEnumerator DelayLoopSFX()
    {
        yield return new WaitForSeconds(droneStart.length);
        if (!active)
        {
            yield break;
        }
        _audioSource.clip = droneLoop;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void PingClosestEnemy()
    {
        var colliders = Physics.OverlapSphere(player.transform.position, 50f, layerMask);
        if(colliders.Length>0)
        {
            float closestDist = Mathf.Infinity;
            foreach (var collider in colliders)
            {
                float dist = Vector3.Distance(transform.position, collider.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    target = collider.transform;
                    crawler = collider.GetComponent<Crawler>();
                }
            }
        }
        else
        {
            target = player.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active)
        {
            return;
        }
        
        if(crawler!=null)
        {
            if (crawler.dead)
            {
                PingClosestEnemy();
                return;
            }
        }

        if (finishedAbility)
        {
            LeaveBattlefield();
            return;
        }


        Vector3 newPos = target.position + hoverOffest;
        Vector3 newDirection = newPos - transform.position;
        newDirection.y = 0;
        transform.forward = Vector3.RotateTowards(transform.forward, newDirection, Time.deltaTime * slewSpeed, 0.0f);

        if(transform.position.y > yPos)
        {
            transform.position -= transform.up * Time.deltaTime * DroneSpeed;
        }

        if (!hovering)
        {
            // Smoothly interpolate the position towards the target
            var newPosition = Vector3.MoveTowards(transform.position, transform.position + transform.forward.normalized, DroneSpeed * Time.deltaTime);
            transform.position = newPosition;
        }

        float dist = (transform.position.y - target.position.y) + stoppingDistance;

        if (Vector3.Distance(transform.position, target.position) < dist)
        {
            StartCoroutine(DelayHover(true));
            TriggerAbility();
        }
        else
        {
            StartCoroutine(DelayHover(false));
        }


        if(currentType != DroneType.Repair)
        {
            pingTimeT -= Time.deltaTime;
            if (pingTimeT <= 0)
            {
                PingClosestEnemy();
                pingTimeT = pingTime;
            }
        }
    }

    private void LeaveBattlefield()
    {
        transform.position += transform.up * Time.deltaTime * DroneSpeed;
        if (transform.position.y > 35)
        {
            ToggleActive();
        }
    }

    private IEnumerator DelayHover(bool value)
    {
        yield return new WaitForSeconds(0.1f);
        hovering = value;
    }

    private void TriggerAbility()
    {
        if(triggerAbility)
        {
            return;
        }
        switch(currentType)
        {
            case DroneType.Repair:
                airDropCrate.Launch();
                finishedAbility = true;
                break;
            case DroneType.Shield:
                airDropCrate.Launch();
                finishedAbility = true;
                break;
            case DroneType.Orbital:
                orbitalStrike.Init(this);
                break;
            case DroneType.Companion:
                StartCoroutine(DelayFinishedAbility(companionTime));
                minigun.damage = companionDamage;
                minigun.autoTurret = true;
                break;
        }
        triggerAbility = true;
    }

    private IEnumerator DelayFinishedAbility(float delay)
    {
        yield return new WaitForSeconds(delay);
        finishedAbility = true;
        minigun.autoTurret = false;
    }
}
