using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Terrain_Generation
{
    public static class TerrainGenerator
    {
        public static Texture2D CreateBlankTexture(int width = 1025, int height = 1025)
        {
            var result =  new Texture2D(width, height, GraphicsFormat.R16_SFloat, TextureCreationFlags.None)
                {
                    filterMode = FilterMode.Bilinear
                };
            return result;
        }
        
        public static Texture2D GenerateNoise(Shader shader, int width = 1025, int height = 1025, int octaves = 8, float seed = 43758.5453f)
        {
            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            rt.enableRandomWrite = false;
            rt.Create();

            Material mat = new Material(shader);
            mat.SetInt("_Octaves", octaves);
            mat.SetFloat("_Seed", seed);

            Graphics.Blit(null, rt, mat);

            Texture2D tex = CreateBlankTexture(width, height);
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

        public static Texture2D CalculateWaterErosion(Texture2D heightmap, ComputeShader cs)
        {
            int kernel = cs.FindKernel("ErodeDroplets");
            int getKernel = cs.FindKernel("GetHeights");

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
            
            RenderTexture tempTex = new RenderTexture(width, height, 0);
            tempTex.graphicsFormat = GraphicsFormat.R32_SFloat;
            tempTex.enableRandomWrite = true;
            tempTex.Create();
            
            Graphics.Blit(Texture2D.blackTexture, tempTex);
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
            //cs.SetFloat("erosionFactor", 10f);     
            cs.SetFloat("depositionFactor", 0.001f);  
            cs.SetFloat("evaporation", 0.002f);      

            cs.SetFloat("capacityFactor", 2f);     
            cs.SetFloat("minCapacity", 0.0002f);
            cs.SetFloat("stepScale", 1f);
            cs.SetFloat("randJitter", Random.value * 5); 

            cs.SetInt("seed", Random.Range(1, 999999));
            
            int groups = (numDrops + 63) / 64; // safe ceil
            cs.SetTexture(kernel, "Height", rt);
            cs.SetTexture(kernel, "Delta", deltaTex);
            cs.SetTexture(kernel, "HeightDelta", tempTex);
            cs.Dispatch(kernel, groups, 1, 1);
            
            cs.SetTexture(getKernel, "Delta", deltaTex);
            cs.SetTexture(getKernel, "HeightDelta", tempTex);
            cs.Dispatch(getKernel, width/8, height/8, 1);

            Texture2D tex = CreateBlankTexture(width, height);
            RenderTexture.active = deltaTex;
            tex.ReadPixels(new Rect(0,0,width,height),0,0);
            tex.Apply();
            RenderTexture.active = null;
            
            rt.Release();
            tempTex.Release();

            return tex;
        }

        public static Texture2D ApplyMaskToHeightmap(Texture2D heightmap, Texture2D deltas, ComputeShader cs)
        {
            int kernel = cs.FindKernel("ApplyDeltas");

            int width = heightmap.width;
            int height = heightmap.height;

            int gx = Mathf.CeilToInt(width / 8.0f);
            int gy = Mathf.CeilToInt(height / 8.0f);
            
            var prevRT = RenderTexture.active;

            RenderTexture heightmapRt = new RenderTexture(width, height, 0);
            heightmapRt.graphicsFormat = GraphicsFormat.R32_SFloat;
            heightmapRt.enableRandomWrite = true;
            heightmapRt.Create();

            RenderTexture deltasRt = new RenderTexture(width, height, 0);
            deltasRt.graphicsFormat = GraphicsFormat.R32_SFloat;
            deltasRt.enableRandomWrite = true;
            deltasRt.Create();
            
            Graphics.Blit(heightmap, heightmapRt);
            Graphics.Blit(deltas, deltasRt);

            cs.SetInt("width", width);
            cs.SetInt("height", height);

            cs.SetTexture(kernel, "Height", heightmapRt);
            cs.SetTexture(kernel, "Delta", deltasRt);
            cs.Dispatch(kernel, gx, gy, 1);
            
            Texture2D tex = CreateBlankTexture(width, height);
            RenderTexture.active = heightmapRt;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = prevRT;

            heightmapRt.Release();
            deltasRt.Release();

            return tex;
        }

        public static Texture2D SaturateDeltas(Texture2D deltas, ComputeShader cs)
        {
            int kCalc = cs.FindKernel("CalculateBrightness");
            int kApply = cs.FindKernel("Saturate");

            int width = deltas.width;
            int height = deltas.height;

            int gx = Mathf.CeilToInt(width / 8.0f);
            int gy = Mathf.CeilToInt(height / 8.0f);

            RenderTexture deltaTex = new RenderTexture(width, height, 0)
            {
                graphicsFormat = GraphicsFormat.R32_SFloat,
                enableRandomWrite = true
            };
            deltaTex.Create();

            RenderTexture saturatedTex = new RenderTexture(width, height, 0)
            {
                graphicsFormat = GraphicsFormat.R32_SFloat,
                enableRandomWrite = true
            };
            saturatedTex.Create();

            RenderTexture min = new RenderTexture(1, 1, 0)
            {
                graphicsFormat = GraphicsFormat.R32_SFloat,
                enableRandomWrite = true
            };
            min.Create();

            RenderTexture max = new RenderTexture(1, 1, 0)
            {
                graphicsFormat = GraphicsFormat.R32_SFloat,
                enableRandomWrite = true
            };
            max.Create();

            var prevRT = RenderTexture.active;
            RenderTexture.active = min;
            GL.Clear(true, true, new Color(1e9f, 0, 0, 0));
            RenderTexture.active = max;
            GL.Clear(true, true, new Color(-1e9f, 0, 0, 0));
            RenderTexture.active = prevRT;

            Graphics.Blit(deltas, deltaTex);

            cs.SetInt("width", width);
            cs.SetInt("height", height);

            cs.SetTexture(kCalc, "Delta", deltaTex);
            cs.SetTexture(kCalc, "MinBrightness", min);
            cs.SetTexture(kCalc, "MaxBrightness", max);
            cs.Dispatch(kCalc, gx, gy, 1);

            cs.SetTexture(kApply, "Delta", deltaTex);
            cs.SetTexture(kApply, "Saturated", saturatedTex);
            cs.SetTexture(kApply, "MinBrightness", min);
            cs.SetTexture(kApply, "MaxBrightness", max);
            cs.Dispatch(kApply, gx, gy, 1);

            Texture2D tex = CreateBlankTexture(width, height);
            RenderTexture.active = saturatedTex;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = prevRT;

            deltaTex.Release();
            saturatedTex.Release();
            min.Release();
            max.Release();

            return tex;
        }
    }
}