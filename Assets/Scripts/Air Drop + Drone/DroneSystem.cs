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
    public DroneAbilityManager droneAbilityManager;
    public float BombingRunTime = 1f;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        droneAbilityManager = DroneAbilityManager.instance;
        player = BattleMech.instance.gameObject;
        transform.position = startPos.position;
        _audioSource = GetComponent<AudioSource>();
        minigun.gameObject.SetActive(false);
        minigun.autoTurret = false;
    }


    public void UseDroneAbility(DroneType droneType, int _charges)
    {
        triggerAbility = false;
        finishedAbility = false;
        hasCreate = false;
        currentType = droneType;
        minigun.gameObject.SetActive(false);
        minigun.autoTurret = false;
        DroneControllerUI _droneControllerUI = droneAbilityManager.droneControllerUI;
        
        int missileAmount = GetChargeInt(droneAbilityManager._droneAbilities[(int)droneType], _charges - 1);
        _droneControllerUI.missileAmount = missileAmount;

        switch (droneType)
        {
            case DroneType.BurstStrike:
                _droneControllerUI.missileLauncher.missilePayload = MissilePayload.Standard;
                _droneControllerUI.missileAmount *= 6;
                BattleMech.instance.droneController.MissileStrike(Ordanance.Burst);
                active = true;
                break;
            case DroneType.Repair:
                airDropCrate.repairAmount = missileAmount;
                InitCrate();
                break;
            case DroneType.BombingRun:
                _droneControllerUI.missileLauncher.missilePayload = MissilePayload.Standard;
                StartCoroutine(MultipleBombingRuns(missileAmount));
                active = true;
                break;
            case DroneType.Guided:
                _droneControllerUI.missileLauncher.missilePayload = MissilePayload.Standard;
                _droneControllerUI.MissileStrike(Ordanance.Guided);
                active = true;
                break;
            case DroneType.Napalm:
                _droneControllerUI.missileLauncher.missilePayload = MissilePayload.Napalm;
                _droneControllerUI.MissileStrike(Ordanance.Burst);
                active = true;
                break;
            case DroneType.Mines:
                _droneControllerUI.missileLauncher.LaunchMines(missileAmount);
                active = true;
                break;
            case DroneType.LittleBoy:
                _droneControllerUI.missileLauncher.missilePayload = MissilePayload.FatMan;
                _droneControllerUI.LittleBoyLaunch();
                active = true;
                break;
            case DroneType.Companion:
                PingClosestEnemy();
                minigun.gameObject.SetActive(true);
                stoppingDistance = 5;
                companionTime = missileAmount;
                break;
            case DroneType.Orbital:
                PingClosestEnemy();
                stoppingDistance = 0.5f;
                orbitalStrike.beamDuration = missileAmount;
                break;

        }
        int droneTypeInt = (int)droneType;
        if (droneTypeInt < droneIncomingSounds.Count)
        {
            _incomingAudioSource.PlayOneShot(droneIncomingSounds[droneTypeInt]);
        }
        droneAbilityManager.droneControllerUI.airDropTimer.UseCharge(_charges);

        ToggleActive();
    }

    public int GetChargeInt(DroneAbility droneAbility, int index)
    {
        return droneAbility.chargeInts[index];
    }

    private IEnumerator MultipleBombingRuns(int index = 1)
    {
        for (int i = 0; i < index; i++)
        {
            droneAbilityManager.droneControllerUI.missileAmount = 5;
            BattleMech.instance.droneController.MissileStrike(Ordanance.Line);
            yield return new WaitForSeconds(BombingRunTime);
        }
    }

    private void InitCrate()
    {
        stoppingDistance = 0.5f;
        target = BattleMech.instance.transform;
        hasCreate = true;
        airDropCrate.transform.SetParent(CratePivot);
        airDropCrate.crateType = currentType;
        airDropCrate.gameObject.SetActive(true);
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
        
        if (finishedAbility)
        {
            LeaveBattlefield();
            return;
        }
        
        if (crawler != null)
        {
            if (crawler.dead)
            {
                PingClosestEnemy();
                return;
            }
        }

        if (target == null)
        {
            target = player.transform;
            print("Target is null so set to player");
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
            case DroneType.Orbital:
                orbitalStrike.Init(this);
                break;
            case DroneType.Companion:
                StartCoroutine(DelayFinishedAbility(companionTime));
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
