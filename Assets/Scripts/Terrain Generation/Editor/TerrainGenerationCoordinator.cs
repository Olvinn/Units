using System.IO;
using Terrain_Generation.Windows;
using Terrain_Generation.Windows.BaseWindow;
using Terrain_Generation.Windows.ChangeWindow;
using Terrain_Generation.Windows.CreateWindow;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Editor
{
    public class TerrainGenerationCoordinator : EditorWindow
    {
        private TerrainGenerationModel _model;
        private Texture2D _heightmap;
        private IWindow _currentWindow;
        
        private VisualElement _windowsRoot;
        private Button _backButton;
        private ObjectField _dataField;
        
        [MenuItem("Generation/Terrain Generator")]
        public static void ShowExample()
        {
            var wnd = GetWindow<TerrainGenerationCoordinator>();
            wnd.titleContent = new GUIContent("Terrain Generator");
        }
        
        private void CreateGUI()
        {
            var assets = AssetDatabase.FindAssets("t: TerrainGenerationModel");
            _model = AssetDatabase.LoadAssetAtPath<TerrainGenerationModel>(AssetDatabase.GUIDToAssetPath(assets[0]));
            
            _windowsRoot = new VisualElement()
            {
                style =
                {
                    borderBottomColor = Color.black,
                    borderLeftColor = Color.black,
                    borderRightColor = Color.black,
                    borderTopColor = Color.black,

                    borderBottomWidth = 1,
                    borderLeftWidth = 1,
                    borderRightWidth = 1,
                    borderTopWidth = 1,
                    
                    marginBottom = 5,
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop = 5,
                    
                    paddingLeft = 5,
                    paddingRight = 5,
                    paddingTop = 5,
                    paddingBottom = 5,
                }
            };
            
            _dataField = new ObjectField("Model")
            {
                objectType = typeof(TerrainGenerationModel),
                value = _model
            };
            rootVisualElement.Add(_dataField);

            _backButton = new Button(OpenBaseWindow)
            {
                text = "Back",
            };
            rootVisualElement.Add(_backButton);
            
            rootVisualElement.Add(_windowsRoot);
            
            OpenBaseWindow();
        }

        private void OpenBaseWindow()
        {
            BaseWindowController baseWindow = new BaseWindowController(_windowsRoot);
            baseWindow.onOpenCreateWindow += OpenCreateWindow;
            baseWindow.onOpenLoadWindow += OpenLoadWindow;
            baseWindow.onOpenReadWindow += OpenReadWindow;
            _currentWindow = baseWindow;
            _backButton.SetEnabled(false);
        }

        private void OpenReadWindow()
        {
            var go = Selection.activeObject as GameObject;
            Terrain terrain = go?.GetComponent<Terrain>();
            if (terrain)
                LoadHeightsFromTerrain(terrain);
            OpenChangeWindow();
        }
        
        private void LoadHeightsFromTerrain(Terrain terrain)
        {
            TerrainData data = terrain.terrainData;

            var heights = data.GetHeights(0,0, data.heightmapResolution, data.heightmapResolution);
            
            _heightmap = TerrainGenerator.CreateBlankTexture(data.heightmapResolution, data.heightmapResolution);

            for (int y = 0; y < data.heightmapResolution; y++)
            {
                for (int x = 0; x < data.heightmapResolution; x++)
                {
                    _heightmap.SetPixel(x, y, new Color(heights[y,x], 0, 0));
                }
            }
            
            _heightmap.Apply();
        }

        private void OpenLoadWindow()
        {
            string path = EditorUtility.OpenFilePanel("Load Heightmap", "", "exr");
            if (string.IsNullOrEmpty(path))
                return;

            byte[] fileData = File.ReadAllBytes(path);

            _heightmap = TerrainGenerator.CreateBlankTexture();
            _heightmap.LoadImage(fileData); 

            OpenChangeWindow(); 
            Debug.Log($"Loaded heightmap from {path}");
        }

        private void OpenCreateWindow()
        {
            CreateWindowController createWindow = new CreateWindowController(_windowsRoot, _model);
            createWindow.onCreateHeightmap += OnHeightmapCreated;
            _currentWindow = createWindow;
            _backButton.SetEnabled(true);
        }

        private void OpenChangeWindow()
        {
            if (_heightmap == null) return;
            _currentWindow.Dispose();
            _currentWindow = new ChangeWindowController(_windowsRoot, _heightmap, _model);
            _backButton.SetEnabled(true);
        }
        
        private void OnHeightmapCreated(Texture2D heightmap)
        {
            _heightmap = heightmap;
            OpenChangeWindow();
        }
    }
}
