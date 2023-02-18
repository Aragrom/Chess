using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool hasTurn = false;

    public bool isWhite = false;

    public Square selectedSquare = null;

    public Piece selectedPiece = null;

    public void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
    }

    public void SelectSquare(Square square)
    {
        selectedSquare = square;
    }

    public void MakeMove()
    { 
    
    }

    public void Forfeit()
    { 
    
    }
}
