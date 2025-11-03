Shader "Unlit/Terrain"
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
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            float4 _PlanesColor, _SlopesColor;
            float _Metallic, _Roughness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 wpos : TEXCOORD0;
                float3 normal : NORMAL;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3 MobileGGXPBR(
                float3 N, float3 V, float3 L,
                float3 albedo, float roughness, float metallic,
                float3 lightColor)
            {
                float3 H = normalize(V + L);

                float NdotL = saturate(dot(N, L));
                float NdotV = saturate(dot(N, V));
                float NdotH = saturate(dot(N, H));
                float VdotH = saturate(dot(V, H));

                float r = roughness * roughness;

                // GGX normal distribution, simplified
                float a2 = r * r;
                float denom = (NdotH * NdotH * (a2 - 1) + 1);
                float D = a2 / max(UNITY_PI * denom * denom, 0.001);

                // Schlick Fresnel
                float3 F0 = lerp(float3(0.04,0.04,0.04), albedo, metallic);
                float3 F = F0 + (1 - F0) * pow(1 - VdotH, 5);

                // Simplified Smith term
                float k = (r + 1) * (r + 1) * 0.125;
                float G = (NdotV / (NdotV * (1 - k) + k)) *
                          (NdotL / (NdotL * (1 - k) + k));

                float3 spec = D * F * G;

                float3 diffuse = albedo * (1 - metallic) / UNITY_PI;

                return (diffuse + spec) * lightColor * NdotL;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = lerp(_SlopesColor, _PlanesColor, pow(dot(float3(0,1,0), i.normal), 2));
                float3 viewDir = _WorldSpaceCameraPos - i.wpos;
                float3 lightDir = _WorldSpaceLightPos0;
                col = float4(MobileGGXPBR(i.normal, viewDir, lightDir, col, _Roughness, _Metallic, float3(1,1,1)), col.a);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
