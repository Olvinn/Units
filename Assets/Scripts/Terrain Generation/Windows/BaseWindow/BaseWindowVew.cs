using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.BaseWindow
{
    public class BaseWindowVew
    {
        public event Action onCreate, onLoad, onRead;
        
        private VisualElement _root;
        private Button _createButton, _loadButton, _readButton;
        
        public BaseWindowVew(VisualElement root)
        {
            _root = root;
            _root.contentContainer.Clear();

            _createButton = new Button(Create)
            {
                text = "Create Heightmap",
            };
            _root.Add(_createButton);

            _loadButton = new Button(Load)
            {
                text = "Load Heightmap",
            };
            _root.Add(_loadButton);

            _readButton = new Button(Read)
            {
                text = "Read Heightmap from Terrain Data",
            };
            _root.Add(_readButton);
        }

        public void SetWorkWithTerrainActive(bool active)
        {
            _readButton.SetEnabled(active);
        }

        private void Read()
        {
            onRead?.Invoke();
        }

        private void Load()
        {
            onLoad?.Invoke();
        }

        private void Create()
        {
            onCreate?.Invoke();
        }
    }
}
