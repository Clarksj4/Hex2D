using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class HexMath
{
    /// <summary>
    /// Determines which side of a line a point lies on. 0 = on line, -1 = one side, +1 = other side
    /// </summary>
    public static float Side(Vector2 edgePoint1, Vector2 edgePoint2, Vector2 point)
    {
        // position = sign((Bx - Ax) * (Y - Ay) - (By - Ay) * (X - Ax))
        float determinant = (edgePoint2.x - edgePoint1.x) * (point.y - edgePoint1.y) - (edgePoint2.y - edgePoint1.y) * (point.x - edgePoint1.x);

        // Mathf.Sign returns 1 when determinant is 0, so need to test for this
        if (determinant == 0) return 0;
        else return Mathf.Sign(determinant);
    }

    public static bool HexContains(Vector2[] vertices, Vector2 point)
    {
        // Assume hex contains points, if ANY side does not contain then it is false
        bool contains = true;

        // Initial side test
        float[] sides = new float[6];

        // Check which side of each hex edge the point lies on
        for (int i = 0; i < 6 && contains == true; i++)
        {
            sides[i] = Side(vertices[i], vertices[(i + 1) % 6], point);

            // Get the first side test that didn't evaluate to zero (or zero if there's none)
            float side = sides.Where(f => f != 0).FirstOrDefault();

            // If there was one that didn't evaluate to zero, check all other tests evaluated the same
            if (side != 0)
                contains = sides.All(s => s == side);
        }

        return contains;
    }
}
