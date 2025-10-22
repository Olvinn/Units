using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Terrain_Generation
{
    public static class TerrainGenerator
    {
        public static Texture2D GenerateNoise(int width = 1025, int height = 1025, int octaves = 8)
        {
#if UNITY_EDITOR
            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            rt.enableRandomWrite = false;
            rt.Create();

            Material mat = new Material(Shader.Find("Hidden/Utils/PerlinNoise"));
            mat.SetInt("_Octaves", octaves);

            Graphics.Blit(null, rt, mat);

            Texture2D tex = new Texture2D(width, height, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            rt.Release();
            Object.DestroyImmediate(mat);

            return tex;
#else
            Debug.LogWarning("GenerateNoise works only in Editor mode.");
            return null;
#endif
        }
    }
}