using System.Collections;
using System.Collections.Generic;
using FORGE3D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MYCharacterController : MonoBehaviour
{
    public Vector2 _moveInputVector;
    public AudioSource runAudio;
    public AudioClip dodgeClip;
    public AudioClip[] footStepAudio;
    private bool isRunning;
    private Rigidbody _rigidbody;
    public Animator CharacterAnimator;
    public float Speed;
    public float maxSpeed;
    public float minSpeed;
    private float bonusSpeed;
    public float rotateSpeed;
    private Vector3 direction = Vector3.forward;
    public bool isAimLocked;
    public Vector3 aimDirectionLoc;
    private Quaternion lookRotation = Quaternion.identity;
    public GameObject aimDirection;
    public GameObject lookingDirection;
    public float distanceTravelled;
    private float distTimer;
    private Vector3 lastPos;
    public bool isDodging;
    public float dashForce;
    public float dashCooldown;
    public float dashDuration;
    public ParticleSystem dashEffect, dashEffect2;
    public MeshRenderer dashShoes, dashShoes2;
    public WeaponController weaponController;
    public ParticleSystem footStep, footStep2;
    public bool candodge;
    public FootprintSystem footprintSystem;
    public ParticleSystem icedEffect;
    public ParticleSystem sandEffect;
    private bool isSlowed;
    public float slowedDuration = 2f;
    public float slowedAmount = 0.5f;
    [HideInInspector]
    public DashModsManager dashModsManager;

    public bool onIce;
    public float iceDrift = 0.8f;

    public float weaponFiringSlowAmount;
    [HideInInspector]
    public BattleMech battleMech;
    [SerializeField]
    private bool canMove;
    [HideInInspector]
    public float savedDodgeDuration;
    [HideInInspector]
    public float savedDodgeCooldown;
    [HideInInspector]
    public float savedDodgeForce;

    [Header("Collision Prevention")]
    [SerializeField] private float collisionCheckDistance = 1f;
    [SerializeField] private LayerMask collisionMask;

    [Header("Ice Movement")]
    [SerializeField] private float iceAcceleration = 2f;
    [SerializeField] private float iceDashForceMultiplier = 0.5f;
    [SerializeField] private float iceDragMultiplier = 0.2f;
    [SerializeField] private float normalDrag = 3f;

    private void Awake()
    {
        savedDodgeForce = dashForce;
        savedDodgeDuration = dashDuration;
        savedDodgeCooldown = dashCooldown;
        _rigidbody = GetComponent<Rigidbody>();
        lastPos = transform.position;
        dashModsManager = GetComponent<DashModsManager>();
        Invoke("CacheBattleManager", 0.1f);
        if(dashModsManager!=null)
        {
            dashModsManager.Init();
        }
    }

    private void CacheBattleManager()
    {
        battleMech = BattleMech.instance;
    }

    private void PlayRunningFSX()
    {

/*
        if (isRunning)
        {
            if (!runAudio.isPlaying)
            {
                runAudio.Play();
            }
            runAudio.pitch = Mathf.Clamp(_moveInputVector.magnitude + 0.2f, 0 , 1f);
            return;
        }
        else
        {
            runAudio.Stop();
        }
        */

    }

    public void PlayFootStep()
    {
        footStep.Play();
        runAudio.clip = footStepAudio[Random.Range(0, footStepAudio.Length)];
        runAudio.pitch = Random.Range(0.9f, 1.1f);
        runAudio.PlayOneShot(runAudio.clip);
    }

    public void PlayFootStep2()
    {
        footStep2.Play();
        runAudio.clip = footStepAudio[Random.Range(0, footStepAudio.Length)];
        runAudio.pitch = Random.Range(0.9f, 1.1f);
        runAudio.PlayOneShot(runAudio.clip);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!canMove)
        {
            return;
        }
        if (!candodge)
        {
            return;
        }
        if (!context.performed)
        {
            return;
        }
        if (isDodging)
        {
            return;
        }
        candodge = false;
        dashShoes.enabled = false;
        dashShoes2.enabled = false;
        dashEffect.Play();
        dashEffect2.Play();
        runAudio.pitch = Random.Range(0.9f, 1.4f);
        runAudio.PlayOneShot(dodgeClip);
        isDodging = true;
        if(dashModsManager!=null)
        {
            dashModsManager.UseMod();
        }
        float _dashForce = dashForce;
        if (onIce)
        {
            _dashForce *= iceDashForceMultiplier;
        }
        _rigidbody.AddForce(direction * _dashForce, ForceMode.Impulse);
        StartCoroutine(DashCooldown());
    }

    public void ResetStats()
    {
        dashDuration = savedDodgeDuration;
        dashCooldown = savedDodgeCooldown;
        dashForce = savedDodgeForce;
    }

    private IEnumerator DashCooldown()
    {
        runAudio.pitch = 1;
        yield return new WaitForSeconds(dashDuration);
        isDodging = false;
        if(dashModsManager!=null)
        {
            if (dashModsManager.invincible)
            {
                dashModsManager.InvincibleCall();
            }
        }
        yield return new WaitForSeconds(dashCooldown);
        candodge = true;
        dashShoes.enabled = true;
        dashShoes2.enabled = true;
    }

    public void Die()
    {
        isRunning = false;
        footprintSystem.IsMoving = false;
        runAudio.Stop();
        canMove = false;
    }

    public void TriggerFootLeft()
    {
        footprintSystem.PlaceLeftFoot();
        PlayFootStep();
        //print("TriggerFootLeft");
    }

    public void TriggerFootRight()
    {
        footprintSystem.PlaceRightFoot();
        PlayFootStep2();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(!enabled)
        {             return;
        }

        _moveInputVector = context.ReadValue<Vector2>();
        float inputedY = _moveInputVector.y;
        CharacterAnimator.SetFloat("InputMag", _moveInputVector.magnitude);
        CharacterAnimator.SetFloat("Forward", _moveInputVector.magnitude);
        CharacterAnimator.SetFloat("Turn", _moveInputVector.x);

        if (_moveInputVector.magnitude > 0)
        {
            CharacterAnimator.speed = _moveInputVector.magnitude + 0.2f;
            isRunning = true;
            footprintSystem.IsMoving = true;
        }
        else
        {
            CharacterAnimator.speed = 1;
            isRunning = false;
            footprintSystem.IsMoving = false;
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAimLocked = true;
        }
        if (context.canceled)
        {
            isAimLocked = false;
            direction = transform.forward;
        }
        CharacterAnimator.SetBool("isAiming", isAimLocked);
    }   

    private void RotateMech()
    {
        if(!canMove)
        {
            return;
        }

        // Smoothly interpolate rotation based on rotateSpeed
        float step = rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, step);
    }

    private void Movement()
    {
        if (isAimLocked)
        {
            if (aimDirectionLoc == Vector3.zero)
            {
                aimDirectionLoc = transform.position + transform.forward *10;
            }
            Vector3 lookDirection = aimDirectionLoc - transform.position;
            lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
        else
        {
            aimDirectionLoc = Vector3.zero;
            lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            Vector3 lookDirection = new Vector3(weaponController.aimX, 0, weaponController.aimZ);
        }

        if (isRunning)
        {
            direction = new Vector3(_moveInputVector.x, 0, _moveInputVector.y);
            direction.Normalize();

            bool canMoveInDirection = !Physics.Raycast(transform.position, direction, collisionCheckDistance, collisionMask);
            if (!canMoveInDirection)
            {
                return;
            }

            Vector3 icedPos = transform.position;
            icedPos.y += 2.6f;
            icedEffect.transform.position = icedPos;
                   
            if(!isDodging)
            {
                float adjustedSpeed = Speed;
                if (bonusSpeed != 0)
                {
                    adjustedSpeed = bonusSpeed;
                    if (adjustedSpeed <= 0)
                    {
                        adjustedSpeed = minSpeed;
                    }
                }

                float inputedSpeed = _moveInputVector.magnitude * adjustedSpeed;
                float weaponFiringSlow = weaponFiringSlowAmount * weaponController.WeaponsFiring();
                if(weaponFiringSlow !=0) 
                { inputedSpeed *= 1-weaponFiringSlow; }

                if (isSlowed)
                {
                    inputedSpeed *= slowedAmount;
                }



                // Updated ice physics
                if (onIce)
                {
                    // Reduce drag for sliding
                    _rigidbody.drag = normalDrag * iceDragMultiplier;

                    if (isRunning && !isDodging)
                    {
                        // Reduced force application on ice
                        Vector3 desiredVelocity = direction * inputedSpeed;
                        Vector3 velocityChange = (desiredVelocity - _rigidbody.velocity) * iceAcceleration;
                        velocityChange.y = 0f; // Prevent vertical movement
                        _rigidbody.AddForce(velocityChange, ForceMode.Force);
                    }
                }
                else
                {
                    // Normal movement
                    _rigidbody.drag = normalDrag;
                    if (_rigidbody.velocity.magnitude < maxSpeed)
                    {
                        _rigidbody.AddForce(direction * inputedSpeed, ForceMode.Force);
                    }
                    else
                    {
                        _rigidbody.velocity = direction * maxSpeed;
                    }
                }
            }

        }
    }

    private void FixedUpdate()
    {
        if(!canMove)
        {
            return;
        }

        Movement();
        RotateMech();
        PlayRunningFSX();
        CheckDistance();
    }

    public void SetBonusSpeed()
    {
        bonusSpeed = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.Speed);
        print("Base Speed: " + Speed + " and Bonus Speed: " + bonusSpeed);
    }

    public void SetDashCooldown(float value)
    {
        float percent = (Mathf.Abs(value) / 100);
        float cooldown = dashCooldown;
        dashCooldown = cooldown * percent;
        print("Base Dash Cooldown: " + savedDodgeCooldown + " and New Dash Cooldown: " + dashCooldown);
    }

    private void CheckDistance()
    {
        distTimer += Time.deltaTime;
        if(distTimer>1)
        {
            distTimer = 0;
            distanceTravelled += Vector3.Distance(transform.position, lastPos);
            lastPos = transform.position;
        }
    }

    public void TriggerSand()
    {
        if(isSlowed)
        {
            return;
        }
        sandEffect.Play();
        sandEffect.transform.parent = null;
        isSlowed = true;
        StartCoroutine(SandEffect());
    }

    private IEnumerator SandEffect()
    {
        yield return new WaitForSeconds(slowedDuration);
        isSlowed = false;
        sandEffect.Stop();
    }


    public void ApplyIce(float amount)
    {
        slowedAmount = amount;
        icedEffect.Play();
        icedEffect.transform.parent = null;
        isSlowed = true;
        StartCoroutine(IcedEffect());
    }

    private IEnumerator IcedEffect()
    {
        yield return new WaitForSeconds(slowedDuration);
        isSlowed = false;
        icedEffect.Stop();
    }

    public void ToggleCanMove(bool value)
    {
        canMove = value;
        _rigidbody.velocity = value? _rigidbody.velocity : Vector3.zero;
        enabled = value;
        if (!value)
        {
            isRunning = false;
            footprintSystem.IsMoving = false;
            runAudio.Stop();
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            CharacterAnimator.SetFloat("InputMag", 0);
            CharacterAnimator.SetFloat("Forward", 0);
            CharacterAnimator.SetFloat("Turn", 0);
            CharacterAnimator.speed = 0;
        }
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            CharacterAnimator.speed = 1;
            onIce = false;
        }
    }

}
