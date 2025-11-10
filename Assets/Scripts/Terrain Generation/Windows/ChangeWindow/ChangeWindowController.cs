using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.ChangeWindow
{
    public class ChangeTerrainGeneratorWindowController : ITerrainGeneratorWindow
    {
        private ChangeWindowView _view;
        
        private Texture2D _heightmap;
        private TerrainGenerationModel _model;
        
        public ChangeTerrainGeneratorWindowController(VisualElement root, Texture2D heightmap, TerrainGenerationModel model)
        {
            _heightmap = heightmap;
            _model = model;
            
            _view = new ChangeWindowView(root, heightmap);

            _view.onErode += ErodeTexture;
            _view.onApply += ApplyHeightmapToTerrain;
            _view.onSave += SaveHeightmapToExr;
        }

        private void SaveHeightmapToExr()
        {
            string path = EditorUtility.SaveFilePanel("Save Heightmap", "", "Heightmap", "exr");
            if (string.IsNullOrEmpty(path))
                return;

            byte[] bytes = _heightmap.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);

            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
        }

        private void ApplyHeightmapToTerrain()
        {
            var go = Selection.activeObject as GameObject;
            Terrain terrain = go?.GetComponent<Terrain>();
            if (terrain)
                ApplyHeightsToTerrain(terrain, TextureToHeights(_heightmap));
        }

        private void ErodeTexture()
        {
            for (int i = 0; i < 20; i++)
            {
                _heightmap = TerrainGenerator.ApplyWaterErosion(_heightmap, _model.WaterErosion);
            }
            _view.UpdatePreview(_heightmap);
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

        public void Dispose()
        {
            _view.onErode -= ErodeTexture;
            _view.onApply -= ApplyHeightmapToTerrain;
            _view.onSave -= SaveHeightmapToExr;
        }

        public void Update(TerrainData terrainData)
        {
            _view.SetWorkWithTerrainActive(terrainData != null);
        }
    }
}