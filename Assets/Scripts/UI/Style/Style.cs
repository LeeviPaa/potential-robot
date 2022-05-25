using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.UI.Style
{
    public class Style : ScriptableObject, IStyle
    {
        public virtual void OnValidate() => UIStyleManager.Instance.UpdateStyle(this);
    }
}
