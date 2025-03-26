using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController => playerController;

    [Header( "Diamond Manager" )]
    [SerializeField] private int diamondCollected;
    [SerializeField] private bool diamondHaveRandomLook;
    public int DiamondCollected => diamondCollected;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddDiamond() => diamondCollected ++;
    public bool DiamondHaveRandomLook() => diamondHaveRandomLook;
}
