using UnityEngine;
using System.Collections.Generic;
using System;
using LinkedList;

public class SaveTilePrefabsTest : MonoBehaviour
{

    // Префабы тайлов
    public List<VoxelTile> TilePrefabs;
    public GameObject Tiles;
    
    public MapList<GameObject> mapList = new MapList<GameObject>();
    public GameObject Map;

    void Start()
    {
        GetTilePrefabs();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            mapList.AddStartMap(Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity));
        }
    }
    public void CreateTopNeighbour(List<VoxelTile> _sideTiles)
    {
        var topMap = Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity);
        topMap.GetComponent<MapPlacerWfc>().forwardSidesTiles = _sideTiles;
        //mapList.AddNeighbour(mapList.start, rightMap, Direction.Right);
    }

    public void CreateBottomNeighbour(List<VoxelTile> _sideTiles)
    {
        var bottomMap = Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity);
        bottomMap.GetComponent<MapPlacerWfc>().backSidesTiles = _sideTiles;
        //mapList.AddNeighbour(mapList.start, rightMap, Direction.Right);
    }

    public void CreateLeftNeighbour(List<VoxelTile> _sideTiles)
    {
        var leftMap = Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity);
        leftMap.GetComponent<MapPlacerWfc>().leftSidesTiles = _sideTiles;
        //mapList.AddNeighbour(mapList.start, leftMap, Direction.Left);
    }

    public void CreateRightNeighbour(List<VoxelTile> _sideTiles)
    {
        var rightMap = Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity);
        rightMap.GetComponent<MapPlacerWfc>().rightSidesTiles = _sideTiles;
        //mapList.AddNeighbour(mapList.start, rightMap, Direction.Right);
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