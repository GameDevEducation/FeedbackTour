Shader "Hidden/PostProcessing/DamageOverlay"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_OverlayTexture, sampler_OverlayTexture);

    half3 _DamageColour;
    half _Intensity;
    half _Smoothness;
    half _Roundness;
    half _Opacity;

    float4 DamageOverlayFrag(VaryingsDefault i) : SV_TARGET
    {
        float3 pixelColour = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb;
        half overlayIntensity = SAMPLE_TEXTURE2D(_OverlayTexture, sampler_OverlayTexture, i.texcoord).r;

        // get the scaled vector from the centre
        half2 vecFromCentre = abs(i.texcoord.xy - half2(0.5, 0.5)) * _Intensity;

        // apply the roundness
        vecFromCentre = pow(saturate(vecFromCentre), _Roundness);

        // calculate the distance factor
        half distFactor = saturate(1 - dot(vecFromCentre, vecFromCentre));

        // apply the smoothness
        distFactor = pow(distFactor, _Smoothness);

        // calculate the tinted pixel colour
        half3 tintedPixelColour = lerp(pixelColour, _DamageColour, 1 - distFactor);

        // apply opacity
        half3 finalPixelColour = lerp(pixelColour, tintedPixelColour, _Opacity * overlayIntensity);

        return float4(finalPixelColour, 1);
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