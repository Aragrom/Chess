using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct EvaluationState
{
    public bool isEvaluating;
    public int numberOfMovesSearched;
    public int depthOfSearch;

    public float duration;
}


public struct EvaluationResult
{
    public string pieceName;
    public string squareName;

    public int piece;       // the piece to move
    public int value;       // the value of the move
    public int square;      // the square to move to
}


public class Evaluator
{
    public EvaluationResult evaluationResult = new EvaluationResult();
    public EvaluationState evaluationState = new EvaluationState();
    public BoardLogic board = null;
    public BoardData boardData = new BoardData();

    public enum PieceValue
    {
        Pawn = 1,
        Knight = 3,
        Bishop = 3,
        Rook = 5,
        Queen = 9,
        King = 10000
    }

    public enum MoveValues 
    {
        Forking = 100
    }

    public static int currentDepth = 0;
    public static int depth = 1;

    // Control the evaluators bias toward certain moves.
    // Set between 0 - 1.
    // 0 = off
    // 1 = on

    public static float attackBias = 1.0f;
    public static float defendingBias = 1.0f;
    public static float stationaryBias = 1.0f;

    public static float controllingCenterBias = 1.0f;
    public static float controllingCastleBias = 1.0f;
    public static float controllingCastleQueenSideBias = 1.0f;
    public static float controllingKingBias = 1.0f;
    public static float supportingKingBias = 1.0f;
    public static float controllingPromotionBias = 1.0f;
    public static float forkingBias = 1.0f;

    public static float defensiveStructureBias = 1.0f;
    public static float pawnWallForKingSupportBias = 1.0f;
    public static float blockingPawnBias = 1.0f;
    public static float weakTakingStrongBias = 1.0f;   // Encourage weak taking strong (Pawn > Bishop)
    public static float strongTakingWeakBias = 1.0f;
    public static float castlingBias = 1.0f;           // Castling our king
    public static float promotingPawns = 1.0f;         // Moving pawns for promotion
    public static float pawnPromotionChoice = 1.0f;    // Choice once a pawn is promoted. (What piece to choose?)

    public static float favouritePiecePawn = 0.0f;
    public static float favouritePieceRook = 0.0f;
    public static float favouritePieceKnight = 0.0f;
    public static float favouritePieceBishop = 0.0f;
    public static float favouritePieceQueen = 0.0f;
    public static float favouritePieceKing = 0.0f;

    // Constructor
    public Evaluator()
    { 
        
    }

    // Entry point of execution
    public BoardData Begin(Board board)
    {
        this.board = new BoardLogic(board);

        boardData = new BoardData(board.pieces, board.squares);

        this.evaluationState.isEvaluating = true;
        this.evaluationState.numberOfMovesSearched = 0;
        this.evaluationState.depthOfSearch = 0;

        Thread thread = new Thread(() => Evaluate());
        thread.Start();

        return boardData;
    }

    public BoardData Evaluate()
    {
        // Temporary containers.

        List<MoveData> whiteMoves = new List<MoveData>();
        List<MoveData> blackMoves = new List<MoveData>();

        for (int i = 0; i < board.pieces.Length; i++)
        {
            boardData.pieceData[i].stationaryValue = CreateStationaryEvaluation(board.pieces[i]);

            if (board.pieces[i].isWhite)
            {
                CalculatePieceValues(whiteMoves, i);
            }
            else
            {
                CalculatePieceValues(blackMoves, i);
            }
        }

        boardData.whiteMoves = whiteMoves.ToArray();
        boardData.blackMoves = blackMoves.ToArray();

        evaluationState.isEvaluating = false;

        return boardData;
    }

    public void CalculatePieceValues(List<MoveData> moveData, int index)
    {
        // Piece value should be adjusted by these values
        // All attack values.
        // All defending.
        // All move values.

        // What is the piece type being attacked?

        for (int i = 0; i < board.pieces[index].attacks.Length; i++)
        {
            moveData.Add(CreateAttackEvaluation(board.pieces[index], 
                board.pieces[index].attacks[i]));
        }

        for (int i = 0; i < board.pieces[index].moves.Length; i++)
        {
            moveData.Add(CreateMoveEvaluation(board.pieces[index], 
                board.pieces[index].moves[i]));
        }
    }

    public MoveData CreateStationaryEvaluation(PieceLogic piece)
    {
        // Being stationary must have a value.
        // To decide if its worth developing the piece or not.

        MoveData moveData = new MoveData(piece.id, piece.positionOnBoard);

        // Are we vulnerable? Can any piece attack us? ==========

        if (board.IsSquareBeingAttacked(piece.isWhite, piece.positionOnBoard))
        {
            moveData.isVulnerable = true;
        }

        // Defence

        List<int> supportingPieces = new List<int>();
        List<int> moves = new List<int> { piece.positionOnBoard };

        board.FindAllPiecesWithMovesWithExceptions(piece.isWhite, moves, supportingPieces, new int[0]);
        int[] defenders = supportingPieces.ToArray();

        int numberOfDefenders = defenders.Length;
        int numberOfMoves = piece.moves.Length;
        int numberOfAttacks = piece.attacks.Length;
        int numberOfDefence = piece.defending.Length;

        // =====================================================

        moveData = EvaluateAttacks(piece, moveData);
        moveData = EvaluateMoves(piece, moveData);
        moveData = EvaluateDefending(piece, moveData);

        // =====================================================


        // Are we forking?

        if (moveData.attacking != null
            && moveData.attacking.Length > 1
            && moveData.isVulnerable == false) 
        {
            moveData.isForking = false;
            moveData.value += (int)MoveValues.Forking * forkingBias;
        }

        // ======================================================

        // Using this give the current position a single total value.

        moveData.value += numberOfAttacks + numberOfMoves + numberOfDefenders + numberOfDefence;

        //Debug.Log("Evaluator.CreateStationaryEvaluation() - moveData.value = " + moveData.value.ToString());

        board.pieces[piece.id].value = moveData.value;

        return moveData;
    }

    public MoveData EvaluateMoves(PieceLogic piece, MoveData moveData)
    {
        if (piece.moves != null
            && piece.moves.Length != 0)
        {
            moveData.moves = new int[piece.moves.Length][];

            for (int i = 0; i < piece.moves.Length; i++)
            {
                moveData.moves[i] = new int[2];
                moveData.moves[i][0] = piece.moves[i];

                // Are we controlling a square that will stop castling?

                moveData.controllingCastling = false;

                // Are we controlling a square the enemy king could move to?

                moveData.controllingKing = false;

                // Are we controlling a square that supports our king?
                // (already done check against the pieces that can support the king in king.cs)

                moveData.supportingKing = false;

                // Are we controlling a square that would stop pawn promotion?
                // (Check does an enemy pawn share this move that would put them onto row 8)

                moveData.controllingPromotion = false;

                // Are we controlling a square in the center of the board?
                // (Simple function to return a value)

                moveData.controllingCenter = false;

                // Are we controlling the only square a piece can move to? (PINNING)

                moveData.isPinning = false;

                moveData.moves[i][1] = 0;

                moveData.value += stationaryBias;
            }
        }

        return moveData;
    }

    public MoveData EvaluateAttacks(PieceLogic piece, MoveData moveData)
    {
        // Are we attacking a piece(s)? ========================

        if (piece.attacks != null
            && piece.attacks.Length != 0)
        {
            moveData.attacking = new int[piece.attacks.Length][];

            for (int i = 0; i < piece.attacks.Length; i++)
            {
                moveData.attacking[i] = new int[2];
                moveData.attacking[i][0] = piece.attacks[i];
                moveData.attacking[i][1] = (int)(GetPieceMaterialValue(board.squares[piece.attacks[i]].piece.type) * attackBias);

                moveData.value += (GetPieceMaterialValue(board.squares[piece.attacks[i]].piece.type)
                    * attackBias);

                if (board.squares[piece.attacks[i]].piece.type == PieceLogic.Type.King)
                {
                    // Is check
                    moveData.isCheck = true;

                    // Check for checkmate
                    if (piece.isWhite)
                    {
                        moveData.isCheckmate = board.blackKing.inCheckMate;
                        if (moveData.isCheckmate) moveData.value += 50000;
                    }
                    else
                    {
                        moveData.isCheckmate = board.whiteKing.inCheckMate;
                        if (moveData.isCheckmate) moveData.value += 50000;
                    }
                }
            }
        }

        return moveData;
    }

    public MoveData EvaluateDefending(PieceLogic piece, MoveData moveData)
    {
        if(piece.defending != null
            && piece.defending.Length == 0)
        {
            moveData.defending = new int[piece.defending.Length][];

            for (int i = 0; i < piece.defending.Length; i++)
            {
                moveData.defending[i] = new int[2];
                moveData.defending[i][0] = piece.defending[i];
                moveData.defending[i][1] = (int)(GetPieceMaterialValue(board.squares[piece.defending[i]].piece.type)
                    * defendingBias);

                // Stop from scoring defending the king as high number

                if (board.squares[piece.defending[i]].piece.type != PieceLogic.Type.King)
                {
                    moveData.value += (GetPieceMaterialValue(board.squares[piece.defending[i]].piece.type)
                                * defendingBias);
                }
                else
                {
                    // There is no reason to defend the square the king is on. 
                    // As it will never be taken. (we are checking piece we are defending!)

                    moveData.value += 0;
                }
            }
        }

        return moveData;
    }


    public MoveData CreateMoveEvaluation(PieceLogic piece, int index)
    {
        MoveData moveData = new MoveData();

        int originalPosition = piece.positionOnBoard;
        int destinationPosition = index;

        // Piece
        // Remove this piece from the old square/current square.

        board.squares[originalPosition].hasPiece = false;
        board.squares[originalPosition].piece = null;

        // Square
        // Move this piece to this square

        board.squares[destinationPosition].hasPiece = true;
        board.squares[destinationPosition].piece = piece;

        piece.positionOnBoard = destinationPosition;

        board.UpdateAllPieceMoves();

        ((KingLogic)board.whiteKing).KingUniqueMoves();
        ((KingLogic)board.blackKing).KingUniqueMoves();

        board.EvaluateLegalityOfAllMoves();

        // ===========================================================

        // Get an evaluation of the new positon after the move is peformed.

        moveData = CreateStationaryEvaluation(piece);

        // ===========================================================

        // Reverse Move

        // Square
        // Remove this piece from the new square/current square.

        board.squares[destinationPosition].hasPiece = false;
        board.squares[destinationPosition].piece = null;

        // Piece
        // Move this piece to the original square

        board.squares[originalPosition].hasPiece = true;
        board.squares[originalPosition].piece = piece;

        piece.positionOnBoard = originalPosition;

        board.UpdateAllPieceMoves();

        ((KingLogic)board.whiteKing).KingUniqueMoves();
        ((KingLogic)board.blackKing).KingUniqueMoves();

        board.EvaluateLegalityOfAllMoves();

        // Return the reverse of result - logic for IS MOVE LEGAL

        return moveData;
    }

    public MoveData CreateAttackEvaluation(PieceLogic piece, int index)
    {
        MoveData moveData = new MoveData();

        int originalPosition = piece.positionOnBoard;
        int destinationPosition = index;

        PieceLogic attackedPiece = board.squares[index].piece;

        // disable attacked piece

        attackedPiece.isActive = false;

        // Piece
        // Remove this piece from previous square

        board.squares[originalPosition].hasPiece = false;
        board.squares[originalPosition].piece = null;

        // Square
        // Move this piece to this square

        board.squares[destinationPosition].hasPiece = true;
        board.squares[destinationPosition].piece = piece;

        piece.positionOnBoard = destinationPosition;

        board.UpdateAllPieceMoves();

        ((KingLogic)board.whiteKing).KingUniqueMoves();
        ((KingLogic)board.blackKing).KingUniqueMoves();

        board.EvaluateLegalityOfAllMoves();

        // Develop here strength of new position. ==========================

        // Get an evaluation of the new positon after the attack is peformed.

        moveData = CreateStationaryEvaluation(piece);

        // Add the value of the piece we are attacking to new stationary value.

        moveData.value += (int)(GetPieceMaterialValue(board.squares[index].piece.type) * attackBias);

        // Is this piece a pawn? if so is it worth stacking pawns?
        // (or is there no pawn in this new file the pawn would attack to?)

        // Result should be adjusted by what type this piece is?
        // (Should taking a piece with a weaker piece be a stronger move?)

        // ====================================================================

        // Reverse Attack

        // Piece - Square
        // Remove this piece from the new square square
        // Add back old piece

        board.squares[destinationPosition].hasPiece = true;
        board.squares[destinationPosition].piece = attackedPiece;

        // Piece - Square
        // Put this piece back to previous square

        board.squares[originalPosition].hasPiece = true;
        board.squares[originalPosition].piece = piece;

        piece.positionOnBoard = originalPosition;

        // Enable attacked piece

        attackedPiece.isActive = true;

        board.UpdateAllPieceMoves();

        ((KingLogic)board.whiteKing).KingUniqueMoves();
        ((KingLogic)board.blackKing).KingUniqueMoves();

        board.EvaluateLegalityOfAllMoves();

        return moveData;
    }
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // NEED TO ADD CHECK FOR EN PASSANT ON PAWNS!!!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    public static int GetPieceMaterialValue(PieceLogic.Type type)
    {
        switch (type)
        {
            case PieceLogic.Type.Pawn:

                return (int)PieceValue.Pawn;

            case PieceLogic.Type.Rook:

                return (int)PieceValue.Rook;

            case PieceLogic.Type.Bishop:

                return (int)PieceValue.Bishop;

            case PieceLogic.Type.Knight:

                return (int)PieceValue.Knight;

            case PieceLogic.Type.Queen:

                return (int)PieceValue.Queen;

            case PieceLogic.Type.King:

                return (int)PieceValue.King;
        }

        return 0;
    }
}
