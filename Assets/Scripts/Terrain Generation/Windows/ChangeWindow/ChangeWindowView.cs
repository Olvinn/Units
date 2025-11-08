using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.ChangeWindow
{
    public class ChangeWindowView
    {
        public event Action onErode, onApply, onSave;
        
        private VisualElement _root;
        
        private Image _preview;
        
        public ChangeWindowView(VisualElement root, Texture2D heightmap)
        {
            _root = root;
            _root.contentContainer.Clear();
            
            VisualElement previewBox = new VisualElement()
            {
                style =
                {
                    borderBottomColor = Color.black,
                    borderLeftColor = Color.black,
                    borderRightColor = Color.black,
                    borderTopColor = Color.black,

                    borderBottomWidth = 1,
                    borderLeftWidth = 1,
                    borderRightWidth = 1,
                    borderTopWidth = 1,
                    
                    marginBottom = 5,
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop = 5,
                    
                    paddingLeft = 5,
                    paddingRight = 5,
                    paddingTop = 5,
                    paddingBottom = 5,
                }
            };
            _root.Add(previewBox);

            Label settingsLabel = new Label("Preview");
            previewBox.Add(settingsLabel);
            
            _preview = new Image
            {
                image = heightmap,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    borderBottomWidth = 2,
                    marginTop = 10
                }
            };
            previewBox.Add(_preview);
            
            Button erosionButton = new Button(OnErode)
            {
                text = "Apply Erosion"
            };
            root.Add(erosionButton);
            
            Button applyButton = new Button(OnApply)
            {
                text = "Apply To Terrain"
            };
            root.Add(applyButton);
            
            Button saveButton = new Button(OnSave)
            {
                text = "Save Heightmap"
            };
            root.Add(saveButton);
        }

        private void OnSave()
        {
            onSave?.Invoke();
        }

        private void OnApply()
        {
            onApply?.Invoke();
        }

        public void UpdatePreview(Texture2D preview)
        {
            _preview.image = preview;
        }

        private void OnErode()
        {
            onErode?.Invoke();
        }
    }
}