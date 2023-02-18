using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MoveData
{
    public int piece;
    public int square;
    public float value;

    // ("The squares index","The value of the move")
    public int[][] attacking;
    public int[][] defending;

    public int[][] attackers;
    public int[][] moves;
    public int[][] defender;    // Pieces that are defending this position

    public int[][] pinnedPieces;

    public bool isVulnerable;
    public bool isTrading;
    public bool isCheck;
    public bool isCheckmate;
    public bool isForking;
    public bool isPinning;
    public bool controllingCenter;
    public bool controllingCastling;
    public bool controllingCastlingQueenSide;
    public bool controllingKing;
    public bool supportingKing;
    public bool controllingPromotion;

    public MoveData(int piece, int square)
    {
        this.piece = piece;
        this.square = square;
        this.value = 0;
        this.attacking = null;
        this.defending = null;

        this.attackers = null;
        this.moves = null;
        this.defender = null;    // Pieces that are defending this piece/stationary move

        this.pinnedPieces = null;

        this.isVulnerable = false;
        this.isTrading = false;
        this.isCheck = false;
        this.isCheckmate = false;
        this.isForking = false;
        this.isPinning = false;
        this.controllingCenter = false;
        this.controllingCastling = false;
        this.controllingCastlingQueenSide = false;
        this.controllingKing = false;
        this.supportingKing = false;
        this.controllingPromotion = false;
    }
}
