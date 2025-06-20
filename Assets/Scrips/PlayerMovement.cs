using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    public Transform cameraHolder;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool jumping = false;
    private float saltoDuracion = 0.8f; // Ajusta esto a la duración real de tu animación
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

        controller.Move(move.normalized * walkSpeed * Time.deltaTime);

        // SALTO
        if (Input.GetButtonDown("Jump") && isGrounded && !jumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumping = true;
            saltoTimer = saltoDuracion;
            animator.Play("Salto");
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ⏳ Esperar que termine animación de salto
        if (jumping)
        {
            saltoTimer -= Time.deltaTime;
            if (saltoTimer <= 0f && isGrounded)
            {
                jumping = false;

                // Volver a caminar o quieto después del salto
                if (move.magnitude >= 0.1f)
                {
                    animator.Play("caminar");
                }
                else
                {
                    animator.Play("Quieto");
                }
            }
        }
        else
        {
            // Si no estamos en salto, controlar animaciones normales
            if (move.magnitude >= 0.1f)
            {
                animator.Play("caminarh");
            }
            else
            {
                animator.Play("Quieto");
            }
        }
    }
}
