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
        private List<LocalizationListElement> _searchResult;

        private bool _isInitialized;
        private int _currentPage;
        private int _itemCountOnPage = 20;
        private Vector2 _scroll;

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
            EditorUtility.DisplayProgressBar("Initializing Localization Editor", "Serializing Properties", 0f);
            float progress = 0f;
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

            EditorUtility.ClearProgressBar();
        }

        private void OnGUIDrawKeyEditor()
        {
            if (_elements == null || _keyAsset.Keys.Count != _elements.Count)
                ReinitializeListItems();

            DrawSearch();

            var elements = string.IsNullOrEmpty(_searchTerm) || _searchResult == null ? _elements : _searchResult;

            EditorGUILayout.BeginVertical();
            {
                DrawPage(elements);
            }
            DrawPageControls(elements.Count);
            EditorGUILayout.EndVertical();

            ApplyChanges();
        }

        private void DrawSearch()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Search:");
                var newTerm = EditorGUILayout.TextField(_searchTerm);
                if (_searchTerm != newTerm)
                {
                    var lower = newTerm.ToLowerInvariant();
                    string[] searchTerms = newTerm.ToLowerInvariant().Split(c_searchSplitter);
                    _searchResult = _elements;
                    foreach (var term in searchTerms)
                    {
                        _searchResult = _searchResult.FindAll(s => s.Key.ToLowerInvariant().Contains(term));
                    }
                    _searchTerm = newTerm;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void ValidatePageIndex(int elementsCount)
        {
            _currentPage = GetPageStartIndex() < elementsCount ? Mathf.Max(_currentPage, 0) : 0; 
        }

        private int GetPageStartIndex() => _currentPage * _itemCountOnPage;
        private int GetPageItemCount(int startIndex, int elementsCount) => Mathf.Min(_itemCountOnPage, elementsCount - startIndex);

        private void DrawPage(List<LocalizationListElement> elements)
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                ValidatePageIndex(elements.Count);
                int startIndex = GetPageStartIndex();
                int itemCount = GetPageItemCount(startIndex, elements.Count);
                for (var i = startIndex; i < itemCount; ++i)
                {
                    DrawEntry(elements[i]);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawEntry(LocalizationListElement element)
        {
            EditorGUILayout.BeginHorizontal();
            {
                var keyProperty = _keyAssetSO.FindProperty("_keys").GetArrayElementAtIndex(element.Index);
                element.Key = EditorGUILayout.TextField(keyProperty.stringValue);
                keyProperty.stringValue = element.Key;
                foreach (var e in _assetsSO)
                {
                    var property = e.FindProperty("_translations").GetArrayElementAtIndex(element.Index);
                    property.FindPropertyRelative("Key").stringValue = element.Key;
                    var translation = property.FindPropertyRelative("Text");
                    translation.stringValue = EditorGUILayout.TextArea(translation.stringValue);
                }
                _elements[element.Index] = element;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddButton(int index)
        {
            if (GUILayout.Button("+", GUILayout.Width(c_smallSizeButton)))
                AddKey(index);
        }

        private void DrawRemoveButton(int index)
        {
            if (GUILayout.Button("-", GUILayout.Width(c_smallSizeButton)))
                RemoveKey(index);
        }

        private void DrawPageControls(int elementsCount)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = _currentPage > 0;
                if (GUILayout.Button("Previous"))
                    _currentPage = Mathf.Max(_currentPage - 1, 0);
                GUI.enabled = true;
                EditorGUILayout.LabelField($"Page {_currentPage + 1}");
                GUI.enabled = (_currentPage + 1) * _itemCountOnPage < elementsCount;
                if (GUILayout.Button("Next"))
                    _currentPage = Mathf.Min(_currentPage + 1, elementsCount);
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        private void ApplyChanges()
        {
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