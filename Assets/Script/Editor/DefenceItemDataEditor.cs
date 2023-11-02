using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(DefenceItemData))]
public class DefenceItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DefenceItemData data = (DefenceItemData)target;

        // Eğer liste içinde aynı tip birden fazla varsa, uyarı ver.
        var duplicates = data.defenceItems
            .GroupBy(x => x.defenceItemType)
            .Where(group => group.Count() > 1 && group.Key != GridDefenceItemType.Invalid)
            .Select(group => group.Key)
            .ToList();

        if (duplicates.Any())
        {
            duplicates[0] = GridDefenceItemType.Invalid;
            EditorGUILayout.HelpBox("Cannot have the same DefenseItemType more than once!", MessageType.Error);
        }
    }
}