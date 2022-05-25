using PotentialRobot.UI.ColorPalettes;
using TMPro;
using UnityEngine;

namespace PotentialRobot.UI.Style
{
    [CreateAssetMenu(menuName = "UIStyle/Style/TMPro Text", fileName = "UIStyle_TMProText_.asset")]
    public class UIStyle_TMProText : Style, IUseColorPalette
    {
        [field: SerializeField]
        public TMP_FontAsset Font { get; private set; }
        [field: SerializeField]
        public float FontSize { get; private set; } = 36f;
        [field: SerializeField]
        public ColorPalette Palette { get; private set; }
        [SerializeField, PaletteColor]
        private int _fontColor = 0;
        //TODO: Cache color so we don't have to get it always from the palette manager. Only update it when it's actually relevant
        public Color FontColor => ColorPaletteManager.Instance.GetColor(Palette, _fontColor);
    }
}
