Shader "Custom/TerrainLitSimple"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Terrain Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "TerrainCompatible" = "True" }

        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            float4 _BaseColor;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(posWS);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 baseColor = tex * _BaseColor;

                float3 normalWS = normalize(IN.normalWS);
                float3 lightDir = normalize(_MainLightPosition.xyz);

                // Wrap lighting for softer terrain shading
                float wrap = 0.5;
                float wrappedNdotL = saturate((dot(normalWS, lightDir) + wrap) / (1.0 + wrap));

                // Ambient light from skybox/environment
                float3 ambient = SampleSH(normalWS);
                float3 finalLight = ambient + (_MainLightColor.rgb * wrappedNdotL);
                finalLight = max(finalLight, 0.2); // clamp min brightness

                baseColor.rgb *= finalLight;
                return baseColor;
            }
            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
