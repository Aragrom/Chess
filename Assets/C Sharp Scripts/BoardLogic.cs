using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardLogic
{
    public SquareLogic[] squares;
    public PieceLogic[] pieces;

    public KingLogic whiteKing;
    public KingLogic blackKing;

    public int boardLength;
    public int boardHeight;

    public BoardLogic(Board board)
    {
        this.boardLength = board.boardLength;
        this.boardHeight = board.boardHeight;

        // Create each square

        squares = new SquareLogic[board.squares.Length];

        for (int i = 0; i < board.squares.Length; i++)
        {
            squares[i] = new SquareLogic();

            squares[i].name = board.squares[i].gameObject.name;
            squares[i].index = board.squares[i].index;
            squares[i].isWhite = board.squares[i].isWhite;
            squares[i].row = board.squares[i].row;
            squares[i].column = board.squares[i].column;
            squares[i].hasPiece = board.squares[i].hasPiece;

            //squares[i].piece = pieces[i].piece;

            squares[i].hasEnPassant = board.squares[i].hasEnPassant;

            //squares[i].enPassant = board.squares[i].enPassant;
        }

        // Create each piece - Directly copying from (Rook.cs > RookLogic.cs etc)

        pieces = new PieceLogic[board.pieces.Length];

        for (int i = 0; i < board.pieces.Length; i++)
        {
            pieces[i] = new PieceLogic();            

            switch (board.pieces[i].type)
            {
                case Piece.Type.Pawn:

                    pieces[i] = new PawnLogic();

                    pieces[i].type = PieceLogic.Type.Pawn;
                    ((PawnLogic)pieces[i]).SetBoard(this);

                    ((PawnLogic)pieces[i]).isPromoted = ((Pawn)board.pieces[i]).isPromoted;
                    ((PawnLogic)pieces[i]).hasBeenPromoted = ((Pawn)board.pieces[i]).hasBeenPromoted;

                    ((PawnLogic)pieces[i]).promotionType = (PawnLogic.Type)((Pawn)board.pieces[i]).promotionType;
                    ((PawnLogic)pieces[i]).enPassantAttacks = ((Pawn)board.pieces[i]).enPassantAttacks;
                    ((PawnLogic)pieces[i]).guarding = ((Pawn)board.pieces[i]).guarding;
                    ((PawnLogic)pieces[i]).developed = ((Pawn)board.pieces[i]).developed;
                    ((PawnLogic)pieces[i]).possibleEnPassantIndex = ((Pawn)board.pieces[i]).possibleEnPassantIndex;
                    ((PawnLogic)pieces[i]).wouldBeEnPassantVulnerableHereIndex = ((Pawn)board.pieces[i]).wouldBeEnPassantVulnerableHereIndex;
                    ((PawnLogic)pieces[i]).enPassantVulnerable = ((Pawn)board.pieces[i]).enPassantVulnerable;

                    if (((PawnLogic)pieces[i]).enPassantVulnerable == true)
                    {
                        squares[((PawnLogic)pieces[i]).wouldBeEnPassantVulnerableHereIndex].enPassant = ((PawnLogic)pieces[i]);
                    }

                    ((PawnLogic)pieces[i]).discoverablesAttacks = ((Pawn)board.pieces[i]).discoverablesAttacks;
                    ((PawnLogic)pieces[i]).discoverableMoves = ((Pawn)board.pieces[i]).discoverableMoves;
                    ((PawnLogic)pieces[i]).discoverableDefending = ((Pawn)board.pieces[i]).discoverableDefending;
                    break;

                case Piece.Type.Rook:

                    pieces[i] = new RookLogic();

                    pieces[i].type = PieceLogic.Type.Rook;
                    ((RookLogic)pieces[i]).SetBoard(this);

                    ((RookLogic)pieces[i]).discoverablesAttacks = ((Rook)board.pieces[i]).discoverablesAttacks;
                    ((RookLogic)pieces[i]).discoverableMoves = ((Rook)board.pieces[i]).discoverableMoves;
                    ((RookLogic)pieces[i]).discoverableDefending = ((Rook)board.pieces[i]).discoverableDefending;
                    
                    break;

                case Piece.Type.Bishop:

                    pieces[i] = new BishopLogic();

                    pieces[i].type = PieceLogic.Type.Bishop;
                    ((BishopLogic)pieces[i]).SetBoard(this);

                    ((BishopLogic)pieces[i]).discoverablesAttacks = ((Bishop)board.pieces[i]).discoverablesAttacks;
                    ((BishopLogic)pieces[i]).discoverableMoves = ((Bishop)board.pieces[i]).discoverableMoves;
                    ((BishopLogic)pieces[i]).discoverableDefending = ((Bishop)board.pieces[i]).discoverableDefending;

                    break;

                case Piece.Type.Knight:

                    pieces[i] = new KnightLogic();

                    pieces[i].type = PieceLogic.Type.Knight;
                    ((KnightLogic)pieces[i]).SetBoard(this);

                    ((KnightLogic)pieces[i]).discoverablesAttacks = ((Knight)board.pieces[i]).discoverablesAttacks;
                    ((KnightLogic)pieces[i]).discoverableMoves = ((Knight)board.pieces[i]).discoverableMoves;
                    ((KnightLogic)pieces[i]).discoverableDefending = ((Knight)board.pieces[i]).discoverableDefending;

                    break;

                case Piece.Type.Queen:

                    pieces[i] = new QueenLogic();

                    pieces[i].type = PieceLogic.Type.Queen;
                    ((QueenLogic)pieces[i]).SetBoard(this);

                    ((QueenLogic)pieces[i]).discoverablesAttacks = ((Queen)board.pieces[i]).discoverablesAttacks;
                    ((QueenLogic)pieces[i]).discoverableMoves = ((Queen)board.pieces[i]).discoverableMoves;
                    ((QueenLogic)pieces[i]).discoverableDefending = ((Queen)board.pieces[i]).discoverableDefending;

                    break;

                case Piece.Type.King:

                    pieces[i] = new KingLogic();

                    pieces[i].type = PieceLogic.Type.King;
                    ((KingLogic)pieces[i]).SetBoard(this);

                    ((KingLogic)pieces[i]).inCheck = ((King)board.pieces[i]).inCheck;
                    ((KingLogic)pieces[i]).inCheckMate = ((King)board.pieces[i]).inCheckMate;
                    ((KingLogic)pieces[i]).inStaleMate = ((King)board.pieces[i]).inStaleMate;

                    ((KingLogic)pieces[i]).canCastle = ((King)board.pieces[i]).canCastle;
                    ((KingLogic)pieces[i]).canCastleQueenSide = ((King)board.pieces[i]).canCastleQueenSide;

                    ((KingLogic)pieces[i]).castleMoves = ((King)board.pieces[i]).castleMoves;
                    ((KingLogic)pieces[i]).castleQueenSideMoves = ((King)board.pieces[i]).castleQueenSideMoves;
                    
                    ((KingLogic)pieces[i]).castleIndex = ((King)board.pieces[i]).castleIndex;
                    ((KingLogic)pieces[i]).castleQueenSideIndex = ((King)board.pieces[i]).castleQueenSideIndex;

                    ((KingLogic)pieces[i]).kingSideRookStartingPosition = ((King)board.pieces[i]).kingSideRookStartingPosition;
                    ((KingLogic)pieces[i]).queenSideRookStartingPosition = ((King)board.pieces[i]).queenSideRookStartingPosition;

                    ((KingLogic)pieces[i]).north = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).north.state,
                        attacker = ((King)board.pieces[i]).north.attacker,
                        attackerDepth = ((King)board.pieces[i]).north.attackerDepth,
                        defender = ((King)board.pieces[i]).north.defender,
                        defenderDepth = ((King)board.pieces[i]).north.defenderDepth,
                        blocker = ((King)board.pieces[i]).north.blocker,
                        blockerDepth = ((King)board.pieces[i]).north.blockerDepth,
                        squares = ((King)board.pieces[i]).north.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).north.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).south = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).south.state,
                        attacker = ((King)board.pieces[i]).south.attacker,
                        attackerDepth = ((King)board.pieces[i]).south.attackerDepth,
                        defender = ((King)board.pieces[i]).south.defender,
                        defenderDepth = ((King)board.pieces[i]).south.defenderDepth,
                        blocker = ((King)board.pieces[i]).south.blocker,
                        blockerDepth = ((King)board.pieces[i]).south.blockerDepth,
                        squares = ((King)board.pieces[i]).south.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).south.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).east = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).east.state,
                        attacker = ((King)board.pieces[i]).east.attacker,
                        attackerDepth = ((King)board.pieces[i]).east.attackerDepth,
                        defender = ((King)board.pieces[i]).east.defender,
                        defenderDepth = ((King)board.pieces[i]).east.defenderDepth,
                        blocker = ((King)board.pieces[i]).east.blocker,
                        blockerDepth = ((King)board.pieces[i]).east.blockerDepth,
                        squares = ((King)board.pieces[i]).east.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).east.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).west = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).west.state,
                        attacker = ((King)board.pieces[i]).west.attacker,
                        attackerDepth = ((King)board.pieces[i]).west.attackerDepth,
                        defender = ((King)board.pieces[i]).west.defender,
                        defenderDepth = ((King)board.pieces[i]).west.defenderDepth,
                        blocker = ((King)board.pieces[i]).west.blocker,
                        blockerDepth = ((King)board.pieces[i]).west.blockerDepth,
                        squares = ((King)board.pieces[i]).west.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).west.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).northEast = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).northEast.state,
                        attacker = ((King)board.pieces[i]).northEast.attacker,
                        attackerDepth = ((King)board.pieces[i]).northEast.attackerDepth,
                        defender = ((King)board.pieces[i]).northEast.defender,
                        defenderDepth = ((King)board.pieces[i]).northEast.defenderDepth,
                        blocker = ((King)board.pieces[i]).northEast.blocker,
                        blockerDepth = ((King)board.pieces[i]).northEast.blockerDepth,
                        squares = ((King)board.pieces[i]).northEast.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).northEast.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).northWest = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).northWest.state,
                        attacker = ((King)board.pieces[i]).northWest.attacker,
                        attackerDepth = ((King)board.pieces[i]).northWest.attackerDepth,
                        defender = ((King)board.pieces[i]).northWest.defender,
                        defenderDepth = ((King)board.pieces[i]).northWest.defenderDepth,
                        blocker = ((King)board.pieces[i]).northWest.blocker,
                        blockerDepth = ((King)board.pieces[i]).northWest.blockerDepth,
                        squares = ((King)board.pieces[i]).northWest.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).northWest.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).southEast = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).southEast.state,
                        attacker = ((King)board.pieces[i]).southEast.attacker,
                        attackerDepth = ((King)board.pieces[i]).southEast.attackerDepth,
                        defender = ((King)board.pieces[i]).southEast.defender,
                        defenderDepth = ((King)board.pieces[i]).southEast.defenderDepth,
                        blocker = ((King)board.pieces[i]).southEast.blocker,
                        blockerDepth = ((King)board.pieces[i]).southEast.blockerDepth,
                        squares = ((King)board.pieces[i]).southEast.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).southEast.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).southWest = new LaneLogic
                    {
                        state = (LaneLogic.State)((King)board.pieces[i]).southWest.state,
                        attacker = ((King)board.pieces[i]).southWest.attacker,
                        attackerDepth = ((King)board.pieces[i]).southWest.attackerDepth,
                        defender = ((King)board.pieces[i]).southWest.defender,
                        defenderDepth = ((King)board.pieces[i]).southWest.defenderDepth,
                        blocker = ((King)board.pieces[i]).southWest.blocker,
                        blockerDepth = ((King)board.pieces[i]).southWest.blockerDepth,
                        squares = ((King)board.pieces[i]).southWest.squares,
                        possibleSupportingPieces = ((King)board.pieces[i]).southWest.possibleSupportingPieces
                    };

                    ((KingLogic)pieces[i]).piecesAttackingThisKing = ((King)board.pieces[i]).piecesAttackingThisKing;
                    ((KingLogic)pieces[i]).possibleSupportIndexes = ((King)board.pieces[i]).possibleSupportIndexes;
                    
                    ((KingLogic)pieces[i]).hasNoMoves = ((King)board.pieces[i]).hasNoMoves;
                    ((KingLogic)pieces[i]).hasNoAttacks = ((King)board.pieces[i]).hasNoAttacks;
                    ((KingLogic)pieces[i]).hasNoSupport = ((King)board.pieces[i]).hasNoSupport;

                    ((KingLogic)pieces[i]).init = true;

                    if (board.pieces[i].isWhite == true)
                    {
                        whiteKing = (KingLogic)pieces[i];
                    }
                    else
                    {
                        blackKing = (KingLogic)pieces[i];
                    }

                    break;
            }

            pieces[i].SetBoard(this);

            pieces[i].isActive = board.pieces[i].gameObject.activeSelf;
            pieces[i].name = board.pieces[i].gameObject.name;
            pieces[i].id = board.pieces[i].id;
            pieces[i].positionOnBoard = board.pieces[i].positionOnBoard;

            // Assign the piece to the square(s) its associated with

            if (pieces[i].isActive == true)
            {
                // Add piece to square

                squares[pieces[i].positionOnBoard].piece = pieces[i];
                squares[pieces[i].positionOnBoard].hasPiece = true;

                // Add handle En Passant Piece to square.

                if (pieces[i].type == PieceLogic.Type.Pawn
                    && ((PawnLogic)pieces[i]).enPassantVulnerable)
                {
                    squares[((PawnLogic)pieces[i]).wouldBeEnPassantVulnerableHereIndex].enPassant = pieces[i];
                    squares[((PawnLogic)pieces[i]).wouldBeEnPassantVulnerableHereIndex].hasEnPassant = true;
                }
            }

            pieces[i].isWhite = board.pieces[i].isWhite;
            pieces[i].hasMoved = board.pieces[i].hasMoved;
            pieces[i].value = board.pieces[i].value;

            pieces[i].defending = board.pieces[i].defending;
            pieces[i].moves = board.pieces[i].moves;
            pieces[i].attacks = board.pieces[i].attacks;
        }
    }

    public int GetColumn(int rows, int columns, int index)
    {
        float i = index;

        // handle if index = 0;

        if (index == 0)
        {
            return index + 1;
        }

        // index / column THEN (round the number to whole number) = row its on

        i /= columns;

        return (int)Mathf.Floor(i) + 1;
    }

    public int GetRow(int rows, int columns, int index)
    {
        // Keep minusing a column amount storing the old and new number.
        // once new number has reach zero use the old number as the row/rank square is on.

        int oldNumber = index;

        int result = index;

        while (result >= 0)
        {
            oldNumber = result;
            result = result - columns;
        }

        // Offset the value by 1 to represent the name not index
        // 0 - 1 - 2 - 3 - 4 - 5 - 6 - 7 actual
        // 1 - 2 - 3 - 4 - 5 - 6 - 7 - 8 needed

        return oldNumber + 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetSquarePosition(int row, int column)
    {
        // Make sure rows and columns start at 0 not 1.

        row = row - 1;
        column = column - 1;

        Vector3 result = new Vector3(-4.375f, 0.1f, -4.375f);

        if (row == 0 && column == 0)
        {
            return result;
        }

        Vector3 step = new Vector3(1.25f * column, 0, 1.25f * row);

        return result + step;
    }

    public Vector3 GetSquarePosition(int index)
    {
        int row = GetRow(boardHeight, boardLength, index);
        int column = GetColumn(boardHeight, boardLength, index);

        return GetSquarePosition(row, column);
    }

    public bool IsMoveInScope(int index)
    {
        if (index >= 0 && index < squares.Length) return true;
        else return false;
    }

    public bool DoesSquareHavePiece(int index)
    {
        return squares[index].hasPiece;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>

    public bool IsWhiteControlled(int index)
    {
        return squares[index].piece.isWhite;
    }


    public bool DoesSquareHaveEnPassantPawn(int index)
    {
        return squares[index].hasEnPassant;
    }

    public bool IsEnPassantWhite(int index)
    {
        return squares[index].enPassant.isWhite;
    }

    public void UpdateAllPieceMoves()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].GetMoves();
        }
    }

    public bool DoesSideHaveMove(bool isWhite)
    {
        // Search through all pieces.

        // is the piece the colour we need?

        // Does the piece have a move?

        // Does the piece have an attack?

        // Depending on the type of piece - Do we have any special moves?

        return false;
    }

    public int[] GetPiecesAttacking(bool isWhite, int index)
    {
        List<int> piecesAttacking = new List<int>();

        for (int i = 0; i < pieces.Length; i++)
        {
            // What colour pieces are we interested in?

            if (pieces[i].isActive == true)
            {
                if (pieces[i].isWhite != isWhite)
                {
                    // is the colour we are interested in.
                    // Search through all the attacks for this piece for the attacked piece

                    for (int j = 0; j < pieces[i].attacks.Length; j++)
                    {
                        // we are now searching through each attack
                        // does the attack of this piece match the index of the attacked piece passed

                        if (pieces[i].attacks[j] == index)
                        {
                            // This piece is attacking!
                            piecesAttacking.Add(pieces[i].positionOnBoard);

                            break;
                        }
                    }
                }
            }
        }

        return piecesAttacking.ToArray();
    }

    public bool IsPieceBeingDefended(bool isWhite, int index)
    {
        // Search through all the pieces.

        for (int i = 0; i < pieces.Length; i++)
        {
            // Is the piece NOT a pawn? A pawn can move to squares that aren't really "attacks"

            if (pieces[i].isActive == true)
            {
                if (pieces[i].type != PieceLogic.Type.Pawn)
                {
                    // Is this piece the colour we need?

                    if (pieces[i].isWhite != isWhite)
                    {
                        // Search through all the pieces attacks.

                        for (int k = 0; k < pieces[i].defending.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (pieces[i].defending[k] == index)
                            {
                                // It does match!

                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (pieces[i].isWhite != isWhite)
                    {
                        for (int k = 0; k < ((PawnLogic)pieces[i]).guarding.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (((PawnLogic)pieces[i]).guarding[k] == index)
                            {
                                // It does match!

                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool IsSquareBeingAttacked(bool isWhite, int index)
    {
        // Search through all the pieces.

        for (int i = 0; i < pieces.Length; i++)
        {
            // Is the piece NOT a pawn? A pawn can move to squares that aren't really "attacks"

            if (pieces[i].isActive == true)
            {
                if (pieces[i].type != PieceLogic.Type.Pawn)
                {
                    // Is this piece the colour we need?

                    if (pieces[i].isWhite != isWhite)
                    {
                        // Search through all the pieces attacks.

                        for (int k = 0; k < pieces[i].moves.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (pieces[i].moves[k] == index)
                            {
                                // It does match!

                                return true;
                            }
                        }
                    }
                }
                else
                {
                    // When the piece is a pawn check there attacks.

                    // Is this piece the colour we need?

                    if (pieces[i].isWhite != isWhite)
                    {
                        // Search through all the pieces attacks.

                        for (int k = 0; k < ((PawnLogic)pieces[i]).attacks.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (((PawnLogic)pieces[i]).attacks[k] == index)
                            {
                                // It does match!

                                return true;
                            }
                        }

                        for (int k = 0; k < ((PawnLogic)pieces[i]).guarding.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (((PawnLogic)pieces[i]).guarding[k] == index)
                            {
                                // It does match!

                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isWhite"></param>
    /// <param name="moves"></param>
    /// <param name="exceptions"> Pieces to be excepted and not included </param>
    /// <returns></returns>
    public void FindAllPiecesWithMovesWithExceptions(bool isWhite, List<int> moves, List<int> supportingPieces, int[] exceptions)
    {
        // Used to store the results of the search
        // We do not store duplicate piece indexes.

        // Searh through all pieces.

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].isActive == true)
            {
                // Is the piece the colour we need?

                if (pieces[i].isWhite == isWhite)
                {
                    // Check that this piece is not one to be ignored

                    bool shouldIgnore = false;

                    for (int b = 0; b < exceptions.Length; b++)
                    {
                        // Does this piece match this exception piece index passed?

                        if (pieces[i].positionOnBoard == exceptions[b])
                        {
                            // Yes it does match we want to ignore this piece.

                            shouldIgnore = true;

                            break;
                        }
                    }

                    // Exceptions handled proceed

                    if (shouldIgnore == false)
                    {
                        // Search through this pieces moves. Then see if you can find this move index.

                        for (int j = 0; j < pieces[i].moves.Length; j++)
                        {
                            // Search this pieces move against the moves we passed into the function.

                            for (int p = 0; p < moves.Count; p++)
                            {
                                // Is this move one of the ones passed?

                                if (pieces[i].moves[j] == moves[p])
                                {
                                    // Yes it is! store it!
                                    // But do we already have it stored?

                                    bool isStoredAlready = false;

                                    // Is the move currently tracked? (Don't store duplicates)

                                    for (int k = 0; k < supportingPieces.Count; k++)
                                    {
                                        // Does this piece index match any already stored?
                                        if (supportingPieces[k] == pieces[i].positionOnBoard)
                                        {
                                            // Do not store it we already have it!
                                            isStoredAlready = true;
                                            break;
                                        }
                                    }

                                    if (isStoredAlready == false) supportingPieces.Add(pieces[i].positionOnBoard);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void RemoveOldEnPassantMove(bool forWhite)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].isWhite == forWhite
                && pieces[i].type == PieceLogic.Type.Pawn)
            {
                // Check for En Passant.

                if (((PawnLogic)pieces[i]).enPassantVulnerable)
                {
                    int enPassantTrailIndex = ((PawnLogic)pieces[i]).wouldBeEnPassantVulnerableHereIndex;

                    // Manage square

                    squares[enPassantTrailIndex].enPassant = null;
                    squares[enPassantTrailIndex].hasEnPassant = false;

                    // Manage piece

                    ((PawnLogic)pieces[i]).enPassantVulnerable = false;
                    ((PawnLogic)pieces[i]).possibleEnPassantIndex = -1;
                    ((PawnLogic)pieces[i]).wouldBeEnPassantVulnerableHereIndex = -1;
                }
            }
        }
    }

    public void EvaluateLegalityOfAllMoves()
    {
        // Create a back up

        BoardData legalBoard = new BoardData(pieces, squares);

        // Legal moves/attacks/unique moves for each piece 
        // will be temporarly stored here.
        // In check moves will not be kept

        // For all pieces

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].isActive == true)
            {
                // For all moves of this piece

                for (int k = 0; k < pieces[i].moves.Length; k++)
                {
                    if (pieces[i].IsMoveLegal(pieces[i].moves[k]))
                    {
                        legalBoard.pieceData[i].legalMoves.Add(pieces[i].moves[k]);
                    }
                }

                // For all attacks of this piece

                for (int k = 0; k < pieces[i].attacks.Length; k++)
                {
                    if (pieces[i].IsAttackLegal(pieces[i].attacks[k]))
                    {
                        legalBoard.pieceData[i].legalAttacks.Add(pieces[i].attacks[k]);
                    }
                }

                if (pieces[i].type == PieceLogic.Type.Pawn)
                {
                    for (int k = 0; k < ((PawnLogic)pieces[i]).enPassantAttacks.Length; k++)
                    {
                        if (((PawnLogic)pieces[i]).IsEnPassantAttackLegal(((PawnLogic)pieces[i]).enPassantAttacks[k]))
                        {
                            legalBoard.pieceData[i].legalEnPassantAttacks.Add(((PawnLogic)pieces[i]).enPassantAttacks[k]);
                        }
                    }
                }
            }
        }



        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].moves = legalBoard.pieceData[i].legalMoves.ToArray();
            pieces[i].attacks = legalBoard.pieceData[i].legalAttacks.ToArray();

            if (pieces[i].type == PieceLogic.Type.Pawn)
            {
                ((PawnLogic)pieces[i]).enPassantAttacks = legalBoard.pieceData[i].legalEnPassantAttacks.ToArray();
            }
        }
    }
}
