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
            int applyKernel = cs.FindKernel("ApplyDeltas");

            int width = heightmap.width;
            int height = heightmap.height;

            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.graphicsFormat = GraphicsFormat.R32_SFloat;
            rt.enableRandomWrite = true;
            rt.Create();
            
            RenderTexture deltaTex = new RenderTexture(width, height, 0);
            deltaTex.graphicsFormat = GraphicsFormat.R32_SFloat;
            deltaTex.enableRandomWrite = true;
            deltaTex.Create();
            Graphics.Blit(Texture2D.blackTexture, deltaTex);

            Graphics.Blit(heightmap, rt);

            int numDrops = 250000; // high enough for 1k map detail
            int stepsPerDrop = 500;

            cs.SetInt("width", width);
            cs.SetInt("height", height);
            cs.SetInt("numDrops", numDrops);
            cs.SetInt("numStepsPerDrop", stepsPerDrop);

            cs.SetInt("searchRadius", 3);     
            cs.SetInt("depositRadius", 3);   
            cs.SetInt("erodeRadius", 3);   

            cs.SetFloat("initialWater", 1.0f);
            cs.SetFloat("initialSpeed", .5f);
            cs.SetFloat("inertia", 0.15f);   

            cs.SetFloat("erosionFactor", .01f);     
            cs.SetFloat("depositionFactor", 0.001f);  
            cs.SetFloat("evaporation", 0.002f);      

            cs.SetFloat("capacityFactor", 2f);     
            cs.SetFloat("minCapacity", 0.0002f);
            cs.SetFloat("stepScale", 1f);
            cs.SetFloat("randJitter", Random.value * 5); 

            cs.SetInt("seed", UnityEngine.Random.Range(1, 999999));
            
            int groups = (numDrops + 63) / 64; // safe ceil
            cs.SetTexture(kernel, "Height", rt);
            cs.SetTexture(kernel, "HeightDelta", deltaTex);
            cs.Dispatch(kernel, groups, 1, 1);
            
            cs.SetTexture(applyKernel, "Height", rt);
            cs.SetTexture(applyKernel, "HeightDelta", deltaTex);
            cs.Dispatch(applyKernel, width/8, height/8, 1);

            Texture2D tex = new Texture2D(width, height, GraphicsFormat.R32_SFloat, TextureCreationFlags.None);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0,0,width,height),0,0);
            tex.Apply();
            RenderTexture.active = null;
            
            rt.Release();
            deltaTex.Release();

            return tex;
        }
    }
}