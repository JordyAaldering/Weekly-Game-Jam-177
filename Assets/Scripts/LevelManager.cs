using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance =>
        instance ?? (instance = FindObjectOfType<LevelManager>());

    [SerializeField] private RuleTile wallTile;
    [SerializeField] private RuleTile acidTile;
    [SerializeField] private RuleTile spikeTile;

    [SerializeField] private Tilemap wallsTileMap;
    [SerializeField] private Tilemap obstaclesTileMap;

    [SerializeField] private Level[] levels;
    private int curLevel;
    private Level GetLevel => levels[curLevel];

    private Transform player;
    private Vector2 playerStartPos;
    private Transform levelEnd;
    private Camera cam;

    public MoveManager MoveManager { get; private set; }

    public bool IsGameWon { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPlaying => !IsGameWon && !IsGameOver;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        curLevel = PlayerPrefs.GetInt("CurLevel", 0);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        levelEnd = GameObject.FindGameObjectWithTag("LevelEnd").transform;
        cam = Camera.main;

        MoveManager = FindObjectOfType<MoveManager>();

        StartLevel();
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) {
            ResetLevel();
		}

        if (IsGameWon && Input.GetKeyDown(KeyCode.N)) {
            curLevel++;
            StartLevel();
		}
	}

    private void StartLevel()
    {
        IsGameWon = false;
        IsGameOver = false;

        MoveManager.Initialize(GetLevel.InitialMoves);
        GetLevel.PopulateGrid();
        SetupGrid();
        SetCamera();
    }

    private void ResetLevel()
    {
        IsGameWon = false;
        IsGameOver = false;

        MoveManager.Initialize(GetLevel.InitialMoves);
        player.position = playerStartPos;
    }

    private void SetCamera()
    {
        cam.transform.position = new Vector3(GetLevel.Width / 2f, GetLevel.Height / 2f, -10f);
        cam.orthographicSize = Mathf.Max(GetLevel.Width, GetLevel.Height) / 2.5f;
    }

    private void SetupGrid()
    {
        ClearGrid();
        CreateBorders(5);

        for (int y = 0; y < GetLevel.Height; y++) {
            for (int x = 0; x < GetLevel.Width; x++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                switch (GetLevel.LevelGrid[x, y]) {
                    case TileType.Wall:
                        wallsTileMap.SetTile(pos, wallTile);
                        break;
                    case TileType.Acid:
                        PlaceObstacle(acidTile, x, y);
                        break;
                    case TileType.Spike:
                        PlaceObstacle(spikeTile, x, y);
                        break;
                    case TileType.Player:
                        playerStartPos = pos + new Vector3(0.5f, 0.5f);
                        player.position = playerStartPos;
                        break;
                    case TileType.End:
                        levelEnd.position = pos + new Vector3(0.5f, 0.5f);
                        break;
                }
			}
		}
    }

    private void PlaceObstacle(RuleTile tile, int x, int y)
	{
        // place a tile inside a wall to let the rule tile oriënt the correct way
        // check if there is a wall above
        if (GetLevel.IsWall(x, y + 1)) {
            obstaclesTileMap.SetTile(new Vector3Int(x, y, 0), tile);
            obstaclesTileMap.SetTile(new Vector3Int(x, y + 1, 0), tile);
            return;
        }

        // check if there is a wall below
        if (GetLevel.IsWall(x, y - 1)) {
            obstaclesTileMap.SetTile(new Vector3Int(x, y, 0), tile);
            obstaclesTileMap.SetTile(new Vector3Int(x, y - 1, 0), tile);
            return;
        }

        Debug.LogWarning($"invalid obstacle position ({x}, {y})");
    }

    private void ClearGrid()
    {
        wallsTileMap.ClearAllTiles();
    }

    private void CreateBorders(int size)
	{
        // top, bottom, and corners
        for (int y = 1; y <= size; y++) {
            for (int x = -size; x < GetLevel.Width + size; x++) {
                Vector3Int posTop = new Vector3Int(x, GetLevel.Height + y - 1, 0);
                Vector3Int posBot = new Vector3Int(x, -y, 0);
                wallsTileMap.SetTile(posTop, wallTile);
                wallsTileMap.SetTile(posBot, wallTile);
            }
		}

        // left and right
        for (int x = 1; x <= size; x++) {
            for (int y = 0; y < GetLevel.Height; y++) {
                Vector3Int posLeft = new Vector3Int(-x, y, 0);
                Vector3Int posRight = new Vector3Int(GetLevel.Width + x - 1, y, 0);
                wallsTileMap.SetTile(posLeft, wallTile);
                wallsTileMap.SetTile(posRight, wallTile);
            }
		}
	}

    public void LevelComplete()
	{
        IsGameWon = true;
        Debug.Log("Game Won");
    }

    public void GameOver()
    {
        IsGameOver = true;
        Debug.Log("Game Over");
    }
}
