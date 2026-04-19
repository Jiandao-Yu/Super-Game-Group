using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.2f;
    public float gravity = -35f;

    [Header("Ceiling Check")]
    public LayerMask groundLayer;
    public float ceilingCheckDistance = 0.1f;

    [Header("Flip Settings")]
    public float flipCooldown = 0.15f;
    public float flipImpulse = 10f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isFlipped = false;
    private float lastFlipTime = -999f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        isFlipped = false;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        CheckGroundState();
        HandleMove();
        HandleJumpAndGravity();

        if (Input.GetMouseButtonDown(0) && isGrounded && Time.time - lastFlipTime > flipCooldown)
        {
            FlipGravity();
            lastFlipTime = Time.time;
        }
    }

    void CheckGroundState()
    {
        if (!isFlipped)
        {
            isGrounded = controller.isGrounded;
        }
        else
        {
            isGrounded = CheckCeiling();
        }
    }

    void HandleMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveX, 0f, 0f);
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleJumpAndGravity()
    {
        if (isGrounded)
        {
            if (!isFlipped)
            {
                if (velocity.y < 0f)
                    velocity.y = -2f;
            }
            else
            {
                if (velocity.y > 0f)
                    velocity.y = 2f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (!isFlipped)
                {
                    velocity.y = jumpVelocity;
                }
                else
                {
                    velocity.y = -jumpVelocity;
                }
            }
        }

        if (!isFlipped)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void FlipGravity()
    {
        isFlipped = !isFlipped;

        if (!isFlipped)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            velocity.y = -flipImpulse;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            velocity.y = flipImpulse;
        }
    }

    bool CheckCeiling()
    {
        Bounds bounds = controller.bounds;
        Vector3 origin = bounds.center;
        float distance = bounds.extents.y + ceilingCheckDistance;

        return Physics.Raycast(origin, Vector3.up, distance, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc == null) return;

        if (isFlipped)
        {
            Gizmos.color = Color.red;
            Bounds bounds = cc.bounds;
            Vector3 origin = bounds.center;
            float distance = bounds.extents.y + ceilingCheckDistance;
            Gizmos.DrawLine(origin, origin + Vector3.up * distance);
        }
    }
}