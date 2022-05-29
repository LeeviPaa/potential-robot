using System.Collections.Generic;
using UnityEditor;

namespace PotentialRobot.Localization.Editor
{
    [System.Serializable]
    public struct LocalizationListElement
    {
        public SerializedProperty KeyProperty;
        public List<SerializedProperty> EntryProperties;
    }

    public class ListElementComparer : IComparer<LocalizationListElement>
    {
        public int Compare(LocalizationListElement x, LocalizationListElement y)
        {
            return x.KeyProperty.stringValue.CompareTo(y.KeyProperty.stringValue);
        }
    }
}