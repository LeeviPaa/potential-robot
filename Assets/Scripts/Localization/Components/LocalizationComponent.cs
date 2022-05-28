using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.Localization.Components
{
    public abstract class LocalizationComponent : MonoBehaviour, ILocalizable
    {
        [field: SerializeField]
        public string LocalizationKey { get; protected set; }
        
        public abstract void SetVisuals();
        public void OnLanguageChanged(string language) => SetVisuals();
        public void Start() => SetVisuals();
        public void OnEnable() => LocalizationManager.Instance.Subscribe(this);
        public void OnDisable() => LocalizationManager.Instance.Unsubscribe(this);
    }
}
