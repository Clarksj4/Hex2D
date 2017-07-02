using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class HexMesh : MonoBehaviour
{
    public float OuterRadius = 1;
    public int CellsWide = 2;
    public int CellsHigh = 2;

    public float OuterDiameter { get; private set; }
    public float InnerRadius { get; private set; }
    public float InnerDiameter { get; private set; }
    public Vector3 Size { get; private set; }
    public Vector3 Extents { get; private set; }

    private Mesh mesh;
    private List<Vector3> verts;
    private List<int> tris;
    private List<Vector3> normals;
    private List<Vector2> uvs;

    private void Awake()
    {
        // Dimensions of a cell
        OuterDiameter = OuterRadius * 2;
        InnerRadius = OuterRadius * 0.866025404f;
        InnerDiameter = InnerRadius * 2;

        // Is there more than a single row? (odd rows are offset which increases width)
        int isRowOffset = CellsHigh > 1 ? 1 : 0;

        // Width and Height of bounding box
        float width = (InnerDiameter * CellsWide) + (isRowOffset * InnerRadius);
        float height = OuterDiameter + (OuterRadius * 1.5f * (CellsHigh - 1));

        // Size and extents of bounding box
        Size = new Vector3(width, height);
        Extents = Size / 2;

        CreateMesh();

        Mesh mesh = CreateHex(Vector3.zero, 1);
        AssetDatabase.CreateAsset(mesh, "Assets/Hex.asset");
        AssetDatabase.SaveAssets();
    }

    private void CreateMesh()
    {
        verts = new List<Vector3>();
        tris = new List<int>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();

        Vector3 bottomLeftHexCentre = -Extents + new Vector3(InnerRadius, OuterRadius);

        for (int row = 0; row < CellsHigh; row++)
        {
            int odd = row % 2;

            for (int column = 0; column < CellsWide; column++)
            {
                // Centre of hex
                float hexCentreX = bottomLeftHexCentre.x + (column * InnerDiameter) + (odd * InnerRadius);
                float hexCentreY = bottomLeftHexCentre.y + (row * 1.5f * OuterRadius);
                Vector3 centre = new Vector3(hexCentreX, hexCentreY);

                CreateHex(centre);
            }
        }

        mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(normals);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void CreateHex(Vector3 centre)
    {
        // Index of to-be-added centre vert
        int centreVertIndex = verts.Count;

        // Add centre point of hex as vert, and 6 outer verts
        verts.Add(centre);
        verts.AddRange(GetVertices(centre, OuterRadius));
        normals.AddRange(Enumerable.Repeat(Vector3.forward, 7));

        for (int i = 0; i < 5; i++)
        {
            tris.Add(centreVertIndex);
            tris.Add(centreVertIndex + i + 2);
            tris.Add(centreVertIndex + i + 1);
        }

        tris.Add(centreVertIndex);
        tris.Add(centreVertIndex + 1);
        tris.Add(centreVertIndex + 6);
    }

    public static Mesh CreateHex(Vector3 centre, float outerRadius)
    {
        float innerRadius = outerRadius * HexMetrics.OUTER_TO_INNER_RADIUS_FACTOR;
        Vector3 extents = new Vector3(innerRadius, outerRadius);

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        // Add centre point of hex as vert, and 6 outer verts
        verts.Add(centre);
        verts.AddRange(GetVertices(centre, outerRadius));
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
