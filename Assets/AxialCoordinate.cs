using System;
using UnityEngine;

[Serializable]
public struct AxialCoordinate
{
    public static AxialCoordinate Zero = new AxialCoordinate(0, 0);
    public static AxialCoordinate UpRight = new AxialCoordinate(0, 1);
    public static AxialCoordinate Right = new AxialCoordinate(1, 0);
    public static AxialCoordinate DownRight = new AxialCoordinate(1, -1);
    public static AxialCoordinate DownLeft = new AxialCoordinate(0, -1);
    public static AxialCoordinate Left = new AxialCoordinate(-1, 0);
    public static AxialCoordinate UpLeft = new AxialCoordinate(-1, 1);

    public static AxialCoordinate[] Directions = new AxialCoordinate[]
        {
            UpRight, 
            Right, 
            DownRight,
            DownLeft,
            Left,
            UpLeft
        };

    public int X { get { return x; } }
    public int Y { get { return y; } }

    /// <summary>
    /// Convert axial coordinate x to a column index in a grid layout.
    /// </summary>
    public int Column { get { return x + y / 2; } }

    [SerializeField]
    private int x;
    [SerializeField]
    private int y;

    public AxialCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Calculate an axial coordinate from a column and row index in a grid layout. Column and Row
    /// indices are assumed to start at the bottom left of the grid layout
    /// </summary>
    public static AxialCoordinate FromGridIndices(int column, int row)
    {
        return new AxialCoordinate(column - row / 2, row);
    }

    public static AxialCoordinate operator +(AxialCoordinate a, AxialCoordinate b)
    {
        return new AxialCoordinate(a.X + b.X, a.Y + b.Y);
    }

    public static AxialCoordinate operator -(AxialCoordinate a, AxialCoordinate b)
    {
        return new AxialCoordinate(a.X - b.X, a.Y - b.Y);
    }

    public static AxialCoordinate operator *(AxialCoordinate a, int scale)
    {
        return new AxialCoordinate(a.X * scale, a.Y * scale);
    }

    public static AxialCoordinate operator /(AxialCoordinate a, int scale)
    {
        return new AxialCoordinate(a.X / scale, a.Y / scale);
    }

    public override string ToString()
    {
        return X.ToString() + ", " + Y.ToString();
    }
}
