using UnityEngine;
using UnityEditor;

namespace PotentialRobot.UI.ColorPalettes
{
    [CustomPropertyDrawer(typeof(PaletteColorAttribute))]
    public class PaletteColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var palette = (property.serializedObject.targetObject as IUseColorPalette)?.Palette;
            if (palette == null)
                return;
            PaletteColorAttribute color = attribute as PaletteColorAttribute;
            if (property.propertyType != SerializedPropertyType.Integer)
                return;
            //property.intValue = Mathf.Clamp(EditorGUI.IntField(new Rect(position.x, position.y, position.width -16, position.height), property.intValue), 0, palette.Colors.Count-1);
            if (EditorGUI.DropdownButton(new Rect(position.x + 16, position.y, position.width - 16, position.height), new GUIContent("Color"), FocusType.Passive))
            {
                var dropdown = ColorDropdown.GetDropdown(palette, property, property.intValue);
                var size = dropdown.GetSize();
                var mousePos = Event.current.mousePosition;
                
                dropdown.ShowAsDropDown(GUIUtility.GUIToScreenRect(new Rect(new Vector2(mousePos.x, position.y - size.y + position.height), size)), size);
            }

            var colorToDisplay = property.intValue >= 0 && property.intValue < palette.Colors.Count ? palette.Colors[property.intValue] : Color.magenta;
            EditorGUI.DrawRect(new Rect(position.x, position.y, 16, position.height), colorToDisplay);
            property.serializedObject.ApplyModifiedProperties();
        }

    }
}
