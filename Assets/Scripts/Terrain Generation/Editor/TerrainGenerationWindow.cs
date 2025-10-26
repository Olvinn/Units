using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Editor
{
    public class TerrainGenerationWindow : EditorWindow
    {
        private TerrainGenerationData _data;

        private ObjectField _dataField;
        private Button _applyButton;
        private IntegerField _octaves;
        private FloatField _seed;
        private Image _preview;
        
        private Texture2D _heightmap;
        
        [MenuItem("Generation/Terrain Generator")]
        public static void ShowExample()
        {
            var wnd = GetWindow<TerrainGenerationWindow>();
            wnd.titleContent = new GUIContent("Terrain Generator");
        }
        
        private void CreateGUI()
        {
            var root = rootVisualElement;
            
            _dataField = new ObjectField("Terrain Generation Data");
            _dataField.objectType = typeof(TerrainGenerationData);
            root.Add(_dataField);

            var assets = AssetDatabase.FindAssets("t: TerrainGenerationData");
            _data = AssetDatabase.LoadAssetAtPath<TerrainGenerationData>(AssetDatabase.GUIDToAssetPath(assets[0]));
            _dataField.value = _data;

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

            _seed = new FloatField("Seed");
            _seed.value = _data.Seed;
            root.Add(_seed);

            _octaves = new IntegerField("Octaves");
            _octaves.value = _data.Octaves;
            root.Add(_octaves);
            
            _applyButton = new Button(ApplyHeightmapToTerrain)
            {
                text = "Apply on Selection"
            };
            
            root.Add(_applyButton);
        }

        private void Update()
        {
            var go = Selection.activeObject as GameObject;
            Terrain terrain = go?.GetComponent<Terrain>();
            _applyButton.SetEnabled(terrain != null);
            GenerateHeightmap();
        }

        private void GenerateHeightmap()
        {
            _heightmap = TerrainGenerator.GenerateNoise(octaves: _octaves.value, seed: _seed.value);
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
