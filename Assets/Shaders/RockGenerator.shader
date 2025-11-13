Shader "Custom/RockGenerator"
{
    
    Properties
    {
        _PerlinScale ("Perlin Scale", Float) = 4.0
        _VoronoiScale ("Voronoi Scale", Float) = 4.0
        _Scale ("Full Scale", Float) = 1
        [HDR] _MudColor ("Mud Color", Color) = (0.38, 0.26, 0.04, 1)
        [HDR] _CracksColor ("Cracks Color", Color) = (0.17, 0.11, 0.05, 1)
        _Stripes ("Stripes", Float) = .5
        _Noise ("Noisy pattern", Range(0, 1)) = .5
        _Tiling ("Tiling", Float) = 10
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
        Pass
        {
            Name "RockGenerator"

            HLSLPROGRAM

            float _PerlinScale, _VoronoiScale, _Stripes, _Tiling, _Scale, _Noise;
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
                i.wpos *= _Scale;
                float l = fbmPerlin3D(i.wpos * _PerlinScale, 12, 2, .5, 200, _Tiling) * .5 + .5;
                float3 streched = i.wpos;
                streched.y *= _Stripes;
                l = lerp(l, perlin3D(i.wpos * _PerlinScale, 200, _Tiling) * .5 + .5, _Noise);
                //l = saturate(l * .5);
                l = saturate((voronoi3d(streched * _VoronoiScale * (1+l), 200, _Tiling) *.5 + .5));
                //l *= .5;
                l = saturate(l);
                return lerp(_MudColor, _CracksColor, l);
            }
            
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
