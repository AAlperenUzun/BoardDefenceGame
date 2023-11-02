using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private LevelData _levelData;

    private static Dictionary<string, Texture2D> _icons = new Dictionary<string, Texture2D>();

    private void OnEnable()
    {
        _icons.Clear();
        _levelData = (LevelData)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_gridSize"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_interactableGridSize"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowedGridObjectTypes"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowedCubeTypes"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowedDefenceItemTypes"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowedEnemyTypes"), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Object Types", EditorStyles.boldLabel);
        SerializedProperty startObjectTypes = serializedObject.FindProperty("_startObjectTypes");

        int gridSizeX = _levelData.GridSize.x;
        int gridSizeY = _levelData.GridSize.y;

        if (GUILayout.Button("Clear"))
        {
            _levelData.StartObjectTypes.Clear();
        }

        if (GUILayout.Button("Randomize"))
        {
            _levelData.Randomize();
        }

        GUILayout.BeginVertical("Box");

        for (int y = gridSizeY - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();

            for (int x = 0; x < gridSizeX; x++)
            {
                int index = y * gridSizeX + x;
                if (index < _levelData.StartObjectTypes.Count)
                {
                    GUILayout.BeginVertical("Box", GUILayout.Width(100), GUILayout.Height(100));

                    GridObjectTypeContainer typeContainer = _levelData.StartObjectTypes[index];

                    typeContainer.GridObjectType =
                        (GridObjectType)EditorGUILayout.Popup((int)typeContainer.GridObjectType, System.Enum.GetNames(typeof(GridObjectType)));

                    if (typeContainer.GridObjectType == GridObjectType.Cube)

                        typeContainer.GridCubeType =
                            (GridCubeType)EditorGUILayout.Popup((int)typeContainer.GridCubeType, System.Enum.GetNames(typeof(GridCubeType)));
                    else
                        typeContainer.GridCubeType = GridCubeType.Invalid;

                    _levelData.StartObjectTypes[index] = typeContainer;

                    Texture2D icon = GetIconForGridObjectType(typeContainer.GridObjectType,
                        typeContainer.GridCubeType);
                    GUILayout.Label(icon, GUILayout.Width(80), GUILayout.Height(80));

                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private Texture2D GetIconForGridObjectType(GridObjectType objectType, GridCubeType cubeType)
    {
        if (objectType == GridObjectType.Invalid)
            return null;

        if (objectType == GridObjectType.Cube && cubeType == GridCubeType.Invalid)
            return null;

        string iconPath = "Assets/Textures/GridObjects/";

        iconPath += objectType.ToString();

        if (objectType == GridObjectType.Cube && cubeType != GridCubeType.Invalid)
            iconPath += cubeType + ".png";
        else
            iconPath += ".png";

        if (_icons.TryGetValue(iconPath, out Texture2D icon))
            return icon;

        icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        _icons.Add(iconPath, icon);

        return icon;
    }
}