using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace SoftKitty.MasterNavigationMap
{
    public class MMN_SettingsProvider : AssetSettingsProvider
    {
        private string searchContext;
        private VisualElement rootElement;
        public static MMN_Settings CurrentSettings
        {
            get
            {
                MMN_Settings settings;
                EditorBuildSettings.TryGetConfigObject(MMN_Settings.CONFIG_NAME, out settings);
                return settings;
            }
            set
            {
                var remove = (value == null);
                if (remove)
                {
                    EditorBuildSettings.RemoveConfigObject(MMN_Settings.CONFIG_NAME);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(MMN_Settings.CONFIG_NAME, value, overwrite: true);
                    var settings = MMN_SettingsProvider.CurrentSettings;
                    var settingsType = settings.GetType();
                    var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();

                    preloadedAssets.RemoveAll(settings => settings.GetType() == settingsType);
                    preloadedAssets.Add(settings);

                    PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
                }
            }
        }

        public MMN_SettingsProvider()
       : base("Project/Master Map Navigation", () => CurrentSettings)
        {
            CurrentSettings = FindMMN_Settings();
            keywords = GetSearchKeywordsFromGUIContentProperties<MMN_Settings>();
        }

        private static MMN_Settings FindMMN_Settings()
        {
            var filter = $"t:{typeof(MMN_Settings).Name}";
            var guids = AssetDatabase.FindAssets(filter);
            var hasGuids = guids.Length > 0;
            var path = hasGuids ? AssetDatabase.GUIDToAssetPath(guids[0]) : string.Empty;
            return AssetDatabase.LoadAssetAtPath<MMN_Settings>(path);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            this.rootElement = rootElement;
            this.searchContext = searchContext;
            base.OnActivate(searchContext, rootElement);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();
            if (CurrentSettings == null)
            {
                CurrentSettings = FindMMN_Settings();
                if (CurrentSettings != null)
                {
                    RefreshEditor();
                    return;
                }
            }

            if (CurrentSettings == null)
            {
                DisplaySettingsCreationGUI();
            }
            else
            {
                DrawCurrentSettingsGUI();
                base.OnGUI(searchContext);
            }
        }

        private void DrawCurrentSettingsGUI()
        {

            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel++;
            var settings = EditorGUILayout.ObjectField("Current Settings", CurrentSettings,
                typeof(MMN_Settings), allowSceneObjects: false) as MMN_Settings;
            if (settings) DrawCurrentSettingsMessage();
            EditorGUI.indentLevel--;

            var newSettings = EditorGUI.EndChangeCheck();
            if (newSettings)
            {
                CurrentSettings = settings;
                RefreshEditor();
            }

        }

        private void RefreshEditor()
        {
            base.OnActivate(searchContext, rootElement);
        }

        private void DisplaySettingsCreationGUI()
        {
            const string message = "You have no Master Map Navigation Settings. Would you like to create one?";
            EditorGUILayout.HelpBox(message, MessageType.Info, wide: true);
            var openCreationdialog = GUILayout.Button("Create");
            if (openCreationdialog)
            {
                CurrentSettings = SaveMMN_Asset();
            }
        }

        private void DrawCurrentSettingsMessage()
        {
            const string message = "This is the current Master Map Navigatio Settings and " +
                "it will be automatically included into any builds.";
            EditorGUILayout.HelpBox(message, MessageType.Info, wide: true);
        }


        private static MMN_Settings SaveMMN_Asset()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                title: "Save Master Map Navigatio Settings", defaultName: "MMN_Settings", extension: "asset",
                message: "Please enter a filename to save the projects Master Map Navigatio settings.");
            var invalidPath = string.IsNullOrEmpty(path);
            if (invalidPath) return null;

            var settings = ScriptableObject.CreateInstance<MMN_Settings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = settings;
            return settings;
        }

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsMenu() => new MMN_SettingsProvider();

    }
}
