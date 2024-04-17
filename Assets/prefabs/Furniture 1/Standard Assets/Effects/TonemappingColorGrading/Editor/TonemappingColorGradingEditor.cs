using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TonemappingColorGrading))]
    public class TonemappingColorGradingEditor : Editor
    {
        private static readonly GUIContent k_HistogramTitle = new("Histogram");

        private static readonly int[] k_EmptyBuffer = new int[256 << 2];

        // settings group <setting, property reference>
        private readonly Dictionary<FieldInfo, List<SerializedProperty>> m_GroupFields = new();
        private ComputeBuffer m_HistogramBuffer;
        private Material m_HistogramMaterial;

        private HistogramMode m_HistogramMode = HistogramMode.RGB;
        private Rect m_HistogramRect;
        private RenderTexture m_HistogramTexture;

        private TonemappingColorGrading concreteTarget => target as TonemappingColorGrading;

        private static float pixelRatio
        {
            get
            {
#if !(UNITY_3 || UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3)
                return EditorGUIUtility.pixelsPerPoint;
#else
                return 1f;
#endif
            }
        }

        private bool isHistogramSupported =>
            concreteTarget.histogramComputeShader != null
            && ImageEffectHelper.supportsDX11
            && concreteTarget.histogramShader != null
            && concreteTarget.histogramShader.isSupported;

        private void OnEnable()
        {
            var settingsGroups = typeof(TonemappingColorGrading)
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x =>
                    x.GetCustomAttributes(typeof(TonemappingColorGrading.SettingsGroup), false).Any());

            foreach (var settingGroup in settingsGroups)
                PopulateMap(settingGroup);

            concreteTarget.onFrameEndEditorOnly = OnFrameEnd;
        }

        private void OnDisable()
        {
            concreteTarget.onFrameEndEditorOnly = null;

            if (m_HistogramMaterial != null)
                DestroyImmediate(m_HistogramMaterial);

            if (m_HistogramTexture != null)
                DestroyImmediate(m_HistogramTexture);

            if (m_HistogramBuffer != null)
                m_HistogramBuffer.Release();
        }

        private void PopulateMap(FieldInfo group)
        {
            var searchPath = group.Name + ".";
            foreach (var setting in group.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                List<SerializedProperty> settingsGroup;
                if (!m_GroupFields.TryGetValue(group, out settingsGroup))
                {
                    settingsGroup = new List<SerializedProperty>();
                    m_GroupFields[group] = settingsGroup;
                }

                var property = serializedObject.FindProperty(searchPath + setting.Name);
                if (property != null)
                    settingsGroup.Add(property);
            }
        }

        private void SetLUTImportSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.linearTexture = true;
            importer.textureFormat = TextureImporterFormat.RGB24;
            importer.SaveAndReimport();
        }

        private void DrawFields()
        {
            foreach (var group in m_GroupFields)
            {
                var enabledField = group.Value.FirstOrDefault(x => x.propertyPath == group.Key.Name + ".enabled");
                var groupProperty = serializedObject.FindProperty(group.Key.Name);

                GUILayout.Space(5);
                var display = EditorGUIHelper.Header(groupProperty, enabledField);
                if (!display)
                    continue;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(3);
                        foreach (var field in group.Value.Where(x => x.propertyPath != group.Key.Name + ".enabled"))
                        {
                            // Special case for the tonemapping curve field
                            if (group.Key.FieldType == typeof(TonemappingColorGrading.TonemappingSettings) &&
                                field.propertyType == SerializedPropertyType.AnimationCurve &&
                                concreteTarget.tonemapping.tonemapper != TonemappingColorGrading.Tonemapper.Curve)
                                continue;

                            // Special case for the neutral tonemapper
                            var neutralParam = field.name.StartsWith("neutral");

                            if (group.Key.FieldType == typeof(TonemappingColorGrading.TonemappingSettings) &&
                                concreteTarget.tonemapping.tonemapper != TonemappingColorGrading.Tonemapper.Neutral &&
                                neutralParam)
                                continue;

                            if (neutralParam)
                                EditorGUILayout.PropertyField(field,
                                    new GUIContent(ObjectNames.NicifyVariableName(field.name.Substring(7))));
                            else
                                EditorGUILayout.PropertyField(field);
                        }

                        // Bake button
                        if (group.Key.FieldType == typeof(TonemappingColorGrading.ColorGradingSettings))
                        {
                            EditorGUI.BeginDisabledGroup(!enabledField.boolValue);

                            if (GUILayout.Button("Export LUT as PNG", EditorStyles.miniButton))
                            {
                                var path = EditorUtility.SaveFilePanelInProject("Export LUT as PNG", "LUT.png", "png",
                                    "Please enter a file name to save the LUT texture to");

                                if (!string.IsNullOrEmpty(path))
                                {
                                    var lut = concreteTarget.BakeLUT();

                                    if (!concreteTarget.isGammaColorSpace)
                                    {
                                        var pixels = lut.GetPixels();

                                        for (var i = 0; i < pixels.Length; i++)
                                            pixels[i] = pixels[i].linear;

                                        lut.SetPixels(pixels);
                                        lut.Apply();
                                    }

                                    var bytes = lut.EncodeToPNG();
                                    File.WriteAllBytes(path, bytes);
                                    DestroyImmediate(lut);

                                    AssetDatabase.Refresh();
                                    var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                                    SetLUTImportSettings(importer);
                                }
                            }

                            EditorGUI.EndDisabledGroup();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }

        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
                s_Styles = new Styles();

            serializedObject.Update();

            GUILayout.Label("All following effects will use LDR color buffers.", EditorStyles.miniBoldLabel);

            if (concreteTarget.tonemapping.enabled)
            {
                var camera = concreteTarget.GetComponent<Camera>();

                if (camera != null && !camera.allowHDR)
                    EditorGUILayout.HelpBox("The camera is not HDR enabled. This will likely break the tonemapper.",
                        MessageType.Warning);
                else if (!concreteTarget.validRenderTextureFormat)
                    EditorGUILayout.HelpBox(
                        "The input to tonemapper is not in HDR. Make sure that all effects prior to this are executed in HDR.",
                        MessageType.Warning);
            }

            if (concreteTarget.lut.enabled && concreteTarget.lut.texture != null)
            {
                if (!concreteTarget.validUserLutSize)
                {
                    EditorGUILayout.HelpBox("Invalid LUT size. Should be \"height = sqrt(width)\" (e.g. 256x16).",
                        MessageType.Error);
                }
                else
                {
                    // Checks import settings on the lut, offers to fix them if invalid
                    var importer =
                        (TextureImporter)AssetImporter.GetAtPath(
                            AssetDatabase.GetAssetPath(concreteTarget.lut.texture));
                    var valid = importer.anisoLevel == 0
                                && importer.mipmapEnabled == false
                                && importer.linearTexture
                                && (importer.textureFormat == TextureImporterFormat.RGB24 ||
                                    importer.textureFormat == TextureImporterFormat.AutomaticTruecolor);

                    if (!valid)
                    {
                        EditorGUILayout.HelpBox("Invalid LUT import settings.", MessageType.Warning);

                        GUILayout.Space(-32);
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Fix", GUILayout.Width(60)))
                            {
                                SetLUTImportSettings(importer);
                                AssetDatabase.Refresh();
                            }

                            GUILayout.Space(8);
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(11);
                    }
                }
            }

            DrawFields();

            serializedObject.ApplyModifiedProperties();
        }

        public override GUIContent GetPreviewTitle()
        {
            return k_HistogramTitle;
        }

        public override bool HasPreviewGUI()
        {
            return isHistogramSupported && targets.Length == 1 && concreteTarget != null && concreteTarget.enabled;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            serializedObject.Update();

            if (Event.current.type == EventType.Repaint)
            {
                // If m_HistogramRect isn't set the preview was just opened so refresh the render to get the histogram data
                if (m_HistogramRect.width == 0 && m_HistogramRect.height == 0)
                    InternalEditorUtility.RepaintAllViews();

                // Sizing
                var width = Mathf.Min(512f, r.width);
                var height = Mathf.Min(128f, r.height);
                m_HistogramRect = new Rect(
                    Mathf.Floor(r.x + r.width / 2f - width / 2f),
                    Mathf.Floor(r.y + r.height / 2f - height / 2f),
                    width, height
                );

                if (m_HistogramTexture != null)
                    GUI.DrawTexture(m_HistogramRect, m_HistogramTexture);
            }

            // Toolbar
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            {
                concreteTarget.histogramRefreshOnPlay = GUILayout.Toggle(concreteTarget.histogramRefreshOnPlay,
                    new GUIContent("Refresh on Play",
                        "Keep refreshing the histogram in play mode; this may impact performances."),
                    EditorStyles.miniButton);
                GUILayout.FlexibleSpace();
                m_HistogramMode = (HistogramMode)EditorGUILayout.EnumPopup(m_HistogramMode);
            }
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
                InternalEditorUtility.RepaintAllViews();
        }

        private void OnFrameEnd(RenderTexture source)
        {
            if (Application.isPlaying && !concreteTarget.histogramRefreshOnPlay)
                return;

            if (Mathf.Approximately(m_HistogramRect.width, 0) || Mathf.Approximately(m_HistogramRect.height, 0) ||
                !isHistogramSupported)
                return;

            // No need to process the full frame to get an histogram, resize the input to a max-size of 512
            var rw = Mathf.Min(Mathf.Max(source.width, source.height), 512);
            var rt = RenderTexture.GetTemporary(rw, rw, 0);
            Graphics.Blit(source, rt);
            UpdateHistogram(rt, m_HistogramRect, m_HistogramMode);
            Repaint();
            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = null;
        }

        private void UpdateHistogram(RenderTexture source, Rect rect, HistogramMode mode)
        {
            if (m_HistogramMaterial == null)
                m_HistogramMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(concreteTarget.histogramShader);

            if (m_HistogramBuffer == null)
                m_HistogramBuffer = new ComputeBuffer(256, sizeof(uint) << 2);

            m_HistogramBuffer.SetData(k_EmptyBuffer);

            var cs = concreteTarget.histogramComputeShader;

            var kernel = cs.FindKernel("KHistogramGather");
            cs.SetBuffer(kernel, "_Histogram", m_HistogramBuffer);
            cs.SetTexture(kernel, "_Source", source);

            int[] channels = null;
            switch (mode)
            {
                case HistogramMode.Luminance:
                    channels = new[] { 0, 0, 0, 1 };
                    break;
                case HistogramMode.RGB:
                    channels = new[] { 1, 1, 1, 0 };
                    break;
                case HistogramMode.Red:
                    channels = new[] { 1, 0, 0, 0 };
                    break;
                case HistogramMode.Green:
                    channels = new[] { 0, 1, 0, 0 };
                    break;
                case HistogramMode.Blue:
                    channels = new[] { 0, 0, 1, 0 };
                    break;
            }

            cs.SetInts("_Channels", channels);
            cs.SetInt("_IsLinear", concreteTarget.isGammaColorSpace ? 0 : 1);
            cs.Dispatch(kernel, Mathf.CeilToInt(source.width / 32f), Mathf.CeilToInt(source.height / 32f), 1);

            kernel = cs.FindKernel("KHistogramScale");
            cs.SetBuffer(kernel, "_Histogram", m_HistogramBuffer);
            cs.SetFloat("_Height", rect.height);
            cs.Dispatch(kernel, 1, 1, 1);

            if (m_HistogramTexture == null)
            {
                DestroyImmediate(m_HistogramTexture);
                m_HistogramTexture =
                    new RenderTexture((int)rect.width, (int)rect.height, 0, RenderTextureFormat.ARGB32);
                m_HistogramTexture.hideFlags = HideFlags.HideAndDontSave;
            }

            m_HistogramMaterial.SetBuffer("_Histogram", m_HistogramBuffer);
            m_HistogramMaterial.SetVector("_Size", new Vector2(m_HistogramTexture.width, m_HistogramTexture.height));
            m_HistogramMaterial.SetColor("_ColorR", redCurveColor);
            m_HistogramMaterial.SetColor("_ColorG", greenCurveColor);
            m_HistogramMaterial.SetColor("_ColorB", blueCurveColor);
            m_HistogramMaterial.SetColor("_ColorL", masterCurveColor);
            m_HistogramMaterial.SetInt("_Channel", (int)mode);
            Graphics.Blit(m_HistogramTexture, m_HistogramTexture, m_HistogramMaterial,
                mode == HistogramMode.RGB ? 1 : 0);
        }

        private enum HistogramMode
        {
            Red = 0,
            Green = 1,
            Blue = 2,
            Luminance = 3,
            RGB
        }

        public static class ColorWheel
        {
            // Constants
            private const float PI_2 = Mathf.PI / 2f;
            private const float PI2 = Mathf.PI * 2f;

            // hue Wheel
            private static Texture2D s_WheelTexture;
            private static float s_LastDiameter;
            private static GUIStyle s_CenteredStyle;

            private static readonly int k_ThumbHash = "colorWheelThumb".GetHashCode();

            public static Color DoGUI(Rect area, string title, Color color, float diameter)
            {
                var labelrect = area;
                labelrect.height = EditorGUIUtility.singleLineHeight;

                if (s_CenteredStyle == null)
                    s_CenteredStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
                    {
                        alignment = TextAnchor.UpperCenter
                    };

                GUI.Label(labelrect, title, s_CenteredStyle);

                // Figure out the wheel draw area
                var wheelDrawArea = area;
                wheelDrawArea.y += EditorGUIUtility.singleLineHeight;
                wheelDrawArea.height = diameter;

                if (wheelDrawArea.width > wheelDrawArea.height)
                {
                    wheelDrawArea.x += (wheelDrawArea.width - wheelDrawArea.height) / 2.0f;
                    wheelDrawArea.width = area.height;
                }

                wheelDrawArea.width = wheelDrawArea.height;

                var radius = diameter / 2.0f;
                Vector3 hsv;
                Color.RGBToHSV(color, out hsv.x, out hsv.y, out hsv.z);

                // Retina/HDPI screens handling
                wheelDrawArea.width /= pixelRatio;
                wheelDrawArea.height /= pixelRatio;
                var scaledRadius = radius / pixelRatio;

                if (Event.current.type == EventType.Repaint)
                {
                    if (!Mathf.Approximately(diameter, s_LastDiameter))
                    {
                        s_LastDiameter = diameter;
                        UpdateHueWheel((int)diameter);
                    }

                    // Wheel
                    GUI.DrawTexture(wheelDrawArea, s_WheelTexture);

                    // Thumb
                    var thumbPos = Vector2.zero;
                    var theta = hsv.x * PI2;
                    var len = hsv.y * scaledRadius;
                    thumbPos.x = Mathf.Cos(theta + PI_2);
                    thumbPos.y = Mathf.Sin(theta - PI_2);
                    thumbPos *= len;
                    var thumbSize = s_Styles.thumb2DSize;
                    var oldColor = GUI.color;
                    GUI.color = Color.black;
                    var thumbSizeH = thumbSize / 2f;
                    Handles.color = Color.white;
                    Handles.DrawAAPolyLine(
                        new Vector2(wheelDrawArea.x + scaledRadius + thumbSizeH.x,
                            wheelDrawArea.y + scaledRadius + thumbSizeH.y),
                        new Vector2(wheelDrawArea.x + scaledRadius + thumbPos.x,
                            wheelDrawArea.y + scaledRadius + thumbPos.y));
                    s_Styles.thumb2D.Draw(
                        new Rect(wheelDrawArea.x + scaledRadius + thumbPos.x - thumbSizeH.x,
                            wheelDrawArea.y + scaledRadius + thumbPos.y - thumbSizeH.y, thumbSize.x, thumbSize.y),
                        false, false, false, false);
                    GUI.color = oldColor;
                }

                hsv = GetInput(wheelDrawArea, hsv, scaledRadius);

                var sliderDrawArea = wheelDrawArea;
                sliderDrawArea.y = sliderDrawArea.yMax;
                sliderDrawArea.height = EditorGUIUtility.singleLineHeight;

                hsv.y = GUI.HorizontalSlider(sliderDrawArea, hsv.y, 1e-04f, 1f);
                color = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
                return color;
            }

            private static Vector3 GetInput(Rect bounds, Vector3 hsv, float radius)
            {
                var e = Event.current;
                var id = GUIUtility.GetControlID(k_ThumbHash, FocusType.Passive, bounds);

                var mousePos = e.mousePosition;
                var relativePos = mousePos - new Vector2(bounds.x, bounds.y);

                if (e.type == EventType.MouseDown && e.button == 0 && GUIUtility.hotControl == 0)
                {
                    if (bounds.Contains(mousePos))
                    {
                        var center = new Vector2(bounds.x + radius, bounds.y + radius);
                        var dist = Vector2.Distance(center, mousePos);

                        if (dist <= radius)
                        {
                            e.Use();
                            GetWheelHueSaturation(relativePos.x, relativePos.y, radius, out hsv.x, out hsv.y);
                            GUIUtility.hotControl = id;
                        }
                    }
                }
                else if (e.type == EventType.MouseDrag && e.button == 0 && GUIUtility.hotControl == id)
                {
                    var center = new Vector2(bounds.x + radius, bounds.y + radius);
                    var dist = Vector2.Distance(center, mousePos);

                    if (dist <= radius)
                    {
                        e.Use();
                        GetWheelHueSaturation(relativePos.x, relativePos.y, radius, out hsv.x, out hsv.y);
                    }
                }
                else if (e.type == EventType.MouseUp && e.button == 0 && GUIUtility.hotControl == id)
                {
                    e.Use();
                    GUIUtility.hotControl = 0;
                }

                return hsv;
            }

            private static void GetWheelHueSaturation(float x, float y, float radius, out float hue,
                out float saturation)
            {
                var dx = (x - radius) / radius;
                var dy = (y - radius) / radius;
                var d = Mathf.Sqrt(dx * dx + dy * dy);
                hue = Mathf.Atan2(dx, -dy);
                hue = 1f - (hue > 0 ? hue : PI2 + hue) / PI2;
                saturation = Mathf.Clamp01(d);
            }

            private static void UpdateHueWheel(int diameter)
            {
                CleanTexture(s_WheelTexture);
                s_WheelTexture = MakeTexture(diameter);

                var radius = diameter / 2.0f;

                var pixels = s_WheelTexture.GetPixels();

                for (var y = 0; y < diameter; y++)
                for (var x = 0; x < diameter; x++)
                {
                    var index = y * diameter + x;
                    var dx = (x - radius) / radius;
                    var dy = (y - radius) / radius;
                    var d = Mathf.Sqrt(dx * dx + dy * dy);

                    // Out of the wheel, early exit
                    if (d >= 1f)
                    {
                        pixels[index] = new Color(0f, 0f, 0f, 0f);
                        continue;
                    }

                    // red (0) on top, counter-clockwise (industry standard)
                    var saturation = d;
                    var hue = Mathf.Atan2(dx, dy);
                    hue = 1f - (hue > 0 ? hue : PI2 + hue) / PI2;
                    var color = Color.HSVToRGB(hue, saturation, 1f);

                    // Quick & dirty antialiasing
                    color.a = saturation > 0.99 ? (1f - saturation) * 100f : 1f;

                    pixels[index] = color;
                }

                s_WheelTexture.SetPixels(pixels);
                s_WheelTexture.Apply();
            }

            private static Texture2D MakeTexture(int dimension)
            {
                return new Texture2D(dimension, dimension, TextureFormat.ARGB32, false, true)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.HideAndDontSave,
                    alphaIsTransparency = true
                };
            }

            private static void CleanTexture(Texture2D texture)
            {
                if (texture != null)
                    DestroyImmediate(texture);
            }

            public static float GetColorWheelHeight(int renderSizePerWheel)
            {
                // wheel height + title label + alpha slider
                return renderSizePerWheel + 2 * EditorGUIUtility.singleLineHeight;
            }
        }

        #region Property drawers

        [CustomPropertyDrawer(typeof(TonemappingColorGrading.ColorWheelGroup))]
        private class ColorWheelGroupDrawer : PropertyDrawer
        {
            private int m_NumberOfWheels;
            private int m_RenderSizePerWheel;

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var wheelAttribute = (TonemappingColorGrading.ColorWheelGroup)attribute;
                property.isExpanded = true;

                m_NumberOfWheels = property.CountInProperty() - 1;
                if (m_NumberOfWheels == 0)
                    return 0f;

                m_RenderSizePerWheel = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / m_NumberOfWheels) - 30;
                m_RenderSizePerWheel = Mathf.Clamp(m_RenderSizePerWheel, wheelAttribute.minSizePerWheel,
                    wheelAttribute.maxSizePerWheel);
                m_RenderSizePerWheel = Mathf.FloorToInt(pixelRatio * m_RenderSizePerWheel);
                return ColorWheel.GetColorWheelHeight(m_RenderSizePerWheel);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (m_NumberOfWheels == 0)
                    return;

                var width = position.width;
                var newPosition = new Rect(position.x, position.y, width / m_NumberOfWheels, position.height);

                foreach (SerializedProperty prop in property)
                {
                    if (prop.propertyType == SerializedPropertyType.Color)
                        prop.colorValue = ColorWheel.DoGUI(newPosition, prop.displayName, prop.colorValue,
                            m_RenderSizePerWheel);

                    newPosition.x += width / m_NumberOfWheels;
                }
            }
        }

        [CustomPropertyDrawer(typeof(TonemappingColorGrading.IndentedGroup))]
        private class IndentedGroupDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return 0f;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                foreach (SerializedProperty prop in property)
                    EditorGUILayout.PropertyField(prop);

                EditorGUI.indentLevel--;
            }
        }

        [CustomPropertyDrawer(typeof(TonemappingColorGrading.ChannelMixer))]
        private class ChannelMixerDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return 0f;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // TODO: Hardcoded variable names, rewrite this function
                if (property.type != "ChannelMixerSettings")
                    return;

                var currentChannel = property.FindPropertyRelative("currentChannel");
                var intCurrentChannel = currentChannel.intValue;

                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Channel");
                    if (GUILayout.Toggle(intCurrentChannel == 0, "Red", EditorStyles.miniButtonLeft))
                        intCurrentChannel = 0;
                    if (GUILayout.Toggle(intCurrentChannel == 1, "Green", EditorStyles.miniButtonMid))
                        intCurrentChannel = 1;
                    if (GUILayout.Toggle(intCurrentChannel == 2, "Blue", EditorStyles.miniButtonRight))
                        intCurrentChannel = 2;
                }
                EditorGUILayout.EndHorizontal();

                var serializedChannel =
                    property.FindPropertyRelative("channels").GetArrayElementAtIndex(intCurrentChannel);
                currentChannel.intValue = intCurrentChannel;

                var v = serializedChannel.vector3Value;
                v.x = EditorGUILayout.Slider("Red", v.x, -2f, 2f);
                v.y = EditorGUILayout.Slider("Green", v.y, -2f, 2f);
                v.z = EditorGUILayout.Slider("Blue", v.z, -2f, 2f);
                serializedChannel.vector3Value = v;

                EditorGUI.indentLevel--;
            }
        }

        [CustomPropertyDrawer(typeof(TonemappingColorGrading.Curve))]
        private class CurveDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var attribute = (TonemappingColorGrading.Curve)this.attribute;

                if (property.propertyType != SerializedPropertyType.AnimationCurve)
                {
                    EditorGUI.LabelField(position, label.text, "Use ClampCurve with an AnimationCurve.");
                    return;
                }

                property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue,
                    attribute.color, new Rect(0f, 0f, 1f, 1f));
            }
        }

        #endregion

        #region Styling

        private static Styles s_Styles;

        private class Styles
        {
            public readonly GUIStyle thumb2D = "ColorPicker2DThumb";
            public readonly Vector2 thumb2DSize;

            internal Styles()
            {
                thumb2DSize = new Vector2(
                    !Mathf.Approximately(thumb2D.fixedWidth, 0f) ? thumb2D.fixedWidth : thumb2D.padding.horizontal,
                    !Mathf.Approximately(thumb2D.fixedHeight, 0f) ? thumb2D.fixedHeight : thumb2D.padding.vertical
                );
            }
        }

        public static readonly Color masterCurveColor = new(1f, 1f, 1f, 2f);
        public static readonly Color redCurveColor = new(1f, 0f, 0f, 2f);
        public static readonly Color greenCurveColor = new(0f, 1f, 0f, 2f);
        public static readonly Color blueCurveColor = new(0f, 1f, 1f, 2f);

        #endregion
    }
}