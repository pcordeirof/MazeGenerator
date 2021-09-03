using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public Vector2Int nodePosition;
    public int gCost;
    public int hCost;
    public int fCost;
    public Node parent;
    public bool isOnOpenList = false;
    public bool isOnClosedList = false;
}
