using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.Localization
{
    [CreateAssetMenu(menuName = "Localization/Localization Keys", fileName = "LocalizationKeys.asset")]
    public class LocalizationKeys : ScriptableObject
    {
        [SerializeField, NonReorderable]
        private List<string> _keys = new List<string>();
        public List<string> Keys => _keys;
    }
}
