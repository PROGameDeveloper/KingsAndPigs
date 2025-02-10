using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController { get => _playerController;}

    [SerializeField] private int _diamondCollected;
    public int DiamondCollected { get => _diamondCollected;}

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddDiamond() => _diamondCollected ++;
}
