using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Editor
{
    public class TerrainGenerationWindow : EditorWindow
    {
        [MenuItem("Tools/Terrain Generator")]
        public static void ShowExample()
        {
            var wnd = GetWindow<TerrainGenerationWindow>();
            wnd.titleContent = new GUIContent("Terrain Generator");
        }
        
        private void CreateGUI()
        {
            var root = rootVisualElement;

            Texture2D texture = TerrainGenerator.GenerateNoise();

            var image = new Image
            {
                image = texture,
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
            image.style.flexGrow = flex;
            root.Add(image);

            var octaves = new IntegerField("Octaves");
            octaves.value = 4;
            root.Add(octaves);

            var generateButton = new Button(() =>
            {
                image.image = TerrainGenerator.GenerateNoise(octaves : octaves.value);
            })
            {
                text = "Generate"
            };
            root.Add(generateButton);

            if (UnityEditor.Selection.activeObject is GameObject gameObject)
            {
                Terrain terrain = gameObject.GetComponent<Terrain>();
                if (terrain == null) return;
                
                var applyButton = new Button(() =>
                {
                    var tex = TerrainGenerator.GenerateNoise(octaves: octaves.value);
                    ApplyHeightsToTerrain(terrain, TextureToHeights(tex));
                })
                {
                    text = "Apply on Selection"
                };
                root.Add(applyButton);
            }
        }
        
        float[,] TextureToHeights(Texture2D tex)
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
        
        void ApplyHeightsToTerrain(Terrain terrain, float[,] heights)
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
