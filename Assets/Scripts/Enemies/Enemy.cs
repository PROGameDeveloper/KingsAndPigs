using UnityEngine;

public class Enemy : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("hit");
    protected Animator animator;
    protected Rigidbody2D rigidBody2D;
    
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float idleDuration;
    protected float idleTimer;
    
    [Header("Basic Collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = 0.7f;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;
    protected bool isGrounded;
    protected bool wasGrounded;
    protected bool isWallDetected;
    protected bool isGroundInFrontDetected;

    [Header("Death settings")] 
    [SerializeField] protected GameObject damageTrigger;
    [SerializeField] protected float deathImpact;
    [SerializeField] protected float deathRotationSpeed;
    private int deathRotationDirection = 1;
    protected bool isDead;

    protected int facingDirection = -1;
    protected bool facingRight = false;
    
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    
    protected virtual void Update()
    {
        idleTimer -= Time.deltaTime;
        CheckLanding();
        
        if (isDead)
            HandleDeath();
    }
    
    public virtual void Die()
    {
        if(damageTrigger != null)
            damageTrigger.SetActive(false);
        animator.SetTrigger(Hit);
        rigidBody2D.linearVelocity = new Vector2(rigidBody2D.linearVelocityX, deathImpact);
        isDead = true;

        if (Random.Range(0, 100) < 50)
            deathRotationDirection = deathRotationDirection * -1;
    }

    private void HandleDeath()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }

    private void CheckLanding()
    {
        if (!wasGrounded && isGrounded)
        {
            idleTimer = idleDuration;
        }
        wasGrounded = isGrounded;
    }

    public void Push(Vector2 force)
    {
        rigidBody2D.AddForce(force, ForceMode2D.Impulse);
    }
    
    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < 0 && facingRight || xValue > 0 && !facingRight)
            Flip();
    }
    
    protected virtual void Flip()
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInFrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDirection), transform.position.y));
    }
}
