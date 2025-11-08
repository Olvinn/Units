using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.CreateWindow
{
    public class CreateWindowController : IWindow
    {
        public event Action<Texture2D> onCreateHeightmap; 
        
        private CreateWindowView _view;
        private TerrainGenerationModel _model;
        
        public CreateWindowController(VisualElement root, TerrainGenerationModel model)
        {
            _model = model;
            _view = new CreateWindowView(root, _model);

            _view.onCreate += OnCreateHeightmap;
            _view.onSaveSettings += OnSaveSettings;
        }

        private void OnSaveSettings(CreationData data)
        {
            if (!_model) return;
            _model.HeightmapResolution = data.Resolution;
            _model.Seed = data.Seed;
            _model.Octaves = data.PerlinOctaves;
        }

        private void OnCreateHeightmap(CreationData data)
        {
            Texture2D result;
            result = TerrainGenerator.GenerateNoise(_model.NoiseShader, data.Resolution.x, data.Resolution.y, 
                data.PerlinOctaves, data.Seed);
            onCreateHeightmap?.Invoke(result);
        }

        public void Dispose()
        {
            _view.onCreate -= OnCreateHeightmap;
        }
    }
}