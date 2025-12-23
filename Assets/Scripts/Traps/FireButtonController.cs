using UnityEngine;

public class FireButtonController : MonoBehaviour
{
    private static readonly int Active = Animator.StringToHash("active");
    private Animator _animator;
    private FireController _fireController;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _fireController = GetComponentInParent<FireController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            _animator.SetTrigger(Active);
            _fireController.SwitchOffFire();
        }
    }
}
