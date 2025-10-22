Shader "Hidden/Utils/PerlinNoise"
{
    Properties
    {
        _Scale ("Scale", Float) = 4.0
        _Octaves ("Octaves", Int) = 4
        _Lacunarity ("Lacunarity", Float) = 2.0
        _Persistence ("Persistence", Float) = 0.5
        _Offset ("Offset", Vector) = (0,0,0,0)
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

            struct appv { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            float _Scale;
            int _Octaves;
            float _Lacunarity;
            float _Persistence;
            float4 _Offset;

            static float fade(float t)
            {
                return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
            }

            static float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            static float2 gradient(float2 ip)
            {
                float h = hash(ip) * 6.28318530718; // 2*PI
                return float2(cos(h), sin(h));
            }

            static float perlin2D(float2 p)
            {
                float2 pi = floor(p);
                float2 pf = p - pi;

                float2 g00 = gradient(pi + float2(0.0, 0.0));
                float2 g10 = gradient(pi + float2(1.0, 0.0));
                float2 g01 = gradient(pi + float2(0.0, 1.0));
                float2 g11 = gradient(pi + float2(1.0, 1.0));

                float2 d00 = pf - float2(0.0, 0.0);
                float2 d10 = pf - float2(1.0, 0.0);
                float2 d01 = pf - float2(0.0, 1.0);
                float2 d11 = pf - float2(1.0, 1.0);

                float n00 = dot(g00, d00);
                float n10 = dot(g10, d10);
                float n01 = dot(g01, d01);
                float n11 = dot(g11, d11);

                float u = fade(pf.x);
                float v = fade(pf.y);

                float nx0 = lerp(n00, n10, u);
                float nx1 = lerp(n01, n11, u);
                float nxy = lerp(nx0, nx1, v);

                return nxy;
            }

            static float fbm(float2 p, int octaves, float lacunarity, float persistence)
            {
                float amplitude = 1.0;
                float frequency = 1.0;
                float sum = 0.0;
                float maxAmp = 0.0;

                for (int i = 0; i < octaves; ++i)
                {
                    sum += perlin2D(p * frequency) * amplitude;
                    maxAmp += amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                return sum / maxAmp;
            }

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

                float n = fbm(p, max(1, _Octaves), _Lacunarity, _Persistence);

                float v = n * 0.5 + 0.5;

                fixed4 col = saturate(v);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
