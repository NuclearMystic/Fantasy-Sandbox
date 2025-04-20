using System;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    [Header("Variables")]
    [Tooltip("How fast the player walks.")]
    public float walkSpeed = 2f;
    [Tooltip("How fast the player runs.")]
    public float runSpeed = 4f;
    [Tooltip("Speed of player rotation on the Y-axis.")]
    public float playerRotationSpeed = 15f;
    [Tooltip("How high the player can jump.")]
    public float jumpHeight = 5;

    [Header("Layer Masks")]
    [Tooltip("Layer mask to identify what counts as ground.")]
    public LayerMask[] groundLayers;

    [Header("Bools")]
    [SerializeField]
    private bool isGrounded;

    // New run toggle state
    [SerializeField]
    private bool isRunning = false;
    private bool isSprinting = false;

    // Vector3s
    private Vector3 moveDirection;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        UIController.Instance.SetCursorState(true);
    }

    void Update()
    {
        HandleRunToggle();
        HandleSprinting();
        HandleMovement();
        HandleJump();
        HandleGravity();
        UpdateAnimator();
    }


    private void HandleRunToggle()
    {
        // Toggle run mode on key press (change this key if you prefer another)
        if (Input.GetButtonDown("Walk/Run Toggle"))
        {
            isRunning = !isRunning;
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump");
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * vertical + right * horizontal;

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        float speed;

        if (isSprinting)
        {
            speed = runSpeed * 2f;
        }
        else
        {
            speed = isRunning ? runSpeed : walkSpeed;
        }

        moveDirection *= speed;

        if (moveDirection.magnitude < 0.01f)
            moveDirection = Vector3.zero;

        controller.Move(moveDirection * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, playerRotationSpeed * Time.deltaTime);
        }
    }

    private void HandleSprinting()
    {
        bool isTryingToSprint = Input.GetButton("Sprinting");
        isSprinting = isTryingToSprint && moveDirection.magnitude > 0.1f && PlayerVitals.instance.currentStamina > 0f;

        if (isSprinting)
        {
            float skill = PlayerStats.Instance.GetSkillTotal(SkillType.Athletics);
            float baseDrain = 0.1f; 
            float scaledDrain = baseDrain / (1f + skill * 0.1f);

            SkillUsageTracker.RegisterSkillUse(SkillType.Athletics, Time.deltaTime * 0.5f);
            PlayerVitals.instance.DrainStamina(scaledDrain);
        }

    }

    void HandleGravity()
    {
        foreach (LayerMask layermask in groundLayers)
        {
            if (Physics.CheckSphere(transform.position, 0.2f, layermask))
            {
                isGrounded = true;
                break;
            }
            else
            {
                isGrounded = false;
            }
        }

        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        float speedParam = 0f;

        if (moveDirection.magnitude > 0.1f)
        {
            speedParam = isSprinting ? 3f : (isRunning ? 2f : 1f);
        }

        animator.SetFloat("Speed", speedParam);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isSprinting", isSprinting);
    }

}
