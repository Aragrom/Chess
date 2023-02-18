using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <note>
/// Piece dimensions
/// 
/// ♔ ♚	♕ ♛	♖ ♜	♗ ♝	♘ ♞	♙ ♟
/// 
/// king base diameter should be roughly 75-80% of the size of the square
/// 
/// pawn base diameter should be roughly 50% of the size of the square
/// 
/// </note>

public class Piece : MonoBehaviour
{
    public Board board = null;

    public enum Type 
    { 
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public enum MoveType : int
    {
        North = 1,          // moving Up = index + 1
        South = -1,         // moving Down = index - 1
        East = 8,           // moving right = index + 8
        West = -8,          // moving left = index - 8

        NorthEast = 9,      // moving up and right = index + 9 
        NorthWest = -7,     // moving up and left = index - 7
        SouthEast = 7,      // moving down and right = index + 7
        SouthWest = -9,     // moving down and left = index - 9
    }

    public int id = -1;
    public int positionOnBoard = 0; // board square index
    public Type type = Type.Pawn; 
    public bool isWhite = false;
    public bool hasMoved = false;

    public float value = 0;

    public int[] defending = new int[0];
    public int[] moves = new int[0];
    public int[] attacks = new int[0];

    public void GetMoves()
    {
        switch (type)
        {
            case Type.Pawn:

                ((Pawn)this).GetMoves();

                break;

            case Type.Rook:

                ((Rook)this).GetMoves();

                break;

            case Type.Bishop:

                ((Bishop)this).GetMoves();

                break;

            case Type.Knight:

                ((Knight)this).GetMoves();

                break;

            case Type.Queen:

                ((Queen)this).GetMoves();

                break;

            case Type.King:

                ((King)this).GetMoves();

                break;
        }
    }

    public bool HasMove(int index)
    {
        Debug.Log("Piece.HasMove() - Ran base class function");

        for (int i = 0; i < moves.Length; i++)
        {
            if (index == moves[i]) return true;
        }

        for (int i = 0; i < attacks.Length; i++)
        {
            if (index == attacks[i]) return true;
        }

        switch (type)
        {
            case Type.Pawn:

                return ((Pawn)this).HasMove(index);

            case Type.King:

                return ((King)this).HasMove(index);
        }

        return false;
    }

    public void Move(int index)
    {
        Debug.Log("Piece.Move() - Ran base class function");

        // Search for move

        // Moves unique to each piece.

        switch (type)
        {
            case Type.Pawn:

                // "en passant" & "promotion"
                ((Pawn)this).Move(index);

                break;

            case Type.Rook:

                ((Rook)this).Move(index);

                break;

            case Type.Bishop:

                ((Bishop)this).Move(index);

                break;

            case Type.Knight:

                ((Knight)this).Move(index);

                break;

            case Type.Queen:

                ((Queen)this).Move(index);

                break;

            case Type.King:

                ((King)this).Move(index);

                break;
        }
    }

    public bool IsMoveLegal(int index)
    {
        bool result;

        int originalPosition = positionOnBoard;
        int destinationPosition = index;

        // Piece
        // Remove this piece from the old square/current square.

        board.squares[originalPosition].hasPiece = false;
        board.squares[originalPosition].piece = null;

        // Square
        // Move this piece to this square

        board.squares[destinationPosition].hasPiece = true;
        board.squares[destinationPosition].piece = this;

        positionOnBoard = destinationPosition;

        board.UpdateAllPieceMoves();

        ((King)board.whiteKing).KingUniqueMoves();
        ((King)board.blackKing).KingUniqueMoves();

        if (isWhite)
        {
            // White king is now in check? - Illegal move

            result = ((King)board.whiteKing).inCheck;
        }

        else
        {
            // Black king is now in check? - Illegal move.

            result = ((King)board.blackKing).inCheck;
        }

        // Reverse Move

        // Square
        // Remove this piece from the new square/current square.

        board.squares[destinationPosition].hasPiece = false;
        board.squares[destinationPosition].piece = null;

        // Piece
        // Move this piece to the original square

        board.squares[originalPosition].hasPiece = true;
        board.squares[originalPosition].piece = this;

        positionOnBoard = originalPosition;

        board.UpdateAllPieceMoves();

        ((King)board.whiteKing).KingUniqueMoves();
        ((King)board.blackKing).KingUniqueMoves();

        // Return the reverse of result - logic for IS MOVE LEGAL
        return !result;
    }

    public bool IsAttackLegal(int index)
    {
        bool result;

        int originalPosition = positionOnBoard;
        int destinationPosition = index;

        Piece attackedPiece = board.squares[index].piece;

        // disable attacked piece

        attackedPiece.gameObject.SetActive(false);

        // Piece
        // Remove this piece from previous square

        board.squares[originalPosition].hasPiece = false;
        board.squares[originalPosition].piece = null;

        // Square
        // Move this piece to this square

        board.squares[destinationPosition].hasPiece = true;
        board.squares[destinationPosition].piece = this;

        positionOnBoard = destinationPosition;

        board.UpdateAllPieceMoves();

        ((King)board.whiteKing).KingUniqueMoves();
        ((King)board.blackKing).KingUniqueMoves();


        if (isWhite)
        {
            // White king is now in check? - Illegal move

            result = ((King)board.whiteKing).inCheck;
        }
        else
        {
            // Black king is now in check? - Illegal move.

            result = ((King)board.blackKing).inCheck;
        }

        // Reverse Attack

        // Piece - Square
        // Remove this piece from the new square square
        // Add back old piece

        board.squares[destinationPosition].hasPiece = true;
        board.squares[destinationPosition].piece = attackedPiece;

        // Piece - Square
        // Put this piece back to previous square

        board.squares[originalPosition].hasPiece = true;
        board.squares[originalPosition].piece = this;

        positionOnBoard = originalPosition;

        // Enable attacked piece

        attackedPiece.gameObject.SetActive(true);

        board.UpdateAllPieceMoves();

        ((King)board.whiteKing).KingUniqueMoves();
        ((King)board.blackKing).KingUniqueMoves();

        return !result;
    }
}
