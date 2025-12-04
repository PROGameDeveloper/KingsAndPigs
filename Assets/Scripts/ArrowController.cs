using UnityEngine;

public class ArrowController : TrampolineController
{
    [Header("Additional Info")]
    [SerializeField] private float respawnTime;
    [SerializeField] private bool rotationRight;
    [SerializeField] private float rotationSpeed = 120;
    private int _direction = -1;
    [Space] 
    [SerializeField] private float scaleUpSpeed = 10;
    [SerializeField] private Vector3 targetScale;
    
    private void Start()
    {
        transform.localScale = new Vector3(0.3f,0.3f,0.3f);
    }

    private void Update()
    {
        HandleScaleUp();
        HandleRotation();
    }

    private void HandleScaleUp()
    {
        if(transform.localScale.x < targetScale.x)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        _direction = rotationRight ? -1 : 1;
        transform.Rotate(0, 0, (rotationSpeed * _direction) * Time.deltaTime);
    }

    private void DestroyMe()
    {
        GameObject arrowPrefab = GameManager.Instance.arrowPrefab;
        GameManager.Instance.CreateObject(arrowPrefab, transform, respawnTime);
        Destroy(gameObject);
    } 
}
