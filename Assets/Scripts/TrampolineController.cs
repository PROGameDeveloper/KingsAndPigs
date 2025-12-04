using UnityEngine;

public class TrampolineController : MonoBehaviour
{
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
            _animator.SetTrigger("active");
        }
    }
}
