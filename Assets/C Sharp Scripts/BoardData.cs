using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BoardData
{
    public PieceData[] pieceData;
    public SquareData[] squareData;

    public MoveData[] whiteMoves;
    public MoveData[] blackMoves;

    public BoardData(Piece[] pieces, Square[] squares)
    {
        this.pieceData = new PieceData[pieces.Length];
        this.squareData = new SquareData[squares.Length];

        this.whiteMoves = null;
        this.blackMoves = null;

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].type == Piece.Type.Pawn)
            {
                this.pieceData[i] = new PieceData(pieces[i].id, pieces[i].positionOnBoard, 
                    pieces[i].gameObject.activeSelf, pieces[i].hasMoved,
                        pieces[i].type, (((Pawn)pieces[i]).isPromoted), (((Pawn)pieces[i]).promotionType));
            }
            else
            {
                this.pieceData[i] = new PieceData(pieces[i].id, pieces[i].positionOnBoard,
                    pieces[i].gameObject.activeSelf, pieces[i].hasMoved,
                            pieces[i].type, false, pieces[i].type);
            }
        }

        for (int i = 0; i < squares.Length; i++)
        {
            SquareData squareData = new SquareData();

            squareData.id = squares[i].index;

            squareData.hasPiece = squares[i].hasPiece;
            if (squares[i].hasPiece) squareData.piece = squares[i].piece.id;
            else squareData.piece = -1;

            squareData.hasEnPassant = squares[i].hasEnPassant;
            if (squares[i].hasEnPassant) squareData.enPassant = squares[i].enPassant.id;
            else squareData.enPassant = -1;

            this.squareData[i] = squareData;
        }
    }

    public BoardData(PieceLogic[] pieces, SquareLogic[] squares)
    {
        this.pieceData = new PieceData[pieces.Length];
        this.squareData = new SquareData[squares.Length];

        this.whiteMoves = null;
        this.blackMoves = null;

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].type == PieceLogic.Type.Pawn)
            {
                this.pieceData[i] = new PieceData(pieces[i].id, pieces[i].positionOnBoard,
                    pieces[i].isActive, pieces[i].hasMoved,
                        (Piece.Type)pieces[i].type, (((PawnLogic)pieces[i]).isPromoted), (Piece.Type)(((PawnLogic)pieces[i]).promotionType));
            }
            else
            {
                this.pieceData[i] = new PieceData(pieces[i].id, pieces[i].positionOnBoard,
                    pieces[i].isActive, pieces[i].hasMoved,
                            (Piece.Type)pieces[i].type, false, (Piece.Type)pieces[i].type);
            }
        }

        for (int i = 0; i < squares.Length; i++)
        {
            SquareData squareData = new SquareData();

            squareData.id = squares[i].index;

            squareData.hasPiece = squares[i].hasPiece;
            if (squares[i].hasPiece) squareData.piece = squares[i].piece.id;
            else squareData.piece = -1;

            squareData.hasEnPassant = squares[i].hasEnPassant;
            if (squares[i].hasEnPassant) squareData.enPassant = squares[i].enPassant.id;
            else squareData.enPassant = -1;

            this.squareData[i] = squareData;
        }
    }
}
