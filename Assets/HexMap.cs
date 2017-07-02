using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HexMap : MonoBehaviour
{
    public float OuterRadius = 1;
    public int CellsWide = 2;
    public int CellsHigh = 2;

    public HexCell this[AxialCoordinate coordinate]
    {
        get
        {
            // Check if map contains cell at given coordinate
            if (!Contains(coordinate))
                return null;

            return cells[coordinate.Column, coordinate.Y];
        }
    }

    public float OuterDiameter { get; private set; }
    public float InnerRadius { get; private set; }
    public float InnerDiameter { get; private set; }
    public Vector3 Size { get; private set; }
    public Vector3 Extents { get; private set; }

    private HexCell[,] cells;

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

        // Set size of bounding box
        GetComponent<BoxCollider2D>().size = Size;

        // Create cell scene objects
        CreateCells();
    }

    //
    // https://stackoverflow.com/questions/7705228/hexagonal-grids-how-do-you-find-which-hexagon-a-point-is-in
    //
    // Checks if the point lies within a rectangular box that covers the bottom 3/4 of the hex. Checks if the
    // point lies on either side of the sloped sides of the bottom of the hexagon and adjusts the coordinate
    // accordingly
    public HexCell GetCell(Vector3 point)
    {
        // Convert to local point then point relative to bottom left of the hex map
        Vector3 localPoint = transform.InverseTransformPoint(point);
        Vector3 fromBottomLeft = localPoint + Extents;

        // Using stackoverflow names
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

        // If there is no cell at point
        if (!Contains(column, row))
            return null;

        return cells[row, column];
    }

    public bool Contains(AxialCoordinate coordinate)
    {
        return Contains(coordinate.Column, coordinate.Y);
    }

    private bool Contains(int x, int y)
    {
        return x >= 0 &&
               x < cells.GetLength(1) &&
               y >= 0 &&
               y < cells.GetLength(0);
    }

    private void CreateCells()
    {
        cells = new HexCell[CellsHigh, CellsWide];
        for (int column = 0; column < CellsWide; column++)
        {
            for (int row = 0; row < CellsHigh; row++)
            {
                // Calculate axial coordinate
                AxialCoordinate coordinate = AxialCoordinate.FromGridIndices(column, row);

                // Create scene object as child, named with coordinate
                GameObject hexObject = new GameObject("HexCell: " + coordinate.ToString());
                hexObject.transform.SetParent(transform);

                // Add cell component, set coordinate, and corresponding position
                HexCell cell = hexObject.AddComponent<HexCell>();
                cell.Coordinate = coordinate;
                cell.transform.position = GetCellCentre(cell.Coordinate);

                // Save to 2d array
                cells[row, column] = cell;
            }
        }
    }

    public Vector3 GetCellCentre(AxialCoordinate cell)
    {
        // Is the row odd (odd rows are offset by InnerRadius distance)
        int odd = cell.Y % 2;

        // Get cell centre's distance from bottom left of map
        float xFromBottomLeft = InnerRadius + (cell.Column * InnerDiameter) + (odd * InnerRadius);
        float yFromBottomLeft = OuterRadius + (cell.Y * 1.5f * OuterRadius);
        Vector3 fromBottomLeft = new Vector3(xFromBottomLeft, yFromBottomLeft);

        // Convert to distance from centre of map
        Vector3 fromCentre = fromBottomLeft - Extents;

        // Convert to world position
        Vector3 worldPosition = transform.TransformPoint(fromCentre);
        return worldPosition;
    }


}
