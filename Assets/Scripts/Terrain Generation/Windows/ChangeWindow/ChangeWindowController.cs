using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.ChangeWindow
{
    public class ChangeTerrainGeneratorWindowController : ITerrainGeneratorWindow
    {
        public event Action onAppliedToTerrainData;
        
        private ChangeWindowView _view;
        
        private Texture2D _heightmap;
        private TerrainGenerationModel _model;
        private TerrainData _terrainData;
        
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
            ApplyHeightsToTerrain(TextureToHeights(_heightmap));
            onAppliedToTerrainData?.Invoke();
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
        
        private void ApplyHeightsToTerrain(float[,] heights)
        {
            int w = heights.GetLength(1);
            int h = heights.GetLength(0);
            
            if (w != _terrainData.heightmapResolution || h != _terrainData.heightmapResolution)
            {
                _terrainData.heightmapResolution = Mathf.Max(w, h);
            }

            _terrainData.SetHeights(0, 0, heights);
        }

        public void Dispose()
        {
            _view.onErode -= ErodeTexture;
            _view.onApply -= ApplyHeightmapToTerrain;
            _view.onSave -= SaveHeightmapToExr;
        }

        public void Update(TerrainData terrainData)
        {
            _terrainData = terrainData;
            _view.SetWorkWithTerrainActive(terrainData != null);
        }
    }
}