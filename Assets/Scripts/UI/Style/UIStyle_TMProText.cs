using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PotentialRobot.UI.Style
{
    [CreateAssetMenu(menuName = "UIStyle/Style/TMPro Text", fileName = "UIStyle_TMProText_.asset")]
    public class UIStyle_TMProText : Style
    {
        [field: SerializeField]
        public TMP_FontAsset Font { get; private set; }
        [field: SerializeField]
        public float FontSize { get; private set; } = 36f;
        [field: SerializeField]
        public Color FontColor { get; private set; }
    }
}