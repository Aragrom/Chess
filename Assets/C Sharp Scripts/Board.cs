using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <Notes>
/// Board = (GameObject) Plane. Scale 1,1,1.
/// 
/// Board width = 10
/// Board height = 10
/// 
/// bottom left corner (-5, 0, -5)
/// upper left corner (-5, 0, 5)
/// bottom right corner (5, 0, -5)
/// upper right corner (5, 0, 5)
/// 
/// Square = (GameObject) Plane. Scale 0.125, 1, 0.125.
/// 
/// Board = 8x8 = 64 squares
/// 
/// Board width / 8
/// 
/// Square width = 1.25
/// Square height = 1.25
/// 
/// A1 position:
/// 
/// x = -5 + (square width / 2) = -4.375
/// z = -5 + (square height / 2) = -4.375
/// 
/// Board Index:
/// 
/// a1 = 0
/// a2 = 1
/// a3 = 2
/// a4 = 3
/// ...
/// h5 = 60
/// h6 = 61
/// h7 = 62
/// h8 = 63
/// </Notes>

[System.Serializable]
public class Board
{
    public Square[] squares;
    public Piece[] pieces;

    public Piece whiteKing;
    public Piece blackKing;

    public int boardLength;
    public int boardHeight;

    public Board(int boardLength, int boardHeight)
    {
        this.boardLength = boardLength;
        this.boardHeight = boardHeight;
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

            if (pieces[i].gameObject.activeSelf == true)
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

            if (pieces[i].gameObject.activeSelf == true)
            {
                if (pieces[i].type != Piece.Type.Pawn)
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
                        for (int k = 0; k < ((Pawn)pieces[i]).guarding.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (((Pawn)pieces[i]).guarding[k] == index)
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

            if (pieces[i].gameObject.activeSelf == true)
            {
                if (pieces[i].type != Piece.Type.Pawn)
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

                        for (int k = 0; k < ((Pawn)pieces[i]).attacks.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (((Pawn)pieces[i]).attacks[k] == index)
                            {
                                // It does match!

                                return true;
                            }
                        }

                        for (int k = 0; k < ((Pawn)pieces[i]).guarding.Length; k++)
                        {
                            // Does any of the piece attack indexes match the passed index?

                            if (((Pawn)pieces[i]).guarding[k] == index)
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
            if (pieces[i].gameObject.activeSelf == true)
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
                && pieces[i].type == Piece.Type.Pawn)
            {
                // Check for En Passant.

                if (((Pawn)pieces[i]).enPassantVulnerable)
                {
                    Debug.Log("Board.RemoveOldEnPassantMove() - TRYING TO REMOVE EN PASSANT piece id = " + i.ToString());

                    int enPassantTrailIndex = ((Pawn)pieces[i]).wouldBeEnPassantVulnerableHereIndex;

                    Debug.Log("Board.RemoveOldEnPassantMove() - Square id = " + enPassantTrailIndex.ToString());

                    // Manage square

                    squares[enPassantTrailIndex].enPassant = null;
                    squares[enPassantTrailIndex].hasEnPassant = false;

                    // Manage piece

                    ((Pawn)pieces[i]).enPassantVulnerable = false;
                    ((Pawn)pieces[i]).possibleEnPassantIndex = -1;
                    ((Pawn)pieces[i]).wouldBeEnPassantVulnerableHereIndex = -1;
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
            if (pieces[i].gameObject.activeSelf)
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

                if (pieces[i].type == Piece.Type.Pawn)
                {
                    for (int k = 0; k < ((Pawn)pieces[i]).enPassantAttacks.Length; k++)
                    {
                        if (((Pawn)pieces[i]).IsEnPassantAttackLegal(((Pawn)pieces[i]).enPassantAttacks[k]))
                        {
                            legalBoard.pieceData[i].legalEnPassantAttacks.Add(((Pawn)pieces[i]).enPassantAttacks[k]);
                        }
                    }
                }
            }
        }



        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].moves = legalBoard.pieceData[i].legalMoves.ToArray();
            pieces[i].attacks = legalBoard.pieceData[i].legalAttacks.ToArray();

            if (pieces[i].type == Piece.Type.Pawn)
            {
                ((Pawn)pieces[i]).enPassantAttacks = legalBoard.pieceData[i].legalEnPassantAttacks.ToArray();
            }
        }
    }
}
