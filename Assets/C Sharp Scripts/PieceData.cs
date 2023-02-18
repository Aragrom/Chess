using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PieceData
{
    public int id;
    public int square;
    public bool isActive;
    public bool hasMoved;

    public Piece.Type type;

    public bool isPromoted;
    public Piece.Type promotionType;

    public List<int> legalMoves;
    public List<int> legalAttacks;
    public List<int> legalEnPassantAttacks;

    // Value of the piece staying stationary;
    public MoveData stationaryValue;

    public PieceData(int id, int square, bool isActive, bool hasMoved,
        Piece.Type type, bool isPromoted, Piece.Type promotionType)
    {
        this.id = id;
        this.square = square;
        this.isActive = isActive;
        this.hasMoved = hasMoved;
        this.type = type;

        this.isPromoted = isPromoted;
        this.promotionType = promotionType;

        this.legalMoves = new List<int>();
        this.legalAttacks = new List<int>();
        this.legalEnPassantAttacks = new List<int>();

        this.stationaryValue = new MoveData();
    }
}
