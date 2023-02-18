using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public Main main = null;

    public Vector2 mousePosition = Vector2.zero;

    public Vector3 mouseWorldPosition = Vector3.zero;

    public Piece hoverOver = null;

    public Square selectedSquare = null;

    public Square moveSquare = null;

    public Piece selectedPiece = null;

    public Vector3 selectedPieceDefaultPosition = Vector3.zero;

    public int moveSelected = 0;

    public void Awake()
    {
        main = GetComponent<Main>();
    }

    public void ControlledUpdate()
    {
        if (selectedPiece != null)
        {
            // Make piece follow cursor.

            selectedPiece.transform.position = mouseWorldPosition;
        }

        // Update screen position.

        mousePosition = Input.mousePosition;

        // Update world position

        var v3 = Input.mousePosition;
        //v3.y = 0.1f;
        //v3.z = -Camera.main.transform.position.z;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(v3);

        mouseWorldPosition.y = 0.1f;

        //if (!main.player.hasTurn) return;

        // Hover over ============================

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Square")
            {
                if (hit.transform.GetComponent<Square>().hasPiece)
                {
                    hoverOver = hit.transform.GetComponent<Square>().piece;
                }
                else
                {
                    hoverOver = null;
                }
            }
            else
            {
                hoverOver = null;
            }
        }

        // Selection =============================

        if (Input.GetMouseButtonDown(0)
            && main.game.step == (main.game.progress.Count - 1)
            && main.userInterface.promotablePawn == null
            && main.evaluator.evaluationState.isEvaluating == false)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Square")
                {
                    Debug.Log("Hit Square = " + hit.transform.gameObject.name);

                    if (selectedSquare == null)
                    {
                        selectedSquare = hit.transform.GetComponent<Square>();

                        if (selectedSquare.hasPiece)
                        {
                            selectedPiece = selectedSquare.piece;

                            selectedPieceDefaultPosition = selectedPiece.transform.position;
                        }
                        else
                        {
                            selectedPiece = null;
                        }
                    }
                    else
                    {
                        if (selectedPiece != null)
                        {
                            // Looking for move square

                            // Check if the square selected for move
                            // index is in the piece move/attack lists

                            Square hitSquare = hit.transform.GetComponent<Square>();

                            if (hitSquare.index == selectedSquare.index)
                            {
                                // trying to deselect piece/square
                                selectedPiece.transform.position = selectedPieceDefaultPosition;
                                selectedSquare = null;
                                selectedPiece = null;
                                moveSquare = null;
                                selectedPieceDefaultPosition = Vector3.zero;

                                return;
                            }

                            // Below are important checks to keep the game in sync and legal.

                            if (main.userInterface.promotablePawn == null
                                && selectedPiece.HasMove(hitSquare.index))
                            {
                                // Check are we currently look back at previous moves?
                                // if not perform move.

                                if (main.game.step == (main.game.progress.Count - 1))
                                {
                                    moveSquare = hitSquare;

                                    selectedPiece.Move(hitSquare.index);

                                    main.game.board.RemoveOldEnPassantMove(!selectedPiece.isWhite);

                                    // Need to update all pieces for the new move.
                                    main.game.board.UpdateAllPieceMoves();

                                    ((King)main.game.board.whiteKing).KingUniqueMoves();
                                    ((King)main.game.board.blackKing).KingUniqueMoves();

                                    main.game.board.EvaluateLegalityOfAllMoves();

                                    // Must be done last after everything else.

                                    if(((King)main.game.board.whiteKing).inCheck)
                                    {
                                        ((King)main.game.board.whiteKing).GetSupportingPieces();
                                        ((King)main.game.board.whiteKing).CheckForCheckMate();
                                    }
                                    if (((King)main.game.board.blackKing).inCheck)
                                    {
                                        ((King)main.game.board.blackKing).GetSupportingPieces();
                                        ((King)main.game.board.blackKing).CheckForCheckMate();
                                    }

                                    if (main.userInterface.promotablePawn == null)
                                    {
                                        BoardData boardData = main.evaluator.Begin(main.game.board);
                                        main.game.UpdateBoardData(boardData);
                                    }

                                    //main.game.UpdateBoardData(new BoardData(main.game.board.pieces, main.game.board.squares));
                                }

                                // Deselect piece/square
                                selectedSquare = null;
                                selectedPiece = null;
                                moveSquare = null;
                                selectedPieceDefaultPosition = Vector3.zero;
                            }
                            else
                            {
                                selectedSquare = hit.transform.GetComponent<Square>();

                                if (selectedSquare.hasPiece)
                                {
                                    if (selectedPiece != null)
                                    {
                                        selectedPiece.transform.position = selectedPieceDefaultPosition;
                                    }
                                    selectedPiece = selectedSquare.piece;
                                    selectedPieceDefaultPosition = selectedPiece.transform.position;
                                    moveSquare = null;
                                }
                                else
                                {
                                    if (selectedPiece != null)
                                    {
                                        selectedPiece.transform.position = selectedPieceDefaultPosition;
                                    }
                                    selectedPiece = null;
                                    moveSquare = null;
                                    selectedPieceDefaultPosition = Vector3.zero;
                                }
                            }
                        }
                        else
                        {
                            selectedSquare = hit.transform.GetComponent<Square>();
                            
                            if (selectedSquare.hasPiece)
                            {
                                selectedPiece = selectedSquare.piece;
                                selectedPieceDefaultPosition = selectedPiece.transform.position;
                                moveSquare = null;
                            }
                            else
                            {
                                if (selectedPiece != null)
                                {
                                    selectedPiece.transform.position = selectedPieceDefaultPosition;
                                }
                                selectedPiece = null;
                                moveSquare = null;
                                selectedPieceDefaultPosition = Vector3.zero;
                            }
                        }
                    }
                }
            }
        }
    }
}
