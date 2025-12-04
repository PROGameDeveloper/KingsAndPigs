using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerRespawnPoint;
    [SerializeField] private float respawnPlayerDelay;
    [SerializeField] private PlayerController playerController;

    [Header("Respawn Settings")]
    public bool hasCheckPointActive;
    public Vector3 checkpointRespawnPosition;
    
    [Header( "Diamond Manager" )]
    [SerializeField] private int diamondCollected;
    [SerializeField] private bool diamondHaveRandomLook;
    [SerializeField] private int totalDiamonds;

    [Header("Traps")]
    public GameObject arrowPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        TotalDiamondsInLevel();
    }

    private void TotalDiamondsInLevel()
    {
        GameObject[] diamonds = GameObject.FindGameObjectsWithTag("Diamond");
        totalDiamonds = diamonds.Length;
    }

    public void RespawnPlayer()
    {
        if (hasCheckPointActive) playerRespawnPoint.position = checkpointRespawnPosition;
        StartCoroutine(RespawnPlayerCoroutine());
    }
    
    IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(respawnPlayerDelay); 
        GameObject newPlayer = Instantiate(playerPrefab, playerRespawnPoint.position, Quaternion.identity);
        newPlayer.name = "Player";
        playerController = newPlayer.GetComponent<PlayerController>();
    }
    
    public void CreateObject(GameObject prefab, Transform target, float delay = 0)
    {
        StartCoroutine(CreateObjectCourutine(prefab, target, delay));
    }
    private IEnumerator CreateObjectCourutine(GameObject prefab, Transform target, float delay)
    {
        Vector3 newPosition = target.position;
        yield return new WaitForSeconds(delay);
        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);
    }

    public void AddDiamond() => diamondCollected ++;
    public bool DiamondHaveRandomLook() => diamondHaveRandomLook;
    
}
