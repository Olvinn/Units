using UnityEngine;

namespace Terrain_Generation
{
    [CreateAssetMenu(fileName = "TerrainGenerationData", menuName = "Generation/TerrainGenerationData")]
    public class TerrainGenerationModel : ScriptableObject
    {
        public ComputeShader WaterErosion;
        public Shader NoiseShader;
        public float Seed;
        public int Octaves;
        public Vector2Int HeightmapResolution;
    }
}
