using UnityEngine;

public class FallingPlatformController : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D[] _boxCollider2D; 
    
    [Header("Platform Settings")]
    [SerializeField] private float speed = 0.75f;
    [SerializeField] private float distance;
    private Vector3[] _wayPoints;
    private int _wayPointIndex;
    private bool canMove = false;

    [Header("Platform Fall Settings")] 
    [SerializeField] private bool canFall;
    [SerializeField] private float fallDelay = 0.5f;
    [Space]
    [SerializeField] private float impactSpeed = 3;
    [SerializeField] private float impactDuration = 0.1f;
    private float impactTimer;
    private bool impactHappened;
    
    [Header("Respawn Settings")]
    [SerializeField] private bool canRespawn = true;
    [SerializeField] private float respawnTime = 2f;
    [SerializeField] private float destroyAfterFallTime = 3f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponents<BoxCollider2D>();
    }

    private void Start()
    {
        SetupWaypoints();
        float randomDelay =  Random.Range(0, 0.6f);
        Invoke(nameof(ActivatePlatform), randomDelay);
    }

    private void Update()
    {
        HandleImpact();
        HandleMovement();
    }

    private void ActivatePlatform() => canMove = true;
    
    private void SetupWaypoints()
    {
        _wayPoints = new Vector3[2];
        float yOffset = distance / 2;
        _wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        _wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }
    
    private void HandleMovement()
    {
        if (!canMove) return;
        
        transform.position = Vector2.MoveTowards(transform.position, _wayPoints[_wayPointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _wayPoints[_wayPointIndex]) < 0.1f)
        {
            _wayPointIndex++;
            if (_wayPointIndex >= _wayPoints.Length)
                _wayPointIndex = 0;
        }
    }
    
    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;

        impactTimer -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10),
            impactSpeed * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController  player = collision.gameObject.GetComponent<PlayerController>();
        
        if (player == null) return;

        if (!canFall) return;
        
        if (impactHappened) return;

        Invoke(nameof(SwitchOffPlatform), fallDelay);
        impactTimer = impactDuration;
        impactHappened = true;
    }
    
    private void SwitchOffPlatform()
    {
        _animator.SetTrigger("deactivate");
        canMove = false;
        
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.gravityScale = 3.5f;
        _rigidbody2D.linearDamping = 0.5f;
        
        foreach (BoxCollider2D collider in _boxCollider2D)
        {
            collider.enabled = false;
        }

        if (!canFall) return;

        if (canRespawn)
        {
            GameObject platformPrefab = GameManager.Instance.fallingPlatformPrefab;
            Vector3 respawnPosition = transform.position;                
            GameManager.Instance.CreateObject(platformPrefab, respawnPosition, respawnTime);
            Invoke(nameof(DestroyPlatform), respawnTime);
        }
        else
        {
            Invoke(nameof(DestroyPlatform), destroyAfterFallTime);
        }
    }

    private void DestroyPlatform()
    {
        Destroy(gameObject);
    }
}
