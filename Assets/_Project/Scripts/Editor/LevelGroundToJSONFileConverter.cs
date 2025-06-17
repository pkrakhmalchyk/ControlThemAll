using System.Collections.Generic;
using System.IO;
using System.Linq;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class LevelGroundToJSONFileConverter : MonoBehaviour
    {
        [MenuItem("EnvironmentConverter/ConvertToJSON")]
        public static void ConvertBricksToJSON()
        {
            List<LevelBrickConfig> levelBricksConfigs = new List<LevelBrickConfig>();
            IEnumerable<GameObject> bricks = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .Where(go => go.layer == LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.Brick));

            foreach (GameObject brick in bricks)
            {
                if (brick.TryGetComponent(out BrickView brickView))
                {
                    LevelBrickConfig levelBrickConfig = new LevelBrickConfig
                    {
                        GameplayLayer = brickView.GameplayLayer,
                        Position = new Vector3(brickView.transform.position.x, brickView.transform.position.y, brickView.transform.position.z),
                        Scale = new Vector3(brickView.transform.localScale.x, brickView.transform.localScale.y, brickView.transform.localScale.z)
                    };

                    levelBricksConfigs.Add(levelBrickConfig);
                }
            }

            string bricksJson = JsonConvert.SerializeObject(levelBricksConfigs, Formatting.Indented);
            string bricksFilePath = Path.Combine(Application.dataPath, "_Project/Configs/Levels/Default/LevelBricks.json");

            File.WriteAllText(bricksFilePath, bricksJson);

            List<LevelEnvironmentPartConfig> environmentPartsConfigs = new List<LevelEnvironmentPartConfig>();
            IEnumerable<GameObject> environmentParts = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .Where(go => go.layer == LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnvironmentPart));

            foreach (GameObject environmentPart in environmentParts)
            {
                LevelEnvironmentPartConfig environmentPartConfig  = new LevelEnvironmentPartConfig()
                {
                    PrefabName = environmentPart.name,
                    Position = new Vector3(environmentPart.transform.position.x, environmentPart.transform.position.y, environmentPart.transform.position.z),
                    Scale = new Vector3(environmentPart.transform.localScale.x, environmentPart.transform.localScale.y, environmentPart.transform.localScale.z)
                };

                environmentPartsConfigs.Add(environmentPartConfig);
            }

            string environmentPartsJson = JsonConvert.SerializeObject(environmentPartsConfigs, Formatting.Indented);
            string environmentPartsFilePath = Path.Combine(Application.dataPath, "_Project/Configs/Levels/Default/LevelEnvironmentParts.json");

            File.WriteAllText(environmentPartsFilePath, environmentPartsJson);
        }
    }
}