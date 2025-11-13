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
        private bool _terrainDirty; 
        
        private TerrainGenerationModel _model;
        private Texture2D _heightmap;
        private ITerrainGeneratorWindow _currentTerrainGeneratorWindow;
        private TerrainData _terrainData;
        
        private VisualElement _windowsRoot;
        private Button _backButton;
        private ObjectField _dataField;
		private Label _infoLabel;
        
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

            VisualElement controlRow = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                }
            };

            _infoLabel = new Label()
            {
                style = { paddingLeft = 3, }
            };
            
			rootVisualElement.Add(_infoLabel);
            rootVisualElement.Add(controlRow);
            rootVisualElement.Add(_windowsRoot);
            
            _dataField = new ObjectField("Model")
            {
                objectType = typeof(TerrainGenerationModel),
                value = _model
            };
            controlRow.Add(_dataField);

            _backButton = new Button(OpenBaseWindow)
            {
                text = "Back",
            };
            controlRow.Add(_backButton);
            
            OpenBaseWindow();
        }

        private void Update()
        {
            var go = Selection.activeObject as GameObject;
            Terrain terrain = go?.GetComponent<UnityEngine.Terrain>();
            SimpleTerrain simpleTerrain = go?.GetComponent<SimpleTerrain>();
            if (terrain)
            {
                _terrainData = terrain.terrainData;
            }
            else if (simpleTerrain)
            {
                _terrainData = simpleTerrain.terrainData;
                if (_terrainDirty)
                    simpleTerrain.Rebuild();
            }
            else if (Selection.activeObject is TerrainData terrainData)
            {
                _terrainData = terrainData;
            }
            else
            {
                _terrainData = null;
            }

            if (_terrainData == null)
                _infoLabel.text = "Warning: No selected Terrain Data. Select Terrain or Terrain Data file";
            else
                _infoLabel.text = $"Terrain Data: {_terrainData.name}";
            
            _currentTerrainGeneratorWindow.Update(_terrainData);
            _terrainDirty = false;
        }

        private void OpenBaseWindow()
        {
            BaseTerrainGeneratorWindowController baseTerrainGeneratorWindow = new BaseTerrainGeneratorWindowController(_windowsRoot, _terrainData);
            baseTerrainGeneratorWindow.onOpenCreateWindow += OpenCreateWindow;
            baseTerrainGeneratorWindow.onOpenLoadWindow += OpenLoadWindow;
            baseTerrainGeneratorWindow.onOpenReadWindow += OpenReadWindow;
            _currentTerrainGeneratorWindow = baseTerrainGeneratorWindow;
            _backButton.SetEnabled(false);
        }

        private void OpenReadWindow()
        {
            LoadHeightsFromTerrain(_terrainData);
            OpenChangeWindow();
        }
        
        private void LoadHeightsFromTerrain(TerrainData data)
        {
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
        }

        private void OpenCreateWindow()
        {
            CreateTerrainGeneratorWindowController createTerrainGeneratorWindow = new CreateTerrainGeneratorWindowController(_windowsRoot, _model);
            createTerrainGeneratorWindow.onCreateHeightmap += OnHeightmapCreated;
            _currentTerrainGeneratorWindow = createTerrainGeneratorWindow;
            _backButton.SetEnabled(true);
        }

        private void OpenChangeWindow()
        {
            if (_heightmap == null) return;
            _currentTerrainGeneratorWindow.Dispose();
            var changeWindow = new ChangeTerrainGeneratorWindowController(_windowsRoot, _heightmap, _model);
            changeWindow.onAppliedToTerrainData += SetTerrainDirty;
            _currentTerrainGeneratorWindow = changeWindow;
            _backButton.SetEnabled(true);
        }

        private void SetTerrainDirty()
        {
            _terrainDirty = true;
        }

        private void OnHeightmapCreated(Texture2D heightmap)
        {
            _heightmap = heightmap;
            OpenChangeWindow();
        }
    }
}
