using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GetMoves() - Must be called when created.
/// 
/// When checking if SetActive(true/false) check isActive
/// 
/// Transform.positions - removed
/// </summary>


public class PawnLogic : PieceLogic
{
    private BoardLogic board;

    // Required to stop recursive search extending depth of 7
    public new void SetBoard(BoardLogic boardLogic)
    {
        board = boardLogic;
    }

    public bool isPromoted = false;
    public bool hasBeenPromoted = false;    // Used to control when the pawn actually need promoted -  Step and backward between boards shouldn't not affect this.

    public PieceLogic.Type promotionType = PieceLogic.Type.Pawn;

    public int[] enPassantAttacks = null;
    public int[] guarding = null;           // For analytics only (square empty that the pawn could attack)

    public int developed = 2;   // how far has the pawn progressed into the board

    // Should be set to represent pawns en passant location.

    public int possibleEnPassantIndex = -1;
    public int wouldBeEnPassantVulnerableHereIndex = -1;
    public bool enPassantVulnerable = false;

    // Promotion

    public int[] discoverablesAttacks = null;
    public int[] discoverableMoves = null;
    public int[] discoverableDefending = null;

    public PawnLogic()
    {

    }

    public void Promote(PieceLogic.Type type)
    {
        isPromoted = true;
        promotionType = type;

        GetMoves();
    }

    public void Demote()
    {
        isPromoted = false;
        promotionType = PieceLogic.Type.Pawn;

        GetMoves();
    }

    public new void GetMoves()
    {
        switch (promotionType)
        {
            case PieceLogic.Type.Pawn:
                GetPawnMoves();
                break;

            case PieceLogic.Type.Rook:
                GetRookMoves();
                break;

            case PieceLogic.Type.Knight:
                GetKnightMoves();
                break;

            case PieceLogic.Type.Bishop:
                GetBishopMoves();
                break;

            case PieceLogic.Type.Queen:
                GetQueenMoves();
                break;

            case PieceLogic.Type.King:

                break;
        }
    }

    public void GetPawnMoves()
    {
        if (isWhite)
        {
            // Get pawn move, attack and defending index information.

            defending = GetWhitePawnDefending();
            moves = GetWhitePawnMoves();
            attacks = GetWhitePawnAttacks();

            // Pawn unique "En Passant!" - 

            enPassantAttacks = GetWhitePawnEnPassantAttacks();
        }
        else    // BLACK MOVES =================
        {
            // Get pawn move, attack and defending index information.

            defending = GetBlackPawnDefending();
            moves = GetBlackPawnMoves();
            attacks = GetBlackPawnAttacks();

            // Pawn unique "En Passant!" - 

            enPassantAttacks = GetBlackPawnEnPassantAttacks();
        }
    }

    public void GetRookMoves()
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

        // Convert list to arrays for later reference. Store in global variable.

        attacks = tempAttacks.ToArray();
        defending = tempDefending.ToArray();
        moves = tempMoves.ToArray();

        discoverablesAttacks = tempDiscoverableAttacks.ToArray();
        discoverableDefending = tempDiscoverableDefending.ToArray();
        discoverableMoves = tempDiscoverableMoves.ToArray();
    }


    public void GetKnightMoves()
    {
        List<int> tempMoves = new List<int>();
        List<int> tempAttacks = new List<int>();
        List<int> tempDefending = new List<int>();

        AddKnightMove((int)Knight.Direction.one,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddKnightMove((int)Knight.Direction.two,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddKnightMove((int)Knight.Direction.three,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddKnightMove((int)Knight.Direction.four,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddKnightMove((int)Knight.Direction.five,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddKnightMove((int)Knight.Direction.six,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddKnightMove((int)Knight.Direction.seven,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddKnightMove((int)Knight.Direction.eight,
            tempMoves,
            tempAttacks,
            tempDefending);

        // Convert list to arrays for later reference. Store in global variable.

        attacks = tempAttacks.ToArray();
        defending = tempDefending.ToArray();
        moves = tempMoves.ToArray();
    }

    public void GetBishopMoves()
    {
        List<int> tempMoves = new List<int>();
        List<int> tempAttacks = new List<int>();
        List<int> tempDefending = new List<int>();
        List<int> tempDiscoverableAttacks = new List<int>();
        List<int> tempDiscoverableMoves = new List<int>();
        List<int> tempDiscoverableDefending = new List<int>();

        // Both white and black will move the same way.

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

        // Convert list to arrays for later reference. Store in field variable.

        attacks = tempAttacks.ToArray();
        defending = tempDefending.ToArray();
        moves = tempMoves.ToArray();

        discoverablesAttacks = tempDiscoverableAttacks.ToArray();
        discoverableDefending = tempDiscoverableDefending.ToArray();
        discoverableMoves = tempDiscoverableMoves.ToArray();
    }

    public void GetQueenMoves()
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
        for (int i = 0; i < enPassantAttacks.Length; i++)
        {
            if (index == enPassantAttacks[i]) return true;
        }

        // did not find the move

        return false;
    }

    public new void Move(int index)
    {
        if (MoveTo(index) == false)
        {
            if (Attack(index) == false)
            {
                EnPassantAttack(index);
            }
        }

        if (isWhite)
        {
            developed = board.GetRow(board.boardHeight, board.boardLength, positionOnBoard);
        }
        else
        {
            developed = (board.boardHeight - board.GetRow(board.boardHeight, board.boardLength, positionOnBoard))
                + 1;
        }

        if (hasBeenPromoted == false
            && developed == board.boardHeight)
        {
            Promote(PieceLogic.Type.Queen);
            hasBeenPromoted = true;

            // Best promotion piece is queen. Assume always queen selected.
        }
    }

    public bool Attack(int index)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i] == index)
            {
                // disable attacked piece

                board.squares[index].piece.isActive = false;

                // Remove this piece from previous square

                board.squares[positionOnBoard].hasPiece = false;
                board.squares[positionOnBoard].piece = null;

                // Move this piece to this square

                board.squares[index].hasPiece = true;
                board.squares[index].piece = this;
                positionOnBoard = index;

                if (!hasMoved) hasMoved = true;

                return true;
            }
        }

        return false;
    }

    public bool EnPassantAttack(int index)
    {
        for (int i = 0; i < enPassantAttacks.Length; i++)
        {
            if (enPassantAttacks[i] == index)
            {
                // is an en passant attack!
                // The pawn during en passant is attackable on both squares+.

                // Remove this piece from previous square

                board.squares[positionOnBoard].hasPiece = false;
                board.squares[positionOnBoard].piece = null;

                // Move this piece to this square

                board.squares[index].hasPiece = true;
                board.squares[index].piece = this;
                positionOnBoard = index;

                // Get the position the attacked en passant piece is actually on

                int enPassantActualPosition = board.squares[index].enPassant.positionOnBoard;

                // Manage en passant move.

                board.squares[index].enPassant = null;
                board.squares[index].hasEnPassant = false;

                board.squares[enPassantActualPosition].piece.isActive = false;

                // Manage the square the pawn is actually on

                board.squares[enPassantActualPosition].hasPiece = false;
                board.squares[enPassantActualPosition].piece = null;

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
                // Are we performing the "en passant" move?

                if (index == possibleEnPassantIndex)
                {
                    // Leave this piece behind on the en passant piece aswell.
                    board.squares[wouldBeEnPassantVulnerableHereIndex].hasEnPassant = true;
                    board.squares[wouldBeEnPassantVulnerableHereIndex].enPassant = this;

                    enPassantVulnerable = true;
                }

                // Remove this piece from the old square/current square.

                board.squares[positionOnBoard].hasPiece = false;
                board.squares[positionOnBoard].piece = null;

                // Move this piece to this square

                board.squares[index].hasPiece = true;
                board.squares[index].piece = this;
                positionOnBoard = index;

                // Move the piece gameobject.

                if (!hasMoved) hasMoved = true;

                return true;
            }
        }

        return false;
    }

    public int[] GetWhitePawnMoves()
    {
        System.Collections.Generic.List<int> tempMoves
            = new System.Collections.Generic.List<int>();

        // Pawn Moves - empty spaces

        bool infrontClear = false;
        int tempIndex = positionOnBoard + 1;

        if (board.IsMoveInScope(tempIndex))
        {
            infrontClear = !board.DoesSquareHavePiece(tempIndex);

            if (infrontClear)
            {
                tempMoves.Add(tempIndex);

                if (!hasMoved) wouldBeEnPassantVulnerableHereIndex = tempIndex;
            }
        }

        // Special moves - If the pawn has never moved.

        tempIndex = positionOnBoard + 2;

        if (!hasMoved
            && infrontClear
            && board.IsMoveInScope(tempIndex))
        {
            // is two spaces infront clear?
            // index + 2

            bool twoInfrontClear = !board.DoesSquareHavePiece(tempIndex);

            if (infrontClear && twoInfrontClear)
            {
                possibleEnPassantIndex = tempIndex;

                tempMoves.Add(tempIndex);
            }
        }

        return tempMoves.ToArray();
    }

    public int[] GetBlackPawnMoves()
    {
        System.Collections.Generic.List<int> tempMoves
            = new System.Collections.Generic.List<int>();

        // Pawn Moves - empty spaces

        bool infrontClear = false;
        int tempIndex = positionOnBoard - 1;

        if (board.IsMoveInScope(tempIndex))
        {
            infrontClear = !board.DoesSquareHavePiece(tempIndex);

            if (infrontClear)
            {
                if (!hasMoved) wouldBeEnPassantVulnerableHereIndex = tempIndex;

                tempMoves.Add(tempIndex);
            }
        }

        // Special moves - If the pawn has never moved.

        tempIndex = positionOnBoard - 2;

        if (!hasMoved
            && infrontClear
            && board.IsMoveInScope(tempIndex))
        {
            // is two spaces infront clear?
            // index - 2

            bool twoInfrontClear = !board.DoesSquareHavePiece(tempIndex);

            if (infrontClear && twoInfrontClear)
            {
                tempMoves.Add(tempIndex);

                possibleEnPassantIndex = tempIndex;
            }
        }

        return tempMoves.ToArray();
    }

    public int[] GetWhitePawnAttacks()
    {
        System.Collections.Generic.List<int> tempAttacks
            = new System.Collections.Generic.List<int>();

        System.Collections.Generic.List<int> tempGuarding
            = new System.Collections.Generic.List<int>();

        // Up and Left attack!
        int tempIndex = positionOnBoard - 7;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex) == false)
                {
                    // Attacking
                    tempAttacks.Add(tempIndex);
                }
                else
                {
                    // Guarding this square.

                    tempGuarding.Add(tempIndex);
                }
            }
            else
            {
                // Guarding this square.

                tempGuarding.Add(tempIndex);
            }
        }

        // Up and Right attack!
        tempIndex = positionOnBoard + 9;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex) == false)
                {
                    // Attacking
                    tempAttacks.Add(tempIndex);
                }
                else
                {
                    // Guarding this square.

                    tempGuarding.Add(tempIndex);
                }
            }
            else
            {
                // Guarding this square.

                tempGuarding.Add(tempIndex);
            }
        }

        guarding = tempGuarding.ToArray();

        return tempAttacks.ToArray();
    }

    public int[] GetBlackPawnAttacks()
    {
        System.Collections.Generic.List<int> tempAttacks
            = new System.Collections.Generic.List<int>();

        System.Collections.Generic.List<int> tempGuarding
            = new System.Collections.Generic.List<int>();

        // Attack! - when enemy occupied

        // Down and Right attack!
        int tempIndex = positionOnBoard + 7;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex))
                {
                    // Attacking
                    tempAttacks.Add(tempIndex);
                }
                else
                {
                    // Guarding this square.

                    tempGuarding.Add(tempIndex);
                }
            }
            else
            {
                // Guarding this square.

                tempGuarding.Add(tempIndex);
            }
        }

        // Down and Left attack!
        tempIndex = positionOnBoard - 9;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex))
                {
                    // Attacking
                    tempAttacks.Add(tempIndex);
                }
                else
                {
                    // Guarding this square.

                    tempGuarding.Add(tempIndex);
                }
            }
            else
            {
                // Guarding this square.

                tempGuarding.Add(tempIndex);
            }
        }

        guarding = tempGuarding.ToArray();

        return tempAttacks.ToArray();
    }

    public int[] GetWhitePawnDefending()
    {
        System.Collections.Generic.List<int> tempDefending
            = new System.Collections.Generic.List<int>();

        // Up and Left defence!
        int tempIndex = positionOnBoard - 7;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex) == true)
                {
                    // Defending
                    tempDefending.Add(tempIndex);
                }
            }
        }

        // Up and Right defence!
        tempIndex = positionOnBoard + 9;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex) == true)
                {
                    // Defending
                    tempDefending.Add(tempIndex);
                }
            }
        }

        return tempDefending.ToArray();
    }

    public int[] GetBlackPawnDefending()
    {
        System.Collections.Generic.List<int> tempDefending
            = new System.Collections.Generic.List<int>();

        // Defending! - when enemy occupied

        // Down and Right defence!
        int tempIndex = positionOnBoard + 7;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex) == false)
                {
                    // Defending
                    tempDefending.Add(tempIndex);
                }
            }
        }

        // Down and Left defence!
        tempIndex = positionOnBoard - 9;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHavePiece(tempIndex))
            {
                if (board.IsWhiteControlled(tempIndex) == false)
                {
                    // Defending
                    tempDefending.Add(tempIndex);
                }
            }
        }

        return tempDefending.ToArray();
    }

    public int[] GetWhitePawnEnPassantAttacks()
    {
        System.Collections.Generic.List<int> tempEnPassantAttacks
            = new System.Collections.Generic.List<int>();

        // En Passant - "in passing"
        // check the squares you can attack for an en passant piece.
        int tempIndex = positionOnBoard - 7;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHaveEnPassantPawn(tempIndex)
                && board.IsEnPassantWhite(tempIndex) == false)
            {
                tempEnPassantAttacks.Add(tempIndex);
            }
        }

        tempIndex = positionOnBoard + 9;
        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHaveEnPassantPawn(tempIndex)
                && board.IsEnPassantWhite(tempIndex) == false)
            {
                tempEnPassantAttacks.Add(tempIndex);
            }
        }

        return tempEnPassantAttacks.ToArray();
    }

    public int[] GetBlackPawnEnPassantAttacks()
    {
        System.Collections.Generic.List<int> tempEnPassantAttacks
            = new System.Collections.Generic.List<int>();

        // En Passant - "in passing"
        // check the squares you can attack for an en passant piece.
        int tempIndex = positionOnBoard + 7;

        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHaveEnPassantPawn(tempIndex)
                && board.IsEnPassantWhite(tempIndex))
            {
                tempEnPassantAttacks.Add(tempIndex);
            }
        }

        tempIndex = positionOnBoard - 9;
        if (board.IsMoveInScope(tempIndex))
        {
            if (board.DoesSquareHaveEnPassantPawn(tempIndex)
                && board.IsEnPassantWhite(tempIndex))
            {
                tempEnPassantAttacks.Add(tempIndex);
            }
        }

        return tempEnPassantAttacks.ToArray();
    }

    // Promotion

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

    public void AddKnightMove(int step,
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

    public int[] AdjustRowAndColumnForKnight(int moveType, int row, int column)
    {
        switch (moveType)
        {
            case (int)Knight.Direction.one:
                row += 2;
                column += 1;
                break;

            case (int)Knight.Direction.two:
                row += 1;
                column += 2;
                break;

            case (int)Knight.Direction.three:
                row -= 1;
                column += 2;
                break;

            case (int)Knight.Direction.four:
                row -= 2;
                column += 1;
                break;

            case (int)Knight.Direction.five:
                row -= 2;
                column -= 1;
                break;

            case (int)Knight.Direction.six:
                row -= 1;
                column -= 2;
                break;

            case (int)Knight.Direction.seven:
                row += 1;
                column -= 2;
                break;

            case (int)Knight.Direction.eight:
                row += 2;
                column -= 1;
                break;
        }

        int[] temp = { row, column };

        return temp;
    }

    public bool IsEnPassantAttackLegal(int index)
    {
        bool result;

        int originalPosition = positionOnBoard;
        int destinationPosition = index;

        // Get the position the attacked En Passant piece is actually on

        int enPassantActualPosition = board.squares[destinationPosition].enPassant.positionOnBoard;
        PawnLogic attackedPawn = (PawnLogic)board.squares[enPassantActualPosition].piece;

        // Remove this piece from previous square

        board.squares[originalPosition].hasPiece = false;
        board.squares[originalPosition].piece = null;

        // Move this piece to this square

        board.squares[destinationPosition].hasPiece = true;
        board.squares[destinationPosition].piece = this;
        positionOnBoard = destinationPosition;

        // Manage En Passant move.

        board.squares[destinationPosition].enPassant = null;
        board.squares[destinationPosition].hasEnPassant = false;

        attackedPawn.isActive = false;

        // Manage the square the pawn is actually on

        board.squares[enPassantActualPosition].hasPiece = false;
        board.squares[enPassantActualPosition].piece = null;

        board.UpdateAllPieceMoves();

        ((KingLogic)board.whiteKing).KingUniqueMoves();
        ((KingLogic)board.blackKing).KingUniqueMoves();

        if (isWhite)
        {
            // White king is now in check? - Illegal move

            result = ((KingLogic)board.whiteKing).inCheck;
        }
        else
        {
            // Black king is now in check? - Illegal move.

            result = ((KingLogic)board.blackKing).inCheck;
        }

        // Reverse move! =====

        // Remove this piece from previous square

        board.squares[originalPosition].hasPiece = true;
        board.squares[originalPosition].piece = this;

        positionOnBoard = originalPosition;

        // Move this piece to this square

        board.squares[destinationPosition].hasPiece = false;
        board.squares[destinationPosition].piece = null;

        // Manage En Passant move.

        board.squares[destinationPosition].enPassant = attackedPawn;
        board.squares[destinationPosition].hasEnPassant = true;

        attackedPawn.isActive = true;

        // Manage the square the pawn is actually on

        board.squares[enPassantActualPosition].hasPiece = true;
        board.squares[enPassantActualPosition].piece = attackedPawn;

        board.UpdateAllPieceMoves();

        ((KingLogic)board.whiteKing).KingUniqueMoves();
        ((KingLogic)board.blackKing).KingUniqueMoves();

        return !result;
    }
}
