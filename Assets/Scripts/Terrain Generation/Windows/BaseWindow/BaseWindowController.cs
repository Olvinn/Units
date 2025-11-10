using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.BaseWindow
{
    public class BaseTerrainGeneratorWindowController : ITerrainGeneratorWindow
    {
        public event Action onOpenCreateWindow, onOpenLoadWindow, onOpenReadWindow;
        
        private TerrainData _terrainData;
        private BaseWindowVew _view;
        
        public BaseTerrainGeneratorWindowController(VisualElement root, TerrainData terrainData)
        {
            _terrainData = terrainData;
            
            _view = new BaseWindowVew(root);
            _view.SetWorkWithTerrainActive(false);

            _view.onCreate += OpenCreateWindow;
            _view.onLoad += OpenLoadWindow;
            _view.onRead += OpenReadWindow;
        }

        private void OpenCreateWindow()
        {
            onOpenCreateWindow?.Invoke();
        }

        private void OpenLoadWindow()
        {
            onOpenLoadWindow?.Invoke();
        }

        private void OpenReadWindow()
        {
            onOpenReadWindow?.Invoke();
        }

        public void Dispose()
        {
            _view.onCreate -= OpenCreateWindow;
            _view.onLoad -= OpenLoadWindow;
            _view.onRead -= OpenReadWindow;
        }

        public void Update(TerrainData terrainData)
        {
            _view.SetWorkWithTerrainActive(terrainData != null);
        }
    }
}
