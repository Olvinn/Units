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

            Texture2D tex = new Texture2D(width, height, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
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
    }
}