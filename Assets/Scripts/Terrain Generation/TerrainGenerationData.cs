using UnityEngine;

namespace Terrain_Generation
{
    [CreateAssetMenu(fileName = "TerrainGenerationData", menuName = "Generation/TerrainGenerationData")]
    public class TerrainGenerationData : ScriptableObject
    {
        public float Seed;
        public int Octaves;
    }
}
