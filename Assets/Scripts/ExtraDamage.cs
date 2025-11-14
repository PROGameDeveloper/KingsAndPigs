using UnityEngine;

public class ExtraDamage : MonoBehaviour
{
    [SerializeField] private Vector2 extraKnockedPower;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        var player = collision.GetComponent<PlayerController>();
        player.KnockedPower = extraKnockedPower;
        player.Knockback(transform.position.x);
    }
}
