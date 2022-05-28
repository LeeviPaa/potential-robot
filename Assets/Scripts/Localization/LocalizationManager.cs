using System.Collections.Generic;
using PotentialRobot.Localization.Components;
using UnityEngine;

namespace PotentialRobot.Localization
{
    public class LocalizationManager
    {
        public const string c_currentLanguageKey = "CurrentLanguage";
        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationManager();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public delegate void LanguageChanged(string language);
        private LanguageChanged _onLanguageChanged;
        private Dictionary<string, string> _localizations;
        private string _previousLanguage;
        public string CurrentLanguage { get; private set; }

        public void Initialize()
        {
            if (PlayerPrefs.HasKey("CurrentLanguage"))
                CurrentLanguage = PlayerPrefs.GetString("CurrentLanguage");
            else
                SetFirstLanguage();

            LoadLocalization(CurrentLanguage);
        }

        private void SetFirstLanguage()
        {
            CurrentLanguage = "English";
            PlayerPrefs.SetString("CurrentLanguage", CurrentLanguage);
        }

        public void LoadLocalization(string language)
        {
            if (_previousLanguage == language)
                return;
            
            LoadLanguageInfo(language);
            
            CurrentLanguage = language;
            _previousLanguage = language;
            _onLanguageChanged?.Invoke(language);
        }

        public void LoadLanguageInfo(string language)
        {
            var locAsset = Resources.Load<LocalizationAsset>($"Localization/LocalizationAsset_{language}.asset");
            if (locAsset == null)
            {
                Debug.LogError("Localization error: failed to load localization file.");
                return;
            }

            //System relies on the fact that all of the localization assets have the keys equally written on them.
            if (_localizations == null)
                AddLocalizations(locAsset);
            else
                ReplaceLocalizations(locAsset);
        }

        public void AddLocalizations(LocalizationAsset asset)
        {
            _localizations = new Dictionary<string, string>();
            foreach (var loc in asset.Translations)
                _localizations.Add(loc.Key, string.IsNullOrEmpty(loc.Text) ? loc.Key : loc.Text);
        }

        public void ReplaceLocalizations(LocalizationAsset asset)
        {
            foreach (var loc in asset.Translations)
                _localizations[loc.Key] = string.IsNullOrEmpty(loc.Text) ? loc.Key : loc.Text;
        }

        public void Subscribe(ILocalizable component) => _onLanguageChanged += component.OnLanguageChanged;

        public void Unsubscribe(ILocalizable component) => _onLanguageChanged -= component.OnLanguageChanged;

        public string GetText(string key)
        {
            string text;
            if (_localizations.TryGetValue(key, out text) && !string.IsNullOrEmpty(text))
                return text;
            return key;
        }
    }
}
