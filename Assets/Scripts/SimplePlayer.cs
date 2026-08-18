using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SimplePlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    [Header("Animation")]
    public float inputDeadZone = 0.1f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 120f;
    public Vector3 cameraOffset = new Vector3(0f, 2f, -4f);
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private CharacterController controller;
    private Animator animator;

    private float verticalVelocity;
    private bool isDead = false;
    private bool isDroneMode = false;

    private float yaw;
    private float pitch;
    private float turnSmoothVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animator.SetBool("Dead", false);
        animator.SetFloat("Blend", 0f);

        if (cameraTransform != null)
            yaw = cameraTransform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isDead)
            return;

        if (isDroneMode)
        {
            animator.SetFloat("Blend", 0f);
            return;
        }

        HandleCursor();
        HandleCameraLook();
        HandleMovement();
        HandleAnimation();
        HandleActions();
    }

    void LateUpdate()
    {
        if (cameraTransform == null || isDroneMode)
            return;

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = transform.position + cameraRotation * cameraOffset;

        cameraTransform.position = targetPosition;
        cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
    }

    void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleCameraLook()
    {
        if (cameraTransform == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(h, 0f, v).normalized;
        bool hasMoveInput = inputDirection.magnitude >= inputDeadZone;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && hasMoveInput;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (hasMoveInput)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + yaw;
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 move = moveDirection.normalized * currentSpeed;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -1f;
            else
                verticalVelocity += gravity * Time.deltaTime;

            move.y = verticalVelocity;
            controller.Move(move * Time.deltaTime);
        }
        else
        {
            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -1f;
            else
                verticalVelocity += gravity * Time.deltaTime;

            Vector3 fallMove = new Vector3(0f, verticalVelocity, 0f);
            controller.Move(fallMove * Time.deltaTime);
        }
    }

    void HandleAnimation()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool hasMoveInput = new Vector3(h, 0f, v).magnitude >= inputDeadZone;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && hasMoveInput;

        float blend = 0f;

        if (!hasMoveInput)
            blend = 0f;
        else if (isRunning)
            blend = 1f;
        else
            blend = 0.5f;

        animator.SetFloat("Blend", blend);
    }

    void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            animator.SetTrigger("Jump");
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        animator.SetBool("Dead", true);

        if (GameManager.instance != null)
            GameManager.instance.PlayerDied();
    }

    public void SetDroneMode(bool value)
    {
        isDroneMode = value;

        if (animator != null)
            animator.SetFloat("Blend", 0f);
    }

    public void ResetAfterRespawn()
    {
        isDead = false;
        isDroneMode = false;
        verticalVelocity = 0f;

        if (animator != null)
        {
            animator.SetBool("Dead", false);
            animator.SetFloat("Blend", 0f);
            animator.Rebind();
            animator.Update(0f);
        }
    }
}