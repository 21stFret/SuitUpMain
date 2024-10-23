using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MYCharacterController : MonoBehaviour
{

    public Vector2 _moveInputVector;
    public AudioSource runAudio;
    private bool isRunning;
    private Rigidbody _rigidbody;
    public Animator CharacterAnimator;
    public float Speed;
    public float minSpeed;
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
    public ParticleSystem dashEffect, dashEffect2;
    public MeshRenderer dashShoes, dashShoes2;
    public WeaponController manualWeaponController;
    public ParticleSystem footStep, footStep2;
    public bool candodge;
    public FootprintSystem footprintSystem;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        lastPos = transform.position;
    }

    private void PlayRunningFSX()
    {

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

    }

    public void PlayFootStep()
    {
        footStep.Play();
    }

    public void PlayFootStep2()
    {
        footStep2.Play();
    }

    public void Dash(InputAction.CallbackContext context)
    {
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
        dashShoes.enabled = false;
        dashShoes2.enabled = false;
        dashEffect.Play();
        dashEffect2.Play();
        isDodging = true;
        _rigidbody.AddForce(direction * dashForce, ForceMode.Impulse);
        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        isDodging = false;
        dashShoes.enabled = true;
        dashShoes2.enabled = true;
    }

    public void Die()
    {
        isRunning = false;
        footprintSystem.IsMoving = false;
        runAudio.Stop();
    }

    public void TriggerFootLeft()
    {
        footprintSystem.PlaceLeftFoot();
        //print("TriggerFootLeft");
    }

    public void TriggerFootRight()
    {
        footprintSystem.PlaceRightFoot();
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotateSpeed);

        if(transform.rotation != lookRotation)
        {
            CharacterAnimator.SetBool("isTurning", true);
        }
        else
        {
            CharacterAnimator.SetBool("isTurning", false);
        }
    }

    private void FixedUpdate()
    {
        if (isAimLocked)
        {
            if (aimDirectionLoc == Vector3.zero)
            {
                aimDirectionLoc = transform.position + transform.forward *10;
            }
            //aimDirection.transform.position = aimDirectionLoc;
            //aimDirection.SetActive(true);
            //lookingDirection.SetActive(false);
            Vector3 lookDirection = aimDirectionLoc - transform.position;
            lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
        else
        {
            //aimDirection.SetActive(false);
            //lookingDirection.SetActive(true);
            aimDirectionLoc = Vector3.zero;
            lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            Vector3 lookDirection = new Vector3(manualWeaponController.aimX, 0, manualWeaponController.aimZ);
            //lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        }

        if (isRunning)
        {
            direction = new Vector3(_moveInputVector.x, 0, _moveInputVector.y);
            direction.Normalize();
            float inputedSpeed = _moveInputVector.magnitude * Speed;
            float bonusSpeed = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.Speed);
            if (bonusSpeed != 0)
            {
                print("Speed Bonus: " + bonusSpeed);
                inputedSpeed = bonusSpeed;
                if (inputedSpeed <= 0)
                {
                    inputedSpeed = minSpeed;
                }
            }
            _rigidbody.AddForce(direction * inputedSpeed);

        }
        RotateMech();
        PlayRunningFSX();
        CheckDistance();
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

}
