using System;
using System.Collections.Generic;

public class XmlLevelRepository : LevelRepository
{
    public override void Save(int level, IReadOnlyList<BlockData> block)
    {
        throw new NotImplementedException();
    }

    public override IReadOnlyList<BlockData> Load(int level)
    {
        throw new NotImplementedException();
    }

    public override bool ExistLevel(int level)
    {
        throw new NotImplementedException();
    }
}