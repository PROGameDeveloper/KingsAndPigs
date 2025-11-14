using System;
using System.Collections;
using UnityEngine;

public class SawController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private int indexWayPoint = 1;
    [SerializeField] private bool canMove = true;
    [SerializeField] private float waitForMove = 1;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private int _moveDirection = 1;
    private Vector3[] _wayPointsPosition;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateWaypoints();
        transform.position = _wayPointsPosition[0];
    }

    private void UpdateWaypoints()
    {
        _wayPointsPosition = new Vector3[wayPoints.Length];

        for (int i = 0; i < wayPoints.Length; i++)
        {
            _wayPointsPosition[i] = wayPoints[i].position;
        }
    }

    private void Update()
    {
        _animator.SetBool("sawActive", canMove);
        if (!canMove)  return;
        transform.position = Vector2.MoveTowards(transform.position, _wayPointsPosition[indexWayPoint], speed * Time.deltaTime);
        
        if(Vector2.Distance(transform.position, _wayPointsPosition[indexWayPoint]) < 0.1f)
        {
            if (indexWayPoint == _wayPointsPosition.Length - 1 || indexWayPoint == 0)
            {
                _moveDirection = _moveDirection * -1;
                StartCoroutine(StopMovement(waitForMove));
            }
            
            indexWayPoint = indexWayPoint + _moveDirection;
        }
    }
    
    IEnumerator StopMovement(float delayTime)
    {
        canMove = false;
        yield return new WaitForSeconds(delayTime);
        canMove = true;
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
}
