// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 

Shader "Rito/Snowy Ground URP"
{
    Properties
    {
        _EdgeLength ("Edge length", Range(2, 50)) = 6
        _ColorIntensity("Color Intensity", Range(0, 5)) = 1
        [MainTexture] _BaseMap("Snow Texture", 2D) = "white" {}
        _HeightMultiplier("Height Multiplier", Range(0, 5)) = 0.5
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry"}

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float _HeightMultiplier;
            float _ColorIntensity;
            float4 _BaseColor;
        CBUFFER_END

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float3 normalOS : NORMAL;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normalWS : TEXCOORD1;
            float3 positionWS : TEXCOORD2;
        };
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                float4 snowTex = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, input.uv, 0);
                float3 vertexOffset = float3(0, snowTex.r * _HeightMultiplier, 0);
                input.positionOS.xyz += vertexOffset;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 snowTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half3 color = snowTex.rgb * _ColorIntensity * _BaseColor.rgb;

                // Calculate lighting
                float3 normalWS = normalize(input.normalWS);
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                half3 lighting = mainLight.color * mainLight.distanceAttenuation * mainLight.shadowAttenuation;

                // Apply simple diffuse lighting
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                color *= lighting * NdotL;

                // Add ambient light
                half3 ambient = SampleSH(normalWS) * color;
                color += ambient;

                return half4(color, 1);
            }
            ENDHLSL
        }

        // Shadow casting support
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
/*ASEBEGIN
Version=18800
278;275;1642;744;1176.936;198.0874;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;5;-557.9357,192.9126;Inherit;False;Property;_HeightMultiplier;Height Multiplier;7;0;Create;True;0;0;0;False;0;False;0.5;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-570.5001,-27.2;Inherit;True;Property;_MainTex;MainTex;6;1;[HideInInspector];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-288.9357,133.9126;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-530.936,-123.0874;Inherit;False;Property;_ColorIntensity;Color Intensity;5;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;4;-165.9357,242.9126;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-174.936,-49.0874;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,-25;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Rito/Snowy Ground;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;6;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;1;1
WireConnection;6;1;5;0
WireConnection;4;1;6;0
WireConnection;7;0;8;0
WireConnection;7;1;1;0
WireConnection;0;0;7;0
WireConnection;0;11;4;0
ASEEND*/
//CHKSM=C34C1BBC044F8064EB5B2AC5B4234D1F9844D7B3