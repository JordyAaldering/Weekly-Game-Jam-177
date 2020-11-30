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
    private Transform levelEnd;
    private Camera cam;

    public MoveManager MoveManager { get; private set; }

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        levelEnd = GameObject.FindGameObjectWithTag("LevelEnd").transform;
        cam = Camera.main;

        MoveManager = FindObjectOfType<MoveManager>();

        level.PopulateGrid();
        MoveManager.Initialize(level.InitialMoves);
        SetupGrid(level);
        SetCamera();
    }

    private void SetCamera()
    {
        cam.transform.position = new Vector3(level.Width / 2f, level.Height / 2f, -10f);
        cam.orthographicSize = Mathf.Max(level.Width, level.Height) / 2.5f;
    }

    private void SetupGrid(Level level)
    {
        ClearGrid();
        CreateBorders(5);

        for (int y = 0; y < level.Height; y++) {
            for (int x = 0; x < level.Width; x++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                switch (level.LevelGrid[x, y]) {
                    case TileType.Wall:
                        wallsTileMap.SetTile(pos, wallTile);
                        break;
                    case TileType.Player:
                        player.position = pos + new Vector3(0.5f, 0.5f);
                        break;
                    case TileType.End:
                        levelEnd.position = pos + new Vector3(0.5f, 0.5f);
                        break;
                }
			}
		}
    }

    private void ClearGrid()
    {
        wallsTileMap.ClearAllTiles();
    }

    private void CreateBorders(int size)
	{
        // top, bottom, and corners
        for (int y = 1; y <= size; y++) {
            for (int x = -size; x < level.Width + size; x++) {
                Vector3Int posTop = new Vector3Int(x, level.Height + y - 1, 0);
                Vector3Int posBot = new Vector3Int(x, -y, 0);
                wallsTileMap.SetTile(posTop, wallTile);
                wallsTileMap.SetTile(posBot, wallTile);
            }
		}

        // left and right
        for (int x = 1; x <= size; x++) {
            for (int y = 0; y < level.Height; y++) {
                Vector3Int posLeft = new Vector3Int(-x, y, 0);
                Vector3Int posRight = new Vector3Int(level.Width + x - 1, y, 0);
                wallsTileMap.SetTile(posLeft, wallTile);
                wallsTileMap.SetTile(posRight, wallTile);
            }
		}
	}

    public void GameOver()
    {
        IsGameOver = true;
        Debug.Log("Game Over");
    }
}
