using UnityEngine;

public class BlockData
{
    public BlockData(string blockName, Vector3 localPosition)
    {
        BlockName = blockName;
        Position = localPosition;
    }

    public string BlockName { get; }
    public Vector3 Position { get; }
}