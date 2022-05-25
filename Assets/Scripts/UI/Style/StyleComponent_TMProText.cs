using TMPro;
using UnityEngine;
using PotentialRobot.UI.ColorPalettes;

namespace PotentialRobot.UI.Style
{
    [RequireComponent(typeof(TMP_Text)), ExecuteInEditMode]
    public class StyleComponent_TMProText : StyleComponent<TMP_Text, UIStyleReference_TMProText>
    {
        public override void Apply(IStyle style)
        {
            var s = (UIStyle_TMProText)style;
            _target.font = s.Font;
            _target.fontSize = s.FontSize;
            _target.color = s.FontColor;
        }
    }
}