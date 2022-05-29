using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PotentialRobot.Localization.Editor
{
    public class LocalizationEditorWindow : EditorWindow
    {
        private const float c_smallSizeButton = 20f;
        private const char c_searchSplitter = ' ';

        private LocalizationKeys _keyAsset;
        private LocalizationAsset[] _assets;
        private SerializedObject _keyAssetSO;
        private List<SerializedObject> _assetsSO;
        private string _searchTerm = string.Empty;

        private List<LocalizationListElement> _elements;
        private List<LocalizationListElement> _validResults;

        private bool _isInitialized;
        private int _currentPage;
        private int _itemCountOnPage = 20;
        private CachedLocalizationEntry[] _cachedEntries;
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
            ReinitializeListItems();
            _isInitialized = true;
        }

        private void LoadKeys()
        {
            var assets = Resources.LoadAll<LocalizationKeys>("");
            _keyAsset = assets.Length > 0 ? assets[0] : null;
        }

        private void LoadAssets()
        {
            _assets = Resources.LoadAll<LocalizationAsset>("");
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
            Repaint();
            ReinitializeListItems();
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
            Repaint();
            ReinitializeListItems();
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

        private void ReinitializeListItems()
        {
            _elements = new List<LocalizationListElement>(_keyAsset.Keys.Count);
            _elements.Clear();

            _keyAssetSO = new SerializedObject(_keyAsset);
            var keysProperty = _keyAssetSO.FindProperty("_keys");

            if (_assetsSO == null) 
                _assetsSO = new List<SerializedObject>();
            _assetsSO.Clear();

            List<SerializedProperty> translations = new List<SerializedProperty>(); 
            foreach (var asset in _assets)
            {
                var so = new SerializedObject(asset);
                _assetsSO.Add(so);
                translations.Add(so.FindProperty("_translations"));
            }
            
            var count = _keyAsset.Keys.Count;
            for (var i = 0; i < count; ++i)
            {
                var element = new LocalizationListElement
                {
                    Key = _keyAsset.Keys[i],
                    Index = i
                };
                _elements.Add(element);
            }
            _validResults = null;
        }

        private void OnGUIDrawKeyEditor()
        {
            if (_elements == null || _keyAsset.Keys.Count != _elements.Count)
                ReinitializeListItems();
            EditorGUI.BeginChangeCheck();
            {
                DrawSearch();
                EditorGUILayout.BeginVertical();
                {
                    DrawPage();
                }
                DrawPageControls(_validResults.Count);
                EditorGUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {
                ApplyChanges();
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
                    _currentPage = 0;
                if (_validResults == null || changed)
                {
                    string[] searchTerms = newTerm.ToLowerInvariant().Split(c_searchSplitter);
                    _validResults = _elements;
                    foreach (var term in searchTerms)
                    {
                        _validResults = _validResults.FindAll(s => s.Key.ToLowerInvariant().Contains(term));
                    }
                    _searchTerm = newTerm;
                    var pageStartIndex = GetPageStartIndex();
                    CacheEntries(pageStartIndex, GetPageItemCount(pageStartIndex, _validResults.Count));
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void ValidatePageIndex(int elementsCount)
        {
            _currentPage = Mathf.Clamp(_currentPage, 0, elementsCount / _itemCountOnPage);
        }

        private int GetPageStartIndex()
        {
            return _currentPage * _itemCountOnPage;
        }
        private int GetPageItemCount(int startIndex, int elementsCount)
            => Mathf.Min(_itemCountOnPage, elementsCount - startIndex);

        private void DrawPage()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                if (_cachedEntries == null || _cachedEntries.Length <= 0)
                {
                    EditorGUILayout.LabelField("No entries found.");
                }
                else
                {
                    for (var i = 0; i < _cachedEntries.Length; ++i)
                    {
                        DrawEntry(_cachedEntries[i]);
                    }
                }
            }
            EditorGUILayout.LabelField(_cachedEntries?.Length.ToString());
            EditorGUILayout.EndScrollView();
        }

        private void CacheEntries(int startIndex, int count)
        {
            if (count <= 0)
            {
                _cachedEntries = null;
                return;
            }
            _cachedEntries = new CachedLocalizationEntry[count];
            for (var i = 0; i < count; ++i)
            {
                var assetsCount = _assetsSO.Count;
                var entryIndex = _validResults[startIndex + i].Index;
                SerializedProperty[] keys = new SerializedProperty[_assetsSO.Count];
                SerializedProperty[] texts = new SerializedProperty[_assetsSO.Count];
                for (var j = 0; j < _assetsSO.Count; ++j)
                {
                    var translationsProperty = _assetsSO[j].FindProperty("_translations");
                    var entryProperty = translationsProperty.GetArrayElementAtIndex(entryIndex);
                    keys[j] = entryProperty.FindPropertyRelative("Key");
                    texts[j] = entryProperty.FindPropertyRelative("Text");
                }
                CachedLocalizationEntry entry = new CachedLocalizationEntry
                {
                    Index = entryIndex,
                    Key = _keyAssetSO.FindProperty("_keys").GetArrayElementAtIndex(entryIndex),
                    Keys = keys,
                    Texts = texts
                };
                _cachedEntries[i] = entry;
            }
        }

        private void DrawEntry(CachedLocalizationEntry element)
        {
            EditorGUILayout.BeginHorizontal();
            {
                DrawRemoveButton(element.Index);
                DrawAddButton(element.Index);
                var previousValue = element.Key.stringValue;
                var value = EditorGUILayout.TextField(previousValue);
                if (value != previousValue)
                {
                    element.Key.stringValue = value;
                    for (var i = 0; i < element.Keys.Length; ++i)
                        element.Keys[i].stringValue = value;
                    var replace = _elements[element.Index];
                    replace.Key = value;
                    _elements[element.Index] = replace;
                }

                foreach (var e in element.Texts)
                {
                    var text = EditorGUILayout.TextArea(e.stringValue);
                    if (text != e.stringValue)
                        e.stringValue = text;
                }
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
            if (GUILayout.Button("-", GUILayout.Width(c_smallSizeButton)))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", $"Do you want to remove entry {index} from the list?", "Yes", "No"))
                {
                    _onButtonPressed = RemoveKey;
                    _onButtonPressedIndex = index;
                }
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
            CacheEntries(startIndex, GetPageItemCount(startIndex, _validResults.Count));
        }

        #endregion

        private void ApplyChanges()
        {
            if (_assetsSO == null || _keyAssetSO == null)
            {
                Initialize();
                return;
            }
            foreach (var serializedObject in _assetsSO)
                serializedObject.ApplyModifiedProperties();
            _keyAssetSO.ApplyModifiedProperties();
        }

        private void OnGUIDrawCreateAssetsHelper()
        {
            EditorGUILayout.LabelField("Unable to load config");
            if (GUILayout.Button("Reload"))
                Initialize();
        }
    }
}
