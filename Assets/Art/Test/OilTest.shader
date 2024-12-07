Shader "Custom/OilShaderURP"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.2, 0.2, 0.2, 1)
        [Normal] _NormalMap("Normal Map", 2D) = "bump" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.95
        _Metallic("Metallic", Range(0,1)) = 0.9
        _IridescenceStrength("Iridescence Strength", Range(0,1)) = 0.5
        _IridescenceSpeed("Iridescence Speed", Range(0,10)) = 1.0
        _DistortionStrength("Distortion Strength", Range(0,1)) = 0.1
        _FlowSpeed("Flow Speed", Range(0,2)) = 0.5
        _EmissionStrength("Emission Strength", Range(0,2)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
            };

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _NormalMap_ST;
                float _Glossiness;
                float _Metallic;
                float _IridescenceStrength;
                float _IridescenceSpeed;
                float _DistortionStrength;
                float _FlowSpeed;
                float _EmissionStrength;
            CBUFFER_END

            float3 Iridescence(float ndotv)
            {
                float3 color1 = float3(1.0, 0.3, 0.3);
                float3 color2 = float3(0.3, 1.0, 0.3);
                float3 color3 = float3(0.3, 0.3, 1.0);
                
                float phase = ndotv * 6.28318530718 + _Time.y * _IridescenceSpeed;
                float3 iridescent = lerp(color1, color2, (sin(phase) + 1.0) * 0.5) +
                                  lerp(color2, color3, (sin(phase * 1.5) + 1.0) * 0.5);
                return iridescent * 1.5; // Increased brightness
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _NormalMap);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // Sample normal map with flow
                float2 flowOffset = _Time.y * _FlowSpeed;
                float3 normalTS1 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv + flowOffset));
                float3 normalTS2 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv * 1.4 - flowOffset));
                float3 normalTS = normalize(normalTS1 + normalTS2);

                // Convert normal to world space
                float3 bitangent = normalize(cross(input.normalWS, input.tangentWS.xyz)) * input.tangentWS.w;
                float3x3 TBN = float3x3(input.tangentWS.xyz, bitangent, input.normalWS);
                float3 normalWS = normalize(mul(normalTS, TBN));

                // Calculate view angle and iridescence
                float ndotv = saturate(dot(normalWS, viewDirWS));
                float3 iridescenceColor = Iridescence(ndotv);
                float3 finalColor = lerp(_BaseColor.rgb, iridescenceColor, _IridescenceStrength * ndotv);

                // Setup lighting
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = viewDirWS;
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(inputData.shadowCoord);
                inputData.bakedGI = 0;
                inputData.vertexLighting = float3(0, 0, 0);
                inputData.normalizedScreenSpaceUV = float2(0, 0);
                inputData.shadowMask = float4(1, 1, 1, 1);

                // Surface data
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = finalColor;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Glossiness;
                surfaceData.normalTS = normalTS;
                surfaceData.emission = iridescenceColor * _EmissionStrength;
                surfaceData.occlusion = 1;
                surfaceData.alpha = 1;

                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }

        // Shadow casting support
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
}