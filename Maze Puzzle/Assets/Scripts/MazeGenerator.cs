using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    private static WallState[,] RecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        var rng = new System.Random(/*seed*/);
        var positionStack = new Stack<Position>();
        var position = new Position
        {
            X = 0,
            Y = 0
        };

        maze[position.X, position.Y] |= WallState.VISITED;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbors = GetUnvisitedNeighbors(current, maze, width, height);

            if (neighbors.Count > 0)
            {
                positionStack.Push(current);

                var rngIndex = rng.Next(0, neighbors.Count);
                var rngNeighbor = neighbors[rngIndex];

                var neighborPosition = rngNeighbor.Position;
                maze[current.X, current.Y] &= ~rngNeighbor.SharedWall;
                maze[neighborPosition.X, neighborPosition.Y] &= ~GetOppositeWall(rngNeighbor.SharedWall);
                maze[neighborPosition.X, neighborPosition.Y] |= WallState.VISITED;

                positionStack.Push(neighborPosition);
            }
        }

        maze[0, 0] &= ~WallState.LEFT;
        maze[width - 1, height - 1] &= ~WallState.RIGHT;

        return maze;
    }

    private static List<Neighbor> GetUnvisitedNeighbors(Position p, WallState[,] maze, int width, int height)
    {
        var listOfNeighbors = new List<Neighbor>();

        if (p.X > 0)
        {
            if (!maze[p.X - 1, p.Y].HasFlag(WallState.VISITED))
            {
                listOfNeighbors.Add(new Neighbor
                {
                    Position = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }

        if (p.Y > 0)
        {
            if (!maze[p.X, p.Y - 1].HasFlag(WallState.VISITED))
            {
                listOfNeighbors.Add(new Neighbor
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }

        if (p.Y < height - 1)
        {
            if (!maze[p.X, p.Y + 1].HasFlag(WallState.VISITED))
            {
                listOfNeighbors.Add(new Neighbor
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        if (p.X < width - 1)
        {
            if (!maze[p.X + 1, p.Y].HasFlag(WallState.VISITED))
            {
                listOfNeighbors.Add(new Neighbor
                {
                    Position = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }

        return listOfNeighbors;
    }

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.RIGHT;
        }
    }

    public static WallState[,] GenerateMaze(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
            }
        }
        return RecursiveBacktracker(maze, width, height);
    }
}
