Shader "Custom/CelShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {} // Texture support
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Ramp ("Ramp Steps", Range(1, 4)) = 2
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.01
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        
        Pass
        {
            Name "CelShadingPass"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Color;
            float _Ramp;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.pos = TransformWorldToHClip(worldPos);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float intensity = dot(i.normal, lightDir);
                intensity = floor(intensity * _Ramp) / _Ramp; // Cel-shading effect

                // Sample texture and apply tint
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return float4(texColor.rgb * intensity * _Color.rgb, texColor.a);
            }
            ENDHLSL
        }

        // Outline Pass
        Pass
        {
            Name "OutlinePass"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Cull Front // Render backfaces for outline

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                float3 normalDir = normalize(TransformObjectToWorldNormal(v.normal)) * _OutlineWidth;
                o.pos = TransformWorldToHClip(worldPos + normalDir); // Expand vertices for outline
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
