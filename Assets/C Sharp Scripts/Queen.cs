using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    // These indexs can never be moved to. More for analytics of potential moves.
    // Only used to help develop which move is best.
    // "Discoverable/blocked attacks defence and moves"

    public int[] discoverablesAttacks = null;
    public int[] discoverableMoves = null;
    public int[] discoverableDefending = null;

    void Awake()
    {
        //board = GameObject.Find("Board").GetComponent<Board>();
    }

    void Start()
    {
        GetMoves();
    }

    public new void GetMoves()
    {
        List<int> tempMoves = new List<int>();
        List<int> tempAttacks = new List<int>();
        List<int> tempDefending = new List<int>();
        List<int> tempDiscoverableAttacks = new List<int>();
        List<int> tempDiscoverableMoves = new List<int>();
        List<int> tempDiscoverableDefending = new List<int>();

        // Both white and black will move the same way.

        AddVerticalMoves((int)Piece.MoveType.North,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        AddVerticalMoves((int)Piece.MoveType.South,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        AddHorizontalMoves((int)Piece.MoveType.East,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        AddHorizontalMoves((int)Piece.MoveType.West,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        AddDiagonalMoves((int)Piece.MoveType.NorthEast,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        AddDiagonalMoves((int)Piece.MoveType.NorthWest,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        AddDiagonalMoves((int)Piece.MoveType.SouthEast,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        AddDiagonalMoves((int)Piece.MoveType.SouthWest,
            tempMoves,
            tempAttacks,
            tempDefending,
            tempDiscoverableAttacks,
            tempDiscoverableMoves,
            tempDiscoverableDefending);

        // Convert list to arrays for later reference. Store in global variable.

        attacks = tempAttacks.ToArray();
        defending = tempDefending.ToArray();
        moves = tempMoves.ToArray();

        discoverablesAttacks = tempDiscoverableAttacks.ToArray();
        discoverableDefending = tempDiscoverableDefending.ToArray();
        discoverableMoves = tempDiscoverableMoves.ToArray();
    }

    public new bool HasMove(int index)
    {
        // No unique moves to check through
        // Not used.

        return false;
    }

    public new void Move(int index)
    {
        Debug.Log("Queen.Move() - Queen = " + this.transform.name + " Index = " + index.ToString());

        if (MoveTo(index) == false)
        {
            Attack(index);
        }
    }

    public bool Attack(int index)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i] == index)
            {
                // disable attacked piece

                board.squares[index].piece.gameObject.SetActive(false);

                // Remove this piece from previous square

                board.squares[positionOnBoard].hasPiece = false;
                board.squares[positionOnBoard].piece = null;

                // Move this piece to this square

                board.squares[index].hasPiece = true;
                board.squares[index].piece = this;
                positionOnBoard = index;

                // Move the piece gameobject.

                transform.position = board.squares[index].transform.position;

                if (!hasMoved) hasMoved = true;

                return true;
            }
        }

        return false;
    }

    public bool MoveTo(int index)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] == index)
            {
                // Remove this piece from the old square/current square.

                board.squares[positionOnBoard].hasPiece = false;
                board.squares[positionOnBoard].piece = null;

                // Move this piece to this square

                board.squares[index].hasPiece = true;
                board.squares[index].piece = this;
                positionOnBoard = index;

                // Move the piece gameobject.

                transform.position = board.squares[index].transform.position;

                if (!hasMoved) hasMoved = true;

                return true;
            }
        }

        return false;
    }

    public void AddDiagonalMoves(int step,
        List<int> tempMoves,
        List<int> tempAttacks,
        List<int> tempDefending,
        List<int> tempDiscoverableAttacks,
        List<int> tempDiscoverableMoves,
        List<int> tempDiscoverableDefending)
    {
        // Both white and black will move the same way.
        // Search using step until you are off the board.

        int tempIndex = positionOnBoard;

        // Keeps the move in the scope of the board but doesn't stop wrap 
        // around of board (7 is edge of board but using +1 will wrap round)

        //float boardLength = Mathf.Sqrt(board.squares.Length);

        int row = board.GetRow(board.boardHeight, board.boardLength, tempIndex);
        int column = board.GetColumn(board.boardHeight, board.boardLength, tempIndex);

        // Used to change how to move information is stored.
        // When path is not blocked store index normally.
        // When path is blocked store index in "discoverable.."

        bool isThisWayBlocked = false;

        tempIndex += step;

        // Increment/Decrement Row and Column

        while (board.IsMoveInScope(tempIndex))
        {
            int[] tempRowColumn = AdjustRowAndColumn(step, row, column);

            row = tempRowColumn[0];
            column = tempRowColumn[1];

            // If we have went onto a new column stop!

            if (column > board.boardLength
                || column < 1
                || row > board.boardHeight
                || row < 1)
            {
                // Stop the move from rapping around and being on a new column and row.

                return;
            }
            else
            {
                if (isThisWayBlocked == false)
                {
                    // THE PATH IS CURRENTLY NOT BLOCKED.

                    // Check is square free?

                    if (board.DoesSquareHavePiece(tempIndex))
                    {
                        // Yes the square has a piece!
                        // What colour is the piece? what colour is "this" piece.

                        if (board.IsWhiteControlled(tempIndex) == isWhite)
                        {
                            // Is our piece
                            // Add to defending

                            tempDefending.Add(tempIndex);
                        }
                        else
                        {
                            // Is not our piece

                            tempAttacks.Add(tempIndex);
                        }

                        // This way is now blocked.

                        isThisWayBlocked = true;
                    }
                    else
                    {
                        // Does not have piece
                        // Add to the move list.

                        tempMoves.Add(tempIndex);
                    }
                }
                else
                {
                    // THE PATH IS CURRENTLY BLOCKED!
                    // All move from here should be added to discovered attacks/defends/moves.

                    // Check is square free?

                    if (board.DoesSquareHavePiece(tempIndex))
                    {
                        // Yes the square has a piece!
                        // What colour is the piece? what colour is "this" piece.

                        if (board.IsWhiteControlled(tempIndex) == isWhite)
                        {
                            // Is our piece
                            // Add to defending

                            tempDiscoverableDefending.Add(tempIndex);
                        }
                        else
                        {
                            // Is not our piece

                            tempDiscoverableAttacks.Add(tempIndex);
                        }
                    }
                    else
                    {
                        // Does not have piece
                        // Add to the move list.

                        tempDiscoverableMoves.Add(tempIndex);
                    }
                }
            }

            // Increment the position on the board by the step

            tempIndex += step;
        }
    }

    public void AddVerticalMoves(int step,
        List<int> tempMoves,
        List<int> tempAttacks,
        List<int> tempDefending,
        List<int> tempDiscoverableAttacks,
        List<int> tempDiscoverableMoves,
        List<int> tempDiscoverableDefending)
    {
        // Both white and black will move the same way.
        // Search using step until you are off the board.

        int tempIndex = positionOnBoard;

        // Keeps the move in the scope of the board but doesn't stop wrap 
        // around of board (7 is edge of board but using +1 will wrap round)

        //float boardLength = Mathf.Sqrt(board.squares.Length);

        int row = board.GetRow(board.boardHeight, board.boardLength, tempIndex);
        int column = board.GetColumn(board.boardHeight, board.boardLength, tempIndex);

        // Used to change how to move information is stored.
        // When path is not blocked store index normally.
        // When path is blocked store index in "discoverable.."

        bool isThisWayBlocked = false;

        tempIndex += step;

        while (board.IsMoveInScope(tempIndex))
        {
            int[] tempRowColumn = AdjustRowAndColumn(step, row, column);

            row = tempRowColumn[0];
            column = tempRowColumn[1];

            // If we have went onto a new column stop!

            if (row > board.boardHeight
                || row < 1)
            {
                // Stop the move from rapping around and being on a new column.

                return;
            }
            else
            {
                if (isThisWayBlocked == false)
                {
                    // THE PATH IS CURRENTLY NOT BLOCKED.

                    // Check is square free?

                    if (board.DoesSquareHavePiece(tempIndex))
                    {
                        // Yes the square has a piece!
                        // What colour is the piece? what colour is "this" piece.

                        if (board.IsWhiteControlled(tempIndex) == isWhite)
                        {
                            // Is our piece
                            // Add to defending

                            tempDefending.Add(tempIndex);
                        }
                        else
                        {
                            // Is not our piece

                            tempAttacks.Add(tempIndex);
                        }

                        // This way is now blocked.

                        isThisWayBlocked = true;
                    }
                    else
                    {
                        // Does not have piece
                        // Add to the move list.

                        tempMoves.Add(tempIndex);
                    }
                }
                else
                {
                    // THE PATH IS CURRENTLY BLOCKED!
                    // All move from here should be added to discovered attacks/defends/moves.

                    // Check is square free?

                    if (board.DoesSquareHavePiece(tempIndex))
                    {
                        // Yes the square has a piece!
                        // What colour is the piece? what colour is "this" piece.

                        if (board.IsWhiteControlled(tempIndex) == isWhite)
                        {
                            // Is our piece
                            // Add to defending

                            tempDiscoverableDefending.Add(tempIndex);
                        }
                        else
                        {
                            // Is not our piece

                            tempDiscoverableAttacks.Add(tempIndex);
                        }
                    }
                    else
                    {
                        // Does not have piece
                        // Add to the move list.

                        tempDiscoverableMoves.Add(tempIndex);
                    }
                }
            }

            // Increment the position on the board by the step

            tempIndex += step;
        }
    }

    public void AddHorizontalMoves(int step,
        List<int> tempMoves,
        List<int> tempAttacks,
        List<int> tempDefending,
        List<int> tempDiscoverableAttacks,
        List<int> tempDiscoverableMoves,
        List<int> tempDiscoverableDefending)
    {
        // Both white and black will move the same way.
        // Search using step until you are off the board.

        int tempIndex = positionOnBoard;

        // Keeps the move in the scope of the board but doesn't stop wrap 
        // around of board (7 is edge of board but using +1 will wrap round)

        //float boardLength = Mathf.Sqrt(board.squares.Length);

        int row = board.GetRow(board.boardHeight, board.boardLength, tempIndex);
        int column = board.GetColumn(board.boardHeight, board.boardLength, tempIndex);

        // Used to change how to move information is stored.
        // When path is not blocked store index normally.
        // When path is blocked store index in "discoverable.."

        bool isThisWayBlocked = false;

        tempIndex += step;

        while (board.IsMoveInScope(tempIndex))
        {
            int[] tempRowColumn = AdjustRowAndColumn(step, row, column);

            row = tempRowColumn[0];
            column = tempRowColumn[1];

            // If we have went onto a new column stop!

            if (column > board.boardLength
                || column < 1)
            {
                // Stop the move from rapping around and being on a new row.

                return;
            }
            else
            {
                if (isThisWayBlocked == false)
                {
                    // THE PATH IS CURRENTLY NOT BLOCKED.

                    // Check is square free?

                    if (board.DoesSquareHavePiece(tempIndex))
                    {
                        // Yes the square has a piece!
                        // What colour is the piece? what colour is "this" piece.

                        if (board.IsWhiteControlled(tempIndex) == isWhite)
                        {
                            // Is our piece
                            // Add to defending

                            tempDefending.Add(tempIndex);
                        }
                        else
                        {
                            // Is not our piece

                            tempAttacks.Add(tempIndex);
                        }

                        // This way is now blocked.

                        isThisWayBlocked = true;
                    }
                    else
                    {
                        // Does not have piece
                        // Add to the move list.

                        tempMoves.Add(tempIndex);
                    }
                }
                else
                {
                    // THE PATH IS CURRENTLY BLOCKED!
                    // All move from here should be added to discovered attacks/defends/moves.

                    // Check is square free?

                    if (board.DoesSquareHavePiece(tempIndex))
                    {
                        // Yes the square has a piece!
                        // What colour is the piece? what colour is "this" piece.

                        if (board.IsWhiteControlled(tempIndex) == isWhite)
                        {
                            // Is our piece
                            // Add to defending

                            tempDiscoverableDefending.Add(tempIndex);
                        }
                        else
                        {
                            // Is not our piece

                            tempDiscoverableAttacks.Add(tempIndex);
                        }
                    }
                    else
                    {
                        // Does not have piece
                        // Add to the move list.

                        tempDiscoverableMoves.Add(tempIndex);
                    }
                }
            }

            // Increment the position on the board by the step

            tempIndex += step;
        }
    }

    public int[] AdjustRowAndColumn(int moveType, int row, int column)
    {
        switch (moveType)
        {
            case (int)Piece.MoveType.North:
                row += 1;
                break;

            case (int)Piece.MoveType.South:
                row -= 1;
                break;

            case (int)Piece.MoveType.East:
                column += 1;
                break;

            case (int)Piece.MoveType.West:
                column -= 1;
                break;

            case (int)Piece.MoveType.NorthEast:
                row += 1;
                column += 1;
                break;

            case (int)Piece.MoveType.NorthWest:
                row += 1;
                column -= 1;
                break;

            case (int)Piece.MoveType.SouthEast:
                row -= 1;
                column += 1;
                break;

            case (int)Piece.MoveType.SouthWest:
                row -= 1;
                column -= 1;
                break;
        }

        int[] temp = { row, column };

        return temp;
    }
}
