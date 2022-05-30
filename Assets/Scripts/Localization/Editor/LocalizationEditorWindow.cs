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
        private string _searchTerm = string.Empty;

        private List<int> _searchResult = new List<int>();
        private List<int> _pageItems = new List<int>();

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
                }
                if (changed && !string.IsNullOrEmpty(_searchTerm))
                {
                    string[] searchTerms = _searchTerm.ToLowerInvariant().Split(c_searchSplitter);
                    if (_searchResult != null)
                        _searchResult.Clear();
                    else
                        _searchResult = new List<int>();
                    
                    for (var i = 0; i < searchTerms.Length; ++i)
                    {
                        if (_searchResult.Count == 0 && i > 0)
                            break;
                        
                        string term = searchTerms[i];
                        if (_searchResult.Count == 0)
                        {
                            for (var j = 0; j < _keyAsset.Keys.Count; ++j)
                            {
                                if (_keyAsset.Keys[j].ToLowerInvariant().Contains(term))
                                    _searchResult.Add(j);
                            }
                        }
                        else
                            _searchResult = _searchResult.FindAll(value => _keyAsset.Keys[value].ToLowerInvariant().Contains(term));
                    }
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
            var startIndex = GetPageStartIndex();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                if (_searchTerm.Length > 0)
                {
                    if (_searchResult.Count > 0)
                    {
                        var itemCountOnPage = GetPageItemCount(startIndex, _searchResult.Count);
                        for (var i = startIndex; i < startIndex + itemCountOnPage; ++i)
                        {
                            DrawEntry(_searchResult[i]);
                        }
                    }
                    else 
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("No entries found...");
                        GUILayout.FlexibleSpace();
                    }
                }
                else
                {
                    var itemCountOnPage = GetPageItemCount(startIndex, _keyAsset.Keys.Count);
                    for (var i = startIndex; i < startIndex + itemCountOnPage; ++i)
                    {
                        DrawEntry(i);
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawEntry(int index)
        {
            LocalizationAsset.Translation cachedValue;
            string newValue;
            EditorGUILayout.BeginHorizontal();
            {
                DrawRemoveButton(index);
                DrawAddButton(index);
                newValue = EditorGUILayout.TextField(_keyAsset.Keys[index]);
                if (!newValue.Equals(_keyAsset.Keys[index]))
                {
                    _keyAsset.Keys[index] = newValue;
                    for (var i = 0; i < _assets.Length; ++i)
                    {
                        cachedValue = _assets[i].Translations[index];
                        cachedValue.Key = newValue;
                        _assets[i].Translations[index] = cachedValue;
                        EditorUtility.SetDirty(_assets[i]);
                    }
                    EditorUtility.SetDirty(_keyAsset);
                }
                for (var i = 0; i < _assets.Length; ++i)
                {
                    cachedValue = _assets[i].Translations[index];
                    newValue = EditorGUILayout.TextField(cachedValue.Text);
                    if (!newValue.Equals(cachedValue.Text))
                    {
                        cachedValue.Text = newValue;
                        _assets[i].Translations[index] = cachedValue;
                        EditorUtility.SetDirty(_assets[i]);
                    }
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
        }

        #endregion

        private void OnGUIDrawCreateAssetsHelper()
        {
            EditorGUILayout.LabelField("Unable to load config");
            if (GUILayout.Button("Reload"))
                Initialize();
        }
    }
}
