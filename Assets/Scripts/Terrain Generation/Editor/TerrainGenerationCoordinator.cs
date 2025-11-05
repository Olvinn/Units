using Terrain_Generation.Windows.BaseWindow;
using Terrain_Generation.Windows.ChangeWindow;
using Terrain_Generation.Windows.CreateWindow;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Editor
{
    public class TerrainGenerationCoordinator : EditorWindow
    {
        private TerrainGenerationModel _model;

        private ObjectField _dataField;
        private Button _generateButton, _applyButton, _erosionButton;
        private IntegerField _octaves;
        private FloatField _seed;
        private Image _preview, _erosion;
        
        private Texture2D _heightmap;
        
        [MenuItem("Generation/Terrain Generator")]
        public static void ShowExample()
        {
            var wnd = GetWindow<TerrainGenerationCoordinator>();
            wnd.titleContent = new GUIContent("Terrain Generator");
        }
        
        private void CreateGUI()
        {
            var root = rootVisualElement;

            OpenBaseWindow();
            
            return;
            
            _dataField = new ObjectField("Terrain Generation Data")
            {
                objectType = typeof(TerrainGenerationModel),
                value = _model
            };
            root.Add(_dataField);

            _heightmap = TerrainGenerator.GenerateNoise();

            _preview = new Image
            {
                image = _heightmap,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    borderBottomWidth = 2,
                    marginTop = 10
                }
            };
            var flex = new StyleFloat
            {
                value = 1f
            };
            _preview.style.flexGrow = flex;
            root.Add(_preview);

            _seed = new FloatField("Seed")
            {
                value = _model.Seed
            };
            root.Add(_seed);

            _octaves = new IntegerField("Octaves")
            {
                value = _model.Octaves
            };
            root.Add(_octaves);
            
            _generateButton = new Button(GenerateHeightmap)
            {
                text = "Generate"
            };
            root.Add(_generateButton);
            
            _erosionButton = new Button(ApplyErosion)
            {
                text = "Apply Erosion"
            };
            root.Add(_erosionButton);
            
            _applyButton = new Button(ApplyHeightmapToTerrain)
            {
                text = "Apply on Selection"
            };
            root.Add(_applyButton);
        }

        private void OpenBaseWindow()
        {
            BaseWindowController baseWindow = new BaseWindowController(rootVisualElement);
            baseWindow.onOpenCreateWindow += () =>
            {
                OpenCreateWindow();
                baseWindow.Dispose();
            };
            baseWindow.onOpenLoadWindow += () =>
            {
                OpwnLoadWindow();
                baseWindow.Dispose();
            };
            baseWindow.onOpenReadWindow += () =>
            {
                OpenReadWindow();
                baseWindow.Dispose();
            };
        }

        private void OpenReadWindow()
        {
            throw new System.NotImplementedException();
        }

        private void OpwnLoadWindow()
        {
            throw new System.NotImplementedException();
        }

        private void OpenCreateWindow()
        {
            CreateWindowController createWindow = new CreateWindowController(rootVisualElement);
            createWindow.onCreateHeightmap += (tex, m) =>
            {
                OpenChangeWindow(tex, m);
                createWindow.Dispose();
            };
        }

        private void OpenChangeWindow(Texture2D heightmap, TerrainGenerationModel model)
        {
            ChangeWindowController changeWindow = new ChangeWindowController(rootVisualElement, heightmap, model);
        }

        private void Update()
        {
            return;
            var go = Selection.activeObject as GameObject;
            Terrain terrain = go?.GetComponent<Terrain>();
            _applyButton.SetEnabled(terrain != null);
            //GenerateHeightmap();
            ApplyInputToData();
        }

        private void ApplyErosion()
        {
            for(int i = 0; i < 20; i++)
            {
                _heightmap = TerrainGenerator.ApplyWaterErosion(_heightmap, _model.WaterErosion);
                _preview.image = _heightmap;
            }
        }

        private void ApplyInputToData()
        {
            _model.Seed = _seed.value;
            _model.Octaves = _octaves.value;
        }

        private void GenerateHeightmap()
        {
            _heightmap = TerrainGenerator.GenerateNoise(width: _model.HeightmapResolution.x, height: _model.HeightmapResolution.y, 
                octaves: _octaves.value, seed: _seed.value, shader: _model.NoiseShader);
            _preview.image = _heightmap;
        }

        private void ApplyHeightmapToTerrain()
        {
            var go = UnityEditor.Selection.activeObject as GameObject;
            Terrain terrain = go?.GetComponent<Terrain>();
            ApplyHeightsToTerrain(terrain, TextureToHeights(_heightmap));
        }

        private float[,] TextureToHeights(Texture2D tex)
        {
            int width = tex.width;
            int height = tex.height;
            float[,] heights = new float[height, width];

            Color[] pixels = tex.GetPixels();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = pixels[y * width + x].r;
                    heights[y, x] = Mathf.Clamp01(v);
                }
            }
            return heights;
        }
        
        private void ApplyHeightsToTerrain(Terrain terrain, float[,] heights)
        {
            TerrainData data = terrain.terrainData;

            int w = heights.GetLength(1);
            int h = heights.GetLength(0);
            
            if (w != data.heightmapResolution || h != data.heightmapResolution)
            {
                data.heightmapResolution = Mathf.Max(w, h);
            }

            data.SetHeights(0, 0, heights);
        }
    }
}
