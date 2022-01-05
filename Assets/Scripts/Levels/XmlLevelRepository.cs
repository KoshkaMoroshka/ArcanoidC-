using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class XmlLevelRepository : LevelRepository
{
    public override void Save(int level, IReadOnlyList<BlockData> blocksData)
    {
        var serializer = new XmlSerializer(typeof(List<BlockData>));
        var writer = new StreamWriter(Path.Combine(Application.streamingAssetsPath, GetFileName(level)));
        serializer.Serialize(writer, blocksData);
    }

    public override IReadOnlyList<BlockData> Load(int level)
    {
        if (ExistLevel(level) == false)
            throw new ArgumentOutOfRangeException($"Level {level} not exist");

        var serializer = new XmlSerializer(typeof(List<BlockData>));
        var writer = new StreamReader(Path.Combine(Application.streamingAssetsPath, GetFileName(level)));
        return (IReadOnlyList<BlockData>) serializer.Deserialize(writer);
    }

    public override bool ExistLevel(int level)
    {
        string path = Path.Combine(Application.streamingAssetsPath, GetFileName(level));
        return File.Exists(path);
    }

    private static string GetFileName(int level) => $"Level{level}.xml";
}