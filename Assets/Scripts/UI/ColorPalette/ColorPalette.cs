using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.UI.ColorPalettes
{
    [CreateAssetMenu(menuName = "ColorPalette/Create new Color Palette", fileName = "ColorPalette_name")]
    public class ColorPalette : ScriptableObject, IColorPalette
    {
        [field: SerializeField]
        public string PaletteType { get; private set; }
        [field: SerializeField]
        public List<Color> Colors { get; private set; }

        public void OnValidate()
        {
            ColorPaletteManager.Instance.UpdatePalette(this);
        }
    }
}