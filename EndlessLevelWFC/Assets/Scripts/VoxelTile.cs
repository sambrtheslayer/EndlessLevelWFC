using System;
using UnityEngine;

public class VoxelTile : MonoBehaviour
{
    public float voxelSize = 0.1f;
    public int tileVoxelSize = 8;

    [Range(1, 100)]
    public int Weight = 50;

    public RotationType Rotation;
    public enum RotationType
    {
        OnlyRotation,
        TwoRotations,
        FourRotations
    }

    [HideInInspector] public byte[] сolorsForwardSide;
    [HideInInspector] public byte[] сolorsBackSide;
    [HideInInspector] public byte[] сolorsLeftSide;
    [HideInInspector] public byte[] сolorsRightSide;

    public void CalculateSidesColors()
    {
        сolorsForwardSide = new byte[tileVoxelSize * tileVoxelSize];
        сolorsBackSide = new byte[tileVoxelSize * tileVoxelSize];
        сolorsLeftSide = new byte[tileVoxelSize * tileVoxelSize];
        сolorsRightSide = new byte[tileVoxelSize * tileVoxelSize];

        // Кэшируем meshCollider
        // Данный компонент использует данные о мэшах (треугольники, вершины)
        // для того, чтобы работать со столкновениям с другими объектами
        var meshCollider = GetComponentInChildren<MeshCollider>();

        for (int layer = 0; layer < tileVoxelSize; layer++)
        {
            for (int offset = 0; offset < tileVoxelSize; offset++)
            {
                сolorsForwardSide[layer * tileVoxelSize + offset] = GetVoxelColor(layer, offset, Direction.Forward, meshCollider);
                сolorsBackSide[layer * tileVoxelSize + offset] = GetVoxelColor(layer, offset, Direction.Back, meshCollider);
                сolorsLeftSide[layer * tileVoxelSize + offset] = GetVoxelColor(layer, offset, Direction.Left, meshCollider);
                сolorsRightSide[layer * tileVoxelSize + offset] = GetVoxelColor(layer, offset, Direction.Right, meshCollider);
            }
        }
    }

    public void Rotate90()
    {
        transform.Rotate(0, 90, 0);

        byte[] colorsForwardNew = new byte[tileVoxelSize * tileVoxelSize];
        byte[] colorsBackNew = new byte[tileVoxelSize * tileVoxelSize];
        byte[] colorsLeftNew = new byte[tileVoxelSize * tileVoxelSize];
        byte[] colorsRightNew = new byte[tileVoxelSize * tileVoxelSize];

        for (int layer = 0; layer < tileVoxelSize; layer++)
        {
            for (int offset = 0; offset < tileVoxelSize; offset++)
            {
                colorsRightNew[layer * tileVoxelSize + offset] = сolorsForwardSide[layer * tileVoxelSize + tileVoxelSize - offset - 1];
                colorsForwardNew[layer * tileVoxelSize + offset] = сolorsLeftSide[layer * tileVoxelSize + offset];
                colorsLeftNew[layer * tileVoxelSize + offset] = сolorsBackSide[layer * tileVoxelSize + tileVoxelSize - offset - 1];
                colorsBackNew[layer * tileVoxelSize + offset] = сolorsRightSide[layer * tileVoxelSize + offset];
            }
        }

        сolorsForwardSide = colorsForwardNew;
        сolorsBackSide = colorsBackNew;
        сolorsLeftSide = colorsLeftNew;
        сolorsRightSide = colorsRightNew;
    }

    private byte GetVoxelColor(int verticalOffset, int horizontalOffset, Direction direction, MeshCollider meshCollider)
    {
        float vox = voxelSize;
        float voxHalf = voxelSize / 2;
        Vector3 rayStart;
        Vector3 rayDir;


        if (direction == Direction.Forward)
        {
            rayStart = meshCollider.bounds.min + new Vector3(voxHalf + horizontalOffset * vox, 0, -voxHalf);
            rayDir = Vector3.forward;
        }
        else if (direction == Direction.Back)
        {
            rayStart = meshCollider.bounds.max + new Vector3(-voxHalf - (tileVoxelSize - horizontalOffset - 1) * vox, 0, voxHalf);
            rayDir = Vector3.back;
        }
        else if (direction == Direction.Left)
        {
            rayStart = meshCollider.bounds.max + new Vector3(voxHalf, 0, -voxHalf - (tileVoxelSize - horizontalOffset - 1) * vox);
            rayDir = Vector3.left;
        }
        else if (direction == Direction.Right)
        {
            rayStart = meshCollider.bounds.min + new Vector3(-voxHalf, 0, voxHalf + horizontalOffset * vox);
            rayDir = Vector3.right;
        }
        else
        {
            throw new ArgumentException("Wrong direction value, should be Direction.left/right/back/forward",
                            nameof(direction));
        }

        rayStart.y = meshCollider.bounds.min.y + voxHalf + verticalOffset * vox;

        // Debug.DrawRay(rayStart, direction * 0.1f, Color.red, 60);

        if (Physics.Raycast(new Ray(rayStart, rayDir), out RaycastHit hit, vox))
        {
            byte colorIndex = (byte)(hit.textureCoord.x * 256);

            if (colorIndex == 0) Debug.Log("Found color 0 in mesh palette, this can cause conflicts");

            return colorIndex;
        }
        return 0;
    }
}