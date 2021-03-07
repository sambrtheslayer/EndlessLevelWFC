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
    public GameObject EmptyMap;

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
    public void CreateTopNeighbour(List<VoxelTile> _sideTiles, GameObject neighbour)
    {
        Vector3 neighbourPos = neighbour.transform.position;
        var topMap = Instantiate(Map, neighbourPos +  new Vector3(0, 0, 6.4f), Quaternion.identity);
        var top = topMap.GetComponent<MapPlacerWfc>();
        top.haveBottomNeighbour = true;
        top.forwardSidesTiles = _sideTiles;
        //mapList.AddNeighbour(mapList.start, topMap, Direction.Forward);
    }

    public void CreateBottomNeighbour(List<VoxelTile> _sideTiles, GameObject neighbour)
    {
        Vector3 neighbourPos = neighbour.transform.position;
        var bottomMap = Instantiate(Map, neighbourPos + new Vector3(0, 0, -6.4f), Quaternion.identity);
        var bottom = bottomMap.GetComponent<MapPlacerWfc>();
        bottom.haveTopNeighbour = true;
        bottom.backSidesTiles = _sideTiles;
        //mapList.AddNeighbour(mapList.start, bottomMap, Direction.Back);
    }

    public void CreateLeftNeighbour(List<VoxelTile> _sideTiles, GameObject neighbour)
    {
        Vector3 neighbourPos = neighbour.transform.position;
        var leftMap = Instantiate(Map, neighbourPos + new Vector3(-6.4f, 0, 0), Quaternion.identity);
        var left = leftMap.GetComponent<MapPlacerWfc>();
        left.haveRightNeighbour = true;
        left.leftSidesTiles = _sideTiles;
        //mapList.AddNeighbour(mapList.start, leftMap, Direction.Left);
    }

    public void CreateRightNeighbour(List<VoxelTile> _sideTiles, GameObject neighbour)
    {
        Vector3 neighbourPos = neighbour.transform.position;
        var rightMap = Instantiate(Map, neighbourPos + new Vector3(6.4f, 0, 0), Quaternion.identity);
        var right = rightMap.GetComponent<MapPlacerWfc>();
        right.haveLeftNeighbour = true;
        right.rightSidesTiles = _sideTiles;
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