using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public InputActionAsset primaryActions;
    InputActionMap gameplayActionMap;
    public PlayerInput playerInput;
    public InputAction FireInputAction;
    public InputAction FireManualInputAction;
    public MechWeapon altWeaponEquiped;
    public MechWeapon mainWeaponEquiped;
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
                mainWeaponEquiped = mechWeapon;
                SetMainWeaponInputs();
            }
            else
            {
                SetAltWeaponInputs();
                altWeaponEquiped = mechWeapon;
            }
        }

        ResetAim();

        initialized = true;
    }

    public void SetFireRate()
    {
        float fireRate = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.FireRate);
        if(fireRate<0)
        {
            fireRate = 0.05f;
        }
        mainWeaponEquiped.fireRate = fireRate;
    }

    private void SetAltWeaponInputs()
    {
        gameplayActionMap = primaryActions.FindActionMap("Gameplay");
        FireInputAction = gameplayActionMap.FindAction("Fire");
        FireInputAction.performed += GetFireInput;
        FireInputAction.canceled += FireRelease;
    }

    private void SetMainWeaponInputs()
    {
        gameplayActionMap = primaryActions.FindActionMap("Gameplay");
        FireInputAction = gameplayActionMap.FindAction("FireP");
        FireInputAction.performed += FirePrimary;
        FireInputAction.canceled += StopPrimary;
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
        mainWeaponEquiped.Fire();
    }

    public void StopP()
    {
        mainWeaponEquiped.Stop();
    }

    public void Fire()
    {
        altWeaponEquiped.Fire();
    }

    public void Stop()
    {
        altWeaponEquiped.Stop();
    }

    public void FireAlt()
    {
        altWeaponEquiped.FireAlt();
    }

    public void StopAlt()
    {
        altWeaponEquiped.StopAlt();
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

        // Check if the lookDirection is not zero
        if (lookDirection.sqrMagnitude > 0.001f)  // Using sqrMagnitude for efficiency
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, transform.up);
            float speed = rotationSpeed * Time.deltaTime;
            rotatingObject.transform.rotation = Quaternion.RotateTowards(rotatingObject.transform.rotation, lookRotation, speed);
        }
        else
        {
            // Handle the case when lookDirection is (nearly) zero
            // For example, you could use the last valid direction or default to a specific direction
            // lookDirection = lastValidLookDirection;  // Assuming you store the last valid direction
        }

        inputTimeOut -= Time.deltaTime;
    }

    private void ResetAim()
    {
        //isAiming = false;
        aimX = 0;
        aimZ = 0;
    }


}
