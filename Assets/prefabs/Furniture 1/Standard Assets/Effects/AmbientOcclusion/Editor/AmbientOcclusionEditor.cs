using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AmbientOcclusion))]
    public class AmbientOcclusionEditor : Editor
    {
        private static readonly GUIContent _textValue = new("Value");

        private static readonly string _textNoGBuffer =
            "G-buffer is currently unavailable. " +
            "Change Renderring Path in camera settings to Deferred.";

        private static readonly string _textNoAmbientOnly =
            "The ambient-only mode is currently disabled; " +
            "it requires G-buffer source and HDR rendering.";

        private static readonly string _textGBufferNote =
            "Forward opaque objects don't go in the G-buffer. " +
            "This may lead to artifacts.";

        private SerializedProperty _ambientOnly;
        private SerializedProperty _debug;
        private SerializedProperty _downsampling;
        private SerializedProperty _intensity;
        private SerializedProperty _occlusionSource;
        private SerializedProperty _radius;
        private SerializedProperty _sampleCount;
        private SerializedProperty _sampleCountValue;

        private void OnEnable()
        {
            _intensity = serializedObject.FindProperty("settings.intensity");
            _radius = serializedObject.FindProperty("settings.radius");
            _sampleCount = serializedObject.FindProperty("settings.sampleCount");
            _sampleCountValue = serializedObject.FindProperty("settings.sampleCountValue");
            _downsampling = serializedObject.FindProperty("settings.downsampling");
            _occlusionSource = serializedObject.FindProperty("settings.occlusionSource");
            _ambientOnly = serializedObject.FindProperty("settings.ambientOnly");
            _debug = serializedObject.FindProperty("settings.debug");
        }

        public override void OnInspectorGUI()
        {
            var targetInstance = (AmbientOcclusion)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(_intensity);
            EditorGUILayout.PropertyField(_radius);
            EditorGUILayout.PropertyField(_sampleCount);

            if (_sampleCount.hasMultipleDifferentValues ||
                _sampleCount.enumValueIndex == (int)AmbientOcclusion.SampleCount.Variable)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_sampleCountValue, _textValue);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_downsampling);
            EditorGUILayout.PropertyField(_occlusionSource);

            if (!_occlusionSource.hasMultipleDifferentValues &&
                _occlusionSource.enumValueIndex == (int)AmbientOcclusion.OcclusionSource.GBuffer)
            {
                if (!targetInstance.isGBufferAvailable)
                    EditorGUILayout.HelpBox(_textNoGBuffer, MessageType.Warning);
                else if (!_ambientOnly.hasMultipleDifferentValues && !_ambientOnly.boolValue)
                    EditorGUILayout.HelpBox(_textGBufferNote, MessageType.Info);
            }

            EditorGUILayout.PropertyField(_ambientOnly);

            if (!_ambientOnly.hasMultipleDifferentValues &&
                _ambientOnly.boolValue &&
                !targetInstance.isAmbientOnlySupported)
                EditorGUILayout.HelpBox(_textNoAmbientOnly, MessageType.Warning);

            EditorGUILayout.PropertyField(_debug);

            serializedObject.ApplyModifiedProperties();
        }
    }
}