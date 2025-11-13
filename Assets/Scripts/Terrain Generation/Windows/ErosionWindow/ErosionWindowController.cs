using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.ErosionWindow
{
    public class ErosionWindowController : ITerrainGeneratorWindow
    {
        public event Action<Texture2D> onChangeHeightmap;
        
        private Texture2D _heightmap, _erosion;
        private TerrainGenerationModel _model;
        private ErosionWindowView _view;
        
        public ErosionWindowController(VisualElement root, Texture2D heightmap, TerrainGenerationModel model)
        {
            _heightmap = heightmap;
            _model = model;

            _erosion = TerrainGenerator.CreateBlankTexture(heightmap.width, heightmap.height);
            
            _view = new ErosionWindowView(root, heightmap, _erosion);

            _view.onCalculateErosion += CalculateErosion;
            _view.onApplyErosion += ApplyErosionToHeightmap;
        }

        private void ApplyErosionToHeightmap()
        {
            _heightmap = TerrainGenerator.ApplyMaskToHeightmap(_heightmap, _erosion, _model.WaterErosion);
            _erosion = TerrainGenerator.CreateBlankTexture(_heightmap.width, _heightmap.height);
            _view.UpdatePreview(_heightmap, _erosion);
            onChangeHeightmap?.Invoke(_heightmap);
        }

        private void CalculateErosion()
        {
            for (int i = 0; i < 20; i++)
            {
                _erosion = TerrainGenerator.CalculateWaterErosion(_heightmap, _model.WaterErosion);
            }
            _view.UpdatePreview(_heightmap, TerrainGenerator.SaturateDeltas(_erosion, _model.WaterErosion));
        }
        
        public void Dispose()
        {
            _view.onCalculateErosion -= CalculateErosion;
            _view.onApplyErosion -= ApplyErosionToHeightmap;
            onChangeHeightmap = null;
        }

        public void Update(TerrainData terrainData)
        {
            
        }
    }
}