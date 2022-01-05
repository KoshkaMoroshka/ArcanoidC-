using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : EditorWindow
{
    private const int MaxGeneratingBlockCount = 5;
    private const float HorizontalOffset = 1.5f;
    private const float VerticalOffset = 1;

    [SerializeField] private List<Block> blocksForSpawn = new List<Block>();

    private readonly Vector2 _blockSize = Vector2.one;

    private readonly List<Block> _blocksOnMap = new List<Block>();
    private readonly List<Block> _blocksPreparedForSpawn = new List<Block>();

    private readonly LevelRepository _repository = new XmlLevelRepository();

    private int _level;
    private Vector2 _positionForSpawn;
    private Vector2 _scrollerPosition;

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical(GUILayout.Width(250));
            {
                DrawButtonClearScene();
                DrawSaveAndLoad();
                _positionForSpawn = EditorGUILayout.Vector2Field("", _positionForSpawn);
                DrawSpawnButtons();
                DrawGenerating();
                DrawListProperty();
            }
            GUILayout.EndVertical();

            DrawBlockButtons();
        }
        GUILayout.EndHorizontal();
    }

    [MenuItem("Tools/LevelGenerator")]
    private static void ShowWindow()
    {
        var window = GetWindow<LevelGenerator>();
        window.titleContent = new GUIContent("LevelGenerator");
        window.Show();
    }

    private void DrawButtonClearScene()
    {
        if (GUILayout.Button("Clear created blocks"))
        {
            _blocksOnMap.ForEach(block => DestroyImmediate(block.gameObject));
            _blocksOnMap.Clear();
        }
    }

    private void DrawBlockButtons()
    {
        //TODO красивый вывод с подстройкой под экран
        foreach (Block block in blocksForSpawn)
        {
            Texture image = block.GetComponent<SpriteRenderer>().sprite.texture;
            var heightPixels = 30;
            GUILayoutOption width = GUILayout.Width(heightPixels / (float) image.height * image.width);
            GUILayoutOption height = GUILayout.Height(heightPixels);
            if (GUILayout.Button(image, width, height) && _blocksPreparedForSpawn.Count < MaxGeneratingBlockCount)
            {
                _blocksPreparedForSpawn.Add(block);
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
        var serializedObject = new SerializedObject(this);
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(blocksForSpawn)));
        serializedObject.ApplyModifiedProperties();
    }

    private void ClearButtonList()
    {
        _blocksPreparedForSpawn.Clear();
    }

    private void DrawButtonList()
    {
        foreach (Block block in _blocksPreparedForSpawn)
        {
            Texture image = block.GetComponent<SpriteRenderer>().sprite.texture;
            var height = 30;
            if (GUILayout.Button(image,
                GUILayout.Width(height / (float) image.height * image.width), GUILayout.Height(height)))
            {
                _blocksPreparedForSpawn.Remove(block);
                break;
            }
        }
    }

    private void SpawnHorizontal()
    {
        Vector2 width = Vector2.right * _blocksOnMap.Count * HorizontalOffset;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(_positionForSpawn + width / 2, width + _blockSize, 0, Vector2.zero);
        if (hits.Any(hit => hit.transform.GetComponent<Block>()))
            return;

        for (var i = 0; i < _blocksPreparedForSpawn.Count; i++)
        {
            _blocksOnMap.Add(Spawn(_blocksPreparedForSpawn[i], Vector2.right * i * HorizontalOffset));
        }
    }

    private void SpawnVertical()
    {
        Vector2 width = Vector2.down * _blocksOnMap.Count * VerticalOffset;
        RaycastHit2D[] hits =
            Physics2D.BoxCastAll(_positionForSpawn + width / 2, width * -1 + _blockSize, 0, Vector2.zero);
        if (hits.Any(hit => hit.transform.GetComponent<Block>()))
            return;

        for (var i = 0; i < _blocksPreparedForSpawn.Count; i++)
        {
            _blocksOnMap.Add(Spawn(_blocksPreparedForSpawn[i], Vector2.down * i * VerticalOffset));
        }
    }

    private Block Spawn(Block block, Vector2 offset)
    {
        var obj = ((Block) PrefabUtility.InstantiatePrefab(block)).GetComponent<Block>();
        obj.transform.position = _positionForSpawn + offset;
        return obj;
    }

    private List<Block> Spawn(IReadOnlyList<BlockData> blocksData)
    {
        var list = new List<Block>();
        foreach (BlockData data in blocksData)
        {
            var block = ((Block) PrefabUtility.InstantiatePrefab(data.Block)).GetComponent<Block>();
            block.transform.localPosition = data.Position;
            list.Add(block);
        }

        return list;
    }

    private void Load()
    {
        _blocksOnMap.ForEach(block => DestroyImmediate(block.gameObject));
        _blocksOnMap.Clear();

        IReadOnlyList<BlockData> blocksData = _repository.Load(_level);
        _blocksOnMap.AddRange(Spawn(blocksData));
    }

    private void Save()
    {
        List<BlockData> blocks = _blocksOnMap
            .Select(block => new BlockData(block, block.transform.localPosition))
            .ToList();
        _repository.Save(_level, blocks);
    }
}