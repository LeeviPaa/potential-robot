using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace PotentialRobot.UI.Style
{
   [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup)), ExecuteInEditMode] 
    public class StyleComponent_LayoutComponent : StyleComponent<LayoutGroup, UIStyleReference_LayoutComponent>
    {
        public override void Apply(IStyle style)
        {
            var s = (UIStyle_LayoutComponent)style;
            _target.padding = s.Padding;
            _target.childAlignment = s.Alignment;
            (_target as HorizontalOrVerticalLayoutGroup).spacing = s.Spacing;
            
            //Nice to have: If no change to padding, don't mark layout for rebuild.
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }
        
    }
}