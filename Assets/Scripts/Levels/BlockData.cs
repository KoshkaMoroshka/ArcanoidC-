using UnityEngine;

public class BlockData
{
    public BlockData(string blockName, Vector3 localPosition)
    {
        BlockName = blockName;
        Position = localPosition;
    }

    public BlockData()
    {
    }

    public string BlockName { get; set; }
    public Vector3 Position { get; set; }
}