using UnityEngine;

public class FireButtonController : MonoBehaviour
{
    private Animator animator;
    private FireController fireController;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        fireController = GetComponentInParent<FireController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            animator.SetTrigger("active");
            fireController.SwitchOffFire();
        }
    }
}
