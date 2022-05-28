using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.Localization
{
    [CreateAssetMenu(menuName = "Localization/Localization Keys", fileName = "LocalizationKeys.asset")]
    public class LocalizationKeys : ScriptableObject
    {
        [field: SerializeField]
        public List<string> Keys { get; private set; }
    }
}
