using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.UI.Style
{
    [CreateAssetMenu(fileName = "UIStyle_RectTransform_.asset", menuName = "UIStyle/Style Asset/RectTransform")]
    public class UIStyle_RectTransform : ScriptableObject, IStyle
    {
        [field: SerializeField]
        public UIStyle_Padding Padding { get; private set; }
    }
}