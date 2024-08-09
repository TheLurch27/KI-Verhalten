using System.Collections.Generic;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    public List<TileData> tiles;

    public GameObject Crossing;
    public GameObject Grass;
    public GameObject NormalRoadLeftRight;
    public GameObject NormalRoadTopDown;
    public GameObject StreetCornerLeftDown;
    public GameObject StreetCornerLeftTop;
    public GameObject StreetCornerRightDown;
    public GameObject StreetCornerTopRight;

    public GameObject startPoint; // GameObject als Startpunkt

    private int width = 10;
    private int height = 10;
    private float tileSize = 2.0f; // Abhängig von der Größe der Tiles
    private TileData[,] map;

    // Neues Dictionary für kategorisierte Tiles
    private Dictionary<string, List<TileData>> categorizedTiles;

    private void Start()
    {
        map = new TileData[width, height];
        TileCollection();
        CategorizeTiles(); // Neue Methode zum Kategorisieren der Tiles

        TestManualKeyMatching();

        GenerateMapFromStartPoint();
    }

    private void TileCollection()
    {
        tiles = new List<TileData>();

        AddTile(new TileData(
            Crossing,
            new Dictionary<string, string>
            {
                { "Oben", "Street" },
                { "Rechts", "Street" },
                { "Unten", "Street" },
                { "Links", "Street" }
            }
        ));
        AddTile(new TileData(
            Grass,
            new Dictionary<string, string>
            {
                { "Oben", "Grass" },
                { "Rechts", "Grass" },
                { "Unten", "Grass" },
                { "Links", "Grass" }
            }
        ));
        AddTile(new TileData(
            NormalRoadLeftRight,
            new Dictionary<string, string>
            {
                { "Oben", "Grass" },
                { "Rechts", "Street" },
                { "Unten", "Grass" },
                { "Links", "Street" }
            }
        ));
        AddTile(new TileData(
            NormalRoadTopDown,
            new Dictionary<string, string>
            {
                { "Oben", "Street" },
                { "Rechts", "Grass" },
                { "Unten", "Street" },
                { "Links", "Grass" }
            }
        ));
        AddTile(new TileData(
            StreetCornerLeftDown,
            new Dictionary<string, string>
            {
                { "Oben", "Grass" },
                { "Rechts", "Grass" },
                { "Unten", "Street" },
                { "Links", "Street" }
            }
        ));
        AddTile(new TileData(
            StreetCornerLeftTop,
            new Dictionary<string, string>
            {
                { "Oben", "Street" },
                { "Rechts", "Grass" },
                { "Unten", "Grass" },
                { "Links", "Street" }
            }
        ));
        AddTile(new TileData(
            StreetCornerRightDown,
            new Dictionary<string, string>
            {
                { "Oben", "Grass" },
                { "Rechts", "Street" },
                { "Unten", "Street" },
                { "Links", "Grass" }
            }
        ));
        AddTile(new TileData(
            StreetCornerTopRight,
            new Dictionary<string, string>
            {
                { "Oben", "Street" },
                { "Rechts", "Street" },
                { "Unten", "Grass" },
                { "Links", "Grass" }
            }
        ));
    }

    // Neue Methode: Kategorisierung der Tiles
    private void CategorizeTiles()
    {
        categorizedTiles = new Dictionary<string, List<TileData>>();

        foreach (var tile in tiles)
        {
            string key = GetConnectionKey(tile.tilePrefab);
            Debug.Log($"Categorizing tile {tile.tilePrefab.name} with key: {key}");
            if (!categorizedTiles.ContainsKey(key))
            {
                categorizedTiles[key] = new List<TileData>();
            }
            categorizedTiles[key].Add(tile);
        }
    }

    private void TestManualKeyMatching()
    {
        string testKey = "StreetStreetStreetStreet"; // Beispiel für eine Kreuzung
        if (categorizedTiles.ContainsKey(testKey))
        {
            Debug.Log($"Manual test: Found {categorizedTiles[testKey].Count} tiles for key {testKey}");
        }
        else
        {
            Debug.LogWarning($"Manual test: No tiles found for key {testKey}");
        }
    }


    // Neue Methode: Erzeugen eines Schlüssels basierend auf den Verbindungen aus den leeren GameObjects
    private string GetConnectionKey(GameObject prefab)
    {
        Transform top = prefab.transform.Find("Street_Oben") ?? prefab.transform.Find("Grass_Oben");
        Transform right = prefab.transform.Find("Street_Rechts") ?? prefab.transform.Find("Grass_Rechts");
        Transform bottom = prefab.transform.Find("Street_Unten") ?? prefab.transform.Find("Grass_Unten");
        Transform left = prefab.transform.Find("Street_Links") ?? prefab.transform.Find("Grass_Links");

        // Fallback zu "None" wenn nichts gefunden wurde
        string topConnection = top != null ? top.name.Split('_')[0] : "None";
        string rightConnection = right != null ? right.name.Split('_')[0] : "None";
        string bottomConnection = bottom != null ? bottom.name.Split('_')[0] : "None";
        string leftConnection = left != null ? left.name.Split('_')[0] : "None";

        return topConnection + rightConnection + bottomConnection + leftConnection;
    }


    // Neue Methode: Abrufen der möglichen Tiles aus dem Pool
    private List<TileData> GetPossibleTilesFromPool(int x, int y)
    {
        string top = (y < height - 1 && map[x, y + 1] != null) ? map[x, y + 1].connections["Unten"] : "Grass";
        string right = (x < width - 1 && map[x + 1, y] != null) ? map[x + 1, y].connections["Links"] : "Grass";
        string bottom = (y > 0 && map[x, y - 1] != null) ? map[x, y - 1].connections["Oben"] : "Grass";
        string left = (x > 0 && map[x - 1, y] != null) ? map[x - 1, y].connections["Rechts"] : "Grass";

        string key = top + right + bottom + left;

        Debug.Log($"Checking possible tiles for key: {key} at position: {x}, {y}");

        if (categorizedTiles.ContainsKey(key))
        {
            return categorizedTiles[key];
        }
        else
        {
            Debug.LogWarning($"No matching category for key: {key} at position: {x}, {y}");
            return new List<TileData>(); // Leere Liste, wenn keine passenden Tiles gefunden wurden
        }
    }

    private void GenerateMapFromStartPoint()
    {
        int startX = Mathf.RoundToInt(startPoint.transform.position.x / tileSize);
        int startY = Mathf.RoundToInt(startPoint.transform.position.z / tileSize);

        // Testweise zufällige Platzierung aller Tiles
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileData randomTile = tiles[Random.Range(0, tiles.Count)];
                PlaceTile(randomTile.tilePrefab, x, y);
            }
        }
    }


    private void PlaceTile(GameObject prefab, int x, int y)
    {
        if (map[x, y] != null)
            return;

        TileData tile = tiles.Find(t => t.tilePrefab == prefab);
        map[x, y] = tile;

        Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        Instantiate(prefab, position, rotation);
    }

    private void ExpandFrom(int startX, int startY)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(new Vector2Int(startX, startY));

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            Debug.Log($"Expanding from position: {current.x}, {current.y}");

            foreach (Vector2Int direction in GetDirectionsClockwise())
            {
                int newX = current.x + direction.x;
                int newY = current.y + direction.y;

                if (newX >= 0 && newX < width && newY >= 0 && newY < height && map[newX, newY] == null)
                {
                    List<TileData> possibleTiles = GetPossibleTilesFromPool(newX, newY);
                    Debug.Log($"Possible tiles at position {newX}, {newY}: {possibleTiles.Count}");

                    if (possibleTiles.Count > 0)
                    {
                        // Zufällige Auswahl eines Tiles aus der Liste der möglichen Tiles
                        TileData selectedTile = possibleTiles[Random.Range(0, possibleTiles.Count)];
                        Debug.Log($"Placing tile at position {newX}, {newY}");
                        PlaceTile(selectedTile.tilePrefab, newX, newY);
                        frontier.Enqueue(new Vector2Int(newX, newY));
                    }
                    else
                    {
                        Debug.LogWarning($"No possible tiles found for position {newX}, {newY}");
                    }
                }
            }
        }
    }

    private Vector2Int[] GetDirectionsClockwise()
    {
        return new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };
    }

    public void AddTile(TileData tileData)
    {
        if (tiles == null)
        {
            tiles = new List<TileData>();
        }
        tiles.Add(tileData);
    }
}
