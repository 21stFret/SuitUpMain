using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public InputActionAsset primaryActions;
    InputActionMap gameplayActionMap;
    public PlayerInput playerInput;
    public InputAction FireAltWeaponInput;
    public InputAction FireMainWeaponInput;
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
        if (!initialized)
        {
            rotatingObject.transform.SetParent(null);
            gameplayActionMap = primaryActions.FindActionMap("Gameplay");
            FireMainWeaponInput = gameplayActionMap.FindAction("FireMain");
            FireAltWeaponInput = gameplayActionMap.FindAction("FireAlt");
            ClearWeaponInputs();
            ResetAim();
            initialized = true;
        }

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
    }

    void OnDisable()
    {
        ClearWeaponInputs();
    }

    public void SetFireRate()
    {
        float fireRate = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.Fire_Rate);
        if (fireRate < 0)
        {
            fireRate = 0.05f;
        }
        mainWeaponEquiped.fireRate = fireRate;
    }

    private void SetMainWeaponInputs()
    {
        FireMainWeaponInput.performed += FireMain;
        FireMainWeaponInput.canceled += HaltFireMain;
    }

    private void SetAltWeaponInputs()
    {
        FireAltWeaponInput.performed += FireAlt;
        FireAltWeaponInput.canceled += HaltFireAlt;
    }

    public void ClearWeaponInputs()
    {
        FireMainWeaponInput.performed -= FireMain;
        FireMainWeaponInput.canceled -= HaltFireMain;
        FireAltWeaponInput.performed -= FireAlt;
        FireAltWeaponInput.canceled -= HaltFireAlt;
    }

    private void FireMain(InputAction.CallbackContext context)
    {
        mainWeaponEquiped.Fire();
    }

    private void HaltFireMain(InputAction.CallbackContext context)
    {
        mainWeaponEquiped.Stop();
    }

    private void FireAlt(InputAction.CallbackContext context)
    {
        altWeaponEquiped.Fire();
    }

    private void HaltFireAlt(InputAction.CallbackContext context)
    {
        altWeaponEquiped.Stop();
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

    public int WeaponsFiring()
    {
        if(mainWeaponEquiped == null || altWeaponEquiped == null)
        {
            return 0;
        }
        int firing = 0;
        if (mainWeaponEquiped.isFiring)
        {
            firing++;
        }
        if (altWeaponEquiped.isFiring)
        {
            firing++;
        }
        return firing;
    }

}
