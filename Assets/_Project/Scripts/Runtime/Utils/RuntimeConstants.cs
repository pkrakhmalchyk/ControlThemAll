using UnityEngine.SceneManagement;

namespace ControllThemAll.Runtime.Utils
{
    public static class RuntimeConstants
    {
        public static class Stats
        {
            public const string Health = "Health";
        }

        public static class PhysicLayers
        {
            public const string Brick = "Brick";
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string PlayerWeapon = "PlayerWeapon";
            public const string EnemyWeapon = "EnemyWeapon";
            public const string PlayerParticle = "PlayerParticle";
            public const string EnemyParticle = "EnemyParticle";
            public const string EnvironmentPart = "EnvironmentPart";
            public const string EnemyExplosionTrigger = "EnemyExplosionTrigger";

            public static readonly string[] AimStopLayers = { Brick, Enemy };
            public static readonly string[] PlayerLayers = { Player, PlayerWeapon, PlayerParticle };
            public static readonly string[] EnemyLayers = { Enemy, EnemyWeapon, EnemyParticle };
            public static readonly string[] PlayerParticleCollisionLayers = { Enemy, Brick };
            public static readonly string[] PlayerWeaponCollisionLayers = { Enemy };
            public static readonly string[] EnemyParticleCollisionLayers = { Player, Brick };
            public static readonly string[] EnemyWeaponCollisionLayers = { Player, EnemyExplosionTrigger };
            public static readonly string[] PlayerAttackLayers = { Enemy };
            public static readonly string[] EnemyAttackLayers = { Player };
            public static readonly string[] EnvironmentPartCollisionLayers = { Player };
        }

        public static class Configs
        {
            public const string AddressablesTextConfigsPath = "TextConfigs";
            public const string AddressablesDefaultLevelConfigsPath = "TextConfigs/Levels/Default";

            public const string PlayerConfigName = "Player.json";
            public const string PlayerGameplayLayersConfigName = "PlayerGameplayLayers.json";
            public const string EnemyGameplayLayersConfigName = "EnemyGameplayLayers.json";
            public const string DefaultGameplayLayersConfigName = "DefaultGameplayLayers.json";
            public const string LevelPlayersWeaponsConfigName = "LevelPlayersWeapons.json";
            public const string LevelEnemiesConfigName = "LevelEnemies.json";
            public const string LevelBricksConfigName = "LevelBricks.json";
            public const string LevelEnvironmentPartsConfigName = "LevelEnvironmentParts.json";
            public const string LevelsConfigsName = "LevelsConfigs.json";
            public const string LevelPlayersConfigName = "LevelPlayers.json";

            public const string PlayersWeaponsConfigsLabel = "playersWeaponsConfigs";
            public const string PlayersWeaponsAbilitiesConfigsLabel = "playersWeaponsAbilitiesConfigs";
            public const string EnemiesWeaponsConfigsLabel = "enemiesWeaponsConfigs";
            public const string EnemiesWeaponsAbilitiesConfigsLabel = "enemiesWeaponsAbilitiesConfigs";
            public const string EnemiesConfigsLabel = "enemiesConfigs";
            public const string ParticlesConfigsLabel = "particlesConfigs";
            public const string ParticlesAbilitiesConfigsLabel = "particlesAbilitiesConfigs";
        }

        public static class Players
        {
            public const string PlayersPath = "Players";
            public const string DefaultPlayer = "DefaultPlayer.prefab";
        }

        public static class Environment
        {
            public const string EnvironmentPath = "Environment";
            public const string DefaultBrick = "DefaultBrick.prefab";
            public const string LevelEndTrigger = "LevelEndTrigger.prefab";
            public const string EnemyExplosionTrigger = "EnemyExplosionTrigger.prefab";
            public const string EnvironmentPartsLabel = "environmentParts";
        }

        public static class Enemies
        {
            public const string EnemiesPath = "Enemies";
            public const string SlimeEnemy = "SlimeEnemy.prefab";
            public const string TurtleEnemy = "TurtleEnemy.prefab";

            public static readonly string[] All = { SlimeEnemy, TurtleEnemy };
        }

        public static class Particles
        {
            public const string ParticlesPath = "Particles";
            public const string BulletParticle = "BulletParticle.prefab";
            public const string BombParticle = "BombParticle.prefab";

            public static readonly string[] All = { BulletParticle, BombParticle };
        }

        public static class Weapons
        {
            public const string WeaponsPath = "Weapons";
            public const string InvisibleWeapon = "InvisibleWeapon.prefab";
            public const string BulletLauncherWeapon = "BulletLauncherWeapon.prefab";
            public const string BombLauncherWeapon = "BombLauncherWeapon.prefab";

            public static readonly string[] All = { InvisibleWeapon, BombLauncherWeapon, BulletLauncherWeapon };
        }


        public static class VisualEffects
        {
            public const string VisualEffectsPath = "VisualEffects";
            public const string JellyHitPrefab = "JellyHitEffect.prefab";
            public const string JellyDamageTakePrefab = "JellyDamageTakeEffect.prefab";
            public const string JellyExplosionPrefab = "JellyExplosionEffect.prefab";
            public const string MultiColorAreaPrefab = "MultiColorAreaEffect.prefab";

            public static readonly string[] All =
            {
                JellyHitPrefab,
                JellyDamageTakePrefab,
                JellyExplosionPrefab,
                MultiColorAreaPrefab
            };
        }

        public static class Scenes
        {
            public static readonly int Bootstrap = SceneUtility.GetBuildIndexByScenePath("Bootstrap");
            public static readonly int MainMenu = SceneUtility.GetBuildIndexByScenePath("MainMenu");
            public static readonly int Empty = SceneUtility.GetBuildIndexByScenePath("Empty");
            public static readonly int Gameplay = SceneUtility.GetBuildIndexByScenePath("Gameplay");
        }

        public static class UI
        {
            public const string AddresablesGameplayUIPath = "UI/Gameplay";
            public const string AddressablesMainMenuUIPath = "UI/MainMenu";
            public const string ResourcesRootUIPath = "Prefabs/UI/Root";
            public const string GameplayHUDWindowPrefabName = "GameplayHUDWindow.prefab";
            public const string GameplayEndLevelPopupPrefabName = "GameplayEndLevelPopup.prefab";
            public const string GameplayPausePopupPrefabName = "GameplayPausePopup.prefab";
            public const string RadialSelectionMenuPrefabName = "RadialSelectionMenu.prefab";
            public const string GameplayLevelStartWindowPrefabName = "GameplayLevelStartWindow.prefab";
            public const string MainMenuWindowPrefabName = "MainMenuWindow.prefab";
            public const string LevelSelectionWindowPrefabName = "LevelSelectionWindow.prefab";
            public const string RootUIPrefabName = "RootUI";
        }

        public static class Materials
        {
            public const string MaterialsPath = "Materials";
            public const string AimMaterial = "MAT_AimTrajectory.mat";
        }

        public static class Data
        {
            public const string GameProgressData = "GameProgressData";
        }

        public static class Settings
        {
            public const float DragAttackInputPlaneDistance = 50f;
            public const float DragAttackInputPlaneAngle = -30f;
            public const float DragAttackInputSphereCastRadius = 0.5f;
            public const float PlayersPositionSwapDistance = 1f;
            public const float PlayersPositionSwapTime = 0.5f;
            public const float LevelBordersDetectionDistance = 1.5f;
            public const float DistanceBetweenTrajectoryPoints = 0.1f;
            public const float MinTrajectoryHeight = -30f;
            public const float CameraShakeAfterPlayerHitDuration = 0.5f;
            public const float GameplaySlowDownSpeed = 0.2f;
        }
    }
}
