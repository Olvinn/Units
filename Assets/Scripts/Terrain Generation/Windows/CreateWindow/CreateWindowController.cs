using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.CreateWindow
{
    public class CreateWindowController
    {
        public event Action<Texture2D, TerrainGenerationModel> onCreateHeightmap; 
        
        private CreateWindowView _view;
        private TerrainGenerationModel _model;
        
        public CreateWindowController(VisualElement root)
        {
            var assets = AssetDatabase.FindAssets("t: TerrainGenerationModel");
            _model = AssetDatabase.LoadAssetAtPath<TerrainGenerationModel>(AssetDatabase.GUIDToAssetPath(assets[0]));
            
            _view = new CreateWindowView(root, _model);

            _view.onCreate += OnCreateHeightmap;
            _view.onSaveSettings += OnSaveSettings;
        }

        private void OnSaveSettings(CreationData data, TerrainGenerationModel model)
        {
            _model = model;
            if (!_model) return;
            _model.HeightmapResolution = data.Resolution;
            _model.Seed = data.Seed;
            _model.Octaves = data.PerlinOctaves;
        }

        private void OnCreateHeightmap(CreationData data)
        {
            Texture2D result;
            if (_model)
            {
                result = TerrainGenerator.GenerateNoise(_model.NoiseShader, data.Resolution.x, data.Resolution.y, 
                    data.PerlinOctaves, data.Seed);
            }
            else
            {
                result = TerrainGenerator.GenerateNoise(data.Resolution.x, data.Resolution.y, 
                    data.PerlinOctaves, data.Seed);
            }
            onCreateHeightmap?.Invoke(result, _model);
        }

        public void Dispose()
        {
            _view.onCreate -= OnCreateHeightmap;
        }
    }
}