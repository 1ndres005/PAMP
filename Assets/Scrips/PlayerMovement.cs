using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float runSpeed = 8f; // Velocidad para correr
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    public Transform cameraHolder;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool jumping = false;
    private float saltoDuracion = 0.8f;
    private float saltoTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        animator.SetBool("Grounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = cameraHolder.forward;
        Vector3 camRight = cameraHolder.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * x + camForward * z;

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Detectar si se presiona Shift
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 moveDirection = move.normalized * currentSpeed;
        moveDirection.y = 0;

        velocity.x = moveDirection.x;
        velocity.z = moveDirection.z;

        // SALTO
        if (Input.GetButtonDown("Jump") && isGrounded && !jumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumping = true;
            saltoTimer = saltoDuracion;
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (jumping)
        {
            saltoTimer -= Time.deltaTime;
            if (saltoTimer <= 0f && isGrounded)
            {
                jumping = false;
            }
        }

        // Parámtero de animación adaptado al Blend Tree: 0 = quieto, 0.5 = caminar, 1 = correr
        float speedPercent = 0f;
        if (move.magnitude > 0.1f)
        {
            speedPercent = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;
        }

        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), speedPercent, 10 * Time.deltaTime));
    }
}
