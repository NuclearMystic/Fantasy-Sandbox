Shader "Custom/WindLitShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _WindFrequency ("Wind Frequency", Float) = 0.5
        _WindSpeed ("Wind Speed", Float) = 1.0
        _WindStrength ("Wind Strength", Float) = 0.05
        _AmbientBoost ("Ambient Boost", Range(0, 2)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off 
            ZWrite On
            AlphaToMask On
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

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
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            float4 _BaseColor;
            float _Cutoff;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _WindFrequency;
            float _WindSpeed;
            float _WindStrength;
            float _AmbientBoost;

            float CalculateSway(float3 posWS)
            {
                return sin(posWS.x * _WindFrequency + _Time.y * _WindSpeed) * _WindStrength;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;

                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                posWS.z += CalculateSway(posWS); 

                OUT.positionHCS = TransformWorldToHClip(posWS);
                OUT.uv = IN.uv;
                OUT.positionWS = posWS;

                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.normalWS = normalize(normalWS);

                OUT.shadowCoord = TransformWorldToShadowCoord(posWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                clip(tex.a - _Cutoff); 

                half4 baseColor = tex * _BaseColor;

                float3 normalWS = normalize(IN.normalWS);

                #ifdef UNITY_FRONT_FACING
                if (!unity_FrontFacing)
                {
                    normalWS *= -1;
                }
                #endif

                float3 lightDir = normalize(_MainLightPosition.xyz);

                float wrap = 0.4;
                float wrappedNdotL = pow(saturate((dot(normalWS, lightDir) + wrap) / (1.0 + wrap)), 1.1);


                float shadowAttenuation = MainLightRealtimeShadow(IN.shadowCoord);

                float3 ambient = SampleSH(normalWS) * _AmbientBoost;
                float3 litColor = _MainLightColor.rgb * wrappedNdotL * shadowAttenuation;

                float3 finalLight = lerp(litColor, ambient + litColor, 0.3); 
                finalLight = pow(finalLight, 0.9); 
                baseColor.rgb *= finalLight;

                return baseColor;
            }
            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
