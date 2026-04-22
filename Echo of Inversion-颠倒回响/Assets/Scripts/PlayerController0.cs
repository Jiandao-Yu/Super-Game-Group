using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController0 : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.2f;
    public float gravity = -70f;

    [Header("Ceiling Check")]
    public LayerMask groundLayer;
    public float ceilingCheckDistance = 0.3f;

    [Header("Flip Settings")]
    public float flipCooldown = 0.2f;
    public float flipImpulse = 14f;

    [Header("Visual")]
    public Transform visualRoot;

    [Header("2.5D Lock")]
    public bool lockZPosition = true;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isFlipped = false;
    private float lastFlipTime = -999f;

    private float fixedZ;

    // 记录模型原本的本地旋转，避免翻转时把朝向写死
    private Quaternion visualBaseRotation;
    private bool visualRotationInitialized = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        isFlipped = false;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        fixedZ = transform.position.z;

        if (visualRoot != null)
        {
            visualBaseRotation = visualRoot.localRotation;
            visualRotationInitialized = true;
        }

        UpdateVisualRotation();
        Force2DPlaneLock();
    }

    void Update()
    {
        CheckGroundState();
        HandleMove();
        HandleJumpAndGravity();
        HandleFlipInput();

        Force2DPlaneLock();
    }

    void HandleFlipInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Click Detected");

            if (!isGrounded)
            {
                Debug.Log("Flip Blocked: Player is not grounded.");
                return;
            }

            if (Time.time - lastFlipTime <= flipCooldown)
            {
                Debug.Log("Flip Blocked: Cooldown.");
                return;
            }

            lastFlipTime = Time.time;

            // 第一关：不做节奏判定，直接翻转
            FlipGravity();
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
        float moveX = Input.GetAxisRaw("Horizontal");
        Vector3 move = new Vector3(moveX, 0f, 0f);

        controller.Move(move * moveSpeed * Time.deltaTime);

        Force2DPlaneLock();
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

        Force2DPlaneLock();
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

        UpdateVisualRotation();
        Force2DPlaneLock();
    }

    void UpdateVisualRotation()
    {
        if (visualRoot == null) return;

        if (!visualRotationInitialized)
        {
            visualBaseRotation = visualRoot.localRotation;
            visualRotationInitialized = true;
        }

        if (!isFlipped)
        {
            // 正常状态：保持你模型原本调好的朝向
            visualRoot.localRotation = visualBaseRotation;
        }
        else
        {
            // 翻转状态：在原本朝向基础上，补一个Y轴180度修正前后反向
            visualRoot.localRotation = visualBaseRotation * Quaternion.Euler(0f, 180f, 0f);
        }
    }

    bool CheckCeiling()
    {
        Bounds bounds = controller.bounds;
        Vector3 origin = bounds.center;
        float distance = bounds.extents.y + ceilingCheckDistance;
        return Physics.Raycast(origin, Vector3.up, distance, groundLayer);
    }

    void Force2DPlaneLock()
    {
        if (!lockZPosition) return;

        Vector3 pos = transform.position;
        pos.z = fixedZ;
        transform.position = pos;
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