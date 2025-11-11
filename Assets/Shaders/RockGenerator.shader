Shader "Custom/RockGenerator"
{
    
    Properties
    {
        _PerlinScale ("Perlin Scale", Float) = 4.0
        _VoronoiScale ("Voronoi Scale", Float) = 4.0
        [HDR] _MudColor ("Mud Color", Color) = (0.38, 0.26, 0.04, 1)
        [HDR] _CracksColor ("Cracks Color", Color) = (0.17, 0.11, 0.05, 1)
        _Stripes ("Stripes", Range(0, 1)) = .5
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

            float _PerlinScale, _VoronoiScale, _Stripes;
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
                float l = fbmPerlin3D(i.wpos * _PerlinScale, 12, 2, .5, 200, 10) * .5 + .5;
                l = lerp(l, perlin3D(i.wpos * _PerlinScale, 123, 10) * .5 + .5, _Stripes);
                //l = saturate(l * .5);
                l = saturate((voronoi3d(i.wpos * _VoronoiScale * (1+l), 200, 10) *.5 + .5));
                //l *= .5;
                l = saturate(l);
                return lerp(_MudColor, _CracksColor, l);
            }
            
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
