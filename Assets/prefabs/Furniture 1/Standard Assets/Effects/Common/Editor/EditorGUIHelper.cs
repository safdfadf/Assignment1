using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    public static class EditorGUIHelper
    {
        private static readonly Styles s_Styles;

        static EditorGUIHelper()
        {
            s_Styles = new Styles();
        }

        public static bool Header(SerializedProperty group, SerializedProperty enabledField)
        {
            var display = group == null || group.isExpanded;
            var enabled = enabledField != null && enabledField.boolValue;
            var title = group == null ? "Unknown Group" : ObjectNames.NicifyVariableName(group.displayName);

            var rect = GUILayoutUtility.GetRect(16f, 22f, s_Styles.header);
            GUI.Box(rect, title, s_Styles.header);

            var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
            if (Event.current.type == EventType.Repaint)
                s_Styles.headerCheckbox.Draw(toggleRect, false, false, enabled, false);

            var e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                if (toggleRect.Contains(e.mousePosition) && enabledField != null)
                {
                    enabledField.boolValue = !enabledField.boolValue;
                    e.Use();
                }
                else if (rect.Contains(e.mousePosition) && group != null)
                {
                    display = !display;
                    group.isExpanded = !group.isExpanded;
                    e.Use();
                }
            }

            return display;
        }

        private class Styles
        {
            public readonly GUIStyle header = "ShurikenModuleTitle";
            public readonly GUIStyle headerCheckbox = "ShurikenCheckMark";

            internal Styles()
            {
                header.font = new GUIStyle("Label").font;
                header.border = new RectOffset(15, 7, 4, 4);
                header.fixedHeight = 22;
                header.contentOffset = new Vector2(20f, -2f);
            }
        }
    }
}