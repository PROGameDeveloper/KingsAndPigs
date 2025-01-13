using System;
using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform m_transform;
    private Rigidbody2D m_rigidbody2D;
    private GatherInput m_gatherInput;
    private Animator m_animator;

    // ANIMATOR IDS
    private int idIsGrounded;
    private int idSpeed;
    private int idIsWallDetected;
    private int idKnockback;

    [Header("Move settings")]
    [SerializeField] private float speed;
    private int direction = 1;

    [Header("Jump settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int extraJumps;
    [SerializeField] private int counterExtraJumps;
    [SerializeField] private bool canDoubleJump;

    [Header("Ground settings")]
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;
    RaycastHit2D lFootRay;
    RaycastHit2D rFootRay;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall settings")]
    [SerializeField] private float checkWallDistance;
    [SerializeField] private bool isWallDetected;
    [SerializeField] private bool canWallSlide;
    [SerializeField] private float slideSpeed;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpDuartion;

    [Header("Knock settings")]
    [SerializeField] private bool isKnocked;
    [SerializeField] private bool canBeKnocked;
    [SerializeField] private Vector2 knockedPower;
    [SerializeField] private float knockedDuration;

    private void Awake()
    {
        m_gatherInput = GetComponent<GatherInput>();
        //m_transform = GetComponent<Transform>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idSpeed = Animator.StringToHash("speed");
        idIsGrounded = Animator.StringToHash("isGrounded");
        idIsWallDetected = Animator.StringToHash("isWallDetected");
        idKnockback = Animator.StringToHash("knockback");
        lFoot = GameObject.Find("LFoot").GetComponent<Transform>();
        rFoot = GameObject.Find("RFoot").GetComponent<Transform>();
        counterExtraJumps = extraJumps;
    }

    private void Update()
    {
        SetAnimatorValues();
    }

    private void SetAnimatorValues()
    {
        m_animator.SetFloat(idSpeed, Mathf.Abs(m_rigidbody2D.linearVelocityX));
        m_animator.SetBool(idIsGrounded, isGrounded);
        m_animator.SetBool(idIsWallDetected,isWallDetected);

    }

    void FixedUpdate()
    {
        if(isKnocked) return;
        CheckCollision();
        Move();
        Jump();
    }

    private void CheckCollision()
    {
        HandleGround();
        HandleWall();
        HandleWallSlide();
    }

    private void HandleGround()
    {
        lFootRay = Physics2D.Raycast(lFoot.position, Vector2.down, rayLength, groundLayer);
        rFootRay = Physics2D.Raycast(rFoot.position, Vector2.down, rayLength, groundLayer);
        if (lFootRay || rFootRay)
        {
            isGrounded = true;
            counterExtraJumps = extraJumps;
            canDoubleJump = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleWall()
    {
        isWallDetected = Physics2D.Raycast(m_transform.position, Vector2.right * direction, checkWallDistance, groundLayer);
    }

    private void HandleWallSlide()
    {
        canWallSlide = isWallDetected;
        if (!canWallSlide) return;
        canDoubleJump = false;
        slideSpeed = m_gatherInput.Value.y < 0 ? 1 : 0.5f;
        m_rigidbody2D.linearVelocity = new Vector2(m_rigidbody2D.linearVelocityX,m_rigidbody2D.linearVelocityY * slideSpeed);
    }

    private void Move()
    {

        if (isWallDetected && !isGrounded) return;
        if (isWallJumping) return;

        Flip();
        m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.Value.x, m_rigidbody2D.linearVelocity.y);
    }

    private void Flip()
    {
        if(m_gatherInput.Value.x * direction < 0)
        {
            HandleDirection();
        }
    }

    private void HandleDirection()
    {
        m_transform.localScale = new Vector3(-m_transform.localScale.x, 1, 1);
        direction *= -1;
    }

    private void Jump()
    {
        if (m_gatherInput.IsJumping) 
        {
            if (isGrounded)
            {
                m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.Value.x, jumpForce);
                canDoubleJump = true;
            }
            else if (isWallDetected) WallJump();
            else if (counterExtraJumps > 0 && canDoubleJump) DoubleJump();
        }
        m_gatherInput.IsJumping = false;
    }

    private void WallJump()
    {
        m_rigidbody2D.linearVelocity = new Vector2(wallJumpForce.x * -direction, wallJumpForce.y);
        HandleDirection();
        StartCoroutine(WallJumpRoutine());
    }

    IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuartion);
        isWallJumping = false;
        Flip();
    }

    private void DoubleJump()
    {
        m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.Value.x, jumpForce);
        counterExtraJumps -= 1;
    }

    public void Knockback()
    {
        StartCoroutine(KnockbackRoutine());
        m_rigidbody2D.linearVelocity = new Vector2(knockedPower.x * -direction, knockedPower.y);
        m_animator.SetTrigger(idKnockback);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        canBeKnocked = false;
        yield return new WaitForSeconds(knockedDuration);
        isKnocked = false;
        canBeKnocked = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_transform.position,new Vector2(m_transform.position.x + (checkWallDistance * direction),m_transform.position.y));
    }
}
