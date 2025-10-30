using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Terrain_Generation
{
    public static class TerrainGenerator
    {
        public static Texture2D GenerateNoise(Shader shader, int width = 1025, int height = 1025, int octaves = 8, float seed = 43758.5453f)
        {
            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            rt.enableRandomWrite = false;
            rt.Create();

            Material mat = new Material(shader);
            mat.SetInt("_Octaves", octaves);
            mat.SetFloat("_Seed", seed);

            Graphics.Blit(null, rt, mat);

            Texture2D tex = new Texture2D(width, height, GraphicsFormat.R32_SFloat, TextureCreationFlags.None);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            rt.Release();
            Object.DestroyImmediate(mat);

            return tex;
        }
        
        public static Texture2D GenerateNoise(int width = 1025, int height = 1025, int octaves = 8, float seed = 43758.5453f)
        {
#if UNITY_EDITOR
            return GenerateNoise(Shader.Find("Utils/PerlinNoise"), width, height, octaves, seed);
#else
            Debug.LogWarning("GenerateNoise works only in Editor mode.");
            return null;
#endif
        }

        public static Texture2D ApplyWaterErosion(Texture2D heightmap, ComputeShader cs)
        {
            int kernel = cs.FindKernel("ErodeDroplets");

            int width = heightmap.width;
            int height = heightmap.height;
            
            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            rt.enableRandomWrite = true;
            rt.format = RenderTextureFormat.RFloat;
            rt.Create();
            
            Graphics.Blit(heightmap, rt);
        
            int numDrops = 65536;
            int stepsPerDrop = 50;
            int searchRadius = 2;
            uint seed = 12345;
            float initialWater = 3f;
            float erosionFactor = 0.01f;
            float depositionFactor = 0.15f;
            float evaporation = 0.03f;
            float capacityFactor = 4f;
            
            cs.SetInt("width", width);
            cs.SetInt("height", height);
            cs.SetInt("numStepsPerDrop", stepsPerDrop);
            cs.SetInt("searchRadius", searchRadius);
            cs.SetFloat("initialWater", initialWater);
            cs.SetFloat("erosionFactor", erosionFactor);
            cs.SetFloat("depositionFactor", depositionFactor);
            cs.SetFloat("evaporation", evaporation);
            cs.SetFloat("capacityFactor", capacityFactor);
            cs.SetTexture(kernel, "Result", rt);
            cs.SetInt("seed", (int)seed);

            int groupsX = Mathf.CeilToInt(width / 8.0f);
            int groupsY = Mathf.CeilToInt(height / 8.0f);
            cs.Dispatch(kernel, groupsX, groupsY, 1);
            
            Texture2D tex = new Texture2D(width, height, GraphicsFormat.R32_SFloat, TextureCreationFlags.None);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            return tex;
        }
    }
}