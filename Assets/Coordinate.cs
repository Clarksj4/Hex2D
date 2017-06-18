using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public struct Coordinate
{
    public static Coordinate zero = new Coordinate(0, 0);
    public static Coordinate one = new Coordinate(1, 1);
    public static Coordinate up = new Coordinate(0, 1);
    public static Coordinate down = new Coordinate(0, -1);
    public static Coordinate left = new Coordinate(-1, 0);
    public static Coordinate right = new Coordinate(1, 0);

    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Coordinate operator +(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x + b.x, a.y + b.y);
    }

    public static Coordinate operator -(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x - b.x, a.y - b.y);
    }

    public static Coordinate operator *(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x * b.x, a.y * b.y);
    }

    public static Coordinate operator *(Coordinate a, int scale)
    {
        return new Coordinate(a.x * scale, a.y * scale);
    }

    public static Vector2 operator *(Coordinate coordinate, Vector2 scale)
    {
        return new Vector2(coordinate.x * scale.x, coordinate.y * scale.y);
    }

    public static Coordinate operator /(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x / b.x, a.y / b.y);
    }

    public static Coordinate operator /(Coordinate a, int scale)
    {
        return new Coordinate(a.x / scale, a.y / scale);
    }

    public static explicit operator Coordinate(Vector2 point)
    {
        return new Coordinate((int)point.x, (int)point.y);
    }

    public override string ToString()
    {
        return x.ToString() + ", " + y.ToString();
    }
}
