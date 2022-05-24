using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.UI.Style
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class StyleComponent<T, T2> : MonoBehaviour, IStyleComponent where T : Component where T2 : IStyleReference
    {
        [SerializeField]
        protected T _target;
        [SerializeField, NaughtyAttributes.Expandable]
        protected T2 _reference;
        public RectTransform RectTransform;
        public IStyleReference Reference => _reference;

        public void OnEnable() => Subscribe(this);

        public void OnDisable() => Unsubscribe(this);

        public abstract void Apply(IStyle style);

        public void Subscribe(IStyleComponent component)
        => UIStyleManager.Instance.Subscribe(component);

        public void Unsubscribe(IStyleComponent component)
        => UIStyleManager.Instance.Unsubscribe(component);

        public void OnValidate()
        {
            if (_target == null) _target = GetComponent<T>();
            if (RectTransform == null) RectTransform = GetComponent<RectTransform>();
            if (_target != null && _reference != null) UIStyleManager.Instance.OnValidateApplyStyle(this, Reference);
        }
    }
}