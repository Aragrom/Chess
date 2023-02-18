using System.Collections;
using System.Collections.Generic;


public class SquareLogic
{
    public string name = null;
    public int index = -1;
    public bool isWhite = false;
    public int row = -1;
    public int column = -1;
    public bool hasPiece = false;
    public PieceLogic piece = null;

    // Will be instantiated during an en passant move
    // Will leave the piece as "attackable" piece on this square
    // for one turn only. (if not taken removed)

    public bool hasEnPassant = false;
    public PieceLogic enPassant = null;

    public SquareLogic()
    { 
        
    }
}
