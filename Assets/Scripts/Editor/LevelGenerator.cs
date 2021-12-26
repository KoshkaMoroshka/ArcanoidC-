using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


    public class LevelGenerator : EditorWindow
    {
        private const int MaxGeneratingBlockCount = 5;
        private const float HorizontalOffset = 1.5f;
        private const float VerticalOffset = 1;

        [SerializeField] private List<Block> blocks = new List<Block>();
        private readonly List<Block> _generatedBlocks = new List<Block>();
        private readonly List<Block> _generatingBlocks = new List<Block>();

        private readonly Vector2 _blockSize = Vector2.one;

        private int _level;
        private Vector2 _position;
        private Vector2 _scrollerPosition;

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.Width(250));
                {
                    DrawButtonClearScene();
                    DrawSaveAndLoad();
                    _position = EditorGUILayout.Vector2Field("", _position);
                    DrawSpawnButtons();
                    DrawGenerating();
                    DrawListProperty();
                }
                GUILayout.EndVertical();

                DrawBlockButtons();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawButtonClearScene()
        {
            if(GUILayout.Button("Clear created blocks"))
            {
                _generatedBlocks.ForEach(block => DestroyImmediate(block.gameObject));
                _generatedBlocks.Clear();
            }
        }

        [MenuItem("Tools/LevelGenerator")]
        private static void ShowWindow()
        {
            var window = GetWindow<LevelGenerator>();
            window.titleContent = new GUIContent("LevelGenerator");
            window.Show();
        }

        private void DrawBlockButtons()
        {
            //TODO красивый вывод с подстройкой под экран
            foreach (Block block in blocks)
            {
                Texture image = block.GetComponent<SpriteRenderer>().sprite.texture;
                var heightPixels = 30;
                GUILayoutOption width = GUILayout.Width(heightPixels / (float) image.height * image.width);
                GUILayoutOption height = GUILayout.Height(heightPixels);
                if (GUILayout.Button(image, width, height) && _generatingBlocks.Count < MaxGeneratingBlockCount)
                {
                    _generatingBlocks.Add(block);
                }
            }
        }

        private void DrawSaveAndLoad()
        {
            if (GUILayout.Button(nameof(Save)))
                Save();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(nameof(Load)))
                    Load();
                _level = EditorGUILayout.IntField(_level, GUILayout.Width(100));
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSpawnButtons()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(nameof(SpawnVertical)))
                    SpawnVertical();
                if (GUILayout.Button(nameof(SpawnHorizontal)))
                    SpawnHorizontal();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawGenerating()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(50));
            {
                _scrollerPosition = GUILayout.BeginScrollView(_scrollerPosition);
                {
                    GUILayout.BeginHorizontal("box");
                    {
                        DrawButtonList();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();


                if (GUILayout.Button("X", GUILayout.Width(50), GUILayout.Height(50)))
                    ClearButtonList();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawListProperty()
        {
            var obj = new SerializedObject(this);
            obj.Update();
            EditorGUILayout.PropertyField(obj.FindProperty(nameof(blocks)));
            obj.ApplyModifiedProperties();
        }

        private void ClearButtonList()
        {
            _generatingBlocks.Clear();
        }

        private void DrawButtonList()
        {
            foreach (Block block in _generatingBlocks)
            {
                Texture image = block.GetComponent<SpriteRenderer>().sprite.texture;
                var height = 30;
                if (GUILayout.Button(image,
                    GUILayout.Width(height / (float) image.height * image.width), GUILayout.Height(height)))
                {
                    _generatingBlocks.Remove(block);
                    break;
                }
            }
        }

        private void SpawnHorizontal()
        {
            Vector2 width = Vector2.right * _generatedBlocks.Count * HorizontalOffset;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(_position + width / 2, width + _blockSize, 0, Vector2.zero);
            if (hits.Any(hit => hit.transform.GetComponent<Block>()))
                return;

            for (var i = 0; i < _generatingBlocks.Count; i++)
            {
                _generatedBlocks.Add(Spawn(_generatingBlocks[i], Vector2.right * i * HorizontalOffset));
            }
        }

        private void SpawnVertical()
        {
            Vector2 width = Vector2.down * _generatedBlocks.Count * VerticalOffset;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(_position + width / 2, width * -1 + _blockSize, 0, Vector2.zero);
            if (hits.Any(hit => hit.transform.GetComponent<Block>()))
                return;

            for (var i = 0; i < _generatingBlocks.Count; i++)
            {
                _generatedBlocks.Add(Spawn(_generatingBlocks[i], Vector2.down * i * VerticalOffset));
            }
        }

        private Block Spawn(Block block, Vector2 offset)
        {
            var obj = ((Block) PrefabUtility.InstantiatePrefab(block)).GetComponent<Block>();
            obj.transform.position = _position + offset;
            return obj;
        }

        private void Load()
        {
            
        }

        private void Save()
        {
            
        }
    }
