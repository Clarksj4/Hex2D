using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapTest : MonoBehaviour
{
    private HexMap map;

    void Awake()
    {
        map = GetComponent<HexMap>();
    }

    private void OnMouseDown()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        HexCell cell = map.GetCell(worldPosition);

        string cellInfo = cell == null ? "None" : cell.Coordinate.ToString();
        print("World Position 1: " + worldPosition + ", Cell: " + cellInfo);
    }
}
