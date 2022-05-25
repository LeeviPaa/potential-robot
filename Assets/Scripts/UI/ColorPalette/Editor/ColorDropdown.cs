using System;
using UnityEditor;
using UnityEngine;

namespace PotentialRobot.UI.ColorPalettes
{
    public class ColorDropdown : EditorWindow
    {
        private const int c_colorSize = 16;
        private const int c_colorsPerRow = 8;
        private const int c_buttonHeight = 16;
        private const int c_spacing = 8;
        public ColorPalette Palette;
        public int CurrentSelection;
        public SerializedProperty Property;
        public Action<SerializedProperty, int> Callback;
        public static ColorDropdown GetDropdown(ColorPalette palette, SerializedProperty property, int currentSelection)
        {
            var dropdown = EditorWindow.CreateInstance<ColorDropdown>();
            dropdown.Palette = palette;
            dropdown.Property = property;
            dropdown.CurrentSelection = currentSelection;
            return dropdown;
        }

        public Vector2 GetSize()
        {
            int height = Mathf.CeilToInt(Palette.Colors.Count / (float)c_colorsPerRow) * c_colorSize + c_spacing + c_buttonHeight;
            return new Vector2(c_colorsPerRow * c_colorSize, height);
        }

        void OnGUI()
        {
            var size = new Vector2(c_colorSize, c_colorSize);
            for (var i = 0; i < Palette.Colors.Count; ++i)
            {
                var pos = new Vector2((int)Mathf.Repeat(i, c_colorsPerRow) * c_colorSize, Mathf.FloorToInt(i / c_colorsPerRow) * c_colorSize);
                if (i == CurrentSelection)
                    EditorGUI.DrawRect(new Rect(pos, new Vector2(c_colorSize, c_colorSize)), Color.black);
                EditorGUI.DrawRect(new Rect(pos.x+4, pos.y+4, c_colorSize-8, c_colorSize-8), Palette.Colors[i]);
            }
            var windowSize = GetSize();
            CheckClick(windowSize);
            EditButton(windowSize);
        }

        public void CheckClick(Vector2 size)
        {
            var clickArea = new Rect(position.x, position.y, size.x, size.y - c_spacing - c_buttonHeight);
            if (clickArea.Contains(Event.current.mousePosition)
                || Event.current.type != EventType.MouseDown)
                return;
            int index = GetIndex(Event.current.mousePosition);
            if (index >= 0 && index < Palette.Colors.Count)
            {
                Property.intValue = index;
                Property.serializedObject?.ApplyModifiedProperties();
                Close();
            }
        }

        public void EditButton(Vector2 size)
        {
            var buttonRect = new Rect(0, size.y - c_buttonHeight, size.x, c_buttonHeight);
            if (GUI.Button(buttonRect, new GUIContent("Edit Palette")))
            {
                Selection.activeObject = Palette;
                Close();
            }
        }

        public int GetIndex(Vector2 position)
        {
            int x =  Mathf.FloorToInt(position.x / c_colorSize);
            int y =  Mathf.FloorToInt(position.y / c_colorSize);
            return x + c_colorsPerRow * y;
        }
    }
}
