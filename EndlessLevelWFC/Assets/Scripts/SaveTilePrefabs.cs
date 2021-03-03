using UnityEngine;
using System.Collections.Generic;
using System;

public class SaveTilePrefabs : MonoBehaviour
{
    // Префабы тайлов
    public List<VoxelTile> TilePrefabs;
    public GameObject Tiles;
    public GameObject TilePlacerWfcPrefab;

    void Start()
    {
        GetTilePrefabs();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(TilePlacerWfcPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    public void GetTilePrefabs()
    {
        foreach (VoxelTile tilePrefab in TilePrefabs)
        {
            tilePrefab.CalculateSidesColors();
        }

        int countBeforeAdding = TilePrefabs.Count;
        for (int i = 0; i < countBeforeAdding; i++)
        {
            VoxelTile clone;
            switch (TilePrefabs[i].Rotation)
            {
                case VoxelTile.RotationType.OnlyRotation:
                    break;

                case VoxelTile.RotationType.TwoRotations:
                    TilePrefabs[i].Weight /= 2;
                    if (TilePrefabs[i].Weight <= 0) TilePrefabs[i].Weight = 1;

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right,
                        Quaternion.identity);
                    clone.transform.parent = Tiles.transform;
                    clone.Rotate90();
                    TilePrefabs.Add(clone);
                    break;

                case VoxelTile.RotationType.FourRotations:
                    TilePrefabs[i].Weight /= 4;
                    if (TilePrefabs[i].Weight <= 0) TilePrefabs[i].Weight = 1;

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right,
                        Quaternion.identity);
                    clone.transform.parent = Tiles.transform;
                    clone.Rotate90();
                    TilePrefabs.Add(clone);

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right * 2,
                        Quaternion.identity);
                    clone.transform.parent = Tiles.transform;
                    clone.Rotate90();
                    clone.Rotate90();
                    TilePrefabs.Add(clone);

                    clone = Instantiate(TilePrefabs[i], TilePrefabs[i].transform.position + Vector3.right * 3,
                        Quaternion.identity);
                    clone.transform.parent = Tiles.transform;
                    clone.Rotate90();
                    clone.Rotate90();
                    clone.Rotate90();
                    TilePrefabs.Add(clone);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}