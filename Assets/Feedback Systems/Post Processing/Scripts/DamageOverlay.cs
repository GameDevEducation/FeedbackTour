using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DamageOverlayEffect
{
    [System.Serializable]
    [PostProcess(typeof(DamageOverlayRenderer), PostProcessEvent.AfterStack, "Damage Overlay")]
    public class DamageOverlay : PostProcessEffectSettings
    {
        [Range(0f, 1f)] public FloatParameter Intensity = new FloatParameter { value = 0f };
        public ColorParameter DamageColour = new ColorParameter { value = Color.red };
        public TextureParameter OverlayTexture = new TextureParameter { value = null };

        [Range(0f, 1f)] public FloatParameter Roundness = new FloatParameter { value = 1f };
        [Range(0f, 1f)] public FloatParameter Smoothness = new FloatParameter { value = 0.5f };
        [Range(0f, 1f)] public FloatParameter Opacity = new FloatParameter { value = 0.5f };
    }

    public sealed class DamageOverlayRenderer : PostProcessEffectRenderer<DamageOverlay>
    {
        const string ShaderPath = "Hidden/PostProcessing/DamageOverlay";

        public override void Render(PostProcessRenderContext context)
        {
            // attempt to retrieve the shader
            var shader = Shader.Find(ShaderPath);
            if (shader == null)
            {
                Debug.LogError("Failed to find shader: " + ShaderPath);
                return;
            }

            // attempt to retrieve the property sheet
            var propertySheet = context.propertySheets.Get(shader);
            if (propertySheet == null)
            {
                Debug.LogError("Failed to retrieve property sheet for shader: " + ShaderPath);
                return;
            }

            if (settings.OverlayTexture.value != null)
                propertySheet.properties.SetTexture("_OverlayTexture", settings.OverlayTexture.value);
            propertySheet.properties.SetColor("_DamageColour", settings.DamageColour);
            propertySheet.properties.SetFloat("_Intensity", Mathf.Lerp(0f, 5f, settings.Intensity.value));
            propertySheet.properties.SetFloat("_Smoothness", Mathf.Lerp(0.1f, 5f, settings.Smoothness));
            propertySheet.properties.SetFloat("_Roundness", Mathf.Lerp(5f, 1f, settings.Roundness.value));
            propertySheet.properties.SetFloat("_Opacity", settings.Opacity);

            context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
        }
    }
}
