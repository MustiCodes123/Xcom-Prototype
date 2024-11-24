using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ArmorItemTemplateEditor : EditorWindow
{
    private List<ArmorItemTemplate> _selectedArmorItems = new List<ArmorItemTemplate>();
    private float _itemWeight = 0f;

    [MenuItem("Tools/Armor Weight Setter")]
    public static void ShowWindow()
    {
        GetWindow<ArmorItemTemplateEditor>("Armor Weight Setter");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Selected Armor Items:");

        EditorGUILayout.Space();

        for (int i = 0; i < _selectedArmorItems.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _selectedArmorItems[i] = (ArmorItemTemplate)EditorGUILayout.ObjectField(_selectedArmorItems[i], typeof(ArmorItemTemplate), false);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                _selectedArmorItems.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Drag and drop ArmorItemTemplates here:");
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "");
        if (dropArea.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            Event.current.Use();
        }
        else if (dropArea.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                ArmorItemTemplate armorItem = obj as ArmorItemTemplate;
                if (armorItem != null && !_selectedArmorItems.Contains(armorItem))
                {
                    _selectedArmorItems.Add(armorItem);
                }
            }
            Event.current.Use();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        _itemWeight = EditorGUILayout.FloatField("Item Weight", _itemWeight);

        EditorGUILayout.Space();

        if (GUILayout.Button("Set Armor Weights"))
        {
            SetArmorWeights();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear Selected Items"))
        {
            _selectedArmorItems.Clear();
        }
    }

    private void SetArmorWeights()
    {
        foreach (ArmorItemTemplate armorItem in _selectedArmorItems)
        {
            if (armorItem != null)
            {
                foreach (ArmorUpgradeStats upgradeStats in armorItem.ArmorUpgradeStats)
                {
                    upgradeStats.ItemWeight = _itemWeight;
                }

                EditorUtility.SetDirty(armorItem);
                Debug.Log($"Set item weight for ArmorItemTemplate: {armorItem.name}");
            }
        }

        AssetDatabase.SaveAssets();
    }
}
