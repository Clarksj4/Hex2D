using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HexMap : MonoBehaviour
{
    public float OuterRadius = 1;
    public int CellsWide = 2;
    public int CellsHigh = 2;

    public float OuterDiameter { get; private set; }
    public float InnerRadius { get; private set; }
    public float InnerDiameter { get; private set; }
    public Vector3 Size { get; private set; }
    public Vector3 Extents { get; private set; }
    public Vector3 RowOffset { get; private set; }

    private void Init()
    {
        OuterDiameter = OuterRadius * 2;
        InnerRadius = OuterRadius * 0.866025404f;
        InnerDiameter = InnerRadius * 2;

        int isRowOffset = CellsHigh > 1 ? 1 : 0;

        float width = (InnerDiameter * CellsWide) + (isRowOffset * InnerRadius);
        float height = OuterDiameter + (OuterRadius * 1.5f * (CellsHigh - 1));

        Size = new Vector3(width, height);
        Extents = Size / 2;

        GetComponent<BoxCollider2D>().size = Size;
    }

    private void OnValidate()
    {
        Init();
    }

    private void OnDrawGizmos()
    {
        for (int column = 0; column < CellsWide; column++)
        {
            for (int row = 0; row < CellsHigh; row++)
            {
                DrawCell(column, row);
            }
        }
    }

    private void DrawCell(int x, int y)
    {
        Vector3 cellCentre = GetCellCentre(new Coordinate(x, y));
        Vector3[] vertices = HexMesh.GetVertices(cellCentre, OuterRadius);

        for (int i = 0; i < vertices.Length - 1; i++)
            Gizmos.DrawLine(vertices[i], vertices[i + 1]);

        Gizmos.DrawLine(vertices[vertices.Length - 1], vertices[0]);
    }

    private void OnMouseDown()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Coordinate cell = GetCell(worldPosition);
        Vector3 cellCentre = GetCellCentre(cell);

        print("World Position 1: " + worldPosition + ", Cell: " + cell + ", " + "Cell Centre: " + cellCentre);
    }
    
    public Vector3 GetCellCentre(Coordinate cell)
    {
        // Is the row odd (odd rows are offset by InnerRadius distance)
        int odd = cell.y % 2;

        // Get cell centre's distance from bottom left of map
        float xFromBottomLeft = InnerRadius + (cell.x * InnerDiameter) + (odd * InnerRadius);
        float yFromBottomLeft = OuterRadius + (cell.y * 1.5f * OuterRadius);
        Vector3 fromBottomLeft = new Vector3(xFromBottomLeft, yFromBottomLeft);

        // Convert to distance from centre of map
        Vector3 fromCentre = fromBottomLeft - Extents;
        
        // Convert to world position
        Vector3 worldPosition = transform.TransformPoint(fromCentre);
        return worldPosition;
    }

    //
    // https://stackoverflow.com/questions/7705228/hexagonal-grids-how-do-you-find-which-hexagon-a-point-is-in
    //
    private Coordinate GetCell(Vector3 point)
    {
        Vector3 localPoint = transform.InverseTransformPoint(point);
        Vector3 fromBottomLeft = localPoint + Extents;

        float gridHeight = 1.5f * OuterRadius;
        float gridWidth = InnerDiameter;
        float halfWidth = InnerRadius;
        float c = 0.5f * OuterRadius;
        float m = c / halfWidth; 

        // Find the row and column of the box that the point falls in.
        int row = (int)(fromBottomLeft.y / gridHeight);
        int column;

        bool rowIsOdd = row % 2 == 1;

        // Is the row an odd number?
        if (rowIsOdd)// Yes: Offset x to match the indent of the row
            column = Mathf.FloorToInt((fromBottomLeft.x - halfWidth) / gridWidth);
        else// No: Calculate normally
            column = (int)(fromBottomLeft.x / gridWidth);

        // Work out the position of the point relative to the box it is in
        double relY = fromBottomLeft.y - (row * gridHeight);
        double relX;

        if (rowIsOdd)
            relX = (fromBottomLeft.x - (column * gridWidth)) - halfWidth;
        else
            relX = fromBottomLeft.x - (column * gridWidth);

        // Work out if the point is above either of the hexagon's top edges
        if (relY < (-m * relX) + c) // LEFT edge
        {
            row--;
            if (!rowIsOdd)
                column--;
        }
        else if (relY < (m * relX) - c) // RIGHT edge
        {
            row--;
            if (rowIsOdd)
                column++;
        }

        return new Coordinate(column, row);
    }
}
