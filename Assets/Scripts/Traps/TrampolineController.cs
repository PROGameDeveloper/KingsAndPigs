using UnityEngine;

public class TrampolineController : MonoBehaviour
{
    private static readonly int Active = Animator.StringToHash("active");
    private Animator _animator;
    [SerializeField] private float pushPower;
    [SerializeField] private float duration = 0.5f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            player.Push(transform.up * pushPower , duration);
            _animator.SetTrigger(Active);
        }
        
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Push(transform.up * pushPower);
            _animator.SetTrigger(Active);
        }
    }
}
