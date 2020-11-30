using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance =>
        instance ?? (instance = FindObjectOfType<LevelManager>());

    [SerializeField] private Level level;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tilemap wallsTileMap;

    private Transform player;

    public MoveManager MoveManager { get; private set; }

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        MoveManager = FindObjectOfType<MoveManager>();

        level.PopulateGrid();
        SetupLevel(level);
    }

    private void SetupLevel(Level level)
    {
        MoveManager.Initialize(level.InitialMoves);

        for (int y = 0; y < level.Height; y++) {
            for (int x = 0; x < level.Width; x++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                switch (level.LevelGrid[x, y]) {
                    case TileType.Wall:
                        wallsTileMap.SetTile(pos, wallTile);
                        break;
                    case TileType.Player:
                        player.position = pos;
                        break;
                }
			}
		}
    }

    public void GameOver()
    {
        IsGameOver = true;
        Debug.Log("Game Over");
    }
}
