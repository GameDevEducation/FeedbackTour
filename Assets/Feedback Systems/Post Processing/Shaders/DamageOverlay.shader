Shader "Hidden/PostProcessing/DamageOverlay"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float4 DamageOverlayFrag(VaryingsDefault i) : SV_TARGET
    {
        float3 pixelColour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb;

        return float4(pixelColour, 1);
    }

    ENDHLSL

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment DamageOverlayFrag

            ENDHLSL
        }
    }
}