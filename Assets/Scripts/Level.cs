using UnityEngine;

[CreateAssetMenu(fileName="New Level", menuName="Level")]
public class Level : ScriptableObject
{
    [SerializeField] private int initialMoves;
    public int InitialMoves => initialMoves;

    [Tooltip("_: None\nw: Wall\np: Player\ne: End")]
    [SerializeField, TextArea(10,20)] private string levelData;
    private string levelDataFormat;

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
            case 'w': return TileType.Wall;
            case 'p': return TileType.Player;
            case 'e': return TileType.End;
        }

        Debug.LogWarning($"unknown tile type '{c}'");
        return TileType.None;
    }
}

public enum TileType
{
    None,
    Wall,
    Player,
    End,
}
