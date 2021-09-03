using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [SerializeField]
    private float size;

    [SerializeField]
    private Transform wallPrefab;

    [SerializeField]
    private Transform floorPrefab;

    public Vector3[,] cellPositions;

    void Start()
    {
        cellPositions = new Vector3[width, height];
        var maze = MazeGenerator.GenerateMaze(width, height);
        DrawMaze(maze);
    }

    private void DrawMaze(WallState[,] maze)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cell = maze[i, j];
                var position = new Vector3(i * size, 0, j * size);

                if (cell.HasFlag(WallState.UP))
                {
                    var wall = Instantiate(wallPrefab, transform) as Transform;
                    wall.position = position + new Vector3(0, 0, size / 2);
                    wall.localScale = new Vector3(size, wall.localScale.y, wall.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var wall = Instantiate(wallPrefab, transform) as Transform;
                    wall.position = position + new Vector3(-size / 2, 0, 0);
                    wall.localScale = new Vector3(size, wall.localScale.y, wall.localScale.z);
                    wall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var wall = Instantiate(wallPrefab, transform) as Transform;
                        wall.position = position + new Vector3(size / 2, 0, 0);
                        wall.localScale = new Vector3(size, wall.localScale.y, wall.localScale.z);
                        wall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var wall = Instantiate(wallPrefab, transform) as Transform;
                        wall.position = position + new Vector3(0, 0, -size / 2);
                        wall.localScale = new Vector3(size, wall.localScale.y, wall.localScale.z);
                    }
                }

                var floor = Instantiate(floorPrefab, transform) as Transform;
                floor.position = position;

                cellPositions[i, j] = new Vector3(position.x, position.y, position.z);
            }
        }
    }

}
