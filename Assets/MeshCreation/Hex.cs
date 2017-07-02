using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class MeshCreation
{
    public static Mesh Hex2D(Vector3 centre, float outerRadius)
    {
        float innerRadius = outerRadius * HexMetrics.OUTER_TO_INNER_RADIUS_FACTOR;
        Vector3 extents = new Vector3(innerRadius, outerRadius);

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        // Add centre point of hex as vert, and 6 outer verts
        verts.Add(centre);
        verts.AddRange(CalculateHex2DVertices(centre, outerRadius));
        normals.AddRange(Enumerable.Repeat(Vector3.forward, 7));

        for (int i = 0; i < 5; i++)
        {
            tris.Add(0);
            tris.Add(i + 2);
            tris.Add(i + 1);
        }

        tris.Add(0);
        tris.Add(1);
        tris.Add(6);

        foreach (Vector3 vert in verts)
        {
            Vector3 localPosition = vert - centre;
            Vector3 fromBottomLeft = localPosition + extents;

            float u = fromBottomLeft.x / (innerRadius * 2);
            float v = fromBottomLeft.y / (outerRadius * 2);
            uvs.Add(new Vector2(u, v));
        }

        Mesh mesh = new Mesh();
        mesh.name = "Hex";
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);

        return mesh;
    }

    public static Vector3[] CalculateHex2DVertices(Vector3 centre, float sideLength)
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
