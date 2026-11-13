using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Making everything public and adjustable
public class TelekinesisController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputActionReference grabAction;
    [SerializeField] private InputActionReference throwAction;
    [SerializeField] private LayerMask grabbableLayer;
    [SerializeField] private Transform holdPoint;

    [Header("Grab Settings")]
    [SerializeField] private float grabRange = 10f;
    [SerializeField] private float pullSpeed = 10f;
    [SerializeField] private float maxPullVelocity = 12f;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 15f;

    private GrabbableObject currentObject;
    private GrabbableObject targetedObject;
    private bool isHolding;

    [Header("Energy Settings")]
    [SerializeField] private FloatVariableSO energyVariable;
    [SerializeField] private TelekinesisGameEventSO energyChangedEvent;
    //[SerializeField] private Slider energyBar;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyDrainRate = 20f;
    [SerializeField] private float energyRegenRate = 15f;
    
    //private float currentEnergy;

    private bool originalUseGravity;

    private void Start()
    {
        energyVariable.value = maxEnergy;
        energyChangedEvent.Raise();
    }


    private void Update()
    {
        UpdateTarget(); // continuously check which object the camera is pointing at 

        if (grabAction.action.WasPressedThisFrame())
        {
            if (isHolding)
            {
                ReleaseObject();
            }
            else if (targetedObject != null && energyVariable.value > 0f) // added energy constraint
            {
                GrabObject(targetedObject);
            }
        }

        if (isHolding &&
            throwAction.action.WasPressedThisFrame())
        {
            ThrowObject();
        }

        UpdateEnergy();
    }

    private void UpdateEnergy()
    {
        float previousEnergy = energyVariable.value;

    
        if (isHolding)
        {
            energyVariable.value -= energyDrainRate * Time.deltaTime;

            if (energyVariable.value <= 0f) // autimatically release object when energy runs out
            {
                energyVariable.value = 0f;
                ReleaseObject();
            }
        }
        else
        {
            energyVariable.value += energyRegenRate * Time.deltaTime; // regen energy when cat is not holding object
        }

        energyVariable.value = Mathf.Clamp(energyVariable.value, 0f, maxEnergy); // prevent energy from going below zero or above max

        if (Mathf.Abs(previousEnergy - energyVariable.value) > 0.001f)
        {
            energyChangedEvent.Raise(); // notify UI when energy chanegs
        }
    }


    // private void UpdateEnergyBar()
    // {
    //     if(energyBar != null)
    //     {
    //         energyBar.value = energyVariable.value; // set slider to match current energy
    //     }
    // }

    private void FixedUpdate()
    {
        if (isHolding && currentObject != null)
        {
            MoveHeldObject();
        }
    }

    private void TryGrabObject()
    {
        // invisble ray starting from camera
        Ray ray = playerCamera.ViewportPointToRay( 
            new Vector3(0.5f, 0.5f, 0f) // ray's statring direction is center of the screen
        );

        float targetingRadius = 0.75f;

        if (Physics.SphereCast(
            ray, targetingRadius,
            out RaycastHit hit, 
            grabRange,
            grabbableLayer))
        {
            GrabbableObject target =
                hit.collider.GetComponentInParent<GrabbableObject>(); // searches hit collider's GameObject and its parents for the Grabbable Object script
            
            if (target != null) // if target is not null
            {
                GrabObject(target);
            }
        }
    }

    private void GrabObject(GrabbableObject target)
    {
        ClearTarget();

        currentObject = target;
        isHolding = true;

        originalUseGravity = currentObject.rb.useGravity;

        currentObject.rb.useGravity = false; // disables gravity so object doesn't fall while being held
        currentObject.rb.linearVelocity = Vector3.zero;
        currentObject.rb.angularVelocity = Vector3.zero;
    }

    private void UpdateTarget()
    {
        if (isHolding)
        {
            ClearTarget();
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f,0.5f,0f)
        );

        float targetingRadius = 0.75f;
        GrabbableObject newTarget = null;

        if (Physics.SphereCast(
            ray, targetingRadius,
            out RaycastHit hit, 
            grabRange,
            grabbableLayer))

        {
            newTarget = hit.collider.GetComponentInParent<GrabbableObject>();
        }

        if (newTarget == targetedObject)
        return;

        ClearTarget();

        targetedObject = newTarget;

        if (targetedObject != null)
        {
            GrabbableHighlight highlight =
            targetedObject.GetComponent<GrabbableHighlight>();

            if (highlight != null)
                highlight.SetHighlight(true);
        }

    }

    private void ClearTarget()
    {
        if (targetedObject == null)
        return;

        GrabbableHighlight highlight =
            targetedObject.GetComponent<GrabbableHighlight>();

        if (highlight != null)
            highlight.SetHighlight(false);

        targetedObject = null;
    }

    private void MoveHeldObject()
    {
        Vector3 targetPosition = holdPoint.position; // moves held object to hold point position

        Vector3 direction = 
            targetPosition - currentObject.rb.position;
        
        Vector3 desiredVelocity =
            direction * pullSpeed;

        currentObject.rb.linearVelocity =
        Vector3.ClampMagnitude(
            desiredVelocity,
            maxPullVelocity
        );
    }

    private void ReleaseObject()
    {
        if (currentObject == null) // check if object is being held
            return; // stops method

        currentObject.rb.useGravity = originalUseGravity;
        currentObject = null;
        isHolding = false; 
    }

    private void ThrowObject()
    {
        if (currentObject == null)
            return;

        Rigidbody rb = currentObject.rb;
        rb.useGravity = originalUseGravity; // restores original gravity setting
        rb.linearVelocity = Vector3.zero;

        rb.AddForce(
            playerCamera.transform.forward * throwForce,
            ForceMode.Impulse
        );

        currentObject = null;
        isHolding = false;
    }
}