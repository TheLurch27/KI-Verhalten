using UnityEngine;
using System.Collections.Generic;

public class TileData
{
    public GameObject tilePrefab;
    public Dictionary<string, string> connections;

    public TileData(GameObject prefab, Dictionary<string, string> connections)
    {
        tilePrefab = prefab;
        this.connections = connections;
    }
}
