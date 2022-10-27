using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    //=====Summary=====
    //This class deals with the presentation of the board, selecting relevat tiles based on data in Cell.cs

    // This game is set entirely within a tilemap
    public Tilemap tilemap { get; private set; }

    // Instantiating all possible tiles to be used across the tilemap

    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;
    

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    //Draws out the Board, based on 2-dimensional array instantiated in Game.cs
    // width and height declared in Game.cs based on user input
    public void Draw(Cell[,] state)
    {
        tilemap.ClearAllTiles();
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    //Initial method to be called in Draw() to determine the properties of each tile to be presented
    //initially only determines if cell is revealed or flagged.
     private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            // GetRevealedTile() determines the tile type
            return GetRevealedTile(cell);
        }
        else if (cell.flagged)
        {
            return tileFlag;
        }
        else
        {
            return tileUnknown;
        }
    }

    //Secondary step to determine the properties of a tile, only called if the tile is revealed
    //Used a tertiary method if the tile type is Number.
    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    //Final method to determine the properties of a tile, only called if the tile is Number type.
    //Basic switch statement to return the relevant number tile.
    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
            
    }


}

