using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TileConnectionEditor : EditorWindow
{
    private string methodName = "GeneratedMethod";
    private List<string> connectionTypes = new List<string>();
    private List<GameObject> tiles = new List<GameObject>();
    private Dictionary<GameObject, Dictionary<string, string>> tileConnections = new Dictionary<GameObject, Dictionary<string, string>>();
    private Dictionary<GameObject, bool> foldouts = new Dictionary<GameObject, bool>();

    private int page = 0;
    private Vector2 scrollPos;

    [MenuItem("Tools/Tile Connection Editor")]
    public static void ShowWindow()
    {
        GetWindow<TileConnectionEditor>("Tile Connection Editor");
    }

    private void OnGUI()
    {
        if (page == 0)
        {
            DrawSetupPage();
        }
        else
        {
            DrawTileConfigurationPage();
        }
    }

    private void DrawSetupPage()
    {
        GUILayout.Label("Setup", EditorStyles.boldLabel);

        methodName = EditorGUILayout.TextField("Method Name", methodName);

        GUILayout.Label("Connection Types");
        if (GUILayout.Button("Add Connection Type"))
        {
            connectionTypes.Add("");
        }
        for (int i = 0; i < connectionTypes.Count; i++)
        {
            connectionTypes[i] = EditorGUILayout.TextField("Type " + (i + 1), connectionTypes[i]);
        }

        if (GUILayout.Button("Next"))
        {
            page = 1;
        }
    }

    private void DrawTileConfigurationPage()
    {
        GUILayout.Label("Tile Configuration", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Tile"))
        {
            tiles.Add(null);
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 100));

        for (int i = 0; i < tiles.Count; i++)
        {
            GUILayout.BeginHorizontal();
            tiles[i] = (GameObject)EditorGUILayout.ObjectField("Tile " + (i + 1), tiles[i], typeof(GameObject), false);

            if (tiles[i] != null && !tileConnections.ContainsKey(tiles[i]))
            {
                tileConnections[tiles[i]] = new Dictionary<string, string>
                {
                    { "Oben", connectionTypes.Count > 0 ? connectionTypes[0] : "" },
                    { "Rechts", connectionTypes.Count > 0 ? connectionTypes[0] : "" },
                    { "Unten", connectionTypes.Count > 0 ? connectionTypes[0] : "" },
                    { "Links", connectionTypes.Count > 0 ? connectionTypes[0] : "" }
                };
                foldouts[tiles[i]] = true; // Initialisiert Foldout-Zustand
            }
            GUILayout.EndHorizontal();

            if (tiles[i] != null)
            {
                foldouts[tiles[i]] = EditorGUILayout.Foldout(foldouts[tiles[i]], "Connections");
                if (foldouts[tiles[i]])
                {
                    foreach (var direction in new[] { "Oben", "Rechts", "Unten", "Links" })
                    {
                        int selectedIndex = connectionTypes.IndexOf(tileConnections[tiles[i]][direction]);
                        selectedIndex = EditorGUILayout.Popup(direction, selectedIndex, connectionTypes.ToArray());
                        if (selectedIndex >= 0 && selectedIndex < connectionTypes.Count)
                        {
                            tileConnections[tiles[i]][direction] = connectionTypes[selectedIndex];
                        }
                    }
                }
            }

            // Trennstrich oder Lücke einfügen
            GUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Space(10);
        }

        EditorGUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate References"))
        {
            GenerateReferences();
        }

        if (GUILayout.Button("Generate Method"))
        {
            GenerateMethod();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Back"))
        {
            page = 0;
        }
    }

    private void GenerateReferences()
    {
        string references = "";

        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                string variableName = tile.name.Replace(" ", "").Replace("-", "").Replace("_", "");
                references += $"public GameObject {variableName};\n";
            }
        }

        EditorGUIUtility.systemCopyBuffer = references;
        Debug.Log("References generated and copied to clipboard:\n" + references);
    }

    private void GenerateMethod()
    {
        string method = $"public void {methodName}()\n{{\n";

        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                string variableName = tile.name.Replace(" ", "").Replace("-", "").Replace("_", "");
                method += $"    // Tile: {tile.name}\n";
                method += $"    AddTile(new TileData(\n";
                method += $"        {variableName},\n";
                method += $"        new Dictionary<string, string>\n";
                method += $"        {{\n";
                foreach (var direction in new[] { "Oben", "Rechts", "Unten", "Links" })
                {
                    method += $"            {{ \"{direction}\", \"{tileConnections[tile][direction]}\" }},\n";
                }
                method += $"        }}\n";
                method += $"    ));\n";
            }
        }

        method += "}\n";

        EditorGUIUtility.systemCopyBuffer = method;
        Debug.Log("Method generated and copied to clipboard:\n" + method);
    }
}
