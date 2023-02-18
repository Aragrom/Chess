using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public int index = -1;
    public bool isWhite = false;
    public int row = -1;
    public int column = -1;
    public bool hasPiece = false;
    public Piece piece = null;

    // Will be instantiated during an en passant move
    // Will leave the piece as "attackable" piece on this square
    // for one turn only. (if not taken removed)

    public bool hasEnPassant = false;
    public Piece enPassant = null;
}
