Shader "Custom/Terrain"
{
    Properties
    {
        _PlanesColor ("Planes Color", Color) = (0.8,0.8,0.9,1)
        _SlopesColor ("Slopes Color", Color) = (0.5,0.5,0.6,1)
        _Metallic ("Metallic", Float) = .3
        _Roughness ("Roughness", Float) = .8
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct attr {
                float4 positionOS : POSITION;
            };

            struct v2f {
                float4 positionHCS : SV_POSITION;
            };

            v2f vert(attr i) {
                v2f o;
                o.positionHCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode"="ShadowCaster"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct attr {
                float4 positionOS : POSITION;
            };

            struct v2f {
                float4 positionHCS : SV_POSITION;
            };

            v2f vert(attr i) {
                v2f o;
                o.positionHCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "GBuffer"
            Tags { "RenderPipeline" = "UniversalPipeline" "LightMode"="UniversalGBuffer" "UniversalMaterialType" = "Lit" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define _NORMALMAP 1

            #include "UnityGBuffer.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GBufferOutput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GBufferInput.hlsl"

            struct attr {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct v2f {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float2 uv          : TEXCOORD1;
                float4  shadows    : TEXCOORD2; // Shadow UVs
            };
            
            v2f vert(attr i)
            {
                v2f o;
                VertexPositionInputs pos = GetVertexPositionInputs(i.positionOS);
                o.positionHCS = pos.positionCS;
                o.normalWS = TransformObjectToWorldNormal(i.normalOS);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);
                o.shadows = GetShadowCoord(vertexInput);
                o.uv = i.uv;
                return o;
            }
            
            GBufferFragOutput frag(v2f i)
            {
                half3 bakedGI = SAMPLE_GI(input.staticLightmapUV, i.positionHCS, i.normalWS);
                half3 lighting = MainLightRealtimeShadow(i.shadows) + bakedGI;

                half4 color = 1.0;
                color.rgb = float3(0,0,0) * lighting;

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.alpha = 1.0;
                surfaceData.occlusion = 1.0;

                InputData inputData = (InputData)0;
                inputData.normalWS = PackGBufferNormal(i.normalWS);
                inputData.positionCS = i.positionHCS;

                return PackGBuffersSurfaceData(surfaceData, inputData, color.rgb);
            }
            
            ENDHLSL
        }
    }
}
