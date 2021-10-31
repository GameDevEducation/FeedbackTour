Shader "Custom/InteractableObject"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _HighlightPeriod ("Highlight Period", float) = 1.5
        _HighlightColour ("Highlight Colour", Color) = (1, 1, 1, 1)
        _HighlightIntensity ("Highlight Intensity", Range(0, 1)) = 0.5

        _DissolveProgress ("Dissolve Progress", Range(0, 1)) = 0
        _DissolveEdgeColour ("Dissolve Edge Colour", Color) = (1, 1, 1, 1)
        _DissolveEdgeDepth ("Dissolve Edge Depth", Range(0, 1)) = 0
        _DissolveTexture ("Dissolve Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        half _HighlightPeriod;
        half _HighlightIntensity;
        fixed4 _HighlightColour;

        half _DissolveProgress;
        fixed4 _DissolveEdgeColour;
        half _DissolveEdgeDepth;
        sampler2D _DissolveTexture;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // sample the dissolve texture
            half dissolveThreshold = tex2D(_DissolveTexture, IN.uv_MainTex);
            half dissolveFactor = dissolveThreshold - _DissolveProgress;

            // clip if below 0
            clip(dissolveFactor);

            // highlight the edge region
            o.Albedo = lerp(_DissolveEdgeColour, o.Albedo, step(_DissolveEdgeDepth, dissolveFactor));

            // THIS is equivalent to the line above
            // if (dissolveFactor < _DissolveEdgeDepth)
            //     o.Albedo = _DissolveEdgeColour;

            // calculate the highlight intensity
            half workingIntensity = _HighlightIntensity * 0.5 * (1 + sin(_Time.y * 3.14159 / _HighlightPeriod));

            // apply the highlight
            o.Emission = lerp(_DissolveEdgeColour, o.Albedo * _HighlightColour * workingIntensity, step(_DissolveEdgeDepth, dissolveFactor));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
