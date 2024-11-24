using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

public class IconReplacer : EditorWindow
{
    private int _selectedMaxSize = 3; // 128 by default
    private readonly int[] _maxSizeOptions = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };

    [MenuItem("Tools/Replace Item Icons")]
    public static void ShowWindow()
    {
        GetWindow<IconReplacer>("Icon Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Max Size for Sprites", EditorStyles.boldLabel);
        _selectedMaxSize = EditorGUILayout.Popup("Max Size", _selectedMaxSize, _maxSizeOptions.Select(x => x.ToString()).ToArray());

        if (GUILayout.Button("Replace Icons"))
        {
            ReplaceIcons();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Delete Old Icons"))
        {
            DeleteOldIcons();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Rename Old Icons"))
        {
            RenameOldIcons();
        }
    }

    private void ReplaceIcons()
    {
        string dataPath = "Assets/Data/Armor";
        string iconPath = "Assets/UI/Items/Armor";

        string[] rarityFolders = { "1Common", "2Rare", "3Epic", "4Legendary", "5Mythic" };
        string[] setTypes = { "OH Sets", "TH Sets" };
        string[] slotTypes = { "Boots", "Chest", "Gloves", "Helmet" };

        Dictionary<string, string> slotToSpriteNameMap = new Dictionary<string, string>
        {
            { "Boots", "Boots" },
            { "Chest", "Body armor" },
            { "Gloves", "Bracers" },
            { "Helmet", "Helmet" }
        };

        foreach (string rarity in rarityFolders)
        {
            foreach (string setType in setTypes)
            {
                foreach (string slotType in slotTypes)
                {
                    string folderPath = Path.Combine(dataPath, rarity, setType, slotType);
                    string[] guids = AssetDatabase.FindAssets("t:ItemTemplate", new[] { folderPath });

                    foreach (string guid in guids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        ItemTemplate item = AssetDatabase.LoadAssetAtPath<ItemTemplate>(assetPath);

                        if (item != null && item.itemSprite != null)
                        {
                            string oldIconName = item.itemSprite.name;
                            string oldIconFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(item.itemSprite));

                            string newIconPath = FindNewIcon(oldIconFolder, slotToSpriteNameMap[slotType]);

                            if (!string.IsNullOrEmpty(newIconPath))
                            {
                                SetTextureImportSettings(newIconPath);
                                Sprite newIcon = AssetDatabase.LoadAssetAtPath<Sprite>(newIconPath);
                                if (newIcon != null)
                                {
                                    item.itemSprite = newIcon;
                                    EditorUtility.SetDirty(item);
                                    Debug.Log($"Replaced icon for {item.name} with {newIcon.name}");
                                }
                            }
                        }
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private string FindNewIcon(string folder, string spriteName)
    {
        string[] files = Directory.GetFiles(folder, "*.png");

        string copyMatch = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == spriteName + " - Copy");
        if (copyMatch != null) return copyMatch;

        string exactMatch = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == spriteName);
        if (exactMatch != null) return exactMatch;

        return null;
    }

    private void SetTextureImportSettings(string texturePath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;

        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = _maxSizeOptions[_selectedMaxSize];

            importer.SaveAndReimport();
        }
    }

    private void DeleteOldIcons()
    {
        string iconPath = "Assets/UI/Items/Armor";
        List<string> usedIcons = new List<string>();
        List<string> allIcons = new List<string>();

        string[] itemTemplateGuids = AssetDatabase.FindAssets("t:ItemTemplate");
        foreach (string guid in itemTemplateGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemTemplate item = AssetDatabase.LoadAssetAtPath<ItemTemplate>(assetPath);
            if (item != null && item.itemSprite != null)
            {
                usedIcons.Add(AssetDatabase.GetAssetPath(item.itemSprite));
            }
        }

        string[] iconGuids = AssetDatabase.FindAssets("t:Sprite", new[] { iconPath });
        foreach (string guid in iconGuids)
        {
            allIcons.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        List<string> unusedIcons = allIcons.Except(usedIcons).ToList();

        int deletedCount = 0;
        foreach (string path in unusedIcons)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                AssetDatabase.DeleteAsset(path);
                deletedCount++;
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Delete Old Icons", $"Deleted {deletedCount} unused icons.", "OK");
    }

    private void RenameOldIcons()
    {
        string basePath = "Assets/UI/ItemsOld/Armor";
        string[] rarityFolders = { "Common OH", "Common TH", "Epic OH", "Epic TH", "Legendary OH", "Legendary TH", "Mythic OH", "Mythic TH", "Rare OH", "Rare TH" };
        string[] newNames = { "Body armor", "Boots", "Bracers", "Helmet" };

        int renamedCount = 0;
        int skippedCount = 0;

        foreach (string rarityFolder in rarityFolders)
        {
            string rarityPath = Path.Combine(basePath, rarityFolder);
            string[] subfolders = Directory.GetDirectories(rarityPath);

            foreach (string subfolder in subfolders)
            {
                string[] files = Directory.GetFiles(subfolder, "ItemIcon*");
                Array.Sort(files);

                for (int i = 0; i < files.Length && i < newNames.Length; i++)
                {
                    string oldPath = files[i];
                    string newName = newNames[i] + Path.GetExtension(oldPath);
                    string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName);

                    if (File.Exists(oldPath))
                    {
                        try
                        {
                            if (File.Exists(newPath))
                            {
                                string copyName = Path.GetFileNameWithoutExtension(newName) + " - Copy" + Path.GetExtension(newName);
                                newPath = Path.Combine(Path.GetDirectoryName(oldPath), copyName);
                            }

                            File.Move(oldPath, newPath);
                            renamedCount++;

                            AssetDatabase.MoveAsset(oldPath, newPath);
                        }
                        catch (IOException ex)
                        {
                            Debug.LogWarning($"Cannot rename file {oldPath}: {ex.Message}");
                            skippedCount++;
                        }
                    }
                }
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Rename Old Icons",
            $"Renamed {renamedCount} icons.\nSkipped {skippedCount} icons.", "OK");
    }
}