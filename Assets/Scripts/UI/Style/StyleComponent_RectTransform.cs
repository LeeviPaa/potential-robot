using UnityEngine;

namespace PotentialRobot.UI.Style
{
    [RequireComponent(typeof(RectTransform))]
    public class StyleComponent_RectTransform : StyleComponent<RectTransform, UIStyleReference_RectTransform>
    {
        public override void Apply(IStyle style)
        {
            var s = (UIStyle_RectTransform)style;
            if (s.Padding != null)
            {
                _target.offsetMax = new Vector2(s.Padding.Left, s.Padding.Top);
                _target.offsetMin = new Vector2(s.Padding.Right, s.Padding.Bottom);
            }
        }
    }
}
