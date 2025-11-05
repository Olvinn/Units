using System;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.BaseWindow
{
    public class BaseWindowController : IDisposable
    {
        public event Action onOpenCreateWindow, onOpenLoadWindow, onOpenReadWindow;
        
        private BaseWindowVew _view;
        
        public BaseWindowController(VisualElement root)
        {
            _view = new BaseWindowVew(root);

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
    }
}
