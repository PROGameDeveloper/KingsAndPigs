using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DeadArea : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        player = other.gameObject.GetComponent<PlayerController>();
        player.Die();
        GameManager.Instance.RespawnPlayer();
    }
}
