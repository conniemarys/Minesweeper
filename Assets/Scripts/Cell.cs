using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell 
{
    //====Summary====
    // Stores the data for each Cell within the tilemap to be sent to the presentation logic on Board.cs

    public enum Type
    {
        Invalid, 
        Empty,
        Mine,
        Number,
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool revealed;
    public bool flagged;
    public bool exploded;
}
