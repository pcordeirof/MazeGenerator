using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinder 
{
    static List<Node> openList;
    static List<Node> closedList;
    static Node[,] allNodes;

    public static List<Vector3> CalculatePath(Vector2Int start, Vector2Int target, WallState[,] maze, Vector3[,] cellPositions, int width, int height)
    {
        List<Vector3> finalPath = new List<Vector3>();
        List<Vector2Int> path = new List<Vector2Int>();

        openList = new List<Node>();
        closedList = new List<Node>();
        allNodes = new Node[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                allNodes[i, j] = new Node();
            }
        }

        Node firstNode = new Node()
        {
            nodePosition = start,
            gCost = 0,
            isOnClosedList = true
        };
        closedList.Add(firstNode);
        path.Add(target);

        foreach (Vector2Int position in GetNeighbors(firstNode.nodePosition, maze))
        {
            Node node = new Node()
            {
                nodePosition = position,
                parent = firstNode,
                hCost = CalculateHCost(position, target),
                gCost = CalculateGCost(firstNode, position)
            };

            node.fCost = node.gCost + node.hCost;
            node.isOnClosedList = true;
            allNodes[position.x, position.y] = node;
            openList.Add(node);
        }

        Node lastNode = new Node();

        while (openList.Count > 0)
        {
            Node node = openList[0];

            foreach (Node pathNode in openList)
            {
                if (node.fCost > pathNode.fCost || (node.fCost == pathNode.fCost && node.hCost > pathNode.hCost)) node = pathNode;
            }

            if (node.nodePosition == target)
            {
                lastNode = node;
                break;
            }

            closedList.Add(node);
            allNodes[node.nodePosition.x, node.nodePosition.y].isOnClosedList = true;
            openList.Remove(node);
            allNodes[node.nodePosition.x, node.nodePosition.y].isOnOpenList = false;

            bool foundTarget = false;

            foreach (Vector2Int position in GetNeighbors(node.nodePosition, maze))
            {
                if (position == target)
                {
                    foundTarget = true;
                    break;
                }

                Node neighbor = new Node()
                {
                    nodePosition = position,
                    parent = node,
                    gCost = CalculateGCost(node, position),
                    hCost = CalculateHCost(position, target),
                    isOnOpenList = true
                };

                neighbor.fCost = neighbor.gCost + neighbor.hCost;
                allNodes[position.x, position.y] = neighbor;
                openList.Add(neighbor);
            }

            if (foundTarget)
            {
                openList.Clear();
                lastNode = node;
                break;
            }
        }

        RetracePath(path, start, lastNode);



        return finalPath;
    }

    private static List<Vector2Int> GetNeighbors(Vector2Int parent, WallState[,] maze)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        
        if(!maze[parent.x, parent.y].HasFlag(WallState.UP))
        {
            neighbors.Add(new Vector2Int(parent.x , parent.y + 1));
        }

        if (!maze[parent.x, parent.y].HasFlag(WallState.DOWN))
        {
            neighbors.Add(new Vector2Int(parent.x, parent.y - 1));
        }

        if (!maze[parent.x, parent.y].HasFlag(WallState.LEFT))
        {
            neighbors.Add(new Vector2Int(parent.x - 1, parent.y));
        }

        if (!maze[parent.x, parent.y].HasFlag(WallState.RIGHT))
        {
            neighbors.Add(new Vector2Int(parent.x + 1, parent.y));
        }

        return neighbors;
    }

    private static int CalculateHCost(Vector2Int position, Vector2Int target)
    {
        int hCost = 0;

        int x = Mathf.Abs(position.x - target.x);
        int y = Mathf.Abs(position.y - target.y);

        hCost = x + y;

        return hCost;
    }

    private static int CalculateGCost(Node parent, Vector2Int position)
    {
        int localG;

        if (position.x != parent.nodePosition.x && position.y != parent.nodePosition.y) localG = 14;
        else localG = 10;

        int gCost = parent.fCost + localG;

        return gCost;
    }

    private static void RetracePath(List<Vector2Int> path, Vector2Int start, Node lastNode)
    {
        Node current = lastNode;

        while (current.nodePosition != start)
        {
            path.Add(current.nodePosition);
            if (current.parent == null) break;
            current = current.parent;
        }

        path.Reverse();
    }

    private static List<Vector3> ConvertPath(List<Vector2Int> path, Vector3[,] cellPositions)
    {
        List<Vector3> finalPath = new List<Vector3>();

        foreach (Vector2Int position in path)
        {
            finalPath.Add(new Vector3(cellPositions[position.x, position.y].x, cellPositions[position.x, position.y].y, cellPositions[position.x, position.y].z));
        }

        return finalPath;
    }
}
