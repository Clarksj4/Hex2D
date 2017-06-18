using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class HexMesh
{
    public static Vector3[] GetVertices(Vector3 centre, float sideLength)
    {
        // Calculate each vert by rotating 60 degrees from 'up' each iteration
        Vector3[] vertices = new Vector3[6];
        for (int i = 0; i < vertices.Length; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(60 * i, Vector3.forward);
            vertices[i] = centre + (rotation * (Vector3.up * sideLength));
        }

        return vertices;
    }
}
