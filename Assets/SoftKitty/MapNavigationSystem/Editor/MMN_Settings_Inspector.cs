using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace SoftKitty.MasterNavigationMap
{
    [CustomEditor(typeof(MMN_Settings))]
    public class MMN_Settings_Inspector : Editor
    {
        Color _activeColor = new Color(0.1F, 0.3F, 0.5F);
        Color _disableColor = new Color(0F, 0.1F, 0.3F);
        Color _actionColor = new Color(1F, 1F, 0F);
        Color _titleColor = new Color(0.3F, 0.5F, 1F);
        Color _buttonColor = new Color(0F, 0.8F, 0.3F);
        GUIStyle _titleButtonStyle;
        bool _sceneMapExpand = false;
        bool _generalExpand = false;
        bool _mapSettingExpand = false;
        bool _miniMapSettingExpand = false;
        string _packRootPath;


     

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            _titleButtonStyle = new GUIStyle(GUI.skin.button);
            _titleButtonStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle header = new GUIStyle();
            header.fontStyle= FontStyle.Bold;
            header.normal.textColor = Color.white;
            header.alignment = TextAnchor.MiddleLeft;
            Color _backgroundColor = GUI.backgroundColor;
            var script = MonoScript.FromScriptableObject(this);
            MMN_Settings myTarget = (MMN_Settings)target;

            string _thePath = AssetDatabase.GetAssetPath(script);
            _thePath = _thePath.Replace("MMN_Settings_Inspector.cs", "");
            _packRootPath = _thePath.Replace("Editor/", "");

            Texture logoIcon = (Texture)AssetDatabase.LoadAssetAtPath(_thePath + "Logo.png", typeof(Texture));
            Texture warningIcon = (Texture)AssetDatabase.LoadAssetAtPath(_thePath + "warning.png", typeof(Texture));
            GUILayout.BeginHorizontal();
            GUILayout.Box(logoIcon);
            GUILayout.EndHorizontal();

            int _lastLayer = 1;
            for (int i = 0; i < 32; i++)
            {
                if (LayerMask.LayerToName(i) != "") _lastLayer=i+1;
            }

            List<string> _layers = new List<string>();
            for (int i = 0; i < _lastLayer; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "") 
                    _layers.Add(layerName);
                else
                    _layers.Add(i.ToString()+" Empty");
            }
            string[] layers = _layers.ToArray();

            TitleButton("General Settings", ref _generalExpand);
            if (_generalExpand)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUI.color = _buttonColor;
                GUILayout.Label("Map Icon Categories:", GUILayout.Width(150));
                GUI.color = Color.white;
                GUI.backgroundColor = _titleColor;
                if (GUILayout.Button("Add",GUILayout.Width(100))) {
                    IconGroupSetting _newSetting = new IconGroupSetting();
                    _newSetting._name = "New Category " + MMN_SettingsProvider.CurrentSettings.IconGroups.Count.ToString();
                    _newSetting._icon = null;
                    _newSetting._fold = false;
                    MMN_SettingsProvider.CurrentSettings.IconGroups.Add(_newSetting);
                }
                GUILayout.Space(40);
                ResetColor();
                GUILayout.EndHorizontal();

                for (int i=0;i< MMN_SettingsProvider.CurrentSettings.IconGroups.Count;i++) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    MMN_SettingsProvider.CurrentSettings.IconGroups[i]._fold = EditorGUILayout.Foldout(
                        MMN_SettingsProvider.CurrentSettings.IconGroups[i]._fold,
                        MMN_SettingsProvider.CurrentSettings.IconGroups[i]._name,true);
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X",GUILayout.Width(20))) {
                        MMN_SettingsProvider.CurrentSettings.IconGroups.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        return;
                    }
                    GUILayout.Space(40);
                    ResetColor();
                    GUILayout.EndHorizontal();

                    if (MMN_SettingsProvider.CurrentSettings.IconGroups[i]._fold) {
                        string _oldName = MMN_SettingsProvider.CurrentSettings.IconGroups[i]._name;
                        TextField(ref MMN_SettingsProvider.CurrentSettings.IconGroups[i]._name, "Category Name:", 80);
                        if (_oldName != MMN_SettingsProvider.CurrentSettings.IconGroups[i]._name && _oldName!="" && MMN_SettingsProvider.CurrentSettings.IconGroups[i]._name!="") {
                            for (int u=0;u< MMN_SettingsProvider.CurrentSettings.IconGroups.Count;u++) {
                                if (u != i && MMN_SettingsProvider.CurrentSettings.IconGroups[u]._name == MMN_SettingsProvider.CurrentSettings.IconGroups[i]._name)
                                {
                                    MMN_SettingsProvider.CurrentSettings.IconGroups[i]._name += " ("+i.ToString()+")";
                                }
                            }
                        }
                        TextureField(ref MMN_SettingsProvider.CurrentSettings.IconGroups[i]._icon, "Category Icon:", 80);
                        Toggle(ref MMN_SettingsProvider.CurrentSettings.IconGroups[i]._visisbleDefault, "Default Visibility:", 80);
                    }
                }

                TextureField(ref MMN_SettingsProvider.CurrentSettings.PlayerIcon, "Player's Icon:", 40);

                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUILayout.Label("Player Orientation:", GUILayout.Width(150));
                myTarget.PlayerOrientation = (MMN_Settings.PlayerOrientations)EditorGUILayout.EnumPopup(myTarget.PlayerOrientation, GUILayout.Width(150));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUILayout.Label("Ground Layer Mask:", GUILayout.Width(150));
                MMN_SettingsProvider.CurrentSettings.GroundLayer= EditorGUILayout.MaskField(MMN_SettingsProvider.CurrentSettings.GroundLayer, layers, GUILayout.Width(150));
                GUILayout.EndHorizontal();

                GUI.color = _buttonColor;
                Toggle(ref MMN_SettingsProvider.CurrentSettings.EnableCustomMarker, "Enable Custom Markers", 40);
                GUI.color = Color.white;
                if (MMN_SettingsProvider.CurrentSettings.EnableCustomMarker)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    GUILayout.Label("When placing custom marker:", GUILayout.Width(250));
                    GUILayout.EndHorizontal();
                    MouseButton(ref MMN_SettingsProvider.CurrentSettings.CustomMarkerConfirmButton, "Confirm Button:", 80);
                    MouseButton(ref MMN_SettingsProvider.CurrentSettings.CustomMarkerCancelButton, "Cancel Button:", 80);
                    KeyButton(ref MMN_SettingsProvider.CurrentSettings.CustomMarkerCancelKey, "Cancel Key:", 80);
                    Toggle(ref MMN_SettingsProvider.CurrentSettings.AllowPlayerToRenameCustomMarker, "Allow player to rename", 60);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    GUILayout.Label("Custom Markers Settings:", GUILayout.Width(150));
                    GUI.backgroundColor = _titleColor;
                    if (GUILayout.Button("Add", GUILayout.Width(100)))
                    {
                        CustomMarkerSetting _newSetting = new CustomMarkerSetting();
                        _newSetting._defaultName = "Custom Marker " + MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings.Count.ToString();
                        _newSetting._uid= "cm" + MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings.Count.ToString();
                        _newSetting._icon = null;
                        MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings.Add(_newSetting);
                    }
                    GUILayout.Space(40);
                    ResetColor();
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(60);
                        if (MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._icon == null) GUI.color = Color.red;
                        MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._fold = EditorGUILayout.Foldout(
                            MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._fold,
                            MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._defaultName, true);
                        ResetColor();
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings.RemoveAt(i);
                            GUILayout.EndHorizontal();
                            return;
                        }
                        GUILayout.Space(40);
                        ResetColor();
                        GUILayout.EndHorizontal();
                        if (MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._fold) {
                            TextField(ref MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._defaultName, "Default Name:", 80);
                            string _oldName = MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._uid;
                            TextField(ref MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._uid, "Unique ID:", 80);
                            if (_oldName != MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._uid && _oldName != "" && MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._uid != "")
                            {
                                for (int u = 0; u < MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings.Count; u++)
                                {
                                    if (u != i && MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[u]._uid == MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._uid)
                                    {
                                        MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._uid += "(" + i.ToString() + ")";
                                    }
                                }
                            }
                            if (MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._icon == null) GUI.color = Color.red;
                            TextureField(ref MMN_SettingsProvider.CurrentSettings.CustomMarkerSettings[i]._icon, "Marker Icon:", 80);
                            ResetColor();
                        }
                    }
                }

                EditorGUILayout.Separator();
            }

            TitleButton("Scene | Map",ref _sceneMapExpand);
            if (_sceneMapExpand) 
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUI.color = _buttonColor;
                GUILayout.Label("Map Mode:",GUILayout.Width(150));
                GUI.color = Color.white;
                GUI.backgroundColor = _buttonColor;
                myTarget.MapMode = (MMN_Settings.MapModes)EditorGUILayout.EnumPopup(myTarget.MapMode, GUILayout.Width(150));
                GUI.backgroundColor = _backgroundColor;
                GUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                if (myTarget.MapMode == MMN_Settings.MapModes.StaticMap)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    EditorGUILayout.HelpBox("Pre-render your 3D scene into a stylized 2D map texture using the Map Generator tool.\nThis mode provides the best performance and works seamlessly for scene-based games with pre-designed levels.", MessageType.Info,true);
                    GUILayout.Space(60);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    EditorGUILayout.HelpBox("Best For:\nGames with pre-designed scenes where the world layout doesn't change, such as RPGs, dungeon crawlers, or strategy games with fixed maps.", MessageType.Info, true);
                    GUILayout.Space(60);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    PreGeneratedMode();
                }
                else if (myTarget.MapMode == MMN_Settings.MapModes.DynamicMap)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    EditorGUILayout.HelpBox("Render a real-time stylized map using a top-down camera that captures the 3D scene dynamically.\nThis mode is ideal for procedurally generated worlds or massive open-world games that require seamless and real-time updates.", MessageType.Info, true);
                    GUILayout.Space(60);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    EditorGUILayout.HelpBox("Best For:\nGames with procedural generation, seamless open-world exploration, or environments where the terrain and objects may change dynamically at runtime.", MessageType.Info, true);
                    GUILayout.Space(60);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    GUI.color = _buttonColor;
                    GUILayout.Label("Render Texture Settings:");
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    GUILayout.Label("RT LayerMask:", GUILayout.Width(150));
                    MMN_SettingsProvider.CurrentSettings.RtLayerMask = EditorGUILayout.MaskField(MMN_SettingsProvider.CurrentSettings.RtLayerMask, layers, GUILayout.Width(150));
                    GUILayout.EndHorizontal();

                    RealTimeMode();
                }
            }

            TitleButton("World Map Settings", ref _mapSettingExpand);
            if (_mapSettingExpand)
            {
                FloatSlider(ref MMN_SettingsProvider.CurrentSettings.WorldMapDefaultZoom,0f,1f, "Default Zoom(normalized):", " ", 40);
                FloatSlider(ref MMN_SettingsProvider.CurrentSettings.WorldMapMaxmiumZoomIn, 2f, 10F, "Maximum Zoom In:", "x", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.PanBeyondBounds, "Pan Beyond Bounds",40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.LockIconScale, "Lock Icon Scale:", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.DisplaySceneTitle, "Display Scene Title:", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.EnableNavigationPath, "Enable Navigation Path", 40);
                if(MMN_SettingsProvider.CurrentSettings.EnableNavigationPath) MouseButton(ref MMN_SettingsProvider.CurrentSettings.StartNavigationMouseButton, "Start Navigation Button:", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.EnableScroll, "Enable scroll with keyboard", 40);
                if (MMN_SettingsProvider.CurrentSettings.EnableScroll)
                {
                    KeyButton(ref MMN_SettingsProvider.CurrentSettings.MapScrollLeftKey, "Map Scroll Left Key:", 40);
                    KeyButton(ref MMN_SettingsProvider.CurrentSettings.MapScrollRightKey, "Map Scroll Right Key:", 40);
                    KeyButton(ref MMN_SettingsProvider.CurrentSettings.MapScrollUpKey, "Map Scroll Up Key:", 40);
                    KeyButton(ref MMN_SettingsProvider.CurrentSettings.MapScrollDownKey, "Map Scroll Down Key:", 40);
                    FloatSlider(ref MMN_SettingsProvider.CurrentSettings.ScrollSpeed, 0.5F, 3F, "Scroll Speed:", "x", 40);
                }
                Toggle(ref MMN_SettingsProvider.CurrentSettings.EnableMarkers, "Allow player place map markers", 40);
                if (MMN_SettingsProvider.CurrentSettings.EnableMarkers)
                {
                    IntSlider(ref MMN_SettingsProvider.CurrentSettings.MaximumMarkers, 1, 99, "Maximum Markers:", "", 60);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    GUILayout.Label("When more than "+ MMN_SettingsProvider.CurrentSettings.MaximumMarkers+" markers:", GUILayout.Width(170));
                    myTarget.MarkerOutNumberMethod = (MMN_Settings.MarkerOutNumberMethods)EditorGUILayout.EnumPopup(myTarget.MarkerOutNumberMethod, GUILayout.Width(150));
                    GUILayout.EndHorizontal();
                    if (myTarget.MarkerOutNumberMethod == MMN_Settings.MarkerOutNumberMethods.Callback)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(60);
                        EditorGUILayout.HelpBox("Callback code example:\n" +
                            "MarkerOutLimitCallback += OnMarkerOutLimit;\n" +
                            "public void OnMapLayerChange(int _layer) {...}", MessageType.Info, true);
                        GUILayout.Space(60);
                        GUILayout.EndHorizontal();
                    }
                }
                if (MMN_SettingsProvider.CurrentSettings.EnableMarkers) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    GUILayout.Label("Create Map Marker Key + Mouse Button:");
                    GUILayout.EndHorizontal();
                    MouseKeyButton(ref MMN_SettingsProvider.CurrentSettings.MapMarkerKey, ref MMN_SettingsProvider.CurrentSettings.MapMarkerButton, 40);
                }
            }

            TitleButton("Mini Map Settings", ref _miniMapSettingExpand);
            if (_miniMapSettingExpand)
            {
                FloatSlider(ref MMN_SettingsProvider.CurrentSettings.MiniMapDefaultZoom, 0f, 1F, "Default Zoom(normalized):", " ", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.AllowZoomInOut, "Allow Zoom In/Out:", 40);
                if (MMN_SettingsProvider.CurrentSettings.AllowZoomInOut) FloatSlider(ref MMN_SettingsProvider.CurrentSettings.MiniMapMaxmiumZoomIn, 2f, 10F, "Maximum Zoom In:", "x", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.MiniLockIconScale, "Lock Icon Scale:", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.MiniDisplaySceneTitle, "Display Scene Title:", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.MiniDisplayCoordinate, "Display Coordinate:", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.ToggleCompassButton, "Enable Player Orientation Button:", 40);
                Toggle(ref MMN_SettingsProvider.CurrentSettings.LockPlayerNorth, "Player Orientation as Default:", 40);
                
            }


            if (GUI.changed && !Application.isPlaying) UnityEditor.EditorUtility.SetDirty(myTarget);
        }


        private void RealTimeMode()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            GUILayout.Label("Render Area Size:", GUILayout.Width(150));
            MMN_SettingsProvider.CurrentSettings.WorldMapSize = Mathf.Max(500, EditorGUILayout.IntField(MMN_SettingsProvider.CurrentSettings.WorldMapSize, GUILayout.Width(50)));
            GUILayout.Label("m x " + MMN_SettingsProvider.CurrentSettings.WorldMapSize + "m", GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            EditorGUILayout.HelpBox("This determines how large the area around the player will be captured by the top-down render texture camera.\n" +
                "Larger sizes allow the player to scroll further on the world map when [Unlimited WorldMap] is unchecked.\n" +
                "A larger size also reduces the frequency of minimap updates but may have a higher performance cost.", MessageType.Info, true);
            GUILayout.Space(60);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            GUILayout.Label("RT Size:", GUILayout.Width(150));
            MMN_SettingsProvider.CurrentSettings.RtSize = (MMN_Settings.TextureSize)EditorGUILayout.EnumPopup(MMN_SettingsProvider.CurrentSettings.RtSize, GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            EditorGUILayout.HelpBox("The render texture size depends on the [Render Area Size].\n" +
                "For optimal clarity, use a larger texture size with a larger render area size (ideally 2 pixels per meter or higher).\n" +
                "However, larger textures may cause noticeable performance spikes when the minimap updates. (The minimap only updates when the player moves out of the previously rendered area.)", MessageType.Info, true);
            GUILayout.Space(60);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            int _size = 1024 * Mathf.FloorToInt(Mathf.Pow(2, (int)MMN_SettingsProvider.CurrentSettings.RtSize));
            GUI.color = _titleColor;
            GUILayout.Label("Resolution: "+ (_size/ MMN_SettingsProvider.CurrentSettings.WorldMapSize).ToString("0.0")+" pixels/meter");
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            Toggle(ref MMN_SettingsProvider.CurrentSettings.UnlimitedWorldMap, "Unlimited WorldMap", 60);

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            EditorGUILayout.HelpBox("When enabled, players can scroll the map to any position in the 3D world.\n" +
                "Ensure that in your script, the 3D scene within MapInteractive.WorldMapInstance.ViewRect is always loaded to match the visible map area.", MessageType.Info, true);
            GUILayout.Space(60);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            GUILayout.Label("RT Camera Height:", GUILayout.Width(150));
            MMN_SettingsProvider.CurrentSettings.RtCameraHeight =GUILayout.HorizontalSlider(MMN_SettingsProvider.CurrentSettings.RtCameraHeight,50F,1000F, GUILayout.Width(150));
            GUILayout.Label(Mathf.FloorToInt(MMN_SettingsProvider.CurrentSettings.RtCameraHeight).ToString()+"m", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            GUILayout.Label("RT Camera Depth:", GUILayout.Width(150));
            MMN_SettingsProvider.CurrentSettings.RtCameraDepth = Mathf.Clamp(MMN_SettingsProvider.CurrentSettings.RtCameraDepth, MMN_SettingsProvider.CurrentSettings.RtCameraHeight+10, MMN_SettingsProvider.CurrentSettings.RtCameraHeight + 500F);
            MMN_SettingsProvider.CurrentSettings.RtCameraDepth = GUILayout.HorizontalSlider(MMN_SettingsProvider.CurrentSettings.RtCameraDepth, MMN_SettingsProvider.CurrentSettings.RtCameraHeight+10, MMN_SettingsProvider.CurrentSettings.RtCameraHeight+500F, GUILayout.Width(150));
            GUILayout.Label(Mathf.FloorToInt(MMN_SettingsProvider.CurrentSettings.RtCameraDepth).ToString() + "m", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            Toggle(ref MMN_SettingsProvider.CurrentSettings.RtStylizedRendering, "Stylized Rendering",60);
            if (MMN_SettingsProvider.CurrentSettings.RtStylizedRendering) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(60);
                GUILayout.Label("RT Material:", GUILayout.Width(150));
                MMN_SettingsProvider.CurrentSettings.RtStylizedeMaterial = (Material)EditorGUILayout.ObjectField(MMN_SettingsProvider.CurrentSettings.RtStylizedeMaterial, typeof(Material),false, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(60);
                EditorGUILayout.HelpBox("Adjust the stylized settings on the material.\n" +
                    "You could create your own custom render texture shader.\n" +
                    "and assign the material here.", MessageType.Info, true);
                GUILayout.Space(60);
                GUILayout.EndHorizontal();

                Toggle(ref MMN_SettingsProvider.CurrentSettings.RtDepthOnly, "Depth Only", 60);

                GUILayout.BeginHorizontal();
                GUILayout.Space(60);
                EditorGUILayout.HelpBox("When using my custom render texture material.\n" +
                    "Please make sure the [ Depth Only ] is checked.", MessageType.Info, true);
                GUILayout.Space(60);
                GUILayout.EndHorizontal();
            }

        }

        private void PreGeneratedMode()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUI.backgroundColor = _actionColor;
            if (GUILayout.Button("Open [Map Generator] Tool (Render Map Texture)"))
            {
                MapCreator _creator = (MapCreator)Object.FindAnyObjectByType<MapCreator>();
                if (_creator == null)
                {
                    GameObject _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_packRootPath + "Prefabs/MapGenerator.prefab");
                    GameObject _tool = Instantiate(_prefab) as GameObject;
                    _tool.name = "Map Generator (MMN Tool)";
                    _creator = _tool.GetComponent<MapCreator>();
                }
                Selection.activeObject = _creator.gameObject;
                _creator.Init();

            }
            ResetColor();
            GUILayout.Space(40);
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUI.backgroundColor = _buttonColor;
            if (GUILayout.Button("Refresh Scene List From Build Settings"))
            {
                int _sceneCount = SceneManager.sceneCountInBuildSettings;
                for (int i = 0; i < _sceneCount; i++)
                {
                    string _scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                    int _index = _scenePath.LastIndexOf("/");
                    string _sceneName = _scenePath.Substring(_index + 1, _scenePath.Length - _index - 1).Replace(".unity", "");
                    AddScene(_sceneName, _scenePath);
                }
                UpdateGenertorTool();
            }
            GUILayout.Space(40);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Add Current Scene"))
            {
                Scene _scene = SceneManager.GetActiveScene();
                string _sceneName = _scene.name;
                string _scenePath = _scene.path;
                AddScene(_sceneName, _scenePath);
                UpdateGenertorTool();
            }
            ResetColor();
            GUILayout.Space(40);
            GUILayout.EndHorizontal();


            for (int i = 0; i < MMN_SettingsProvider.CurrentSettings.SceneMaps.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                SubTitleButton(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._sceneName, ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._fold);
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    MMN_SettingsProvider.CurrentSettings.SceneMaps.RemoveAt(i);
                    GUILayout.EndHorizontal();
                    UpdateGenertorTool();
                    return;
                }
                ResetColor();
                GUILayout.Space(40);
                GUILayout.EndHorizontal();

                if (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._fold)
                {
                    GUI.color = _titleColor;
                    Label(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._scenePath, 60);
                    ResetColor();
                    TextField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._sceneTitle, "Display Title:", 60);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    GUI.backgroundColor = MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.Count == 0? Color.red: _titleColor;
                    if (GUILayout.Button("Add New Map Layer", GUILayout.Width(150)))
                    {
                        MapSetting _map = new MapSetting();
                        _map._name = MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.Count == 0 ? "Base" : ("Layer" + (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.Count + 1).ToString());
                        _map._fold = false;
                        _map._map = SearchMap(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._sceneName, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.Count);
                        MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.Add(_map);
                        UpdateGenertorTool();
                    }
                    ResetColor();
                    GUILayout.Space(40);
                    GUILayout.EndHorizontal();
                    if (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.Count == 0)
                    {
                        Warnning("You must have at least one map layer, please click above button to add one.", 60);
                    }

                    for (int u = 0; u < MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.Count; u++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(60);
                        SubTitleButton("Map Layer " + u.ToString() + " (" + MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._name + ")",
                            ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._fold);
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps.RemoveAt(u);
                            GUILayout.EndHorizontal();
                            UpdateGenertorTool();
                            return;
                        }
                        ResetColor();
                        GUILayout.Space(40);
                        GUILayout.EndHorizontal();

                        if (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._fold)
                        {
                            TextField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._name, "Name:", 80);
                            TextureField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._map, "Map Texture:", 80);
                            FloatSlider(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._height, -50F, 500F, "Layer Height:", "m", 80);
                            FloatSlider(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._ceil, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._maps[u]._height+2F, 500F, "Layer Ceil:", "m", 80);
                        }
                    }

                    Vector3Field(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._bottomLeft, "Bottom Left Corner Position:", 60);
                    bool _outside= Vector3FieldLimited(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._topRight, "Top Right Corner Position:", 60,
                        new Vector2(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._bottomLeft.x,100000000),
                        new Vector2(-50F, 500F),
                        new Vector2(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._bottomLeft.z, 100000000)
                        );
                    if (_outside)
                    {
                        Warnning("The x and z value of the top right corner can not be less than the bottom left corner.", 60);
                    }


                    EditorGUILayout.Separator();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    GUI.color = _buttonColor;
                    SubTitleButton("Detailed Location Sub-Maps", ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMapFold);
                    ResetColor();
                    GUILayout.EndHorizontal();

                    if (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMapFold) {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(60);
                        EditorGUILayout.HelpBox("Enhance navigation by adding high-detail sub-maps for key locations like dungeons or castles.\n" +
                            "The system will seamlessly transition to these sub-maps upon entry,\n" +
                            "or they can be accessed via an interactive icon on your primary map.", MessageType.Info, true);
                        GUILayout.Space(60);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(80);
                        GUI.backgroundColor = _titleColor;
                        if (GUILayout.Button("Add New Sub-Map", GUILayout.Width(150)))
                        {
                            SubMapSetting _newSubMap = new SubMapSetting();
                            _newSubMap._areaTitle = "New Sub-Map " + MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps.Count;
                            _newSubMap.uid="submap" + MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps.Count;
                            _newSubMap._bottomLeft = MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._bottomLeft;
                            _newSubMap._topRight= MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._topRight;
                            _newSubMap._maps = new List<MapSetting>();
                            _newSubMap._fold = false;
                            MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps.Add(_newSubMap);
                        }
                        ResetColor();
                        GUILayout.EndHorizontal();

                        for (int u = 0; u < MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps.Count; u++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(80);
                            SubTitleButton("(UID: "+ MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u].uid+ ") "+
                                MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._areaTitle, ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._fold);
                            GUILayout.EndHorizontal();

                            if (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._fold) {
                                TextField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u].uid, "(Sub-Map) UID:", 100);
                                TextField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._areaTitle, "(Sub-Map) Display Title:", 100);

                                GUILayout.BeginHorizontal();
                                GUILayout.Space(100);
                                GUI.backgroundColor = MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.Count==0?Color.red: _titleColor;
                                if (GUILayout.Button("(Sub-Map) Add New Map Layer", GUILayout.Width(200)))
                                {
                                    MapSetting _map = new MapSetting();
                                    _map._name = MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.Count == 0 ? "Base" : ("Layer" + (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.Count + 1).ToString());
                                    _map._fold = false;
                                    _map._map = SearchMap(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._sceneName, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.Count,"_sub"+u.ToString());
                                    MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.Add(_map);
                                    UpdateGenertorTool();
                                }
                                ResetColor();
                                GUILayout.Space(40);
                                GUILayout.EndHorizontal();

                                if (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.Count == 0)
                                {
                                    Warnning("You must have at least one map layer, please click above button to add one.",100);
                                }

                                for (int y = 0; y < MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.Count; y++)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(100);
                                    SubTitleButton("(Sub-Map) Map Layer " + y.ToString() + " (" + MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._name + ")",
                                        ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._fold);
                                    GUI.backgroundColor = Color.red;
                                    if (GUILayout.Button("X", GUILayout.Width(20)))
                                    {
                                        MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps.RemoveAt(y);
                                        GUILayout.EndHorizontal();
                                        UpdateGenertorTool();
                                        return;
                                    }
                                    ResetColor();
                                    GUILayout.Space(40);
                                    GUILayout.EndHorizontal();

                                    if (MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._fold)
                                    {
                                        TextField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._name, "Name:", 120);
                                        TextureField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._map, "Map Texture:", 120);
                                        FloatSlider(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._height, -50F, 500F, "Layer Height:", "m", 120);
                                        FloatSlider(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._ceil, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._maps[y]._height+2F, 500F, "Layer Ceil:", "m", 120);
                                    }
                                }

                                bool _outrange= Vector3FieldLimited(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._bottomLeft, "(Sub-Map) Bottom Left Corner Position:", 100,
                                  new Vector2(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._bottomLeft.x, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._topRight.x),
                                  new Vector2(-50,500),
                                  new Vector2(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._bottomLeft.z, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._topRight.z));
                                if (_outrange) {
                                    Warnning("The bottom left corner can not be outside the main map rect.", 100);
                                }
                                _outrange = Vector3FieldLimited(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._topRight, "(Sub-Map) Top Right Corner Position:", 100,
                                  new Vector2(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._bottomLeft.x, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._topRight.x),
                                  new Vector2(-50, 500),
                                  new Vector2(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._subMaps[u]._bottomLeft.z, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._topRight.z));
                                if (_outrange)
                                {
                                    Warnning("The top right corner can not be outside the main map rect.", 100);
                                }
                            }
                        }
                    }

                    ColorField(ref MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._backgroundColor, "Map Background:", 60);

                }
            }
        }


        private void Warnning(string _text, int _space)
        {
            GUI.backgroundColor = Color.red;
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            EditorGUILayout.HelpBox(_text, MessageType.Warning, true);
            GUILayout.Space(60);
            GUILayout.EndHorizontal();
            ResetColor();
        }

        private Texture2D SearchMap(string _sceneName, int _layer, string _extra="")
        {
            string _path = _packRootPath + "MapTextures/" + _sceneName + _extra + "_" +(_layer+1).ToString()+ ".png";
            return (Texture2D)AssetDatabase.LoadAssetAtPath(_path, typeof(Texture2D));
        }

        private void Vector3Field(ref Vector3 _value,string _name, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name);
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(_space+20);
            GUILayout.Label("x:", GUILayout.Width(20));
            _value.x = EditorGUILayout.FloatField(_value.x, GUILayout.Width(50));
            GUILayout.Label("y:", GUILayout.Width(20));
            _value.y = EditorGUILayout.FloatField(_value.y, GUILayout.Width(50));
            GUILayout.Label("z:", GUILayout.Width(20));
            _value.z = EditorGUILayout.FloatField(_value.z, GUILayout.Width(50));
            GUILayout.Space(10);
            GUI.backgroundColor = Selection.activeTransform!=null?_titleColor:Color.gray;
            if (GUILayout.Button("Copy from selected Transform"))
            {
                _value = Selection.activeTransform.position;
            }
            ResetColor();
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();

        }

        private bool Vector3FieldLimited(ref Vector3 _value, string _name, int _space,Vector2 _xRange, Vector2 _yRange, Vector2 _zRange)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name);
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(_space + 20);
            GUI.backgroundColor = (_value.x < _xRange.x || _value.x > _xRange.y) ? Color.red : Color.white;
            GUILayout.Label("x:", GUILayout.Width(20));
            _value.x = EditorGUILayout.FloatField(_value.x, GUILayout.Width(50));
            GUI.backgroundColor = (_value.y < _yRange.x || _value.y > _yRange.y) ? Color.red : Color.white;
            GUILayout.Label("y:", GUILayout.Width(20));
            _value.y = EditorGUILayout.FloatField(_value.y, GUILayout.Width(50));
            GUI.backgroundColor = (_value.z < _zRange.x || _value.z > _zRange.y) ? Color.red : Color.white;
            GUILayout.Label("z:", GUILayout.Width(20));
            _value.z = EditorGUILayout.FloatField(_value.z, GUILayout.Width(50));
            GUILayout.Space(10);
            GUI.backgroundColor = Selection.activeTransform != null ? _titleColor : Color.gray;
            if (GUILayout.Button("Copy from selected Transform"))
            {
                _value = Selection.activeTransform.position;
            }
            ResetColor();
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
            return (_value.x < _xRange.x || _value.x > _xRange.y || _value.y < _yRange.x || _value.y > _yRange.y || _value.z < _zRange.x || _value.z > _zRange.y);
        }

        private void ColorField(ref Color _value, string _name, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name, GUILayout.Width(150));
            _value = EditorGUILayout.ColorField(_value, GUILayout.Width(150));
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }

        private void FloatSlider(ref float _value,float _min,float _max,  string _name, string _unit, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name, GUILayout.Width(150));
            _value = GUILayout.HorizontalSlider(_value, _min,_max);
            GUILayout.Label(_value.ToString("0.0")+ _unit, GUILayout.Width(60));
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }

        private void IntSlider(ref int _value, int _min, int _max, string _name, string _unit, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name, GUILayout.Width(150));
            _value = EditorGUILayout.IntSlider(_value, _min, _max);
            GUILayout.Label(_unit, GUILayout.Width(50));
            GUILayout.Space(Mathf.Max(0, _space-40));
            GUILayout.EndHorizontal();
        }
        private void TextureField(ref Texture2D _object,  string _name, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name, GUILayout.Width(150));
            _object = (Texture2D)EditorGUILayout.ObjectField(_object,typeof(Texture2D),false);
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }

        private void Toggle(ref bool _value, string _name, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name, GUILayout.Width(200));
            _value = GUILayout.Toggle(_value,"",GUILayout.Width(35));
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }

        private void TextField (ref string _text,string _name, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name,GUILayout.Width(150));
            _text = GUILayout.TextArea(_text);
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }
        private void Label(string _text,int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_text);
            GUILayout.EndHorizontal();
        }

        private void KeyButton(ref KeyCode _value,string _name,int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name, GUILayout.Width(150));
            _value = (KeyCode)EditorGUILayout.EnumPopup(_value, GUILayout.Width(150));
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }

        private void MouseKeyButton(ref KeyCode _keyValue, ref MouseButton _mouseValue, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            _keyValue = (KeyCode)EditorGUILayout.EnumPopup(_keyValue, GUILayout.Width(140));
            GUILayout.Label("+",GUILayout.Width(20));
            _mouseValue = (MouseButton)EditorGUILayout.EnumPopup(_mouseValue, GUILayout.Width(140));
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }

        private void MouseButton(ref MouseButton _value, string _name, int _space)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(_space);
            GUILayout.Label(_name, GUILayout.Width(150));
            _value = (MouseButton)EditorGUILayout.EnumPopup(_value, GUILayout.Width(150));
            GUILayout.Space(_space);
            GUILayout.EndHorizontal();
        }

        private void UpdateGenertorTool()
        {
            MapCreator [] _creator = FindObjectsByType<MapCreator>(FindObjectsSortMode.None);
            foreach (var obj in _creator)
            {
                if (obj != null) obj.Init();
            }
        }

        private void AddScene(string _sceneName, string _scenePath)
        {
            Dictionary<string, SceneMapSetting> _sceneMapDic = new Dictionary<string, SceneMapSetting>();
            for (int i = 0; i < MMN_SettingsProvider.CurrentSettings.SceneMaps.Count; i++)
            {
                _sceneMapDic.Add(MMN_SettingsProvider.CurrentSettings.SceneMaps[i]._sceneName, MMN_SettingsProvider.CurrentSettings.SceneMaps[i]);
            }
            if (_sceneMapDic.ContainsKey(_sceneName))
            {
                _sceneMapDic[_sceneName]._scenePath = _scenePath;
            }
            else
            {
                SceneMapSetting _newSetting = new SceneMapSetting();
                _newSetting._scenePath = _scenePath;
                _newSetting._sceneName = _sceneName;
                _newSetting._sceneTitle = _sceneName;
                _newSetting._maps = new List<MapSetting>();
                _newSetting._bottomLeft = new Vector3(-500F,-10F,-500F);
                _newSetting._topRight = new Vector3(500F, -10F, 500F);
                MapSetting _map = new MapSetting();
                _map._name = "Base";
                _map._fold = false;
                _map._map = SearchMap(_sceneName,0);
                _newSetting._maps.Add(_map);
                _newSetting._fold = false;
                MMN_SettingsProvider.CurrentSettings.SceneMaps.Add(_newSetting);
            }
        }

        private void TitleButton(string _text,ref bool _toggle)
        {
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = _toggle ? _activeColor : _disableColor;
            GUILayout.Label(_toggle?"[-]":"[+]",GUILayout.Width(20));
            if (GUILayout.Button(_text, _titleButtonStyle))
            {
                _toggle = !_toggle;
            }
            ResetColor();
            GUILayout.Space(40);
            GUILayout.EndHorizontal();
        }

        private void SubTitleButton(string _text, ref bool _toggle)
        {
            _toggle = EditorGUILayout.Foldout(_toggle, _text,true);
        }

        private void ResetColor()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
        }

    }
}
