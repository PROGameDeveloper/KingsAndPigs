using System.Collections;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private static readonly int Active = Animator.StringToHash("active");
    [SerializeField] private float offDuration;
    [SerializeField] private FireButtonController fireButton;

    private Animator _animator;
    private CapsuleCollider2D _capsuleCollider2D;
    private bool _isActive;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }
    
    private void Start()
    {
        if (fireButton == null)
            Debug.LogWarning("No fire button assigned" + gameObject.name);

        SetFire(true);
    }

    public void SwitchOffFire()
    {
        if (_isActive == false) return;
        StartCoroutine(FireCoroutine());
    }

    private IEnumerator FireCoroutine()
    {
        SetFire(false);
        yield return new WaitForSeconds(offDuration);
        SetFire(true);
    }

    
    private void SetFire(bool active)
    {
        _animator.SetBool(Active, active);
        _capsuleCollider2D.enabled = active;
        _isActive = active;
    }


}
