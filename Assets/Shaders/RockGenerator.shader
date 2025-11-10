Shader "Custom/RockGenerator"
{
    
    Properties
    {
        _Scale ("Scale", Float) = 4.0
        [HDR] _MudColor ("Mud Color", Color) = (0.38, 0.26, 0.04, 1)
        [HDR] _CracksColor ("Cracks Color", Color) = (0.17, 0.11, 0.05, 1)
    }
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        #include "Noise.hlsl"
        ENDHLSL
        
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite On Cull Off
        Pass
        {
            Name "RockGenerator"

            HLSLPROGRAM

            float _Scale;
            float4 _MudColor, _CracksColor;
            
            #pragma vertex vert
            #pragma fragment frag

            struct appv { float4 vertex : POSITION; float2 uv : TEXCOORD0; float3 normal : NORMAL; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; float3 wpos : TEXCOORD1; float3 wnormal : NORMAL; };

            v2f vert (appv v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.wpos = TransformObjectToWorld(v.vertex);
                o.wnormal = TransformObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {                
                float l = fbm3D(i.wpos * _Scale * 2, max(1, 8), 2, .5, 200) * .5 + .5;
                l += voronoi3d(i.wpos * _Scale, 200) * .5;
                l *= .75;
                return lerp(_MudColor, _CracksColor, l);
            }
            
            ENDHLSL
        }
    }
}
