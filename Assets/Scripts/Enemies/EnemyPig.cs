using UnityEngine;

public class EnemyPig : Enemy
{
    private static readonly int XVelocity = Animator.StringToHash("xVelocity");
    private BoxCollider2D boxCollider2D;

    protected override void Awake()
    {
        base.Awake();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead) return;
        
        HandleCollision();
        HandleMovement();
        
        float xVelocity = Mathf.Abs(rigidBody2D.linearVelocity.x);
        if (!isGrounded)
            xVelocity = 0f;
        
        animator.SetFloat(XVelocity, xVelocity);

        if (isGrounded)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            if (!isGrounded) return;
            
            Flip();
            idleTimer = idleDuration;
            rigidBody2D.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0) return;
        if (!isGrounded) return;

        rigidBody2D.linearVelocity = new Vector2(moveSpeed * facingDirection, rigidBody2D.linearVelocityY);
    }

    public override void Die()
    {
        base.Die();
        boxCollider2D.enabled = false;
    }
}
