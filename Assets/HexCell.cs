using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class HexCell : MonoBehaviour
{
    public AxialCoordinate Coordinate;
    public IEnumerable<HexCell> Neighbours { get { return AxialCoordinate.Directions.Select(d => map[Coordinate + d]); } }

    private HexMap map;
    
    void Awake()
    {
        map = GetComponentInParent<HexMap>();
    }
}
