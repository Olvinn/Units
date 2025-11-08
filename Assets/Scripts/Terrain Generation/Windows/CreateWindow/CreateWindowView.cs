using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Terrain_Generation.Windows.CreateWindow
{
    public class CreateWindowView
    {
        public event Action<CreationData> onCreate;
        public event Action<CreationData> onSaveSettings;
        
        private VisualElement _root;
        private Vector2IntField _resolution;
        private IntegerField _octaves;
        private FloatField _seed;
        private CreationData _data;
        
        public CreateWindowView(VisualElement root, TerrainGenerationModel model)
        {
            InitializeCreationData(model);
            
            _root = root;
            _root.contentContainer.Clear();

            VisualElement settingsBox = new VisualElement()
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
            _root.Add(settingsBox);

            Label settingsLabel = new Label("Settings");
            settingsBox.Add(settingsLabel);

            VisualElement resolutionRow = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row }
            };
            settingsBox.Add(resolutionRow);
            
            Label resolutionLabel = new Label("Resolution")
            {
                style =
                {
                    flexGrow = 1,
                    
                    marginLeft = 3
                }
            };
            resolutionRow.Add(resolutionLabel);

            _resolution = new Vector2IntField
            {
                value = _data.Resolution,
                style =
                {
                    marginRight = -1
                }
            };
            resolutionRow.Add(_resolution);
            
            VisualElement octavesRow = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row }
            };
            settingsBox.Add(octavesRow);
            
            Label octavesLabel = new Label("Perlin Noise Octaves")
            {
                style =
                {
                    flexGrow = 1,
                    
                    marginLeft = 3
                }
            };
            octavesRow.Add(octavesLabel);
            
            _octaves = new IntegerField
            {
                value = _data.PerlinOctaves
            };
            octavesRow.Add(_octaves);
            
            VisualElement seedRow = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row }
            };
            settingsBox.Add(seedRow);
            
            Label seedLabel = new Label("Random Seed")
            {
                style =
                {
                    flexGrow = 1,
                    
                    marginLeft = 3
                }
            };
            seedRow.Add(seedLabel);

            _seed = new FloatField
            {
                value = _data.Seed,
            };
            seedRow.Add(_seed);

            VisualElement buttonsRow = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row }
            };
            settingsBox.Add(buttonsRow);

            Button loadButton = new Button(() => UpdateView(model))
            {
                text = "Reset Settings",
                style =
                {
                    flexGrow = 1
                }
            };
            buttonsRow.Add(loadButton);

            Button saveButton = new Button(OnSaveSettings)
            {
                text = "Save Settings",
                style =
                {
                    flexGrow = 1
                }
            };
            buttonsRow.Add(saveButton);

            Button createButton = new Button(Create)
            {
                text = "Create Heightmap",
            };
            _root.Add(createButton);
        }

        private void OnSaveSettings()
        {
            LoadDataFromFields();
            onSaveSettings?.Invoke(_data);
        }

        private void UpdateView(TerrainGenerationModel model)
        {
            InitializeCreationData(model);
            _resolution.value = _data.Resolution;
            _octaves.value = _data.PerlinOctaves;
            _seed.value = _data.Seed;
        }

        private void InitializeCreationData(TerrainGenerationModel model)
        {
            _data = new CreationData();
            if (model)
            {
                _data.Seed = model.Seed;
                _data.PerlinOctaves =  model.Octaves;
                _data.Resolution = model.HeightmapResolution;
            }
            else
            {
                _data.Seed = 43758.5453f;
                _data.PerlinOctaves = 8;
                _data.Resolution = new Vector2Int(1025, 1025);
            }
        }

        private void LoadDataFromFields()
        {
            _data = new CreationData
            {
                Resolution = _resolution.value,
                PerlinOctaves = _octaves.value,
                Seed = _seed.value
            };
        }

        private void Create()
        {
            onCreate?.Invoke(_data);
        }
    }
}