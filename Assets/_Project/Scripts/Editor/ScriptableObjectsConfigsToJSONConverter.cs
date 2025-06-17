using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    public class ScriptableObjectsConfigsToJSONConverter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            HandleImportedAssets(importedAssets.Except(movedAssets).ToArray());
            HandleDeletedAssets(deletedAssets);
            HandleMovedAssets(movedAssets, movedFromAssetPaths);
        }

        private static void HandleImportedAssets(string[] importedAssets)
        {
            foreach (var path in importedAssets)
            {
                JsonConvertableSO asset = AssetDatabase.LoadAssetAtPath<JsonConvertableSO>(path);

                if (asset != null)
                {
                    string json = asset.ToJson();
                    string jsonPath = Path.ChangeExtension(path, ".json");
                    string directory = Path.GetDirectoryName(jsonPath);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllText(jsonPath, json);
                    Debug.Log($"Saved config to {jsonPath}");
                }
            }
        }

        private static void HandleDeletedAssets(string[] deletedAssets)
        {
            foreach (var path in deletedAssets)
            {
                if (path.EndsWith(".asset"))
                {
                    string jsonPath = Path.ChangeExtension(path, ".json");

                    if (File.Exists(jsonPath))
                    {
                        File.Delete(jsonPath);
                        Debug.Log($"Deleted JSON {jsonPath} because SO was deleted");
                    }

                    string metaPath = jsonPath + ".meta";

                    if (File.Exists(metaPath))
                    {
                        File.Delete(metaPath);
                    }
                }
            }
        }

        private static void HandleMovedAssets(string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                string oldPath = movedFromAssetPaths[i];
                string newPath = movedAssets[i];

                JsonConvertableSO asset = AssetDatabase.LoadAssetAtPath<JsonConvertableSO>(newPath);

                if (asset != null)
                {
                    string oldJsonPath = Path.ChangeExtension(oldPath, ".json");
                    string newJsonPath = Path.ChangeExtension(newPath, ".json");

                    if (File.Exists(oldJsonPath))
                    {
                        string newDir = Path.GetDirectoryName(newJsonPath);

                        if (!Directory.Exists(newDir))
                        {
                            Directory.CreateDirectory(newDir);
                        }

                        File.Move(oldJsonPath, newJsonPath);
                        Debug.Log($"Moved JSON {oldJsonPath} ➔ {newJsonPath}");
                    }

                    string oldMetaPath = oldJsonPath + ".meta";
                    string newMetaPath = newJsonPath + ".meta";

                    if (File.Exists(oldMetaPath))
                    {
                        string newDir = Path.GetDirectoryName(newMetaPath);

                        if (!Directory.Exists(newDir))
                        {
                            Directory.CreateDirectory(newDir);
                        }

                        File.Move(oldMetaPath, newMetaPath);
                    }
                }
            }
        }
    }
}