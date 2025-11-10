Shader "Utils/PerlinNoise"
{
    Properties
    {
        _Scale ("Scale", Float) = 4.0
        _Octaves ("Octaves", Int) = 4
        _Persistence ("Persistence", Float) = 0.5
        _Offset ("Offset", Vector) = (0,0,0,0)
        _Seed ("Seed", Float) = 43758.5453
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Noise.hlsl"

            struct appv { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            float _Scale;
            int _Octaves;
            float _Persistence;
            float _Seed;
            float4 _Offset;

            v2f vert (appv v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 p = (uv * _Scale);

                float n = fbm2D(p, max(1, _Octaves), 2, _Persistence, _Seed);

                float v = n * 0.5 + 0.5;

                fixed4 col = saturate(v);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
