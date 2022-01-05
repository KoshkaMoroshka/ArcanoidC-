using UnityEngine;

public class BlockData
{
    public BlockData(Block block, Vector3 localPosition)
    {
        Block = block;
        Position = localPosition;
    }

    public Vector3 Position { get; }
    public Block Block { get; }
}