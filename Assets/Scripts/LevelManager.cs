using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int initialMoves;

    private static LevelManager instance;
    public static LevelManager Instance =>
        instance ?? (instance = FindObjectOfType<LevelManager>());

    public bool IsGameOver { get; private set; }

    public MoveManager MoveManager { get; private set; }

    private void Awake()
    {
        MoveManager = FindObjectOfType<MoveManager>();
        MoveManager.Initialize(initialMoves);
    }

    public void GameOver()
    {
        IsGameOver = true;
        Debug.Log("Game Over");
    }
}
