using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 0.5f;
    public float runSpeed = 1f;
    public float crouchSpeed = 0.25f;
    public float turnSpeed = 10f;
    public float gravity = -9.18f;
    public float jumpHeight = 2f;
    public SprintCameraEffect sprintCameraEffect;
    // public float standingHeight = 0.3f;
    // public float crouchingHeight = 0.1f;
    // public float crouchingCenterY = 0.12f;

    public float standingHeight = 0.5f;
    public float standingCenterY = 0.25f;

    public float crouchingHeight = 0.3f;
    public float crouchingCenterY = 0.12f;
    
    // Audio
    public AudioSource landingAudioSource;
    public float minimumFallHeight = 2f;
    private float fallStartHeight;
    private bool wasGrounded;

    

    private Vector2 moveInput;
    private bool sprinting;
    private Animator animator;
    private CharacterController characterController;

    private Vector3 velocity;
    private bool jumpPressed;
    private bool crouching;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
        }
    }

    // public void OnCrouch(InputValue value)
    // {
    //     crouching = value.isPressed;
    // }


    // public void OnSprint(InputValue value)
    // {
    //     sprinting = value.isPressed;
    // }


    void Update()
    {

        Debug.Log("Crouching: " + crouching);
        
        // Check if Left Shift is currently being held
        sprinting = Keyboard.current != null &&
                    Keyboard.current.leftShiftKey.isPressed;

        crouching = Keyboard.current != null &&
            Keyboard.current.cKey.isPressed;
        

        // Animation
        float animationSpeed = sprinting ? 1f : 0.5f;

        animator.SetFloat("MoveX", moveInput.x * animationSpeed);
        animator.SetFloat("MoveY", moveInput.y * animationSpeed);

        animator.SetBool("Crouch", crouching && characterController.isGrounded);

        // Get camera directions
        Transform cameraTransform = Camera.main.transform;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Keep movement horizontal
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Camera-relative movement
        Vector3 movement =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;

        // Movement Speed
        // float currentSpeed = sprinting ? runSpeed : walkSpeed;
        float currentSpeed;
        if (crouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = sprinting ? runSpeed : walkSpeed;
        }

        
        // Change crouching height and center
        float targetHeight = crouching
            ? crouchingHeight
            : standingHeight;

        float targetCenterY = crouching
            ? crouchingCenterY
            : standingCenterY;

        characterController.height = targetHeight;

        characterController.center = new Vector3(
            0f,
            targetCenterY,
            0f
        );

        // characterController.height = crouching
        //     ? crouchingHeight
        //     : standingHeight;

        // characterController.center = new Vector3(
        //     0f,
        //     characterController.height / 2f,
        //     0f
        // );
        // characterController.center = new Vector3(
        // characterController.center.x,
        // characterController.height / 2f,
        // characterController.center.z
        // );


        if (sprintCameraEffect != null)
        {
            sprintCameraEffect.SetSprinting(sprinting);
        }

        
        // Jump animation trigger
        if (characterController.isGrounded)
    {
        if (velocity.y < 0f)
        {
        velocity.y = -2f;
        }

        if (jumpPressed)
        {
            velocity.y = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );

            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }
    }

        // Crouch animation trigger
        // animator.SetBool("Crouch", crouching && characterController.isGrounded);

        // Gravity
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        jumpPressed = false;
        velocity.y += gravity * Time.deltaTime;

        // Combine horizontal movement and gravity
        Vector3 finalMovement =
            movement * currentSpeed + velocity;

        
        // Thump sound
        // bool isGrounded = characterController.isGrounded;

        // if (isGrounded && !wasGrounded && velocity.y <= 0f)
        // {
        //     if (landingAudioSource != null)
        //     {
        //         landingAudioSource.Play();
        //     }
        // }

        // wasGrounded = isGrounded;

        // Thump sound
        bool isGrounded = characterController.isGrounded;

        // Record the height when the cat starts falling
        if (wasGrounded && !isGrounded && velocity.y < 0f)
        {
            fallStartHeight = transform.position.y;
        }

        // Check the height when the cat lands
        if (!wasGrounded && isGrounded)
        {
            float fallDistance = fallStartHeight - transform.position.y;
            if (fallDistance >= minimumFallHeight)
            {
                if (landingAudioSource != null &&
                    landingAudioSource.clip != null)
                {
                    landingAudioSource.PlayOneShot(landingAudioSource.clip);
                 }
            }           

        }

        wasGrounded = isGrounded;

        
        // Character Controller movement
        characterController.Move(
            finalMovement * Time.deltaTime
        );

        // Rotation
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }
}