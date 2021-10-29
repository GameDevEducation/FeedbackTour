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

            context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
        }
    }
}
