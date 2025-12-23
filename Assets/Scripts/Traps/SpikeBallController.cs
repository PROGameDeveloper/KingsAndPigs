using UnityEngine;

public class SpikeBallController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private float pushForce;

    private void Start()
    {
        Vector2 pushVector = new Vector2(pushForce, 0);
        rigidBody2D.AddForce(pushVector, ForceMode2D.Impulse);
    }
}
