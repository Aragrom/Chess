using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SquareData
{
    public int id;
    public bool hasPiece;
    public int piece;
    public bool hasEnPassant;
    public int enPassant;

    public SquareData(int id, bool hasPiece, int piece, bool hasEnPassant, int enPassant)
    {
        this.id = id;
        this.hasPiece = hasPiece;
        this.piece = piece;
    
        this.hasEnPassant = hasEnPassant;
        this.enPassant = enPassant;
    }
}
