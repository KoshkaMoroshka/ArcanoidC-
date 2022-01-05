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

    private readonly List<(Block block, Block parent)> _blocksOnMap = new List<(Block, Block)>();
    private readonly List<Block> _blocksPreparedForSpawn = new List<Block>();

    private readonly LevelRepository _repository = new XmlLevelRepository();

    private int _level;
    private Vector2 _positionForSpawn;
    private Vector2 _scrollerPosition;

    private readonly Color _spawnAreaColor = new Color(1, 1, 1, 0.3f);
    private GameObject _spawnAreaHorizontal;
    private GameObject _spawnAreaVertical;

    private void OnEnable()
    {
        _spawnAreaHorizontal = InitSpawnArea(nameof(_spawnAreaHorizontal));
        _spawnAreaVertical = InitSpawnArea(nameof(_spawnAreaVertical));
    }

    private void OnDisable()
    {
        DestroyImmediate(_spawnAreaHorizontal);
        DestroyImmediate(_spawnAreaVertical);
        _blocksOnMap.ForEach(blockPair => DestroyImmediate(blockPair.block.gameObject));
    }

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

        DrawSpawnAreaOnScene();
    }

    [MenuItem("Tools/LevelGenerator")]
    private static void ShowWindow()
    {
        var window = GetWindow<LevelGenerator>();
        window.titleContent = new GUIContent("LevelGenerator");
        window.Show();
    }

    private GameObject InitSpawnArea(string spawnAreaName)
    {
        var spawnArea = new GameObject(spawnAreaName);
        var renderer = spawnArea.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(Texture2D.whiteTexture,
            new Rect(Vector2.zero, Vector2.one), Vector2.one / 2,
            1);
        renderer.color = _spawnAreaColor;
        spawnArea.tag = "EditorOnly";

        return spawnArea;
    }

    private void DrawSpawnAreaOnScene()
    {
        _spawnAreaHorizontal.SetActive(_blocksPreparedForSpawn.Count > 0);
        _spawnAreaVertical.SetActive(_blocksPreparedForSpawn.Count > 0);

        Vector2 widthForHorizontal = Vector2.right * _blocksPreparedForSpawn.Count * HorizontalOffset;
        _spawnAreaHorizontal.transform.position =
            _positionForSpawn + widthForHorizontal / 2 + Vector2.left * HorizontalOffset / 2;
        _spawnAreaHorizontal.transform.localScale = (Vector3) (widthForHorizontal + _blockSize) + Vector3.forward;

        Vector2 widthForVertical = Vector2.down * _blocksPreparedForSpawn.Count * VerticalOffset;
        _spawnAreaVertical.transform.position =
            _positionForSpawn + widthForVertical / 2 + Vector2.up * VerticalOffset / 2;
        _spawnAreaVertical.transform.localScale = (Vector3) (widthForVertical * -1 + _blockSize) + Vector3.forward;
    }

    private void DrawButtonClearScene()
    {
        if (GUILayout.Button("Clear created blocks"))
            DestroyBlocksOnMap();
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
        Vector2 width = Vector2.right * _blocksPreparedForSpawn.Count * HorizontalOffset;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(_positionForSpawn + width / 2, width + _blockSize, 0, Vector2.zero);
        if (hits.Any(hit => hit.transform.GetComponent<Block>()))
            return;

        for (var i = 0; i < _blocksPreparedForSpawn.Count; i++)
        {
            Block prefab = _blocksPreparedForSpawn[i];
            Block block = Spawn(prefab, Vector2.right * i * HorizontalOffset);
            _blocksOnMap.Add((block, prefab));
        }
    }

    private void SpawnVertical()
    {
        Vector2 width = Vector2.down * _blocksPreparedForSpawn.Count * VerticalOffset;
        RaycastHit2D[] hits =
            Physics2D.BoxCastAll(_positionForSpawn + width / 2, width * -1 + _blockSize, 0, Vector2.zero);
        if (hits.Any(hit => hit.transform.GetComponent<Block>()))
            return;

        for (var i = 0; i < _blocksPreparedForSpawn.Count; i++)
        {
            Block prefab = _blocksPreparedForSpawn[i];
            Block block = Spawn(prefab, Vector2.down * i * VerticalOffset);
            _blocksOnMap.Add((block, prefab));
        }
    }

    private Block Spawn(Block block, Vector2 offset)
    {
        var obj = ((Block) PrefabUtility.InstantiatePrefab(block)).GetComponent<Block>();
        obj.transform.position = _positionForSpawn + offset;
        return obj;
    }

    private List<(Block block, Block parent)> Spawn(IReadOnlyList<BlockData> blocksData)
    {
        var list = new List<(Block block, Block parent)>();
        foreach (BlockData data in blocksData)
        {
            Block prefab = blocksForSpawn.FirstOrDefault(blockForSpawn => blockForSpawn.name == data.BlockName);
            var block = ((Block) PrefabUtility.InstantiatePrefab(prefab)).GetComponent<Block>();
            block.transform.localPosition = data.Position;
            list.Add((block, prefab));
        }

        return list;
    }

    private void Load()
    {
        DestroyBlocksOnMap();
        IReadOnlyList<BlockData> blocksData = _repository.Load(_level);
        _blocksOnMap.AddRange(Spawn(blocksData));
    }

    private void Save()
    {
        List<BlockData> blockData = _blocksOnMap.Select(blockPair =>
            new BlockData(blockPair.parent.name, blockPair.block.transform.localPosition)).ToList();
        _repository.Save(_level, blockData);
    }

    private void DestroyBlocksOnMap()
    {
        _blocksOnMap.ForEach(blockData => DestroyImmediate(blockData.block.gameObject));
        _blocksOnMap.Clear();
    }
}