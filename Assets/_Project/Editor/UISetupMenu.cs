using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiceNDungeon.Inputs;
using DiceNDungeon.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DiceNDungeon.Editor
{
    /// <summary>
    /// Menu "Dice n Dungeon/UI": builds the Splash and Main Menu scenes, the PanelSettings asset,
    /// the Build Profiles scene list, and adds the in-game UI to the open scene. Safe to run again.
    /// </summary>
    public static class UISetupMenu
    {
        private const string UiFolder = "Assets/_Project/UI";
        private const string ScenesFolder = "Assets/_Project/Scenes";
        private const string PanelSettingsPath = UiFolder + "/PanelSettings.asset";
        private const string ThemePath = UiFolder + "/Theme/DiceTheme.tss";
        private const string SplashScenePath = ScenesFolder + "/Splash.unity";
        private const string MainMenuScenePath = ScenesFolder + "/MainMenu.unity";
        private const string DefaultGameScenePath = "Assets/Scenes/Daniel.unity";

        [MenuItem("Dice n Dungeon/UI/1. Create Splash and Main Menu Scenes")]
        public static void CreateMenuScenes()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if ((File.Exists(SplashScenePath) || File.Exists(MainMenuScenePath)) &&
                !EditorUtility.DisplayDialog("Dice n Dungeon", "Splash/MainMenu scenes already exist. Overwrite them?", "Overwrite", "Cancel"))
            {
                return;
            }

            EnsureFolder(ScenesFolder);
            PanelSettings panel = GetOrCreatePanelSettings();

            CreateUiScene(SplashScenePath, UiFolder + "/Splash/Splash.uxml", panel, typeof(SplashController));
            CreateUiScene(MainMenuScenePath, UiFolder + "/MainMenu/MainMenu.uxml", panel, typeof(MainMenuController));
            AddScenesToBuild(SplashScenePath, MainMenuScenePath, DefaultGameScenePath);

            EditorSceneManager.OpenScene(SplashScenePath);
            Debug.Log("Dice n Dungeon: Splash and MainMenu scenes created and added to the scene list (Splash first). " +
                      "Now open your gameplay scene and run 'Dice n Dungeon/UI/2. Add Game UI To Open Scene'.");
        }

        [MenuItem("Dice n Dungeon/UI/2. Add Game UI To Open Scene")]
        public static void AddGameUiToOpenScene()
        {
            var existing = Object.FindFirstObjectByType<GameMenusController>();
            if (existing != null)
            {
                Selection.activeGameObject = existing.gameObject;
                Debug.Log("Dice n Dungeon: this scene already has a Game UI.", existing);
                return;
            }

            var gameUi = new GameObject("Game UI");
            Undo.RegisterCreatedObjectUndo(gameUi, "Add Game UI");
            var document = gameUi.AddComponent<UIDocument>();
            document.panelSettings = GetOrCreatePanelSettings();
            document.visualTreeAsset = LoadRequired<VisualTreeAsset>(UiFolder + "/Game/GameUI.uxml");
            gameUi.AddComponent<HudController>();
            gameUi.AddComponent<GameMenusController>(); // also adds MenuInputReader (RequireComponent)

            AddCameraSensitivity();

            Scene scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            AddScenesToBuild(SplashScenePath, MainMenuScenePath, scene.path);
            Selection.activeGameObject = gameUi;
            Debug.Log($"Dice n Dungeon: Game UI added to '{scene.name}'. Save the scene (Ctrl+S).", gameUi);
        }

        private static void CreateUiScene(string scenePath, string uxmlPath, PanelSettings panel, Type controllerType)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // A camera avoids the "No cameras rendering" message; the UI itself does not need it.
            var cameraObject = new GameObject("Main Camera") { tag = "MainCamera" };
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            cameraObject.AddComponent<AudioListener>();

            var ui = new GameObject("UI");
            var document = ui.AddComponent<UIDocument>();
            document.panelSettings = panel;
            document.visualTreeAsset = LoadRequired<VisualTreeAsset>(uxmlPath);
            ui.AddComponent(controllerType); // RequireComponent adds MenuInputReader where needed

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        private static PanelSettings GetOrCreatePanelSettings()
        {
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panel != null) return panel;

            panel = ScriptableObject.CreateInstance<PanelSettings>();
            panel.themeStyleSheet = LoadRequired<ThemeStyleSheet>(ThemePath);
            panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panel.referenceResolution = new Vector2Int(1920, 1080);
            panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panel.match = 0.5f;
            AssetDatabase.CreateAsset(panel, PanelSettingsPath);
            AssetDatabase.SaveAssets();
            return panel;
        }

        /// <summary>Puts the given scenes first (in order) in the Build Profiles scene list, keeping the others.</summary>
        private static void AddScenesToBuild(params string[] firstScenePaths)
        {
            var ordered = firstScenePaths.Where(p => !string.IsNullOrEmpty(p) && File.Exists(p)).Distinct().ToList();
            var scenes = new List<EditorBuildSettingsScene>(ordered.Select(p => new EditorBuildSettingsScene(p, true)));
            scenes.AddRange(EditorBuildSettings.scenes.Where(s => !ordered.Contains(s.path)));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        /// <summary>Adds CameraSensitivity to Cinemachine cameras with an Input Axis Controller (by name, no hard dependency).</summary>
        private static void AddCameraSensitivity()
        {
            Type axisControllerType = Type.GetType("Unity.Cinemachine.CinemachineInputAxisController, Unity.Cinemachine");
            Type sensitivityType = Type.GetType("DiceNDungeon.CameraControl.CameraSensitivity, Game.Cinemachine");
            if (axisControllerType == null || sensitivityType == null) return;

            foreach (Object found in Object.FindObjectsByType(axisControllerType, FindObjectsSortMode.None))
            {
                var go = ((Component)found).gameObject;
                if (go.GetComponent(sensitivityType) == null) Undo.AddComponent(go, sensitivityType);
            }
        }

        private static T LoadRequired<T>(string path) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null) Debug.LogError($"Dice n Dungeon: missing asset {path}");
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
