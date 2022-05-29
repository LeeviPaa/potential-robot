using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.Localization
{
    [CreateAssetMenu(menuName = "Localization/Localization Asset", fileName = "LocalizationAsset_languageName.asset")]
    public class LocalizationAsset : ScriptableObject
    {
        [System.Serializable]
        public struct Translation
        {
            public string Key;
            public string Text;
            public Translation(string key, string text)
            {
                Key = key;
                Text = text;
            }
        }

        [SerializeField]
        private string _languageName;
        public string LanguageName => _languageName;
        [SerializeField, NonReorderable]
        private List<Translation> _translations = new List<Translation>();
        public List<Translation> Translations => _translations;
    }
}
