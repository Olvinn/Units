using System;
using UnityEngine;

namespace Terrain_Generation.Windows
{
    public interface ITerrainGeneratorWindow : IDisposable
    {
        void Update(TerrainData terrainData);
    }
}