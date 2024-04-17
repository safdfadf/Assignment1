using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Rendering/Global Fog")]
    internal class GlobalFog : PostEffectsBase
    {
        [Tooltip("Apply distance-based fog?")] public bool distanceFog = true;

        [Tooltip("Exclude far plane pixels from distance-based fog? (Skybox or clear color)")]
        public bool excludeFarPixels = true;

        [Tooltip("Distance fog is based on radial distance from camera when checked")]
        public bool useRadialDistance;

        [Tooltip("Apply height-based fog?")] public bool heightFog = true;

        [Tooltip("Fog top Y coordinate")] public float height = 1.0f;

        [Range(0.001f, 10.0f)] public float heightDensity = 2.0f;

        [Tooltip("Push fog away from the camera by this amount")]
        public float startDistance;

        public Shader fogShader;
        private Material fogMaterial;

        [ImageEffectOpaque]
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false || (!distanceFog && !heightFog))
            {
                Graphics.Blit(source, destination);
                return;
            }

            var cam = GetComponent<Camera>();
            var camtr = cam.transform;
            var camNear = cam.nearClipPlane;
            var camFar = cam.farClipPlane;
            var camFov = cam.fieldOfView;
            var camAspect = cam.aspect;

            var frustumCorners = Matrix4x4.identity;

            var fovWHalf = camFov * 0.5f;

            var toRight = camtr.right * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
            var toTop = camtr.up * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            var topLeft = camtr.forward * camNear - toRight + toTop;
            var camScale = topLeft.magnitude * camFar / camNear;

            topLeft.Normalize();
            topLeft *= camScale;

            var topRight = camtr.forward * camNear + toRight + toTop;
            topRight.Normalize();
            topRight *= camScale;

            var bottomRight = camtr.forward * camNear + toRight - toTop;
            bottomRight.Normalize();
            bottomRight *= camScale;

            var bottomLeft = camtr.forward * camNear - toRight - toTop;
            bottomLeft.Normalize();
            bottomLeft *= camScale;

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);

            var camPos = camtr.position;
            var FdotC = camPos.y - height;
            var paramK = FdotC <= 0.0f ? 1.0f : 0.0f;
            var excludeDepth = excludeFarPixels ? 1.0f : 2.0f;
            fogMaterial.SetMatrix("_FrustumCornersWS", frustumCorners);
            fogMaterial.SetVector("_CameraWS", camPos);
            fogMaterial.SetVector("_HeightParams", new Vector4(height, FdotC, paramK, heightDensity * 0.5f));
            fogMaterial.SetVector("_DistanceParams", new Vector4(-Mathf.Max(startDistance, 0.0f), excludeDepth, 0, 0));

            var sceneMode = RenderSettings.fogMode;
            var sceneDensity = RenderSettings.fogDensity;
            var sceneStart = RenderSettings.fogStartDistance;
            var sceneEnd = RenderSettings.fogEndDistance;
            Vector4 sceneParams;
            var linear = sceneMode == FogMode.Linear;
            var diff = linear ? sceneEnd - sceneStart : 0.0f;
            var invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
            sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
            sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
            sceneParams.z = linear ? -invDiff : 0.0f;
            sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;
            fogMaterial.SetVector("_SceneFogParams", sceneParams);
            fogMaterial.SetVector("_SceneFogMode", new Vector4((int)sceneMode, useRadialDistance ? 1 : 0, 0, 0));

            var pass = 0;
            if (distanceFog && heightFog)
                pass = 0; // distance + height
            else if (distanceFog)
                pass = 1; // distance only
            else
                pass = 2; // height only
            CustomGraphicsBlit(source, destination, fogMaterial, pass);
        }


        public override bool CheckResources()
        {
            CheckSupport(true);

            fogMaterial = CheckShaderAndCreateMaterial(fogShader, fogMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial,
            int passNr)
        {
            RenderTexture.active = dest;

            fxMaterial.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho();

            fxMaterial.SetPass(passNr);

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

            GL.End();
            GL.PopMatrix();
        }
    }
}