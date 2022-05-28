using UnityEngine;
using TMPro;

namespace PotentialRobot.Localization.Components
{
    public class Localization_TMProText : LocalizationComponent
    {
        [SerializeField]
        TMP_Text _textField;

        public override void SetVisuals()
        {
            _textField.text = LocalizationManager.Instance.GetText(LocalizationKey);
        }

        public void OnValidate()
        {
            _textField = GetComponent<TMP_Text>();
        }
    }
}
