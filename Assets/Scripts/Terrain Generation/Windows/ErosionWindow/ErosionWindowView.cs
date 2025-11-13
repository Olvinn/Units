using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.ErosionWindow
{
    public class ErosionWindowView
    {
        public event Action onCalculateErosion, onApplyErosion;
        
        private Image _heightmapImage, _erosionImage;
        
        public ErosionWindowView(VisualElement root, Texture2D heightmap, Texture2D erosion)
        {
            root.contentContainer.Clear();
            
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
            root.Add(previewBox);

            var previewRow = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row },
            };
            previewBox.Add(previewRow);

            _heightmapImage = new Image
            {
                image = heightmap,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    borderBottomWidth = 2,
                    marginTop = 10
                }
            };
            previewRow.Add(_heightmapImage);

            _erosionImage = new Image
            {
                image = erosion,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    borderBottomWidth = 2,
                    marginTop = 10
                }
            };
            previewRow.Add(_erosionImage);

            var calculateButton = new Button(OnErode)
            {
                text = "Calculate Water Erosion"
            };
            root.Add(calculateButton);

            var applyButton = new Button(OnApplyErosion)
            {
                text = "Apply Erosion to Heightmap"
            };
            root.Add(applyButton);
        }

        public void UpdatePreview(Texture2D heightmap, Texture2D erosion)
        {
            _heightmapImage.image = heightmap;
            _erosionImage.image = erosion;
        }

        private void OnErode()
        {
            onCalculateErosion?.Invoke();
        }

        private void OnApplyErosion()
        {
            onApplyErosion?.Invoke();
        }
    }
}