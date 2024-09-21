using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManualWeaponController : MonoBehaviour
{
    public InputActionAsset primaryActions;
    InputActionMap gameplayActionMap;
    public PlayerInput playerInput;
    public InputAction FireInputAction;
    public InputAction FireManualInputAction;
    public MechWeapon equipedWeapon;
    public MechWeapon equipedWeaponP;
    public GameObject rotatingObject;
    public Transform rotatingPivot;

    public float inputTimeOut = 1f;

    public float aimZ;
    public float aimX;
    public float rotationSpeed;

    public bool isAiming;
    private bool initialized;
    public void Init(MechWeapon mechWeapon = null)
    {
        rotatingObject.transform.SetParent(null);
        if (mechWeapon != null)
        {
            if (mechWeapon.weaponData.mainWeapon)
            {
                equipedWeaponP = mechWeapon;
                SetPrimaryWeaponInputs();
            }
            else
            {
                SetManualWeaponInputs();
                equipedWeapon = mechWeapon;
            }
        }

        ResetAim();

        initialized = true;
    }

    private void SetManualWeaponInputs()
    {
        gameplayActionMap = primaryActions.FindActionMap("Gameplay");
        FireInputAction = gameplayActionMap.FindAction("Fire");
        FireInputAction.performed += GetFireInput;
        FireInputAction.canceled += FireRelease;
    }

    private void SetPrimaryWeaponInputs()
    {
        gameplayActionMap = primaryActions.FindActionMap("Gameplay");
        FireInputAction = gameplayActionMap.FindAction("FireP");
        FireInputAction.performed += FirePrimary;
        FireInputAction.canceled += StopPrimary;
    }

    public void SetAltWeaponInputs()
    {
        FireManualInputAction = gameplayActionMap.FindAction("FireAlt");
        FireManualInputAction.performed += GetFireAltInput;
        FireManualInputAction.canceled += FireAltRelease;
    }

    public void ClearWeaponInputs()
    {
        FireInputAction.performed -= GetFireInput;
        FireInputAction.canceled -= FireRelease;
        FireManualInputAction.performed -= GetFireAltInput;
        FireManualInputAction.canceled -= FireAltRelease;
    }

    private void GetFireInput(InputAction.CallbackContext context)
    {
        Fire();
    }

    private void FireRelease(InputAction.CallbackContext context)
    {
        Stop();
    }

    private void GetFireAltInput(InputAction.CallbackContext context)
    {
        FireAlt();
    }

    private void FireAltRelease(InputAction.CallbackContext context)
    {
        StopAlt();
    }

    private void FirePrimary(InputAction.CallbackContext context)
    {
        FireP();
    }

    private void StopPrimary(InputAction.CallbackContext context)
    {
        StopP();
    }

    public void FireP()
    {
        equipedWeaponP.Fire();
    }

    public void StopP()
    {
        equipedWeaponP.Stop();
    }

    public void Fire()
    {
        equipedWeapon.Fire();
    }

    public void Stop()
    {
        equipedWeapon.Stop();
    }

    public void FireAlt()
    {
        equipedWeapon.FireAlt();
    }

    public void StopAlt()
    {
        equipedWeapon.StopAlt();
    }

    public void FixedUpdate()
    {
        rotatingObject.transform.position = rotatingPivot.position;
        if (!initialized)
        {
            return;
        }
        /*
        if (inputTimeOut <= 0)
        {
            float speed = rotationSpeed * Time.deltaTime;
            rotatingObject.transform.rotation = Quaternion.RotateTowards(rotatingObject.transform.rotation, transform.rotation, speed);
            isAiming = false;
            return;
            //ResetAim();
        }
        */
        Aiming();

    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 movementVector = context.ReadValue<Vector2>();

        if (movementVector == Vector2.zero)
        {
            return;
        }

        var cam = Camera.main;
        if (playerInput.currentControlScheme == "PC")
        {
            movementVector.x -= cam.pixelWidth / 2;
            movementVector.y -= cam.pixelHeight / 2;
        }

        isAiming = true;
        inputTimeOut = 1f;
        aimX = movementVector.x;
        aimZ = movementVector.y;
    }

    private void Aiming()
    {
        if (!isAiming)
        {
            return;
        }
        Vector3 lookDirection = new Vector3(aimX, 0, aimZ);
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection, transform.up);
        //equipedWeapon.transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, rotationSpeed * Time.deltaTime);
        float speed = rotationSpeed * Time.deltaTime;
        rotatingObject.transform.rotation = Quaternion.RotateTowards(rotatingObject.transform.rotation, lookRotation, speed);

        inputTimeOut -= Time.deltaTime;
    }

    private void ResetAim()
    {
        //isAiming = false;
        aimX = 0;
        aimZ = 0;
    }


}
