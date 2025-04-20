Shader "Custom/BillboardSimpleLit_YFacing_Scaled_FacingDirection"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _ColorTint ("Color Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _Brightness ("Brightness", Range(0.5, 3)) = 1.2
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _ColorTint;
                float _Cutoff;
                float _Brightness;
            CBUFFER_END

            sampler2D _BaseMap;
            float3 _CameraForward; // Set from C# every frame

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 lighting : TEXCOORD1;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                // Step 1: Get object position in world space
                float3 worldPivot = unity_ObjectToWorld._m03_m13_m23;

                // Step 2: Use camera forward direction to build billboard axes
                float3 forward = normalize(_CameraForward);
                forward.y = 0;
                forward = normalize(forward);

                float3 right = normalize(cross(float3(0, 1, 0), forward));
                float3 up = float3(0, 1, 0);

                // Step 3: Apply object scale
                float3 objectScale = float3(
                    length(unity_ObjectToWorld._m00_m10_m20),
                    length(unity_ObjectToWorld._m01_m11_m21),
                    length(unity_ObjectToWorld._m02_m12_m22)
                );

                // Step 4: Rebuild rotated billboard quad in world space
                float3 offset = (-IN.positionOS.x) * right * objectScale.x +
                                 IN.positionOS.y  * up    * objectScale.y;

                float3 worldPos = worldPivot + offset;

                // Step 5: Output to clip space
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                // Step 6: Lighting
                float3 normalOS = float3(0, 0, -1); // Treat all billboard quads as facing "forward"
                float3 rotatedNormal = normalize((-right * normalOS.x) + (up * normalOS.y) - (forward * normalOS.z));
                float3 normalWS = normalize(mul((float3x3)unity_ObjectToWorld, rotatedNormal));
                Light mainLight = GetMainLight();
                OUT.lighting = _ColorTint.rgb * mainLight.color.rgb;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 tex = tex2D(_BaseMap, IN.uv);
                clip(tex.a - _Cutoff);
                half3 litColor = tex.rgb * IN.lighting * _Brightness;
                return half4(litColor, 1.0);
            }
            ENDHLSL
        }
    }

}
