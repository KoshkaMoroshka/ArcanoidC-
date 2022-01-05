﻿using System;
using System.Collections.Generic;

public abstract class LevelRepository
{
    public abstract void Save(int level, IReadOnlyList<BlockData> block);
    public abstract IReadOnlyList<BlockData> Load(int level);
    public abstract bool ExistLevel(int level);
}