using UnityEngine;

namespace PotentialRobot.UI.Style
{
    public class UIStyleReference<T> : ScriptableObject, IStyleReference where T : IStyle
    {
        [SerializeField, NaughtyAttributes.Expandable]
        protected T _default;
        public IStyle Default => _default;

        public void OnValidate() => UIStyleManager.Instance.UpdateStyle(_default);
    }
}
