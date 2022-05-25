using UnityEngine;
using UnityEngine.UI;

namespace PotentialRobot.UI.Style
{
    [CreateAssetMenu(fileName = "UIStyle_LayoutComponent_.asset", menuName = "UIStyle/Style Asset/LayoutComponent")]
    public class UIStyle_LayoutComponent : Style
    {
        [field: SerializeField]
        public RectOffset Padding { get; private set; }
        [field: SerializeField]
        public float Spacing { get; private set; }
        [field: SerializeField]
        public TextAnchor Alignment { get; private set; }
    }
}
