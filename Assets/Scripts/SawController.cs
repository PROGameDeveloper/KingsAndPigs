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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        transform.position = wayPoints[0].position;
    }

    private void Update()
    {
        _animator.SetBool("sawActive", canMove);
        if (!canMove)  return;
        transform.position = Vector2.MoveTowards(transform.position, wayPoints[indexWayPoint].position, speed * Time.deltaTime);
        if(!(Vector2.Distance(transform.position, wayPoints[indexWayPoint].position) < 0.1f)) return;
        indexWayPoint++;
        if (indexWayPoint >= wayPoints.Length)
        {
            indexWayPoint = 0;
            StartCoroutine(StopMovement(waitForMove));
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
