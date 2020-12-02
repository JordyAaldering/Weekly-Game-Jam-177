using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Pickups")]
    [SerializeField] private GameObject rollerPrefab;
    [SerializeField] private GameObject healthPrefab;

    [Header("Tiles")]
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private RuleTile acidTile;
    [SerializeField] private RuleTile spikeTile;

    [Header("GameObjects")]
    [SerializeField] private Tilemap wallsTileMap;
    [SerializeField] private Tilemap obstaclesTileMap;
    [SerializeField] private Transform pickupParent;

    private Level[] levels;
    private Level GetLevel => levels[curLevel];
    private int curLevel;

    private Transform player;
    private Vector2 playerStartPos;
    private Transform levelEnd;
    private Camera cam;

    public MoveManager MoveManager { get; private set; }
    public LevelOverlayMenu Overlay { get; private set; }

    public bool IsGameWon { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPlaying => !IsGameWon && !IsGameOver;

    private void Awake()
    {
        Instance = this;

        levels = Resources.LoadAll<Level>("Levels");
        curLevel = PlayerPrefs.GetInt("CurLevel", 0);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        levelEnd = GameObject.FindGameObjectWithTag("LevelEnd").transform;
        cam = Camera.main;

        MoveManager = FindObjectOfType<MoveManager>();
        Overlay = FindObjectOfType<LevelOverlayMenu>();

        StartLevel();
    }

    private void StartLevel()
    {
        ResetLevel();

        player.GetComponent<PlayerController>().Initialize();

        GetLevel.PopulateGrid();
        SetupGrid();
        SetCamera();
    }

    public void ResetLevel()
    {
        IsGameWon = false;
        IsGameOver = false;

        Overlay.CloseOverlay();
        Overlay.SetHintText(GetLevel.LevelHint);
        MoveManager.Initialize(GetLevel.InitialMoves);

        foreach (Transform child in pickupParent) {
            child.gameObject.SetActive(true);
        }

        player.GetComponent<PlayerController>().Initialize();
        player.position = playerStartPos;
    }

    public void NextLevel()
    {
        if (++curLevel > PlayerPrefs.GetInt("MaxLevel", 0)) {
            PlayerPrefs.SetInt("MaxLevel", curLevel);
        }

        if (curLevel >= levels.Length) {
            SceneManager.LoadScene(0);
        } else {
            StartLevel();
		}
    }

    public void MainMenu()
	{
        SceneManager.LoadScene(0);
    }

    private void SetCamera()
    {
        cam.transform.position = new Vector3(GetLevel.Width / 2f, GetLevel.Height / 2f, -10f);
        cam.orthographicSize = Mathf.Max(GetLevel.Width, GetLevel.Height) / 2.5f;
    }

    private void SetupGrid()
    {
        ClearGrid();
        CreateBorders(10);

        for (int y = 0; y < GetLevel.Height; y++) {
            for (int x = 0; x < GetLevel.Width; x++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                switch (GetLevel.LevelGrid[x, y]) {
                    case TileType.Wall:
                        wallsTileMap.SetTile(pos, wallTile);
                        break;
                    case TileType.Player:
                        playerStartPos = pos + new Vector3(0.5f, 0.5f);
                        player.position = playerStartPos;
                        break;
                    case TileType.Acid:
                        PlaceObstacle(acidTile, x, y);
                        break;
                    case TileType.Spike:
                        PlaceObstacle(spikeTile, x, y);
                        break;
                    case TileType.Roller:
                        PlacePickup(rollerPrefab, x, y);
                        break;
                    case TileType.Health:
                        PlacePickup(healthPrefab, x, y);
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

    private void PlacePickup(GameObject obj, int x, int y)
	{
        Instantiate(obj, new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity, pickupParent);
	}

    private void ClearGrid()
    {
        wallsTileMap.ClearAllTiles();
        obstaclesTileMap.ClearAllTiles();

        foreach (Transform child in pickupParent) {
            Destroy(child.gameObject);
        }
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
        Overlay.OpenOverlay();
    }

    public void GameOver()
    {
        IsGameOver = true;
        Overlay.OpenOverlay();
    }
}
