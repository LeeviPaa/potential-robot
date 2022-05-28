using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.Localization
{
    [CreateAssetMenu(menuName = "Localization/Localization Asset", fileName = "LocalizationAsset_languageName.asset")]
    public class LocalizationAsset : ScriptableObject
    {
        [field: SerializeField]
        public string Language { get; private set; }
        [field: SerializeField]
        public List<(string Key, string Text)> Translations { get; private set; }
    }
}
