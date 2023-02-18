using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Main main = null;

    public bool whitesTurn = true;
    public bool playerIsWhite = true;

    public bool play = false;
    public bool forward = false;
    public bool backward = false;
    public bool start = false;
    public int step = 0;

    // Board containing all the square components

    public Board board = null;

    //public BoardData boardData;

    public List<BoardData> progress = new List<BoardData>();

    public bool TEST = false;
    public int TEST_INDEX = 0;

    // Prefabs - GameObjects 
    // White Pieces

    public GameObject whitePawnPrefab = null;
    public GameObject whiteRookPrefab = null;
    public GameObject whiteBishopPrefab = null;
    public GameObject whiteKnightPrefab = null;
    public GameObject whiteQueenPrefab = null;
    public GameObject whiteKingPrefab = null;

    // Black Pieces

    public GameObject blackPawnPrefab = null;
    public GameObject blackRookPrefab = null;
    public GameObject blackBishopPrefab = null;
    public GameObject blackKnightPrefab = null;
    public GameObject blackQueenPrefab = null;
    public GameObject blackKingPrefab = null;

    // Squares

    public GameObject whiteSquarePrefab = null;
    public GameObject blackSquarePrefab = null;

    // Sprites
    // White Pieces

    public Sprite whitePawnSprite = null;
    public Sprite whiteRookSprite = null;
    public Sprite whiteBishopSprite = null;
    public Sprite whiteKnightSprite = null;
    public Sprite whiteQueenSprite = null;
    public Sprite whiteKingSprite = null;

    // Black Pieces

    public Sprite blackPawnSprite = null;
    public Sprite blackRookSprite = null;
    public Sprite blackBishopSprite = null;
    public Sprite blackKnightSprite = null;
    public Sprite blackQueenSprite = null;
    public Sprite blackKingSprite = null;

    Dictionary<int, Piece.Type> pieceTypes
        = new Dictionary<int, Piece.Type>();

    Dictionary<int, bool> pieceWhite
        = new Dictionary<int, bool>();

    void Awake()
    {
        this.main = this.GetComponent<Main>();

        LoadResources();

        CreatePieceTypeDictionary();
        CreatePieceWhiteDictionary();

        // TEMP =======

        board = new Board(8, 8)
        {
            whiteKing = GameObject.Find("WhiteKing").GetComponent<King>(),
            blackKing = GameObject.Find("BlackKing").GetComponent<King>()
        };

        GameObject[] squareGameObjects = GameObject.FindGameObjectsWithTag("Square");

        board.squares = new Square[squareGameObjects.Length];

        for (int i = 0; i < squareGameObjects.Length; i++)
        {
            Square square = squareGameObjects[i].GetComponent<Square>();

            board.squares[square.index] = square;

            //board.squares[i].index = i;
        }

        GameObject[] pieceGameObjects = GameObject.FindGameObjectsWithTag("Piece");

        board.pieces = new Piece[pieceGameObjects.Length];

        for (int i = 0; i < pieceGameObjects.Length; i++)
        {
            Piece piece = pieceGameObjects[i].GetComponent<Piece>();
            piece.id = i;
            piece.board = board;
            board.pieces[piece.id] = piece;
        }

        // =====================

        UpdateBoardData(new BoardData(main.game.board.pieces, main.game.board.squares));
    }

    public void Update()
    {
        if (forward)
        {
            forward = false;
            if(step < (progress.Count - 1)) step++;
            LoadBoardData(step);
        }

        if (backward)
        {
            backward = false;
            if(step > 0) step--;
            LoadBoardData(step);
        }

        if (play)
        {
            play = false;
            step = progress.Count - 1;
            LoadBoardData(step);
        }

        if (start)
        {
            start = false;
            step = 0;
            LoadBoardData(step);
        }

        if (TEST)
        {
            TEST = false;

            //pieces = new Piece[32];
            //squares = new Square[64];

            List<Piece> tempPiece = new List<Piece>();

            while (TEST_INDEX < 64)
            {
                // Square

                int column = board.GetColumn(8, 8, TEST_INDEX);
                int row = board.GetRow(8, 8, TEST_INDEX);
                string squareName = GetSquareLetterName(8, TEST_INDEX) + GetSquareNumberName(8, TEST_INDEX);
                bool isSquareWhite = ShouldSquareBeWhite(column, TEST_INDEX);
                Vector3 squarePosition = board.GetSquarePosition(row, column);

                bool hasPiece = board.DoesSquareHavePiece(TEST_INDEX);
                bool isPieceWhite = false;
                Piece.Type pieceType = Piece.Type.Pawn;

                string pieceOutputStringDebug = "";

                if (hasPiece)
                {
                    pieceType = pieceTypes[TEST_INDEX];
                    isPieceWhite = pieceWhite[TEST_INDEX];
                    pieceOutputStringDebug = " Piece Type = " + pieceTypes[TEST_INDEX].ToString();
                    pieceOutputStringDebug += " Piece White = " + isPieceWhite.ToString();

                    // CHECK IF PIECE TYPE IS KING!!!
                    // IF SO SAVE THE KING TO EITHER WHITE OR BLACK - BOARD.WHITEKING - BOARD.BLACKKING
                }

                Debug.Log(squareName
                    + " (Row = " + row.ToString() + " Column = " + column.ToString()
                    + ") Is White = " + isSquareWhite.ToString()
                    + " At position = " + squarePosition.ToString()
                    + " Has Piece = " + hasPiece.ToString()
                    + pieceOutputStringDebug);

                TEST_INDEX++;

                /*
                // Set Square Variables

                Square square = CreateSquare(TEST_INDEX, squareName, isSquareWhite, row, column, squarePosition);

                if (hasPiece)
                {
                    // Set Piece Variables

                    Piece piece = CreatePiece(TEST_INDEX, "Debug", pieceType, isPieceWhite, squarePosition);

                    // Create link between piece and square.

                    square.hasPiece = true;
                    square.piece = piece;

                    tempPieces.Add(piece);
                }

                square[TEST_INDEX] = square;
                */
            }

            // Pieces
            board.pieces = tempPiece.ToArray();
        }
    }

    public void UpdateBoardData(BoardData board)
    {
        progress.Add(board);
        step = progress.Count - 1;
    }

    public void LoadBoardData(int index)
    {
        for (int i = 0; i < progress[index].pieceData.Length; i++)
        {
            board.pieces[i].positionOnBoard = progress[index].pieceData[i].square;

            // Set is active
            
            board.pieces[i].gameObject.SetActive(progress[index].pieceData[i].isActive);
            board.pieces[i].hasMoved = progress[index].pieceData[i].hasMoved;

            // Move the piece to the square.

            board.pieces[i].transform.position = board.squares[board.pieces[i].positionOnBoard].transform.position;

            // Pawns promotion/demotion information

            if (progress[index].pieceData[i].type == Piece.Type.Pawn)
            {
                // Are we promoted? Promote

                if (progress[index].pieceData[i].isPromoted)
                {
                    ((Pawn)board.pieces[i]).Promote(progress[index].pieceData[i].promotionType);
                }
                else    // if not promoted? - Demote!
                {
                    ((Pawn)board.pieces[i]).Demote();
                }
            }
        }

        for (int i = 0; i < progress[index].squareData.Length; i++)
        {
            board.squares[i].hasPiece = progress[index].squareData[i].hasPiece;

            if (board.squares[i].hasPiece) board.squares[i].piece = board.pieces[progress[index].squareData[i].piece];
            else board.squares[i].piece = null;

            board.squares[i].hasEnPassant = progress[index].squareData[i].hasEnPassant;

            if (board.squares[i].hasEnPassant) board.squares[i].enPassant = board.pieces[progress[index].squareData[i].enPassant];
            else board.squares[i].enPassant = null;
        }

        board.UpdateAllPieceMoves();

        ((King)board.whiteKing).KingUniqueMoves();
        ((King)board.blackKing).KingUniqueMoves();

        board.EvaluateLegalityOfAllMoves();

        // Must be done last after everything else.

        if (((King)main.game.board.whiteKing).inCheck)
        {
            ((King)main.game.board.whiteKing).GetSupportingPieces();
            ((King)main.game.board.whiteKing).CheckForCheckMate();
        }
        if (((King)main.game.board.blackKing).inCheck)
        {
            ((King)main.game.board.blackKing).GetSupportingPieces();
            ((King)main.game.board.blackKing).CheckForCheckMate();
        }
    }

    public void LoadResources()
    {
        // GameObjects
        // White Piece

        whitePawnPrefab = Resources.Load<GameObject>("Prefabs/WhitePawn");
        whiteRookPrefab = Resources.Load<GameObject>("Prefabs/WhiteRook");
        whiteBishopPrefab = Resources.Load<GameObject>("Prefabs/WhiteBishop");
        whiteKnightPrefab = Resources.Load<GameObject>("Prefabs/WhiteKnight");
        whiteQueenPrefab = Resources.Load<GameObject>("Prefabs/WhiteQueen");
        whiteKingPrefab = Resources.Load<GameObject>("Prefabs/WhiteKing");

        // Black Pieces

        blackPawnPrefab = Resources.Load<GameObject>("Prefabs/BlackPawn");
        blackRookPrefab = Resources.Load<GameObject>("Prefabs/BlackRook");
        blackBishopPrefab = Resources.Load<GameObject>("Prefabs/BlackBishop");
        blackKnightPrefab = Resources.Load<GameObject>("Prefabs/BlackKnight");
        blackQueenPrefab = Resources.Load<GameObject>("Prefabs/BlackQueen");
        blackKingPrefab = Resources.Load<GameObject>("Prefabs/BlackKing");

        // Squares

        whiteSquarePrefab = Resources.Load<GameObject>("Prefabs/WhiteSquare");
        blackSquarePrefab = Resources.Load<GameObject>("Prefabs/BlackSquare");

        // Sprites
        // White Pieces

        whitePawnSprite = Resources.Load<Sprite>("Prefabs/WhitePawnSprite");
        whiteRookSprite = Resources.Load<Sprite>("Prefabs/WhiteRookSprite");
        whiteBishopSprite = Resources.Load<Sprite>("Prefabs/WhiteBishopSprite");
        whiteKnightSprite = Resources.Load<Sprite>("Prefabs/WhiteKnightSprite");
        whiteQueenSprite = Resources.Load<Sprite>("Prefabs/WhiteQueenSprite");
        whiteKingSprite = Resources.Load<Sprite>("Prefabs/WhiteKingSprite");

        // Black Pieces

        blackPawnSprite = Resources.Load<Sprite>("Prefabs/BlackPawnSprite");
        blackRookSprite = Resources.Load<Sprite>("Prefabs/BlackRookSprite");
        blackBishopSprite = Resources.Load<Sprite>("Prefabs/BlackBishopSprite");
        blackKnightSprite = Resources.Load<Sprite>("Prefabs/BlackKnightSprite");
        blackQueenSprite = Resources.Load<Sprite>("Prefabs/BlackQueenSprite");
        blackKingSprite = Resources.Load<Sprite>("Prefabs/BlackKingSprite");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numberOfRows"> The number of ranks the board should have </param>
    /// <param name="numberOfColumns"> The number of files the board should have </param>

    public void GenerateBoard(int numberOfRows, int numberOfColumns)
    {
        // LOOK IN UPDATE. DEBUGGING. <<=========================================================

        // Implemented currently in the unity scene and objects are found for script access.

        // Assumed no piece or board exist.

        // Default board

        // 8 x 8 (64) squares created.

        //  - Name. (use rows(ranks) and columns(files) for name selection)
        //  - positon in scene.
        //  - Set index.
        //  - hasPiece = false
        //  - piece = null;
        //  - hasEnPassant = false;
        //  - enPassant = null;
        //  - Set the correct material.

        // Create Pieces
    }

    public Square CreateSquare(int index, string name, bool isWhite, int row, int column, Vector3 position)
    {
        GameObject go = null;

        if (isWhite)
        {
            go = GameObject.Instantiate(whiteSquarePrefab, position, Quaternion.identity, this.transform);
        }
        else
        {
            go = GameObject.Instantiate(blackSquarePrefab, position, Quaternion.identity, this.transform);
        }

        go.name = name;

        Square square = go.GetComponent<Square>();

        // Set square

        square.index = index;
        square.isWhite = isWhite;
        square.row = row;
        square.column = column;

        return square;
    }

    public Piece CreatePiece(int index, string name, Piece.Type type, bool isWhite, Vector3 position)
    {
        GameObject go = null;

        if (isWhite)
        {
            switch (type)
            {
                case Piece.Type.Pawn:

                    go = GameObject.Instantiate(whitePawnPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Rook:

                    go = GameObject.Instantiate(whiteRookPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Bishop:

                    go = GameObject.Instantiate(whiteBishopPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Knight:

                    go = GameObject.Instantiate(whiteKnightPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Queen:

                    go = GameObject.Instantiate(whiteQueenPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.King:

                    go = GameObject.Instantiate(whiteKingPrefab, position, Quaternion.identity, this.transform);
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case Piece.Type.Pawn:

                    go = GameObject.Instantiate(blackPawnPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Rook:

                    go = GameObject.Instantiate(blackRookPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Bishop:

                    go = GameObject.Instantiate(blackBishopPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Knight:

                    go = GameObject.Instantiate(blackKnightPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.Queen:

                    go = GameObject.Instantiate(blackQueenPrefab, position, Quaternion.identity, this.transform);
                    break;

                case Piece.Type.King:

                    go = GameObject.Instantiate(blackQueenPrefab, position, Quaternion.identity, this.transform);
                    break;
            }
        }

        go.name = name;

        Piece piece = go.GetComponent<Piece>();

        piece.positionOnBoard = index;
        piece.type = type;
        piece.isWhite = isWhite;

        return piece;
    }

    public void ResetBoard()
    {
        // Assumed the board is in default set up.
        // reuse piece and square objects.
    }

    public string GetSquareLetterName(int columns, int index)
    {
        float i = index;

        string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h",
            "i", "j", "k", "l", "m", "n", "o", "p", "q",
            "r", "s", "t", "u", "v", "w", "x", "y", "z"};

        // handle if index = 0;

        if (index == 0)
        {
            return letters[0];
        }

        // index / column THEN (round the number to whole number) = row its on

        i /= columns;

        return letters[(int)Mathf.Floor(i)];
    }

    public int GetSquareNumberName(int columns, int index)
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

    public bool ShouldSquareBeWhite(int column, int index)
    {
        // First square is always white.

        if (index == 0) return true;

        // Use modulus operator

        if (column % 2 == 0)
        {
            if (index % 2 == 0)
            {
                // even - white
                return false;
            }
            else
            {
                // odd - black
                return true;
            }
        }
        else
        {
            if (index % 2 == 0)
            {
                // even - white
                return true;
            }
            else
            {
                // odd - black
                return false;
            }
        }
    }

    /// <summary>
    /// Used to decide using the current index what type of piece should be used.
    /// </summary>

    public void CreatePieceTypeDictionary()
    {
        // White back row

        pieceTypes.Add(0, Piece.Type.Rook);     //1
        pieceTypes.Add(8, Piece.Type.Knight);
        pieceTypes.Add(16, Piece.Type.Bishop);
        pieceTypes.Add(24, Piece.Type.Queen);
        pieceTypes.Add(32, Piece.Type.King);
        pieceTypes.Add(40, Piece.Type.Bishop);  // 2
        pieceTypes.Add(48, Piece.Type.Knight);
        pieceTypes.Add(56, Piece.Type.Rook);

        // White Pawn row

        pieceTypes.Add(1, Piece.Type.Pawn);     // 1
        pieceTypes.Add(9, Piece.Type.Pawn);
        pieceTypes.Add(17, Piece.Type.Pawn);
        pieceTypes.Add(25, Piece.Type.Pawn);
        pieceTypes.Add(33, Piece.Type.Pawn);
        pieceTypes.Add(41, Piece.Type.Pawn);
        pieceTypes.Add(49, Piece.Type.Pawn);
        pieceTypes.Add(57, Piece.Type.Pawn);    // 8

        // Black Pawn row

        pieceTypes.Add(6, Piece.Type.Pawn);     // 8
        pieceTypes.Add(14, Piece.Type.Pawn);
        pieceTypes.Add(22, Piece.Type.Pawn);
        pieceTypes.Add(30, Piece.Type.Pawn);
        pieceTypes.Add(38, Piece.Type.Pawn);
        pieceTypes.Add(46, Piece.Type.Pawn);
        pieceTypes.Add(54, Piece.Type.Pawn);
        pieceTypes.Add(62, Piece.Type.Pawn);    //  1

        // Black back row

        pieceTypes.Add(7, Piece.Type.Rook);     //  2
        pieceTypes.Add(15, Piece.Type.Knight);
        pieceTypes.Add(23, Piece.Type.Bishop);
        pieceTypes.Add(31, Piece.Type.Queen);
        pieceTypes.Add(39, Piece.Type.King);
        pieceTypes.Add(47, Piece.Type.Bishop);  // 1
        pieceTypes.Add(55, Piece.Type.Knight);
        pieceTypes.Add(63, Piece.Type.Rook);
    }

    /// <summary>
    /// Used to decide using the current index should the piece be white.
    /// </summary>

    public void CreatePieceWhiteDictionary()
    {
        // White back row

        pieceWhite.Add(0, true);    //1
        pieceWhite.Add(8, true);
        pieceWhite.Add(16, true);
        pieceWhite.Add(24, true);
        pieceWhite.Add(32, true);
        pieceWhite.Add(40, true);   // 2
        pieceWhite.Add(48, true);
        pieceWhite.Add(56, true);

        // White Pawn row

        pieceWhite.Add(1, true);    // 1
        pieceWhite.Add(9, true);
        pieceWhite.Add(17, true);
        pieceWhite.Add(25, true);
        pieceWhite.Add(33, true);
        pieceWhite.Add(41, true);
        pieceWhite.Add(49, true);
        pieceWhite.Add(57, true);   // 8

        // Black Pawn row

        pieceWhite.Add(6, false);   // 8
        pieceWhite.Add(14, false);
        pieceWhite.Add(22, false);
        pieceWhite.Add(30, false);
        pieceWhite.Add(38, false);
        pieceWhite.Add(46, false);
        pieceWhite.Add(54, false);
        pieceWhite.Add(62, false);  //  1

        // Black back row

        pieceWhite.Add(7, false);   //  2
        pieceWhite.Add(15, false);
        pieceWhite.Add(23, false);
        pieceWhite.Add(31, false);
        pieceWhite.Add(39, false);
        pieceWhite.Add(47, false);  // 1
        pieceWhite.Add(55, false);
        pieceWhite.Add(63, false);
    }

    /// <summary>
    /// Using the current index should there be a piece on this square
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>

    public bool ShouldSquareHavePiece(int index)
    {
        // Index for all pieces
        int[] pieceIndexes =
        {
            0,  // White Rook 1
            8,  // White Knight 1
            16, // White Bishop 1
            24, // White Queen
            32, // White King
            40, // White Bishop 2
            48, // White Knight 2
            56, // White Rook 2

            1,  // White Pawn 1
            9,  // White Pawn 2
            17, // White Pawn 3
            25, // White Pawn 4
            33, // White Pawn 5
            41, // White Pawn 6
            49, // White Pawn 7
            57, // White Pawn 8

            6,  // Black Pawn 8
            14, // Black Pawn 7
            22, // Black Pawn 6
            30, // Black Pawn 5
            38, // Black Pawn 4
            46, // Black Pawn 3
            54, // Black Pawn 2
            62, // Black Pawn 1

            7,  // Black Rook 2
            15, // Black Knight 2
            23, // Black Bishop 2
            31, // Black Queen
            39, // Black King
            47, // Black Bishop 1
            55, // Black Knight 1
            63  // Black Rook 1
        };

        for (int i = 0; i < pieceIndexes.Length; i++)
        {
            if (pieceIndexes[i] == index) return true;
        }

        return false;
    }
}
