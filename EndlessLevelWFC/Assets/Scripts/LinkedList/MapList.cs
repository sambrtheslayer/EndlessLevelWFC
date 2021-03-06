using System;
using System.Collections.Generic;

namespace LinkedList
{
    public class MapList<T>
    {
        public int count = 0;
        public MapNode<T> start;
        private Queue<MapNode<T>> queueBFC = new Queue<MapNode<T>>();

        public void AddStartMap(T map)
        {
            MapNode<T> node = new MapNode<T>(map);
            start = node;
            count++;
        }

        public void AddNeighbour(MapNode<T> currentMap, T newMap, Direction direction)
        {
            //MapNode<T> currMapNode = FindMap(currentMap);
            MapNode<T> newMapNode = new MapNode<T>(newMap);

            switch (direction)
            {
                case Direction.Left:
                    currentMap.Left = newMapNode;
                    newMapNode.Right = currentMap;
                    count++;
                    break;
                case Direction.Right:
                    currentMap.Right = newMapNode;
                    newMapNode.Left = currentMap;
                    count++;
                    break;
                case Direction.Forward:
                    currentMap.Top = newMapNode;
                    newMapNode.Bottom = currentMap;
                    count++;
                    break;
                case Direction.Back:
                    currentMap.Bottom = newMapNode;
                    newMapNode.Top = currentMap;
                    count++;
                    break;
                default:
                    throw new ArgumentException("Wrong direction value, should be Direction.left/right/back/forward",
                            nameof(direction));
            }
        }

        // breadth-first search
        public MapNode<T> FindMap(T map)
        {
            List<MapNode<T>> visited = new List<MapNode<T>>();

            if (start.Data.Equals(map))
            {
                return start;
            }
            else
            {
                if (!visited.Contains(start))
                {
                    if (start.Left != null)
                    {
                        queueBFC.Enqueue(start.Left);
                    }
                    if (start.Top != null)
                    {
                        queueBFC.Enqueue(start.Top);
                    }
                    if (start.Right != null)
                    {
                        queueBFC.Enqueue(start.Right);
                    }
                    if (start.Bottom != null)
                    {
                        queueBFC.Enqueue(start.Bottom);
                    }

                    visited.Add(start);
                }

                while (queueBFC.Count > 0)
                {
                    MapNode<T> mapNode = queueBFC.Dequeue();

                    if (mapNode.Data.Equals(map))
                    {
                        queueBFC.Clear();
                        return mapNode;
                    }
                    else
                    {
                        if (!visited.Contains(mapNode))
                        {
                            if (mapNode.Left != null)
                            {
                                queueBFC.Enqueue(mapNode.Left);
                            }
                            if (mapNode.Top != null)
                            {
                                queueBFC.Enqueue(mapNode.Top);
                            }
                            if (mapNode.Right != null)
                            {
                                queueBFC.Enqueue(mapNode.Right);
                            }
                            if (mapNode.Bottom != null)
                            {
                                queueBFC.Enqueue(mapNode.Bottom);
                            }

                            visited.Add(mapNode);
                        }
                    }
                }
                queueBFC.Clear();
                return new MapNode<T>("-1");
            }
        }
    }
}