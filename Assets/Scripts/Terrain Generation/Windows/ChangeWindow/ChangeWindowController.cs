using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.ChangeWindow
{
    public class ChangeTerrainGeneratorWindowController : ITerrainGeneratorWindow
    {
        public event Action<Texture2D> onAppliedToTerrainData;
        public event Action onOpenErosion;
        
        private ChangeWindowView _view;
        
        private Texture2D _heightmap;
        private TerrainGenerationModel _model;
        private TerrainData _terrainData;
        
        public ChangeTerrainGeneratorWindowController(VisualElement root, Texture2D heightmap, TerrainGenerationModel model)
        {
            _heightmap = heightmap;
            _model = model;
            
            _view = new ChangeWindowView(root, heightmap);

            _view.onErode += OpenErosionWindow;
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
            onAppliedToTerrainData?.Invoke(_heightmap);
        }

        private void OpenErosionWindow()
        {
            onOpenErosion?.Invoke();
        }

        public void Dispose()
        {
            _view.onErode -= OpenErosionWindow;
            _view.onApply -= ApplyHeightmapToTerrain;
            _view.onSave -= SaveHeightmapToExr;
            
            onAppliedToTerrainData = null;
        }

        public void Update(TerrainData terrainData)
        {
            _terrainData = terrainData;
            _view.SetWorkWithTerrainActive(terrainData != null);
        }
    }
}