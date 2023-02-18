using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    // These indexs can never be moved to. More for analytics of potential moves.
    // Only used to help develop which move is best.
    // "Discoverable/blocked attacks defence and moves"

    public int[] discoverablesAttacks = null;
    public int[] discoverableMoves = null;
    public int[] discoverableDefending = null;


    // Refers to clock hands
    public enum Direction
    { 
        one = 10,
        two = 17,
        three = 15,
        four = 6,
        five = -10,
        six = -17,
        seven = -15,
        eight = -6
    }

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

        AddMove((int)Direction.one,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMove((int)Direction.two,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMove((int)Direction.three,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMove((int)Direction.four,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMove((int)Direction.five,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMove((int)Direction.six,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMove((int)Direction.seven,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMove((int)Direction.eight,
            tempMoves,
            tempAttacks,
            tempDefending);

        // Convert list to arrays for later reference. Store in global variable.

        attacks = tempAttacks.ToArray();
        defending = tempDefending.ToArray();
        moves = tempMoves.ToArray();
    }

    public new bool HasMove(int index)
    {
        // No unique moves to check through
        // Not used.

        return false;
    }

    public new void Move(int index)
    {
        Debug.Log("Knight.Move() - Pawn = " + this.transform.name + " Index = " + index.ToString());

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

    public void AddMove(int step,
        List<int> tempMoves,
        List<int> tempAttacks,
        List<int> tempDefending)
    {
        int index = positionOnBoard;

        int tempIndex = index + step;

        int row = board.GetRow(board.boardHeight, board.boardLength, index);
        int column = board.GetColumn(board.boardHeight, board.boardLength, index);

        int[] tempRowColumn = AdjustRowAndColumn(step, row, column);

        row = tempRowColumn[0];
        column = tempRowColumn[1];

        if (column > board.boardLength
            || column < 1
            || row > board.boardHeight
            || row < 1)
        {
            // Stop the move from rapping around and being on a new column and row.

            return;
        }

        if (board.IsMoveInScope(tempIndex))
        {
            // Is the square occupied?
            if (board.DoesSquareHavePiece(tempIndex))
            {
                // Yes the square is occupied.
                // What colour is the piece?
                if (board.IsWhiteControlled(tempIndex) == isWhite)
                {
                    // is our piece - defend.

                    tempDefending.Add(tempIndex);
                }
                else
                {
                    // is not our piece - attack
                    tempAttacks.Add(tempIndex);
                }
            }
            else
            {
                // The square is free. Add it to moves.
                tempMoves.Add(tempIndex);
            }
        }
    }

    public int[] AdjustRowAndColumn(int moveType, int row, int column)
    {
        switch (moveType)
        {
            case (int)Direction.one:
                row += 2;
                column += 1;
                break;

            case (int)Direction.two:
                row += 1;
                column += 2;
                break;

            case (int)Direction.three:
                row -= 1;
                column += 2;
                break;

            case (int)Direction.four:
                row -= 2;
                column += 1;
                break;

            case (int)Direction.five:
                row -= 2;
                column -= 1;
                break;

            case (int)Direction.six:
                row -= 1;
                column -= 2;
                break;

            case (int)Direction.seven:
                row += 1;
                column -= 2;
                break;

            case (int)Direction.eight:
                row += 2;
                column -= 1;
                break;
        }

        int[] temp = { row, column };

        return temp;
    }
}