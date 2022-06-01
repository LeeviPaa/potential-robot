using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PotentialRobot.Localization.Editor
{
    public class LocalizationEditorWindow : EditorWindow
    {
        private const float c_smallSizeButton = 20f;
        private const char c_searchSplitter = ' ';
        private const string c_textConfirmationPopupTitle = "Are you sure?";
        private const string c_textConfirmationPopupRemoveEntry = "Do you want to remove entry {0} from the list?";
        private const string c_textOK = "OK";
        private const string c_textCancel = "Cancel";

        private LocalizationKeys _keyAsset;
        private LocalizationAsset[] _assets;
        private string _searchTerm = string.Empty;

        private List<int> _searchResult = new List<int>();

        private bool _isInitialized;
        private int _currentPage;
        private int _itemCountOnPage = 20;
        private Vector2 _scroll;
        private System.Action<int> _onButtonPressed;
        private int _onButtonPressedIndex;

        [MenuItem("PotentialRobot/LocalizationEditor")]
        public static void OpenWindow()
        {
            var window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window?.Initialize();
            window.Focus();
            window.name = "Localization Editor";
        }

        public void Initialize()
        {
            LoadKeys();
            LoadAssets();
            _isInitialized = true;
        }

        private void LoadKeys()
        {
            var assets = Resources.LoadAll<LocalizationKeys>(string.Empty);
            _keyAsset = assets.Length > 0 ? assets[0] : null;
        }

        private void LoadAssets()
        {
            _assets = Resources.LoadAll<LocalizationAsset>(string.Empty);
        }

        private void StartRecordingUndo()
        {
            var undoObjects = new List<Object>();
            undoObjects.Add(_keyAsset);
            foreach (var asset in _assets)
                undoObjects.Add(asset);
            Undo.RecordObjects(undoObjects.ToArray(), "AddKey");
        }

        private void StopRecordingUndo()
        {
            Undo.FlushUndoRecordObjects();
        }

        private void SetObjectsDirty()
        {
            EditorUtility.SetDirty(_keyAsset);
            foreach (var asset in _assets)
            {
                EditorUtility.SetDirty(asset);
            }
        }

        private void AddKey(int index)
        {
            StartRecordingUndo();
            {
                _keyAsset.Keys.Insert(index, string.Empty);
                foreach (var asset in _assets)
                {
                    asset.Translations.Insert(index, new LocalizationAsset.Translation(string.Empty, string.Empty));
                }
            }
            StopRecordingUndo();
            SetObjectsDirty();
        }

        private void RemoveKey(int index)
        {
            StartRecordingUndo();
            {
                _keyAsset.Keys.RemoveAt(index);
                foreach (var asset in _assets)
                    asset.Translations.RemoveAt(index);
            }
            StopRecordingUndo();
            SetObjectsDirty();
        }

        public void OnProjectChange()
        {
            Initialize();
        }

        public bool ShouldDrawWindow() => _keyAsset != null;

        #region Draw
        private void OnGUI()
        {
            if (!_isInitialized)
                Initialize();
            if (ShouldDrawWindow())
                OnGUIDrawKeyEditor();
            else
                OnGUIDrawCreateAssetsHelper();
        }

        private void OnGUIDrawKeyEditor()
        {
            EditorGUI.BeginChangeCheck();
            {
                DrawSearch();
                EditorGUILayout.BeginVertical();
                {
                    DrawPage();
                }
                DrawPageControls(_searchTerm.Length > 0 ? _searchResult.Count : _keyAsset.Keys.Count);
                EditorGUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _onButtonPressed?.Invoke(_onButtonPressedIndex);
                _onButtonPressed = null;
            }
        }

        private void DrawSearch()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Search:");
                var newTerm = EditorGUILayout.TextField(_searchTerm);
                var changed = _searchTerm != newTerm;

                if (changed)
                {
                    _currentPage = 0;
                    _searchTerm = newTerm;
                    if (!string.IsNullOrEmpty(_searchTerm))
                        Search();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Search()
        {
            _searchResult.Clear();
            string[] searchTerms = _searchTerm.ToLowerInvariant().Split(c_searchSplitter);
            for (var i = 0; i < searchTerms.Length; ++i)
            {
                if (_searchResult.Count == 0 && i > 0)
                    break;
                SearchTerm(searchTerms[i]);
            }
        }

        private void SearchTerm(string term)
        {
            if (_searchResult.Count == 0)
            {
                for (var i = 0; i < _keyAsset.Keys.Count; ++i)
                {
                    if (_keyAsset.Keys[i].ToLowerInvariant().Contains(term))
                        _searchResult.Add(i);
                }
            }
            else
                _searchResult = _searchResult.FindAll(value => _keyAsset.Keys[value].ToLowerInvariant().Contains(term));
        }

        private int GetPageStartIndex()
        {
            return _currentPage * _itemCountOnPage;
        }
        private int GetPageItemCount(int startIndex, int elementsCount)
            => Mathf.Min(_itemCountOnPage, elementsCount - startIndex);

        private void DrawPage()
        {
            var startIndex = GetPageStartIndex();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                if (_searchTerm.Length > 0)
                    DrawSearchResult(startIndex);
                else
                    DrawPageDefault(startIndex);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawSearchResult(int startIndex)
        {
            if (_searchResult.Count > 0)
            {
                var itemCountOnPage = GetPageItemCount(startIndex, _searchResult.Count);
                for (var i = startIndex; i < startIndex + itemCountOnPage; ++i)
                    DrawEntry(_searchResult[i]);
            }
            else 
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("No entries found...");
                GUILayout.FlexibleSpace();
            }
        }

        private void DrawPageDefault(int startIndex)
        {
            var itemCountOnPage = GetPageItemCount(startIndex, _keyAsset.Keys.Count);
            for (var i = startIndex; i < startIndex + itemCountOnPage; ++i)
            {
                DrawEntry(i);
            }
        }

        private LocalizationAsset.Translation _cachedTranslation;
        private string _cachedValue;

        private void DrawEntry(int index)
        {
            EditorGUILayout.BeginHorizontal();
            {
                DrawRemoveButton(index);
                DrawAddButton(index);
                DrawKeyField(index);
                DrawTranslations(index);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddButton(int index)
        {
            if (GUILayout.Button("+", GUILayout.Width(c_smallSizeButton)))
            {
                _onButtonPressed = AddKey;
                _onButtonPressedIndex = index;
            }
        }

        private void DrawRemoveButton(int index)
        {
            if (GUILayout.Button("-", GUILayout.Width(c_smallSizeButton))
                && EditorUtility.DisplayDialog(c_textConfirmationPopupTitle, string.Format(c_textConfirmationPopupRemoveEntry, index), c_textOK, c_textCancel))
            {
                _onButtonPressed = RemoveKey;
                _onButtonPressedIndex = index;
            }
        }

        private void DrawKeyField(int index)
        {
            _cachedValue = EditorGUILayout.TextField(_keyAsset.Keys[index]);
            if (_cachedValue.Equals(_keyAsset.Keys[index]))
                return;
            
            StartRecordingUndo();
            {
                _keyAsset.Keys[index] = _cachedValue;
                for (var i = 0; i < _assets.Length; ++i)
                {
                    _cachedTranslation = _assets[i].Translations[index];
                    _cachedTranslation.Key = _cachedValue;
                    _assets[i].Translations[index] = _cachedTranslation;
                    EditorUtility.SetDirty(_assets[i]);
                }
                EditorUtility.SetDirty(_keyAsset);
            }
            StopRecordingUndo();
        }

        private void DrawTranslations(int index)
        {
            for (var i = 0; i < _assets.Length; ++i)
            {
                _cachedTranslation = _assets[i].Translations[index];
                _cachedValue = EditorGUILayout.TextField(_cachedTranslation.Text);
                
                if (_cachedValue.Equals(_cachedTranslation.Text))
                    continue;
                
                StartRecordingUndo();
                {
                    _cachedTranslation.Text = _cachedValue;
                    _assets[i].Translations[index] = _cachedTranslation;
                    EditorUtility.SetDirty(_assets[i]);
                }
                StopRecordingUndo();
            }
        }

        private void DrawPageControls(int elementsCount)
        {
            var newPage = _currentPage;
            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = newPage > 0;
                if (GUILayout.Button("Previous"))
                    newPage = Mathf.Max(newPage - 1, 0);
                GUI.enabled = true;
                newPage = Mathf.Clamp(EditorGUILayout.IntField("Page: ", newPage + 1) - 1, 0, elementsCount / _itemCountOnPage);
                EditorGUILayout.LabelField($"/ {Mathf.CeilToInt(elementsCount / (float)_itemCountOnPage)}");
                GUI.enabled = (newPage + 1) * _itemCountOnPage < elementsCount;
                if (GUILayout.Button("Next"))
                    newPage = Mathf.Min(newPage + 1, elementsCount);
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            if (_currentPage != newPage)
            {
                _onButtonPressed = ChangePage;
                _onButtonPressedIndex = newPage;
            }
        }

        private void ChangePage(int newPage)
        {
            _currentPage = newPage;
            int startIndex = GetPageStartIndex();
        }

        #endregion

        private void OnGUIDrawCreateAssetsHelper()
        {
            EditorGUILayout.LabelField("No config files created.");
            if (GUILayout.Button("Reload"))
                Initialize();
        }
    }
}
