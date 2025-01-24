using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes 
{
    public int width;
    public int height;
    public Node[,] gridNodeArray;

    public GridNodes(int width,int height)
    {
        this.width = width;
        this.height = height;

        gridNodeArray = new Node[width, height];
        
        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
            {
                gridNodeArray[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int x,int y)
    {
        if (x < width && y < height)
        {
            return gridNodeArray[x, y];
        }
        else
        {
            Debug.Log("Grid x y is out of");
            return null;
        }
    }
} 
