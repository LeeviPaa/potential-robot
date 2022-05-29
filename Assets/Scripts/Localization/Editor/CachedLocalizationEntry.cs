using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PotentialRobot.Localization.Editor
{
    public struct CachedLocalizationEntry
    {
        public int Index;
        public SerializedProperty Key;
        public SerializedProperty[] Keys;
        public SerializedProperty[] Texts;
    }
}