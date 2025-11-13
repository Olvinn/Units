using System.Collections.Generic;
using UnityEngine;

namespace Terrain_Generation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SimpleTerrain : MonoBehaviour
    {
        public TerrainData terrainData;
        [SerializeField] private int _detalization;
        [SerializeField] private float _size;
        [SerializeField] private MeshFilter _meshFilter;

        public void ApplyTerrainData(TerrainData terrainData)
        {
            this.terrainData = terrainData;
            CreateMesh();
        }

        private void OnValidate()
        {
            if (_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();
            CreateMesh();
        }

        private void CreateMesh()
        {
            Mesh mesh = new Mesh();

            var d = _detalization;

            var vertices = new Vector3[d * d];
            var uv = new Vector2[d * d];
            float growth = _size / (_detalization - 1);
            Vector3 offset = new Vector3(-_size * 0.5f, 0, -_size * 0.5f);
            
            for (int i = 0; i < d; i++)
            for (int j = 0; j < d; j++)
            {
                int dx = (int)((float)i / d * terrainData?.heightmapResolution ?? 0);
                int dy = (int)((float)j / d * terrainData?.heightmapResolution ?? 0);
                var height = terrainData?.GetHeight(dx, dy) ?? 0;
                vertices[i * d + j] = new Vector3(i * growth, height, j * growth);
                uv[i * d + j] = new Vector2((float)i / (d - 1), (float)j / (d - 1));
            }

            var triangles = new List<int>();
            for (int i = 0; i < d - 1; i++)
            {
                for (int j = 0; j < d - 1; j++)
                {
                    triangles.Add(i + j * d);
                    triangles.Add(i + j * d + 1);
                    triangles.Add(i + j * d + d);
                    triangles.Add(i + j * d + 1);
                    triangles.Add(i + j * d + d + 1);
                    triangles.Add(i + j * d + d);
                }
            } 

            mesh.vertices = vertices;
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv;
        
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            _meshFilter.mesh = mesh;
        }
    }
}