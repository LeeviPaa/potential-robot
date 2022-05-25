using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.UI.ColorPalettes
{
    public class ColorPaletteManager
    {
        private Dictionary<ColorPalette, List<Color>> _palettes;
        private static ColorPaletteManager _instance;
        public static ColorPaletteManager Instance => GetInstance();
        private static ColorPaletteManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ColorPaletteManager();
                _instance.Initialize();
            }
            return _instance;
        }

        public delegate void PaletteDelegate(ColorPalette palette);
        public PaletteDelegate OnPaletteChanged;

        private void Initialize()
        {
            _palettes = new Dictionary<ColorPalette, List<Color>>();
            var palettes = Resources.LoadAll<ColorPalette>("");
            foreach(var palette in palettes)
                AddPalette(palette);
        }

        private void AddPalette(ColorPalette palette)
        {
            if (palette == null)
                return;
            _palettes.Add(palette, new List<Color>());
            CopyColors(palette);
        }

        public Color GetColor(ColorPalette palette, int index)
        {
            if (palette == null || !_palettes.ContainsKey(palette) || index < 0 || index >= palette.Colors.Count)
                return Color.magenta;
            return _palettes[palette][index];
        }

        private void CopyColors(ColorPalette palette)
        {
            if (palette == null)
                return;
            _palettes[palette].Clear();
            for (var i = 0; i < palette.Colors.Count; ++i)
            {
                _palettes[palette].Add(palette.Colors[i]);
            }
        }

        public void UpdatePalette(ColorPalette palette)
        {
            CopyColors(palette);
            OnPaletteChanged?.Invoke(palette);
        }
        
    }
}
