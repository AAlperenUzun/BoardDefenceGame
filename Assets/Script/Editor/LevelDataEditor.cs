using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private LevelData _levelData;

    private void OnEnable()
    {
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
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


}