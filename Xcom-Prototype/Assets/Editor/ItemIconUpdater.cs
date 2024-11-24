using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ItemIconUpdater : EditorWindow
{
    private TextAsset _jsonFile;
    private Dictionary<string, string> _iconMappings;

    [MenuItem("Tools/Item Icon Updater")]
    public static void ShowWindow()
    {
        GetWindow<ItemIconUpdater>("Item Icon Updater");
    }

    private void OnGUI()
    {
        GUILayout.Label("Item Icon Updater", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        _jsonFile = EditorGUILayout.ObjectField("JSON File", _jsonFile, typeof(TextAsset), false) as TextAsset;
        if (EditorGUI.EndChangeCheck() && _jsonFile != null)
        {
            LoadJsonData();
        }

        if (GUILayout.Button("Update Icons"))
        {
            UpdateIcons();
        }
    }

    private void LoadJsonData()
    {
        if (_jsonFile != null)
        {
            string jsonContent = _jsonFile.text;
            _iconMappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            Debug.Log($"Loaded {_iconMappings.Count} icon mappings.");
        }
    }

    private void UpdateIcons()
    {
        if (_iconMappings == null || _iconMappings.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No icon mappings loaded. Please select a valid JSON file.", "OK");
            return;
        }

        int updatedCount = 0;
        int errorCount = 0;

        foreach (var mapping in _iconMappings)
        {
            string itemTemplatePath = mapping.Key;
            string iconPath = mapping.Value;

            ItemTemplate itemTemplate = AssetDatabase.LoadAssetAtPath<ItemTemplate>(itemTemplatePath);
            if (itemTemplate == null)
            {
                Debug.LogError($"Failed to load ItemTemplate at path: {itemTemplatePath}");
                errorCount++;
                continue;
            }

            Sprite newIcon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            if (newIcon == null)
            {
                Debug.LogError($"Failed to load Sprite at path: {iconPath}");
                errorCount++;
                continue;
            }

            itemTemplate.itemSprite = newIcon;
            EditorUtility.SetDirty(itemTemplate);
            updatedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Update Complete",
            $"Updated {updatedCount} icons.\nErrors encountered: {errorCount}", "OK");
    }
}