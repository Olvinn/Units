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
            baseWindow.onOpenLoadWindow += OpwnLoadWindow;
            baseWindow.onOpenReadWindow += OpenReadWindow;
            _currentWindow = baseWindow;
            _backButton.SetEnabled(false);
        }

        private void OpenReadWindow()
        {
        }

        private void OpwnLoadWindow()
        {
            string path = EditorUtility.OpenFilePanel("Select Heightmap", Application.dataPath, "png");
            if (path.Length != 0)
            {
                byte[] fileData = File.ReadAllBytes(path);

                _heightmap = new Texture2D(2, 2);
                _heightmap.LoadImage(fileData);
                OpenChangeWindow();
            }
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
