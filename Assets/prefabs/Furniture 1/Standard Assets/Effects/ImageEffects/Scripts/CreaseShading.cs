using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Edge Detection/Crease Shading")]
    public class CreaseShading : PostEffectsBase
    {
        public float intensity = 0.5f;
        public int softness = 1;
        public float spread = 1.0f;

        public Shader blurShader;

        public Shader depthFetchShader;

        public Shader creaseApplyShader;
        private Material blurMaterial;
        private Material creaseApplyMaterial;
        private Material depthFetchMaterial;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            var rtW = source.width;
            var rtH = source.height;

            var widthOverHeight = 1.0f * rtW / (1.0f * rtH);
            var oneOverBaseSize = 1.0f / 512.0f;

            var hrTex = RenderTexture.GetTemporary(rtW, rtH, 0);
            var lrTex1 = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);

            Graphics.Blit(source, hrTex, depthFetchMaterial);
            Graphics.Blit(hrTex, lrTex1);

            for (var i = 0; i < softness; i++)
            {
                var lrTex2 = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);
                blurMaterial.SetVector("offsets", new Vector4(0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
                Graphics.Blit(lrTex1, lrTex2, blurMaterial);
                RenderTexture.ReleaseTemporary(lrTex1);
                lrTex1 = lrTex2;

                lrTex2 = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);
                blurMaterial.SetVector("offsets",
                    new Vector4(spread * oneOverBaseSize / widthOverHeight, 0.0f, 0.0f, 0.0f));
                Graphics.Blit(lrTex1, lrTex2, blurMaterial);
                RenderTexture.ReleaseTemporary(lrTex1);
                lrTex1 = lrTex2;
            }

            creaseApplyMaterial.SetTexture("_HrDepthTex", hrTex);
            creaseApplyMaterial.SetTexture("_LrDepthTex", lrTex1);
            creaseApplyMaterial.SetFloat("intensity", intensity);
            Graphics.Blit(source, destination, creaseApplyMaterial);

            RenderTexture.ReleaseTemporary(hrTex);
            RenderTexture.ReleaseTemporary(lrTex1);
        }


        public override bool CheckResources()
        {
            CheckSupport(true);

            blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
            depthFetchMaterial = CheckShaderAndCreateMaterial(depthFetchShader, depthFetchMaterial);
            creaseApplyMaterial = CheckShaderAndCreateMaterial(creaseApplyShader, creaseApplyMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }
    }
}