using UnityEngine;

[CreateAssetMenu(fileName="New Level", menuName="Level")]
public class Level : ScriptableObject
{
    [SerializeField] private int initialMoves;
    public int InitialMoves => initialMoves;

    [Tooltip("_: None\np: Player\nw: Wall\na: Acid\ns: Spike\nr: Roller\ne: End")]
    [SerializeField, TextArea(10,20)] private string levelData;
    private string levelDataFormat;

    [SerializeField] private string levelHint;
    public string LevelHint => levelHint;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public TileType[,] LevelGrid { get; private set; }

    public void PopulateGrid()
    {
        levelDataFormat = levelData.ToLower();
        string[] lines = levelDataFormat.Split(new[] { '\n' });

        Width = lines[0].Length;
        Height = lines.Length;
        LevelGrid = new TileType[Width, Height];

        for (int y = 0; y < Height; y++) {
            string line = lines[y];
            for (int x = 0; x < Width; x++) {
                char c = line[x];
                int dy = Height - y - 1;
                LevelGrid[x, dy] = CharToTile(c);
            }
        }
    }

    private TileType CharToTile(char c)
	{
        switch (c) {
            case '-': return TileType.None;
            case 'p': return TileType.Player;
            case 'w': return TileType.Wall;
            case 'a': return TileType.Acid;
            case 's': return TileType.Spike;
            case 'r': return TileType.Roller;
            case 'h': return TileType.Health;
            case 'e': return TileType.End;
        }

        Debug.LogWarning($"unknown tile type '{c}'");
        return TileType.None;
    }

    public bool IsWall(int x, int y)
	{
        if (x < 0 || x >= Width || y < 0 || y >= Height) {
            return true;
		}

        return LevelGrid[x, y] == TileType.Wall;
	}
}

public enum TileType
{
    None,
    Player,
    Wall,
    Acid,
    Spike,
    Roller,
    Health,
    End,
}
