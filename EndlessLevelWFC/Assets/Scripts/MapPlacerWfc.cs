using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapPlacerWfc : MonoBehaviour
{

    private int seed;

    // Префабы тайлов
    private List<VoxelTile> TilePrefabs;
    GameObject TilePrefabsStorage;

    public Vector2Int MapSize = new Vector2Int(10, 10);

    // Тайлы, которые уже заспавнились на террейне
    public static VoxelTile[,] spawnedTiles;

    // Очередь из векторов положений тайлов, которые нужно пересчитать
    private Queue<Vector2Int> recalcPossibleTilesQueue = new Queue<Vector2Int>();

    // Список возможных тайлов для каждой из ячеек карты
    private List<VoxelTile>[,] possibleTiles;

    // Для теста
    public GameObject Storage;
    private GameObject[] LeftSide;
    public List<VoxelTile> LeftSideTiles = new List<VoxelTile>();
    public List<VoxelTile> sidesTiles = new List<VoxelTile>();

    void Awake()
    {
        seed = Random.Range(0, 999);
        Debug.Log("seed: " + seed.ToString());
    }

    private void Start()
    {
        TilePrefabsStorage = GameObject.FindGameObjectWithTag("Storage");
        TilePrefabs = TilePrefabsStorage.GetComponent<SaveTilePrefabsTest>().TilePrefabs;

        spawnedTiles = new VoxelTile[MapSize.x, MapSize.y];
        Random.InitState(seed);
        Generate();
    }

    private void Update()
    {
        // Delete map
        if (Input.GetKeyDown(KeyCode.D))
        {

            foreach (VoxelTile spawnedTile in spawnedTiles)
            {
                if (spawnedTile != null) Destroy(spawnedTile.gameObject);
            }
        }

        // Revert map
        if (Input.GetKeyDown(KeyCode.B))
        {
            Random.InitState(seed);
            Generate();
        }

        // Для теста
        if (Input.GetKeyDown(KeyCode.A))
        {
            LeftSideTiles.Add(GameObject.FindGameObjectWithTag("LeftBottom").GetComponent<VoxelTile>());

            LeftSide = GameObject.FindGameObjectsWithTag("Left");

            for (int i = 0; i < LeftSide.Length; i++)
            {
                LeftSideTiles.Add(LeftSide[i].GetComponent<VoxelTile>());
            }

            LeftSideTiles.Add(GameObject.FindGameObjectWithTag("LeftTop").GetComponent<VoxelTile>());

            //NeighboursManager.CreateLeftNeighbour(LeftSideTiles);
            Storage.GetComponent<SaveTilePrefabsTest>().CreateLeftNeighbour(LeftSideTiles);
            /*float offsetZ = 0; 
            for (int i = 0; i < LeftSideTiles.Count; i++)
            {
                var item = LeftSideTiles[i];
                print(item);
                Instantiate(item , new Vector3(0, 0, offsetZ), item.transform.rotation);
                offsetZ += 0.8f;
            }*/
        }
    }

    void Generate()
    {
        possibleTiles = new List<VoxelTile>[MapSize.x, MapSize.y];
        int maxAttempts = 10;
        int attempts = 0;

        while (attempts++ < maxAttempts)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    // Иницализация списка, в каждую ячейку карты кладем все Тайлы, так как они все могут там быть на старте
                    possibleTiles[x, y] = new List<VoxelTile>(TilePrefabs);
                }
            }

            

            //Размещаем рандомный тайл в центр карты
            //VoxelTile tileInStart = GetRandomTile(sidesTiles);

            //possibleTiles[MapSize.x / 2, MapSize.y / 2] = new List<VoxelTile> { tileInCenter };
            recalcPossibleTilesQueue.Clear();
            for (int i = 1; i < sidesTiles.Count + 1; i++)
            {
                VoxelTile tile = sidesTiles[i - 1];
                possibleTiles[MapSize.x - 2, i] = new List<VoxelTile> { tile };
                EnqueueNeighboursToRecalc(new Vector2Int(MapSize.x - 2, i));
                //sidesTiles.Remove(sidesTiles.First());

            }

            /*possibleTiles[1, 1] = new List<VoxelTile> { tileInStart };
            possibleTiles[1, 2] = new List<VoxelTile> { tileInStart };
            possibleTiles[1, 3] = new List<VoxelTile> { tileInStart };
            possibleTiles[1, 4] = new List<VoxelTile> { tileInStart };
            possibleTiles[1, 5] = new List<VoxelTile> { tileInStart };
            possibleTiles[1, 6] = new List<VoxelTile> { tileInStart };
            possibleTiles[1, 7] = new List<VoxelTile> { tileInStart };
            possibleTiles[1, 8] = new List<VoxelTile> { tileInStart };
          
            recalcPossibleTilesQueue.Clear();
            EnqueueNeighboursToRecalc(new Vector2Int(1, 1));
            EnqueueNeighboursToRecalc(new Vector2Int(2, 2));
            EnqueueNeighboursToRecalc(new Vector2Int(1, 3));
            EnqueueNeighboursToRecalc(new Vector2Int(1, 4));
            EnqueueNeighboursToRecalc(new Vector2Int(1, 5));
            EnqueueNeighboursToRecalc(new Vector2Int(1, 6));
            EnqueueNeighboursToRecalc(new Vector2Int(1, 7));
            EnqueueNeighboursToRecalc(new Vector2Int(1, 8));*/


            bool success = GenerateAllPossibleTiles();

            if (success) break;

        }

        StartCoroutine(PlaceAllTiles());
        //PlaceAllTiles();
    }

    private bool GenerateAllPossibleTiles()
    {
        int maxIterations = MapSize.x * MapSize.y;
        int iterations = 0;
        int backtracks = 0;

        while (iterations++ < maxIterations)
        {
            int maxInnerIterations = 500;
            int innerIterations = 0;
            while (recalcPossibleTilesQueue.Count > 0 && innerIterations++ < maxInnerIterations)
            {

                Vector2Int position = recalcPossibleTilesQueue.Dequeue();

                if (position.x == 0 || position.y == 0 ||
                    position.x == MapSize.x - 1 || position.y == MapSize.y - 1)
                {
                    continue;
                }

                List<VoxelTile> possibleTilesHere = possibleTiles[position.x, position.y];

                int countRemoved = possibleTilesHere.RemoveAll(t => !IsTilePossible(t, position));

                if (countRemoved > 0) EnqueueNeighboursToRecalc(position);

                //что если possibleTilesHere - пустой?
                if (possibleTilesHere.Count == 0)
                {
                    // Зашли в тупик, в этих координатах невозможен ни один тайл, попробуем еще раз, разрешим все тайлы
                    // в этих и соседних координатах, и посмотрим устаканится ли всё
                    possibleTilesHere.AddRange(TilePrefabs);
                    possibleTiles[position.x + 1, position.y] = new List<VoxelTile>(TilePrefabs);
                    possibleTiles[position.x - 1, position.y] = new List<VoxelTile>(TilePrefabs);
                    possibleTiles[position.x, position.y + 1] = new List<VoxelTile>(TilePrefabs);
                    possibleTiles[position.x, position.y - 1] = new List<VoxelTile>(TilePrefabs);

                    EnqueueNeighboursToRecalc(position);
                    backtracks++;
                }
            }

            if (innerIterations == maxIterations)
            {
                break;
            }

            List<VoxelTile> maxCountTile = possibleTiles[1, 1];
            Vector2Int maxCountTilePosition = new Vector2Int(1, 1);

            for (int x = 1; x < MapSize.x - 1; x++)
            {
                for (int y = 1; y < MapSize.y - 1; y++)
                {
                    if (possibleTiles[x, y].Count > maxCountTile.Count)
                    {
                        maxCountTile = possibleTiles[x, y];
                        maxCountTilePosition = new Vector2Int(x, y);
                    }
                }
            }

            if (maxCountTile.Count == 1)
            {
                Debug.Log($"Generated for {iterations} iterations, with {backtracks} backtracks");
                return true;
            }

            VoxelTile tileToCollapse = GetRandomTile(maxCountTile);
            possibleTiles[maxCountTilePosition.x, maxCountTilePosition.y] = new List<VoxelTile> { tileToCollapse };
            EnqueueNeighboursToRecalc(maxCountTilePosition);
        }

        Debug.Log($"Failed, run out of iterations with {backtracks} backtracks");
        return false;

    }

    private bool IsTilePossible(VoxelTile tile, Vector2Int position)
    {
        bool isAllRightImpossible = possibleTiles[position.x - 1, position.y]
            .All(rightTile => !CanAppendTile(tile, rightTile, Direction.Right));
        if (isAllRightImpossible) return false;

        bool isAllLeftImpossible = possibleTiles[position.x + 1, position.y]
            .All(leftTile => !CanAppendTile(tile, leftTile, Direction.Left));
        if (isAllLeftImpossible) return false;

        bool isAllForwardImpossible = possibleTiles[position.x, position.y - 1]
            .All(forwardTile => !CanAppendTile(tile, forwardTile, Direction.Forward));
        if (isAllForwardImpossible) return false;

        bool isAllBackImpossible = possibleTiles[position.x, position.y + 1]
            .All(backTile => !CanAppendTile(tile, backTile, Direction.Back));
        if (isAllBackImpossible) return false;

        return true;
    }

    private IEnumerator PlaceAllTiles()
    {
        for (int x = 1; x < MapSize.x - 1; x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {

                PlaceTile(x, y);
                yield return new WaitForSeconds(0.0001f);
            }
        }
    }

    private void EnqueueNeighboursToRecalc(Vector2Int position)
    {
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x + 1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x - 1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y + 1));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y - 1));
    }

    // вспомог ф-я, которая размещает тайл на определнном участке террейна
    private void PlaceTile(int x, int y)
    {
        if (possibleTiles[x, y].Count == 0) return;

        VoxelTile selectedTile = GetRandomTile(possibleTiles[x, y]);
        Vector3 position = selectedTile.voxelSize * selectedTile.tileVoxelSize * new Vector3(x, 0, y);

        spawnedTiles[x, y] = Instantiate(selectedTile, position, selectedTile.transform.rotation);
        spawnedTiles[x, y].transform.parent = gameObject.transform;

        if (x == 1)
        {
            if (y == 1)
            {
                spawnedTiles[x, y].transform.GetChild(0).tag = "LeftBottom";
            }
            else if (y == MapSize.y - 2)
            {
                spawnedTiles[x, y].transform.GetChild(0).tag = "LeftTop";
            }
            else
            {
                spawnedTiles[x, y].transform.GetChild(0).tag = "Left";
            }
        }
        else if (x == MapSize.x - 2)
        {
            if (y == 1)
            {
                spawnedTiles[x, y].transform.GetChild(0).tag = "RightBottom";
            }
            else if (y == MapSize.y - 2)
            {
                spawnedTiles[x, y].transform.GetChild(0).tag = "RightTop";
            }
            else
            {
                spawnedTiles[x, y].transform.GetChild(0).tag = "Right";
            }

        }
        else if (y == 1)
        {

            spawnedTiles[x, y].transform.GetChild(0).tag = "Bottom";

        }
        else if (y == MapSize.y - 2)
        {

            spawnedTiles[x, y].transform.GetChild(0).tag = "Top";
        }

        /*if (x == 1 || x == MapSize.x - 2
         || y == 1 || y == MapSize.y - 2)
        {
            spawnedTiles[x, y].transform.GetChild(0).tag = "MapEnd";
        }*/
    }

    private VoxelTile GetRandomTile(List<VoxelTile> availableTiles)
    {
        List<int> chances = new List<int>();
        for (int i = 0; i < availableTiles.Count; i++)
        {
            chances.Add(availableTiles[i].Weight);
        }

        int value = Random.Range(0, chances.Sum());
        int sum = 0;

        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (value < sum)
            {
                return availableTiles[i];
            }
        }

        return availableTiles[availableTiles.Count - 1];

    }

    private bool CanAppendTile(VoxelTile existingTile, VoxelTile tileToAppend, Direction direction)
    {
        // Если тайла сбоку нет, то к пустоте можем любой тайл поставить
        if (existingTile == null) return true;

        if (direction == Direction.Right)
        {
            return Enumerable.SequenceEqual(existingTile.сolorsRightSide, tileToAppend.сolorsLeftSide);
        }
        else if (direction == Direction.Left)
        {
            return Enumerable.SequenceEqual(existingTile.сolorsLeftSide, tileToAppend.сolorsRightSide);
        }
        else if (direction == Direction.Forward)
        {
            return Enumerable.SequenceEqual(existingTile.сolorsForwardSide, tileToAppend.сolorsBackSide);
        }
        else if (direction == Direction.Back)
        {
            return Enumerable.SequenceEqual(existingTile.сolorsBackSide, tileToAppend.сolorsForwardSide);
        }
        else
        {
            throw new ArgumentException("Wrong direction value, should be Direction.left/right/back/forward",
                nameof(direction));
        }
    }
}