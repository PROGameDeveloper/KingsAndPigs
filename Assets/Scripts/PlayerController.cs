using System;
using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform mTransform;
    private Rigidbody2D _mRigidbody2D;
    private GatherInput _mGatherInput;
    private Animator _mAnimator;

    // ANIMATOR IDS
    private int _idIsGrounded;
    private int _idSpeed;
    private int _idIsWallDetected;
    private int _idKnockback;

    [Header("Move settings")]
    [SerializeField] private float speed;
    [SerializeField] private bool canMove;
    [SerializeField] private float moveDelay;
    private int _direction = 1;

    [Header("Jump settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int extraJumps;
    [SerializeField] private int counterExtraJumps;
    [SerializeField] private bool canDoubleJump;

    [Header("Ground settings")]
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;
    private RaycastHit2D _lFootRay;
    private RaycastHit2D _rFootRay;
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
    [SerializeField] private float wallJumpDuration;

    [Header("Knock settings")]
    [SerializeField] private bool isKnocked;
    //[SerializeField] private bool canBeKnocked;
    [SerializeField] private Vector2 knockedPower;
    [SerializeField] private float knockedDuration;
    
    [Header("Death VFX")]
    [SerializeField] private GameObject deathVFX;

    private void Awake()
    {
        _mGatherInput = GetComponent<GatherInput>();
        mTransform = GetComponent<Transform>();
        _mRigidbody2D = GetComponent<Rigidbody2D>();
        _mAnimator = GetComponent<Animator>();
        canMove = false;
        StartCoroutine(CanMoveRoutine());
    }


    private void Start()
    {
        _idSpeed = Animator.StringToHash("speed");
        _idIsGrounded = Animator.StringToHash("isGrounded");
        _idIsWallDetected = Animator.StringToHash("isWallDetected");
        _idKnockback = Animator.StringToHash("knockback");
        counterExtraJumps = extraJumps;
    }

    private void Update()
    {
        SetAnimatorValues();
    }

    private void SetAnimatorValues()
    {
        _mAnimator.SetFloat(_idSpeed, Mathf.Abs(_mRigidbody2D.linearVelocityX));
        _mAnimator.SetBool(_idIsGrounded, isGrounded);
        _mAnimator.SetBool(_idIsWallDetected,isWallDetected);
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
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
        _lFootRay = Physics2D.Raycast(lFoot.position, Vector2.down, rayLength, groundLayer);
        _rFootRay = Physics2D.Raycast(rFoot.position, Vector2.down, rayLength, groundLayer);
        if (_lFootRay || _rFootRay)
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
        isWallDetected = Physics2D.Raycast(mTransform.position, Vector2.right * _direction, checkWallDistance, groundLayer);
    }
    private void HandleWallSlide()
    {
        canWallSlide = isWallDetected;
        if (!canWallSlide) return;
        canDoubleJump = false;
        slideSpeed = _mGatherInput.Value.y < 0 ? 1 : 0.5f;
        _mRigidbody2D.linearVelocity = new Vector2(_mRigidbody2D.linearVelocityX,_mRigidbody2D.linearVelocityY * slideSpeed);
    }
    
    private void Move()
    {
        if (!canMove) return;
        if (isWallDetected && !isGrounded) return;
        if (isWallJumping) return;

        Flip();
        _mRigidbody2D.linearVelocity = new Vector2(speed * _mGatherInput.Value.x, _mRigidbody2D.linearVelocity.y);
    }
    private IEnumerator CanMoveRoutine()
    {
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }

    private void Flip()
    {
        if(_mGatherInput.Value.x * _direction < 0)
        {
            HandleDirection();
        }
    }

    private void HandleDirection()
    {
        mTransform.localScale = new Vector3(-mTransform.localScale.x, 1, 1);
        _direction *= -1;
    }

    private void Jump()
    {
        if (_mGatherInput.IsJumping) 
        {
            if (isGrounded)
            {
                _mRigidbody2D.linearVelocity = new Vector2(speed * _mGatherInput.Value.x, jumpForce);
                canDoubleJump = true;
            }
            else if (isWallDetected) WallJump();
            else if (counterExtraJumps > 0 && canDoubleJump) DoubleJump();
        }
        _mGatherInput.IsJumping = false;
    }

    private void WallJump()
    {
        _mRigidbody2D.linearVelocity = new Vector2(wallJumpForce.x * -_direction, wallJumpForce.y);
        HandleDirection();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
        Flip();
    }

    private void DoubleJump()
    {
        _mRigidbody2D.linearVelocity = new Vector2(speed * _mGatherInput.Value.x, jumpForce);
        counterExtraJumps -= 1;
    }

    public void Knockback()
    {
        StartCoroutine(KnockbackRoutine());
        _mRigidbody2D.linearVelocity = new Vector2(knockedPower.x * -_direction, knockedPower.y);
        _mAnimator.SetTrigger(_idKnockback);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        //canBeKnocked = false;
        yield return new WaitForSeconds(knockedDuration);
        isKnocked = false;
        //canBeKnocked = true;
    }

    public void Die()
    {
        GameObject deathVFXPrefab = Instantiate(deathVFX, mTransform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mTransform.position,new Vector2(mTransform.position.x + (checkWallDistance * _direction),mTransform.position.y));
    }
}
