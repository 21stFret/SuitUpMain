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
    public float rotateSpeed;
    private Vector3 direction = Vector3.forward;
    public bool isAimLocked;
    public Vector3 aimDirectionLoc;
    private Quaternion lookRotation = Quaternion.identity;
    public GameObject aimDirection;
    public GameObject lookingDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
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

    public void Die()
    {
        isRunning = false;
        runAudio.Stop();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInputVector = context.ReadValue<Vector2>();
        float inputedY = _moveInputVector.y;
        CharacterAnimator.SetFloat("InputMag", _moveInputVector.magnitude);
        CharacterAnimator.SetFloat("Forward", _moveInputVector.magnitude);
        CharacterAnimator.SetFloat("Turn", _moveInputVector.x);

        if (isAimLocked)
        {
            if (Vector3.Dot(transform.forward, new Vector3(_moveInputVector.x, 0, _moveInputVector.y)) < 0)
            {
                CharacterAnimator.SetFloat("Forward", _moveInputVector.y);

            }
        }



        if (_moveInputVector.magnitude > 0)
        {
            CharacterAnimator.speed = _moveInputVector.magnitude + 0.2f;
            isRunning = true;
        }
        else
        {
            CharacterAnimator.speed = 1;
            isRunning = false;
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
            aimDirection.transform.position = aimDirectionLoc;
            aimDirection.SetActive(true);
            //lookingDirection.SetActive(false);
            Vector3 lookDirection = aimDirectionLoc - transform.position;
            lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
        else
        {
            aimDirection.SetActive(false);
            //lookingDirection.SetActive(true);
            aimDirectionLoc = Vector3.zero;
            lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        if (isRunning)
        {
            direction = new Vector3(_moveInputVector.x, 0, _moveInputVector.y);
            direction.Normalize();

            float inputedSpeed = _moveInputVector.magnitude * Speed;
            _rigidbody.AddForce(direction * inputedSpeed);

        }
        RotateMech();
        PlayRunningFSX();
    }

}
