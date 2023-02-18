using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lane
{
    public enum State { Attacked, DefendedAttack, DefendedMove, Defended, DoubleDefence, Blocked, Open, OffBoard }

    public State state;
    public int attacker = -1;
    public int attackerDepth = -1;
    public int defender = -1;
    public int defenderDepth = -1;
    public int blocker = -1;
    public int blockerDepth = -1;
    public int[] squares = null;
    public int[] possibleSupportingPieces = null;

    public Lane()
    {
        // Constructor

        state = State.Open;
        attacker = -1;
        attackerDepth = -1;
        defender = -1;
        defenderDepth = -1;
        blocker = -1;
        blockerDepth = -1;
        squares = null;
        possibleSupportingPieces = null;
    }

    public void Reset()
    {
        state = State.Open;
        attacker = -1;
        attackerDepth = -1;
        defender = -1;
        defenderDepth = -1;
        blocker = -1;
        blockerDepth = -1;
        squares = null;
        possibleSupportingPieces = null;
    }
}

public class King : Piece
{
    public bool inCheck = false;
    public bool inCheckMate = false;
    public bool inStaleMate = false;

    public bool canCastle = false;
    public bool canCastleQueenSide = false;

    public int[] castleMoves = null;
    public int[] castleQueenSideMoves = null;

    public int castleIndex = -1;
    public int castleQueenSideIndex = -1;

    public int kingSideRookStartingPosition = -1;
    public int queenSideRookStartingPosition = -1;

    public enum CastleDirection 
    { 
        QueenSide = 8,
        KingSide = -8
    }

    public Lane north = null;
    public Lane south = null;
    public Lane east = null;
    public Lane west = null;
    public Lane northEast = null;
    public Lane northWest = null;
    public Lane southEast = null;
    public Lane southWest = null;

    public int[] piecesAttackingThisKing = null;
    public int[] possibleSupportIndexes = null;

    public bool hasNoMoves = false;
    public bool hasNoAttacks = false;
    public bool hasNoSupport = false;

    // Cant do everything in awake and start. Need to use 
    // custom init in update once all other awake/start are done.

    private bool init = false;

    void Awake()
    {
        //board = GameObject.Find("Board").GetComponent<Board>();

        // initiation can't be done here as there will be "syncing" issues. 
        // Need all pieces start/awake done first.
    }

    void Start()
    {
        // initiation can't be done here as there will be "syncing" issues. 
        // Need all pieces start/awake done first.
    }

    void Update()
    {
        if (init == false)
        {
            init = true;

            if (isWhite)
            {
                kingSideRookStartingPosition = 56;
                queenSideRookStartingPosition = 0;
            }
            else
            {
                kingSideRookStartingPosition = 63;
                queenSideRookStartingPosition = 7;
            }

            KingUniqueMoves();  // Must be in start as it uses all scripts that are initiated in Awake()
        }
    }

    public void ResetOnTurn()
    {
        // Manage all of the values used to assess the kings situation.

        inCheck = false;
        inCheckMate = false;
        inStaleMate = false;

        canCastle = true;
        canCastleQueenSide = true;

        castleMoves = null;
        castleQueenSideMoves = null;

        // Reset each lane

        north.Reset();
        south.Reset();
        east.Reset();
        west.Reset();
        northEast.Reset();
        northWest.Reset();
        southEast.Reset();
        southWest.Reset();

        piecesAttackingThisKing = null;
        possibleSupportIndexes = null;

        hasNoMoves = false;
        hasNoAttacks = false;
        hasNoSupport = false;
}

    public void KingUniqueMoves()
    {
        ResetOnTurn();
        CheckCanCastle();
        SetPiecesAttackingKing();
        GetMoves();
        RemoveKingsCheckedMoves();
        GetVulnerabilities();
        GetSupportingPieces();
        CheckForCheck();
        CheckForStaleMate();
        if(inCheck) CheckForCheckMate();
    }

    public void GetVulnerabilities()
    {
        // Get all the "lanes" (8 lanes) from the kings position out.
        // Depending on type of lane will only be vulnerable to certain pieces

        // A lane can be the length or height of the board if a square is in a corner.

        // North, South, East and West
        //  - Queen + Bishop

        // NorthEast, NorthWest, SouthEast and SouthWest
        //  - Queen + Rook

        // Both white and black will move the same way.

        north = AddVerticalVulnerabilities((int)Piece.MoveType.North,
            north);

        south = AddVerticalVulnerabilities((int)Piece.MoveType.South,
            south);

        east = AddHorizontalVulnerabilities((int)Piece.MoveType.East,
            east);

        west = AddHorizontalVulnerabilities((int)Piece.MoveType.West,
            west);

        northEast = AddDiagonalVulnerabilities((int)Piece.MoveType.NorthEast,
            northEast);

        northWest = AddDiagonalVulnerabilities((int)Piece.MoveType.NorthWest,
            northWest);

        southEast = AddDiagonalVulnerabilities((int)Piece.MoveType.SouthEast,
            southEast);

        southWest = AddDiagonalVulnerabilities((int)Piece.MoveType.SouthWest,
            southWest);
    }

    public void CheckForCheckMate()
    {
        // King is under attack 
        //      - can not move and has
        //      - No support

        // Is there more than one piece attack?

        if (possibleSupportIndexes == null || possibleSupportIndexes.Length == 0) 
        {
            hasNoSupport = true;
        }

        if (moves == null || moves.Length == 0)
        {
            hasNoMoves = true;
        }
        else
        {
            Debug.Log("Nah no checkmate lol. Moves.Length = " + moves.Length.ToString());
        }

        if (attacks == null || attacks.Length == 0)
        {
            hasNoAttacks = true;
        }

        if (hasNoSupport && hasNoMoves && hasNoAttacks)
        {
            inCheckMate = true;
        }
    }

    public void SetPiecesAttackingKing()
    {
        piecesAttackingThisKing = board.GetPiecesAttacking(isWhite, positionOnBoard);
    }

    public void CheckForCheck()
    {
        // The king must move - the king is under attack and can move.
        // or another piece can intervene.

        // Is the King under attack?

        if (piecesAttackingThisKing.Length > 0)
        {
            inCheck = true;
            canCastle = false;
            canCastleQueenSide = false;
        }
        else
        {
            inCheck = false;
        }
    }

    public void CheckForStaleMate()
    {
        // Does this side(white/black) have no move/attack/special move - for any piece?

        bool hasNoLegalMove = board.DoesSideHaveMove(isWhite);

        // Stalemate is a situation in the game of chess where the player
        // whose turn it is to move is not in check but has no legal move.

        if (hasNoLegalMove == true && inCheck == false)
        {
            inStaleMate = true;
        }
        else 
        {
            inStaleMate = false;
        }

        // or...

        // Has the same sequence of moves been performed by both sides 3 times?

        // or....?
    }

    public void CheckCanCastle()
    {
        // Can not castle is king has moved

        if (hasMoved)
        {
            canCastleQueenSide = false;
            canCastle = false;

            return;
        }

        // Can not castle if in check.

        if (inCheck)
        {
            canCastle = false;
            canCastleQueenSide = false;

            return;
        }

        if (isWhite)
        {
            // (below is pretty janky) will break with a custom board. <<=================<<<
            // (Any piece that moves to this square will have hasMoved = true)

            if (board.squares[0].piece == null || board.squares[0].piece.hasMoved == true)
            {
                canCastleQueenSide = false;
            }

            if (board.squares[56].piece == null || board.squares[56].piece.hasMoved == true)
            {
                canCastle = false;
            }

            // ^^^^ < ===================================== <<<

            // Are squares empty towards that side
            // Ready the squares to empty.

            int[] squares = new int[3];

            squares[0] = 24;
            squares[1] = 16;
            squares[2] = 8;

            castleQueenSideIndex = 16;

            for (int i = 0; i < squares.Length; i++)
            {
                bool hasPiece = board.DoesSquareHavePiece(squares[i]);

                // The King cannot castle if it has to cross a square
                // which is being attacked by an enemy piece.

                bool isAttacked = board.IsSquareBeingAttacked(isWhite, squares[i]);

                // Its only the King that can't pass over an attacked square.
                // If '8' is under attack no problem.

                if (squares[i] != 8)
                {
                    if (hasPiece || isAttacked)
                    {
                        canCastleQueenSide = false;
                    }
                }
                else
                {
                    if (hasPiece)
                    {
                        canCastleQueenSide = false;
                    }
                }
            }

            if (canCastleQueenSide)
            {
                // Adjust squares that we can castle to as the 
                // square closest to the king is a king move aswell.

                squares = new int[2];

                squares[0] = 16;
                squares[1] = 8;

                castleQueenSideMoves = squares;
            }

            squares = new int[2];

            squares[0] = 40;
            squares[1] = 48;

            castleIndex = 48;

            for (int i = 0; i < squares.Length; i++)
            {
                bool hasPiece = board.DoesSquareHavePiece(squares[i]);

                // The King cannot castle if it has to cross a square
                // which is being attacked by an enemy piece.

                bool isAttacked = board.IsSquareBeingAttacked(isWhite, squares[i]);

                if (hasPiece || isAttacked)
                {
                    canCastle = false;
                }
            }

            if (canCastle)
            {
                // Adjust squares that we can castle to as the 
                // square closest to the king is a king move aswell.

                squares = new int[1];

                squares[0] = 48;

                castleMoves = squares;
            }
        }
        else
        {
            // For Black

            // (below is pretty janky) will break with a custom board. <<=================<<<
            // (Any piece that moves to this square will have hasMoved = true)

            if (board.squares[63].piece == null || board.squares[63].piece.hasMoved == true)
            {
                canCastle = false;
            }

            if (board.squares[7].piece == null || board.squares[7].piece.hasMoved == true)
            {
                canCastleQueenSide = false;
            }

            // ^^^^ < ===================================== <<<

            // Are squares empty towards that side
            // Ready the squares to empty.

            int[] squares = new int[3];

            squares[0] = 31;
            squares[1] = 23;
            squares[2] = 15;

            castleQueenSideIndex = 23;

            for (int i = 0; i < squares.Length; i++)
            {
                bool hasPiece = board.DoesSquareHavePiece(squares[i]);

                // The King cannot castle if it has to cross a square
                // which is being attacked by an enemy piece.

                bool isAttacked = board.IsSquareBeingAttacked(isWhite, squares[i]);

                // Its only the King that can't pass over an attacked square.
                // If '15' is under attack no problem.

                if (squares[i] != 8)
                {
                    if (hasPiece || isAttacked)
                    {
                        canCastleQueenSide = false;
                    }
                }
                else
                {
                    if (hasPiece)
                    {
                        canCastleQueenSide = false;
                    }
                }
            }

            if (canCastleQueenSide)
            {
                // Adjust squares that we can castle to as the 
                // square closest to the king is a king move aswell.

                squares = new int[2];

                squares[0] = 23;
                squares[1] = 15;

                castleQueenSideMoves = squares;
            }

            squares = new int[2];

            squares[0] = 47;
            squares[1] = 55;

            castleIndex = 55;

            for (int i = 0; i < squares.Length; i++)
            {
                bool hasPiece = board.DoesSquareHavePiece(squares[i]);

                // The King cannot castle if it has to cross a square
                // which is being attacked by an enemy piece.

                bool isAttacked = board.IsSquareBeingAttacked(isWhite, squares[i]);

                if (hasPiece || isAttacked)
                {
                    canCastle = false;
                }
            }

            if (canCastle)
            {
                // Adjust squares that we can castle to as the 
                // square closest to the king is a king move aswell.

                squares = new int[1];

                squares[0] = 55;

                castleMoves = squares;
            }
        }
    }

    public void GetSupportingPieces()
    {
        List<int> allSupportingPieces = new List<int>();

        // Exceptions are added to ignore the kings.
        // (So they dont return results that would indicate they can support themselves)

        int[] exceptions = new int[2];

        exceptions[0] = board.whiteKing.positionOnBoard;
        exceptions[1] = board.blackKing.positionOnBoard;

        // For all the surrounding piece squares that are free.

        if (north.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(north, exceptions, allSupportingPieces);
        }

        if (south.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(south, exceptions, allSupportingPieces);
        }

        if (east.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(east, exceptions, allSupportingPieces);
        }

        if (west.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(west, exceptions, allSupportingPieces);
        }

        if (northEast.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(northEast, exceptions, allSupportingPieces);
        }

        if (northWest.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(northWest, exceptions, allSupportingPieces);
        }

        if (southEast.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(southEast, exceptions, allSupportingPieces);
        }

        if (southWest.state == Lane.State.Attacked)
        {
            allSupportingPieces = GetLaneSupportingPieces(southWest, exceptions, allSupportingPieces);
        }

        // Add supporting piece that can attack "the piece attacking the king"
        // The is specifically how we handle the Knight that have put the king in check
        // and the piece that can attack those knights and the rooks/queen/bishops being directly attacked to defend.
        // This code is redudant for lane piece as it is already added and stored when each lane is searched through.

        if (piecesAttackingThisKing == null || piecesAttackingThisKing.Length == 0)
        {

        }
        else
        {
            for (int i = 0; i < piecesAttackingThisKing.Length; i++)
            {
                int[] tempSupport = board.GetPiecesAttacking(!isWhite, piecesAttackingThisKing[i]);

                for (int j = 0; j < tempSupport.Length; j++)
                {
                    //if (allSupportingPieces.Contains(tempSupport[j]) == false) 
                    allSupportingPieces.Add(tempSupport[j]);
                }
            }
        }

        // Store all the possible supporting pieces.

        possibleSupportIndexes = allSupportingPieces.ToArray();
    }

    public List<int> GetLaneSupportingPieces(Lane lane, int[] exceptions, List<int> allSupportingPieces)
    {
        int counter = 0;
        List<int> tempMoves = new List<int>();

        while (counter < lane.attackerDepth)
        {
            tempMoves.Add(lane.squares[counter]);
            counter++;
        }

        // For all the surrounding piece squares that are free.

        List<int> tempPieces = new List<int>();

        board.FindAllPiecesWithMovesWithExceptions(isWhite, tempMoves, tempPieces, exceptions);

        int[] attackingPieces = board.GetPiecesAttacking(!isWhite, lane.attacker);

        for (int i = 0; i < attackingPieces.Length; i++)
        {
            bool hasPiece = false;

            // Search Piece isn't already in list.

            for (int j = 0; j < tempPieces.Count; j++)
            {
                if (attackingPieces[i] == tempPieces[j])
                {
                    // Has piece
                    hasPiece = true;
                }
            }

            // So if we dont have the piece stored - store it.

            if (hasPiece == false)
            {
                tempPieces.Add(attackingPieces[i]);
            }
        }

        lane.possibleSupportingPieces = tempPieces.ToArray();

        // Add these all to the entire list of supporting pieces.

        for (int i = 0; i < lane.possibleSupportingPieces.Length; i++)
        {
            bool hasPiece = false;

            for (int j = 0; j < allSupportingPieces.Count; j++)
            {
                // Already storing piece?

                if (allSupportingPieces[j] == lane.possibleSupportingPieces[i])
                {
                    hasPiece = true;
                }
            }

            // Not tracking piece - store it!

            if (hasPiece == false)
            {
                allSupportingPieces.Add(lane.possibleSupportingPieces[i]);
            }
        }

        return allSupportingPieces;
    }

    public void RemoveKingsCheckedMoves()
    {
        List<int> actualMoves = new List<int>();

        // Remove the moves that the king can move that would put the king in check.
        // Which moves can attack the king

        for (int i = 0; i < moves.Length; i++)
        {
            // Is this moves being attacked? If NOT we should store it.

            if (board.IsSquareBeingAttacked(isWhite, moves[i]) == false)
            {
                actualMoves.Add(moves[i]);
            }
        }

        moves = actualMoves.ToArray();
    }

    public new void GetMoves()
    {
        List<int> tempMoves = new List<int>();
        List<int> tempAttacks = new List<int>();
        List<int> tempDefending = new List<int>();

        AddMoves((int)Piece.MoveType.North,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMoves((int)Piece.MoveType.South,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMoves((int)Piece.MoveType.East,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMoves((int)Piece.MoveType.West,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMoves((int)Piece.MoveType.NorthEast,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMoves((int)Piece.MoveType.NorthWest,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMoves((int)Piece.MoveType.SouthEast,
            tempMoves,
            tempAttacks,
            tempDefending);

        AddMoves((int)Piece.MoveType.SouthWest,
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
        Debug.Log("King.HasMove() - Ran inherited class function");

        if (canCastle)
        {
            if (index == castleIndex) return true;
        }

        if (canCastleQueenSide)
        {
            if (index == castleQueenSideIndex) return true;
        }

        return false;
    }

    public new void Move(int index)
    {
        Debug.Log("King.Move() - King = " + this.transform.name + " Index = " + index.ToString());

        if (MoveTo(index) == false)
        {
            if (Attack(index) == false)
            {
                Castle(index);
            }
        }
    }

    public bool Castle(int index)
    {
        // For King side castle moves check if this index matches
        
        if (castleIndex == index)
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

            // Rook ====================

            int rooksIndex = kingSideRookStartingPosition;

            // What is rooks new position?

            int rooksNewPosition = index - 8;

            // Move the rook piece to this square

            board.squares[rooksNewPosition].hasPiece = true;
            board.squares[rooksNewPosition].piece = board.squares[rooksIndex].piece;
            board.squares[rooksNewPosition].piece.positionOnBoard = rooksNewPosition;

            // Remove the piece from the old square.

            board.squares[rooksIndex].hasPiece = false;
            board.squares[rooksIndex].piece = null;

            // Move the piece gameobject.

            board.squares[rooksNewPosition].piece.transform.position = board.squares[rooksNewPosition].transform.position;
            
            hasMoved = true;
            board.squares[rooksNewPosition].piece.hasMoved = true;

            return true;
        }

        if (castleQueenSideIndex == index)
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

            // Rook ====================

            int rooksIndex = queenSideRookStartingPosition;

            // What is rooks new position?

            int rooksNewPosition = index + 8;

            // Move the rook piece to this square

            board.squares[rooksNewPosition].hasPiece = true;
            board.squares[rooksNewPosition].piece = board.squares[rooksIndex].piece;
            board.squares[rooksNewPosition].piece.positionOnBoard = rooksNewPosition;

            // Remove the piece from the old square.

            board.squares[rooksIndex].hasPiece = false;
            board.squares[rooksIndex].piece = null;

            // Move the piece gameobject.

            board.squares[rooksNewPosition].piece.transform.position = board.squares[rooksNewPosition].transform.position;
            
            hasMoved = true;
            board.squares[rooksNewPosition].piece.hasMoved = true;

            return true;
        }

        return false;
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

    public void AddMoves(int step,
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

                    // is the piece being defended?

                    if (board.IsPieceBeingDefended(isWhite, tempIndex) == false)
                    {
                        tempAttacks.Add(tempIndex);
                    }
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

    // ====================================

    public Lane AddDiagonalVulnerabilities(int step,
        Lane lane)
    {
        List<int> tempSquares = new List<int>();

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

        bool hasLaneInfo = false;

        // Used to check if pawns are in reach to attack.
        // Once depth goes beyond one enemy pawns are not close enough
        // Track depth of attacker and defender pieces

        int depth = 0;

        tempIndex += step;

        if (board.IsMoveInScope(tempIndex) == false)
        {
            lane.state = Lane.State.OffBoard;
        }

        // Increment/Decrement Row and Column

        while (board.IsMoveInScope(tempIndex))
        {
            depth++;

            // Use to correctly keep move in scope

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

                if (depth == 1)
                {
                    lane.state = Lane.State.OffBoard;
                }

                break;  // Break while loop
            }
            else
            {
                tempSquares.Add(tempIndex);

                if (hasLaneInfo == false)
                {
                    if (lane.state == Lane.State.Open)
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

                                lane.state = Lane.State.Defended;
                                lane.defenderDepth = depth;
                                lane.defender = tempIndex;
                            }
                            else
                            {
                                // Check if piece is Rook or Queen

                                if (board.squares[tempIndex].piece.type == Piece.Type.Bishop
                                    || board.squares[tempIndex].piece.type == Piece.Type.Queen
                                    ||
                                    (board.squares[tempIndex].piece.type == Piece.Type.Pawn
                                    && (((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Bishop
                                    || ((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Queen))
                                    )
                                {
                                    lane.state = Lane.State.Attacked;
                                    lane.attackerDepth = depth;
                                    lane.attacker = tempIndex;
                                    hasLaneInfo = true;
                                }
                                else
                                {
                                    // Check if piece is pawn and is close enough to attack

                                    if (board.squares[tempIndex].piece.type == Piece.Type.Pawn
                                        && depth == 1)
                                    {
                                        lane.state = Lane.State.Attacked;
                                        lane.attackerDepth = depth;
                                        lane.attacker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                    else
                                    {
                                        lane.state = Lane.State.Blocked;
                                        lane.blockerDepth = depth;
                                        lane.blocker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Lane is still open.

                            lane.state = Lane.State.Open;
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

                                lane.state = Lane.State.DoubleDefence;
                                lane.blockerDepth = depth;
                                lane.blocker = tempIndex;
                                hasLaneInfo = true;
                            }
                            else
                            {
                                // Is not our piece

                                if (lane.state == Lane.State.Defended)
                                {
                                    if (board.squares[tempIndex].piece.type == Piece.Type.Bishop
                                    || board.squares[tempIndex].piece.type == Piece.Type.Queen
                                    ||
                                    (board.squares[tempIndex].piece.type == Piece.Type.Pawn
                                    && (((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Bishop
                                    || ((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Queen))
                                    )
                                    {
                                        lane.state = Lane.State.DefendedAttack;
                                        lane.attackerDepth = depth;
                                        lane.attacker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                    else
                                    {
                                        lane.state = Lane.State.DefendedMove;
                                        lane.blockerDepth = depth;
                                        lane.blocker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Does not have piece
                            // Add to the move list.
                        }
                    }
                }
            }

            // Increment the position on the board by the step

            tempIndex += step;
        }

        lane.squares = tempSquares.ToArray();

        return lane;
    }

    public Lane AddVerticalVulnerabilities(int step,
        Lane lane)
    {
        List<int> tempSquares = new List<int>();

        // Both white and black will move the same way.
        // Search using step until you are off the board.

        int tempIndex = positionOnBoard;

        // Keeps the move in the scope of the board but doesn't stop wrap 
        // around of board (7 is edge of board but using +1 will wrap round)

        //float boardLength = Mathf.Sqrt(board.squares.Length);

        int row = board.GetRow(board.boardHeight, board.boardLength, tempIndex);
        int column = board.GetColumn(board.boardHeight, board.boardLength, tempIndex);

        bool hasLaneInfo = false;

        // Used to check if pawns are in reach to attack.
        // Once depth goes beyond one enemy pawns are not close enough
        // Track depth of attacker and defender pieces

        int depth = 0;

        tempIndex += step;

        while (board.IsMoveInScope(tempIndex))
        {
            depth++;

            int[] tempRowColumn = AdjustRowAndColumn(step, row, column);

            row = tempRowColumn[0];
            column = tempRowColumn[1];

            // If we have went onto a new column stop!

            if (row > board.boardHeight
                || row < 1)
            {
                if (depth == 1)
                {
                    lane.state = Lane.State.OffBoard;
                }

                // Stop the move from rapping around and being on a new column.

                break;  // Break out while loops
            }
            else
            {
                tempSquares.Add(tempIndex);

                if (hasLaneInfo == false)
                {
                    if (lane.state == Lane.State.Open)
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

                                lane.state = Lane.State.Defended;
                                lane.defenderDepth = depth;
                                lane.defender = tempIndex;
                            }
                            else
                            {
                                // Check if piece is Rook or Queen

                                if (board.squares[tempIndex].piece.type == Piece.Type.Rook
                                    || board.squares[tempIndex].piece.type == Piece.Type.Queen
                                    ||
                                    (board.squares[tempIndex].piece.type == Piece.Type.Pawn
                                    && (((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Rook
                                    || ((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Queen))
                                    )
                                {
                                    lane.state = Lane.State.Attacked;
                                    lane.attackerDepth = depth;
                                    lane.attacker = tempIndex;
                                    hasLaneInfo = true;
                                }
                                else
                                {
                                    lane.state = Lane.State.Blocked;
                                    lane.blockerDepth = depth;
                                    lane.blocker = tempIndex;
                                    hasLaneInfo = true;
                                }
                            }
                        }
                        else
                        {
                            // Lane is still open.

                            lane.state = Lane.State.Open;
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

                                lane.state = Lane.State.DoubleDefence;
                                hasLaneInfo = true;
                            }
                            else
                            {
                                // Is not our piece

                                if (lane.state == Lane.State.Defended)
                                {
                                    if (board.squares[tempIndex].piece.type == Piece.Type.Rook
                                    || board.squares[tempIndex].piece.type == Piece.Type.Queen
                                    ||
                                    (board.squares[tempIndex].piece.type == Piece.Type.Pawn
                                    && (((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Rook
                                    || ((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Queen))
                                    )
                                    {
                                        lane.state = Lane.State.DefendedAttack;
                                        lane.attackerDepth = depth;
                                        lane.attacker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                    else
                                    {
                                        lane.state = Lane.State.DefendedMove;
                                        lane.blockerDepth = depth;
                                        lane.blocker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Does not have piece
                            // Add to the move list.
                        }
                    }
                }
            }

            // Increment the position on the board by the step

            tempIndex += step;
        }

        lane.squares = tempSquares.ToArray();

        return lane;
    }

    public Lane AddHorizontalVulnerabilities(int step,
        Lane lane)
    {
        List<int> tempSquares = new List<int>();

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

        bool hasLaneInfo = false;

        // Used to check if pawns are in reach to attack.
        // Once depth goes beyond one enemy pawns are not close enough
        // Track depth of attacker and defender pieces

        int depth = 0;

        tempIndex += step;

        if (board.IsMoveInScope(tempIndex) == false)
        {
            lane.state = Lane.State.OffBoard;
        }

        while (board.IsMoveInScope(tempIndex))
        {
            depth++;

            int[] tempRowColumn = AdjustRowAndColumn(step, row, column);

            row = tempRowColumn[0];
            column = tempRowColumn[1];

            // If we have went onto a new row stop!

            if (column > board.boardLength
                || column < 1)
            {
                // WILL NEVER BE REACHED DUE THE NATURE OF board.IsMoveInScope() - controlling the while loop.
                //      - This is due to rap around at the top and bottom of the board is not as easily caught as the sides are.

                if (depth == 1)
                {
                    lane.state = Lane.State.OffBoard;
                }

                // Stop the move from rapping around and being on a new row.

                break;  // Break out while loop
            }
            else
            {
                tempSquares.Add(tempIndex);

                if (hasLaneInfo == false)
                {
                    if (lane.state == Lane.State.Open)
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

                                lane.state = Lane.State.Defended;
                                lane.defenderDepth = depth;
                                lane.defender = tempIndex;
                            }
                            else
                            {
                                // Check if piece is Rook or Queen

                                if (board.squares[tempIndex].piece.type == Piece.Type.Rook
                                || board.squares[tempIndex].piece.type == Piece.Type.Queen
                                ||
                                (board.squares[tempIndex].piece.type == Piece.Type.Pawn
                                && (((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Rook
                                || ((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Queen))
                                )
                                {
                                    lane.state = Lane.State.Attacked;
                                    lane.attackerDepth = depth;
                                    lane.attacker = tempIndex;
                                    hasLaneInfo = true;
                                }
                                else
                                {
                                    lane.state = Lane.State.Blocked;
                                    lane.blockerDepth = depth;
                                    lane.blocker = tempIndex;
                                    hasLaneInfo = true;
                                }
                            }
                        }
                        else
                        {
                            // Lane is still open.

                            lane.state = Lane.State.Open;
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

                                lane.state = Lane.State.DoubleDefence;
                                hasLaneInfo = true;
                            }
                            else
                            {
                                // Is not our piece

                                if (lane.state == Lane.State.Defended)
                                {
                                    if (board.squares[tempIndex].piece.type == Piece.Type.Rook
                                    || board.squares[tempIndex].piece.type == Piece.Type.Queen
                                    ||
                                    (board.squares[tempIndex].piece.type == Piece.Type.Pawn
                                    && (((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Rook
                                    || ((Pawn)board.squares[tempIndex].piece).promotionType == Piece.Type.Queen))
                                    )
                                    {
                                        lane.state = Lane.State.DefendedAttack;
                                        lane.attackerDepth = depth;
                                        lane.attacker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                    else
                                    {
                                        lane.state = Lane.State.DefendedMove;
                                        lane.blockerDepth = depth;
                                        lane.blocker = tempIndex;
                                        hasLaneInfo = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Does not have piece
                            // Add to the move list.
                        }
                    }
                }
            }

            // Increment the position on the board by the step

            tempIndex += step;
        }

        lane.squares = tempSquares.ToArray();

        return lane;
    }
}