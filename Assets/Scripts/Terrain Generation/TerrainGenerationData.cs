using UnityEngine;

namespace Terrain_Generation
{
    [CreateAssetMenu(fileName = "TerrainGenerationData", menuName = "Generation/TerrainGenerationData")]
    public class TerrainGenerationData : ScriptableObject
    {
        public ComputeShader WaterErosion;
        public Shader NoiseShader;
        public float Seed;
        public int Octaves;
    }
}
