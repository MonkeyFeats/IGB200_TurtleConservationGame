Shader "Custom/URPVertexGradientLighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 color : COLOR; // Vertex color
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 vertexColor : COLOR; // Pass vertex color to fragment shader
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Convert object space position to homogenous clip space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);

                // Pass the vertex color to the fragment shader
                OUT.vertexColor = IN.color;

                return OUT;
            }

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            half4 frag(Varyings IN) : SV_Target
            {
                // Use vertex color to fake lighting
                half4 color = IN.vertexColor;

                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
