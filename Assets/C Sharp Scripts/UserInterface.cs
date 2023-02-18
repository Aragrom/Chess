using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
	private Main main = null;

    public enum State { MainMenu, PlayAI, PlayNetwork, PeerToPeer, Host, Join, Options, InGame }

    public State state = State.MainMenu;

    private float menuButtonWidth = 300;
	private float menuButtonHeight = 30;

	public float squareScreenWidth = 0;
	public float squareScreenHeight = 0;

	public GUILayoutOption[] menuLayoutOptions = null;
	public GUIStyle guiStyle = null;
	public GUIStyle greyGuiStyle = null;
	public GUIStyle darkGreyGuiStyle = null;

	public Texture2D redTexture2D = null;
	public Texture2D yellowTexture2D = null;
	public Texture2D orangeTexture2D = null;
	public Texture2D greenTexture2D = null;
	public Texture2D blueTexture2D = null;
	public Texture2D purpleTexture2D = null;
	public Texture2D whiteTexture2D = null;
	public Texture2D blackTexture2D = null;

	public Texture2D redSquareHighlight = null;
	public Texture2D yellowSquareHighlight = null;
	public Texture2D orangeSquareHighlight = null;
	public Texture2D greenSquareHighlight = null;
	public Texture2D blueSquareHighlight = null;
	public Texture2D purpleSquareHighlight = null;
	public Texture2D whiteSquareHighlight = null;
	public Texture2D blackSquareHighlight = null;

	public Texture2D playButton = null;
	public Texture2D fastForwardButton = null;
	public Texture2D rewindButton = null;
	public Texture2D playFromBeginningButton = null;

	public Pawn promotablePawn = null;

	void Awake() 
	{
		main = GetComponent<Main>();

		menuLayoutOptions = new GUILayoutOption[]
		 {
			 GUILayout.Width(menuButtonWidth),
			 GUILayout.Height(menuButtonHeight),
             //add more layout options
         };
	}

    private void OnGUI()
    {
		guiStyle = new GUIStyle(GUI.skin.label);
		guiStyle.alignment = TextAnchor.LowerRight;

		greyGuiStyle = new GUIStyle(GUI.skin.label);
		greyGuiStyle.alignment = TextAnchor.LowerRight;
		greyGuiStyle.normal.textColor = new Color(26.0f / 255.0f, 26.0f / 255.0f, 26.0f / 255.0f, 1);
		//greyGuiStyle.normal.textColor = Color.white;

		darkGreyGuiStyle = new GUIStyle(GUI.skin.label);
		darkGreyGuiStyle.alignment = TextAnchor.LowerRight;
		darkGreyGuiStyle.normal.textColor = new Color(63.0f / 255.0f, 63.0f / 255.0f, 63.0f / 255.0f, 1);

		CalculateSquareScreenDimensions();

		DisplayStats();

		if (promotablePawn != null)
		{
			DisplayPromotionOptions();
		}

		DisplayMoves(); // Render first so its the most bottom layer and all other menus ontop

		DisplaySquareInfo();

		DisplayStepControls();

		switch (state)
        {
            case State.MainMenu:

                MainMenu();

                break;

            case State.PlayAI:

                PlayAI();

                break;

            case State.PlayNetwork:

                PlayNetwork();

                break;

			case State.PeerToPeer:

				PeerToPeer();

				break;

			case State.Host:

				Host();

				break;

			case State.Join:

				Join();

				break;

			case State.Options:

                Options();

                break;

            case State.InGame:

                InGame();

                break;

            default:

                Debug.Log("UserInterface.OnGUI() - Default switch statement reached!");

                break;
        }

		DisplayEvaluator();

		if (!show)
		{
			return;
		}

		windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
	}

	void DisplayEvaluator()
	{
		if (main.evaluator.evaluationState.isEvaluating)
		{
			GUILayout.BeginVertical(menuLayoutOptions);

			GUILayout.Label("EVALUATING...");

			GUILayout.EndVertical();
		}
	}

	void CalculateSquareScreenDimensions()
	{ 
		// Get screen positon of two squares.

		Vector3 squareA = Camera.main.WorldToScreenPoint(main.game.board.squares[0].transform.position);
		Vector3 squareB = Camera.main.WorldToScreenPoint(main.game.board.squares[1].transform.position);

		squareScreenWidth = squareB.y - squareA.y;
		squareScreenHeight = squareB.y - squareA.y;
	}

	public void DisplayStats()
	{
		GUILayout.BeginVertical(menuLayoutOptions);

		GUILayout.Label("FPS:" + main.fps);
		GUILayout.Label("Instant FPS:" + main.instantFps);

		GUILayout.EndVertical();
	}

	public void DisplayPromotionOptions()
	{
		GUILayout.BeginVertical(menuLayoutOptions);

		if (GUILayout.Button("Queen")
			&& main.evaluator.evaluationState.isEvaluating == false)
		{
			promotablePawn.Promote(Piece.Type.Queen);
			promotablePawn.hasBeenPromoted = true;
			promotablePawn = null;

			// Need to update all pieces for the new move.
			main.game.board.UpdateAllPieceMoves();

			((King)main.game.board.whiteKing).KingUniqueMoves();
			((King)main.game.board.blackKing).KingUniqueMoves();

			main.game.board.EvaluateLegalityOfAllMoves();

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

			BoardData boardData = main.evaluator.Begin(main.game.board);
			main.game.UpdateBoardData(boardData);
		}

		if (GUILayout.Button("Rook")
			&& main.evaluator.evaluationState.isEvaluating == false)
		{
			promotablePawn.Promote(Piece.Type.Rook);
			promotablePawn.hasBeenPromoted = true;
			promotablePawn = null;

			// Need to update all pieces for the new move.
			main.game.board.UpdateAllPieceMoves();

			((King)main.game.board.whiteKing).KingUniqueMoves();
			((King)main.game.board.blackKing).KingUniqueMoves();

			main.game.board.EvaluateLegalityOfAllMoves();

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

			BoardData boardData = main.evaluator.Begin(main.game.board);
			main.game.UpdateBoardData(boardData);
		}

		if (GUILayout.Button("Bishop")
			&& main.evaluator.evaluationState.isEvaluating == false)
		{
			promotablePawn.Promote(Piece.Type.Bishop);
			promotablePawn.hasBeenPromoted = true;
			promotablePawn = null;

			// Need to update all pieces for the new move.
			main.game.board.UpdateAllPieceMoves();

			((King)main.game.board.whiteKing).KingUniqueMoves();
			((King)main.game.board.blackKing).KingUniqueMoves();

			main.game.board.EvaluateLegalityOfAllMoves();

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

			BoardData boardData = main.evaluator.Begin(main.game.board);
			main.game.UpdateBoardData(boardData);
		}

		if (GUILayout.Button("Knight")
			&& main.evaluator.evaluationState.isEvaluating == false)
		{
			promotablePawn.Promote(Piece.Type.Knight);
			promotablePawn.hasBeenPromoted = true;
			promotablePawn = null;

			// Need to update all pieces for the new move.
			main.game.board.UpdateAllPieceMoves();

			((King)main.game.board.whiteKing).KingUniqueMoves();
			((King)main.game.board.blackKing).KingUniqueMoves();

			main.game.board.EvaluateLegalityOfAllMoves();

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

			BoardData boardData = main.evaluator.Begin(main.game.board);
			main.game.UpdateBoardData(boardData);
		}

		GUILayout.EndVertical();
	}

	private void DisplayStepControls()
	{
		GUILayout.BeginVertical(menuLayoutOptions);

		GUILayout.BeginHorizontal(menuLayoutOptions);

		if (GUILayout.Button(playFromBeginningButton)
			&& promotablePawn == null)
		{
			if (main.mouse.selectedPiece != null)
			{
				main.mouse.selectedPiece.transform.position = main.mouse.selectedPieceDefaultPosition;
				main.mouse.selectedPiece = null;
				main.mouse.selectedSquare = null;
			}

			main.game.step = 0;
			main.game.LoadBoardData(main.game.step);
		}

		if (GUILayout.Button(rewindButton)
			&& promotablePawn == null)
		{
			if (main.mouse.selectedPiece != null)
			{
				main.mouse.selectedPiece.transform.position = main.mouse.selectedPieceDefaultPosition;
				main.mouse.selectedPiece = null;
				main.mouse.selectedSquare = null;
			}

			if (main.game.step > 0) main.game.step--;
			main.game.LoadBoardData(main.game.step);
		}

		if (GUILayout.Button(playButton)
			&& promotablePawn == null)
		{
			if (main.mouse.selectedPiece != null)
			{
				main.mouse.selectedPiece.transform.position = main.mouse.selectedPieceDefaultPosition;
				main.mouse.selectedPiece = null;
				main.mouse.selectedSquare = null;
			}

			main.game.step = main.game.progress.Count - 1;
			main.game.LoadBoardData(main.game.step);
		}

		if (GUILayout.Button(fastForwardButton)
			&& promotablePawn == null)
		{
			if (main.mouse.selectedPiece != null)
			{
				main.mouse.selectedPiece.transform.position = main.mouse.selectedPieceDefaultPosition;
				main.mouse.selectedPiece = null;
				main.mouse.selectedSquare = null;
			}

			if (main.game.step < (main.game.progress.Count - 1)) main.game.step++;
			main.game.LoadBoardData(main.game.step);
		}

		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	private void DisplayMoves()
	{
		if (main.mouse.selectedPiece != null)
		{
			for (int i = 0; i < main.mouse.selectedPiece.moves.Length; i++)
			{
				Vector3 worldPosition = main.game.board.GetSquarePosition(main.mouse.selectedPiece.moves[i]);
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
				GUI.DrawTexture(new Rect(
					(screenPositon.x - (squareScreenWidth / 2)),
					(Screen.height - screenPositon.y - (squareScreenHeight / 2)),
					squareScreenWidth,
					squareScreenHeight),
					greenSquareHighlight);
			}

			/*for (int i = 0; i < main.mouse.selectedPiece.defending.Length; i++)
			{
				Vector3 worldPosition = main.game.board.GetSquarePosition(main.mouse.selectedPiece.defending[i]);
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
				GUI.DrawTexture(new Rect(
					(screenPositon.x - (squareScreenWidth / 2)),
					(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
					squareScreenWidth,
					squareScreenHeight),
					blueSquareHighlight);
			}*/

			for (int i = 0; i < main.mouse.selectedPiece.attacks.Length; i++)
			{
				Vector3 worldPosition = main.game.board.GetSquarePosition(main.mouse.selectedPiece.attacks[i]);
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
				GUI.DrawTexture(new Rect(
					(screenPositon.x - (squareScreenWidth / 2)),
					(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
					squareScreenWidth,
					squareScreenHeight),
					orangeSquareHighlight);
			}

			switch (main.mouse.selectedPiece.type)
			{
				case Piece.Type.Pawn:

					Pawn pawn = (Pawn)main.mouse.selectedPiece;

					for (int i = 0; i < pawn.enPassantAttacks.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.enPassantAttacks[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							redSquareHighlight);
					}

					/*if (pawn.isPromoted)
					{
						for (int i = 0; i < pawn.discoverablesAttacks.Length; i++)
						{
							Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.discoverablesAttacks[i]);
							Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
							GUI.DrawTexture(new Rect(
								(screenPositon.x - (squareScreenWidth / 2)),
								(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
								squareScreenWidth,
								squareScreenHeight),
								redSquareHighlight);
						}

						for (int i = 0; i < pawn.discoverableMoves.Length; i++)
						{
							Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.discoverableMoves[i]);
							Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
							GUI.DrawTexture(new Rect(
								(screenPositon.x - (squareScreenWidth / 2)),
								(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
								squareScreenWidth,
								squareScreenHeight),
								yellowSquareHighlight);
						}

						for (int i = 0; i < pawn.discoverableDefending.Length; i++)
						{
							Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.discoverableDefending[i]);
							Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
							GUI.DrawTexture(new Rect(
								(screenPositon.x - (squareScreenWidth / 2)),
								(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
								squareScreenWidth,
								squareScreenHeight),
								purpleSquareHighlight);
						}
					}*/

					break;

				/*
			case Piece.Type.Bishop:

				Bishop bishop = (Bishop)main.mouse.selectedPiece;

				for (int i = 0; i < bishop.discoverablesAttacks.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(bishop.discoverablesAttacks[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						redSquareHighlight);
				}

				for (int i = 0; i < bishop.discoverableMoves.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(bishop.discoverableMoves[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						yellowSquareHighlight);
				}

				for (int i = 0; i < bishop.discoverableDefending.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(bishop.discoverableDefending[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						purpleSquareHighlight);
				}

				break;

			case Piece.Type.Rook:

				Rook rook = (Rook)main.mouse.hoverOver;

				for (int i = 0; i < rook.discoverablesAttacks.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(rook.discoverablesAttacks[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						redSquareHighlight);
				}

				for (int i = 0; i < rook.discoverableMoves.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(rook.discoverableMoves[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						yellowSquareHighlight);
				}

				for (int i = 0; i < rook.discoverableDefending.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(rook.discoverableDefending[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						purpleSquareHighlight);
				}

				break;

			case Piece.Type.Queen:

				Queen queen = (Queen)main.mouse.hoverOver;

				for (int i = 0; i < queen.discoverablesAttacks.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(queen.discoverablesAttacks[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						redSquareHighlight);
				}

				for (int i = 0; i < queen.discoverableMoves.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(queen.discoverableMoves[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						yellowSquareHighlight);
				}

				for (int i = 0; i < queen.discoverableDefending.Length; i++)
				{
					Vector3 worldPosition = main.game.board.GetSquarePosition(queen.discoverableDefending[i]);
					Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
					GUI.DrawTexture(new Rect(
						(screenPositon.x - (squareScreenWidth / 2)),
						(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
						squareScreenWidth,
						squareScreenHeight),
						purpleSquareHighlight);
				}

				break;*/

				case Piece.Type.King:

					King king = (King)main.mouse.selectedPiece;

					for (int i = 0; i < king.possibleSupportIndexes.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.possibleSupportIndexes[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							whiteSquareHighlight);
					}

					for (int i = 0; i < king.piecesAttackingThisKing.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.piecesAttackingThisKing[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							redSquareHighlight);
					}

					if (king.canCastle)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.castleIndex);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							greenSquareHighlight);
					}

					if (king.canCastleQueenSide)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.castleQueenSideIndex);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							greenSquareHighlight);
					}

					break;
			}
		}

		if (main.mouse.selectedPiece == null
			&& main.mouse.hoverOver != null)
		{
			for (int i = 0; i < main.mouse.hoverOver.moves.Length; i++)
			{
				Vector3 worldPosition = main.game.board.GetSquarePosition(main.mouse.hoverOver.moves[i]);
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
				GUI.DrawTexture(new Rect(
					(screenPositon.x - (squareScreenWidth / 2)),
					(Screen.height - screenPositon.y - (squareScreenHeight / 2)),
					squareScreenWidth,
					squareScreenHeight), 
					greenSquareHighlight);
			}

			/*for (int i = 0; i < main.mouse.hoverOver.defending.Length; i++)
			{
				Vector3 worldPosition = main.game.board.GetSquarePosition(main.mouse.hoverOver.defending[i]);
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
				GUI.DrawTexture(new Rect(
					(screenPositon.x - (squareScreenWidth / 2)),
					(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
					squareScreenWidth,
					squareScreenHeight),
					blueSquareHighlight);
			}*/

			for (int i = 0; i < main.mouse.hoverOver.attacks.Length; i++)
			{
				Vector3 worldPosition = main.game.board.GetSquarePosition(main.mouse.hoverOver.attacks[i]);
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
				GUI.DrawTexture(new Rect(
					(screenPositon.x - (squareScreenWidth / 2)),
					(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
					squareScreenWidth,
					squareScreenHeight),
					orangeSquareHighlight);
			}

			// Display unique moves and discoverable/moves/attacks/defence pieces.

			switch (main.mouse.hoverOver.type)
			{
				case Piece.Type.Pawn:

					Pawn pawn = (Pawn)main.mouse.hoverOver;

					for (int i = 0; i < pawn.enPassantAttacks.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.enPassantAttacks[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							redSquareHighlight);
					}

					/*if (pawn.isPromoted)
					{
						for (int i = 0; i < pawn.discoverablesAttacks.Length; i++)
						{
							Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.discoverablesAttacks[i]);
							Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
							GUI.DrawTexture(new Rect(
								(screenPositon.x - (squareScreenWidth / 2)),
								(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
								squareScreenWidth,
								squareScreenHeight),
								redSquareHighlight);
						}

						for (int i = 0; i < pawn.discoverableMoves.Length; i++)
						{
							Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.discoverableMoves[i]);
							Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
							GUI.DrawTexture(new Rect(
								(screenPositon.x - (squareScreenWidth / 2)),
								(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
								squareScreenWidth,
								squareScreenHeight),
								yellowSquareHighlight);
						}

						for (int i = 0; i < pawn.discoverableDefending.Length; i++)
						{
							Vector3 worldPosition = main.game.board.GetSquarePosition(pawn.discoverableDefending[i]);
							Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
							GUI.DrawTexture(new Rect(
								(screenPositon.x - (squareScreenWidth / 2)),
								(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
								squareScreenWidth,
								squareScreenHeight),
								purpleSquareHighlight);
						}
					}*/

					break;

					/*
				case Piece.Type.Bishop:

					Bishop bishop = (Bishop)main.mouse.hoverOver;

					for (int i = 0; i < bishop.discoverablesAttacks.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(bishop.discoverablesAttacks[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							redSquareHighlight);
					}

					for (int i = 0; i < bishop.discoverableMoves.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(bishop.discoverableMoves[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							yellowSquareHighlight);
					}

					for (int i = 0; i < bishop.discoverableDefending.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(bishop.discoverableDefending[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							purpleSquareHighlight);
					}

					break;

				case Piece.Type.Rook:

					Rook rook = (Rook)main.mouse.hoverOver;

					for (int i = 0; i < rook.discoverablesAttacks.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(rook.discoverablesAttacks[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							redSquareHighlight);
					}

					for (int i = 0; i < rook.discoverableMoves.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(rook.discoverableMoves[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							yellowSquareHighlight);
					}

					for (int i = 0; i < rook.discoverableDefending.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(rook.discoverableDefending[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							purpleSquareHighlight);
					}

					break;

				case Piece.Type.Queen:

					Queen queen = (Queen)main.mouse.hoverOver;

					for (int i = 0; i < queen.discoverablesAttacks.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(queen.discoverablesAttacks[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							redSquareHighlight);
					}

					for (int i = 0; i < queen.discoverableMoves.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(queen.discoverableMoves[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							yellowSquareHighlight);
					}

					for (int i = 0; i < queen.discoverableDefending.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(queen.discoverableDefending[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							purpleSquareHighlight);
					}

					break;*/

				case Piece.Type.King:

					King king = (King)main.mouse.hoverOver;

					for (int i = 0; i < king.possibleSupportIndexes.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.possibleSupportIndexes[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							whiteSquareHighlight);
					}

					for (int i = 0; i < king.piecesAttackingThisKing.Length; i++)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.piecesAttackingThisKing[i]);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							redSquareHighlight);
					}

                    if (king.canCastle)
                    {
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.castleIndex);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							greenSquareHighlight);
					}

					if (king.canCastleQueenSide)
					{
						Vector3 worldPosition = main.game.board.GetSquarePosition(king.castleQueenSideIndex);
						Vector3 screenPositon = Camera.main.WorldToScreenPoint(worldPosition);
						GUI.DrawTexture(new Rect(
							(screenPositon.x - (squareScreenWidth / 2)),
							(Screen.height - screenPositon.y - (squareScreenWidth / 2)),
							squareScreenWidth,
							squareScreenHeight),
							greenSquareHighlight);
					}

					break;
			}
		}
	}

	public bool IsOdd(int index)
	{
		if ((main.game.board.GetColumn(main.game.board.boardHeight, main.game.board.boardLength, index)) % 2 == 0)
		{
			return index % 2 != 0;
		}
		else
		{
			return (index + 1) % 2 != 0;
		}
	}

	private void DisplaySquareInfo()
	{
		for (int i = 0; i < main.game.board.squares.Length; i++)
		{
			if (IsOdd(main.game.board.squares[i].index))
			{
				string labelText = main.game.board.squares[i].name;
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(main.game.board.squares[i].transform.position);
				GUI.Label(new Rect(screenPositon.x - (squareScreenWidth / 2), Screen.height - screenPositon.y - (squareScreenHeight / 2), squareScreenWidth, squareScreenHeight), labelText, greyGuiStyle);
			}
			else
			{
				string labelText = main.game.board.squares[i].name;
				Vector3 screenPositon = Camera.main.WorldToScreenPoint(main.game.board.squares[i].transform.position);
				GUI.Label(new Rect(screenPositon.x - (squareScreenWidth / 2), Screen.height - screenPositon.y - (squareScreenHeight / 2), squareScreenWidth, squareScreenHeight), labelText, darkGreyGuiStyle);

			}
		}
	}

	private void MainMenu()
    {
		GUILayout.BeginVertical(menuLayoutOptions);

		if (GUILayout.Button("♞ Play AI"))
        {
            state = State.PlayAI;
        }

        if (GUILayout.Button("Play Network"))
        {
            state = State.PlayNetwork;
        }

        if (GUILayout.Button("Options"))
        {
            //state = State.Options;
        }

		GUILayout.EndVertical();
	}

    private void PlayAI()
    {
		GUILayout.BeginVertical(menuLayoutOptions);

		if (GUILayout.Button("Play"))
        {
            state = State.InGame;
        }

        if (GUILayout.Button("Back"))
        {
			state = State.MainMenu;
        }

		GUILayout.EndVertical();
	}

    private void PlayNetwork()
    {
		GUILayout.BeginVertical(menuLayoutOptions);

		if (GUILayout.Button("Peer To Peer"))
        {
            state = State.PeerToPeer;
        }

        if (GUILayout.Button("Back"))
        {
            state = State.MainMenu;
        }

		GUILayout.EndVertical();
	}

	private void PeerToPeer()
	{
		GUILayout.BeginVertical(menuLayoutOptions);

		if (GUILayout.Button("Join"))
		{
			state = State.Join;
		}

		if (GUILayout.Button("Host"))
		{
			state = State.Host;
		}

		if (GUILayout.Button("Back"))
		{
			state = State.PlayNetwork;
		}

		GUILayout.EndVertical();
	}

	private void Host()
	{
		GUILayout.BeginVertical(menuLayoutOptions);

		GUILayout.Label("Internet Access: " + main.network.hasAccess.ToString());

		GUILayout.Label("External IP Address: " + main.network.internetAccess.publicIpAddress);

		for (int i = 0; i < main.network.localIpAddresses.Count; i++)
		{
			GUILayout.Label("Local IP Address [" + i.ToString() + "]: " + main.network.localIpAddresses[i]);
		}

		GUILayout.BeginHorizontal(menuLayoutOptions);		
		GUILayout.Label("IP Address: ");
		main.network.server.address = GUILayout.TextField(main.network.server.address);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal(menuLayoutOptions);
		GUILayout.Label("Port: ");
		main.network.server.port = int.Parse(GUILayout.TextField(main.network.server.port.ToString()));
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Host Game"))
		{
			if (main.network.server.tcpListener == null)
			{
				main.network.isServer = true;

				main.network.server.Initiate();
			}
		}

		if (main.network.server.tcpListener != null)
		{
			GUILayout.Label("Connections: " + main.network.server.numberOfClients);
		}

		if (main.network.havingTroubleConnecting == false)
		{
			if (GUILayout.Button("Having trouble connecting?"))
			{
				main.network.havingTroubleConnecting = true;
			}
		}
		else
		{
			// Display Port Forwarding

			if (GUILayout.Button("Create Port Forward"))
			{
				Network.CreatePortMap(main.network.server.port, main.network.server.address, main.network.internetAccess);
			}

			if (GUILayout.Button("Delete Port Mapping"))
			{
				Network.DeletePortMap(main.network.server.port, main.network.internetAccess);
			}

			GUILayout.Label("Status: " + main.network.internetAccess.openDotNatStatus);

			GUILayout.Label("Port Mapping:");

			if (main.network.internetAccess.openDotNatResult.Count == 0)
			{
				GUILayout.Label("None...");
			}

			for (int i = 0; i < main.network.internetAccess.openDotNatResult.Count; i++)
			{
				GUILayout.Label(main.network.internetAccess.openDotNatResult[i]);
			}
		}

		if (GUILayout.Button("Back"))
		{
			state = State.PeerToPeer;
		}

		GUILayout.EndVertical();
	}

	private void Join()
	{
		GUILayout.BeginVertical(menuLayoutOptions);

		GUILayout.Label("Internet Access: " + main.network.hasAccess.ToString());

		GUILayout.Label("External IP Address: " + main.network.internetAccess.publicIpAddress);

		for (int i = 0; i < main.network.localIpAddresses.Count; i++)
		{
			GUILayout.Label("Local IP Address [" + i.ToString() + "]: " + main.network.localIpAddresses[i]);
		}

		GUILayout.BeginHorizontal(menuLayoutOptions);
		GUILayout.Label("IP Address: ");
		main.network.client.address = GUILayout.TextField(main.network.client.address);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal(menuLayoutOptions);
		GUILayout.Label("Port: ");
		main.network.client.port = int.Parse(GUILayout.TextField(main.network.client.port.ToString()));
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Join Game"))
		{
			if (main.network.client.tcpClient == null)
			{
				main.network.isClient = true;
				Thread connectThread = new Thread(main.network.client.Connect);
				connectThread.Start();
			}
		}

		if (main.network.havingTroubleConnecting == false)
		{
			if (GUILayout.Button("Having trouble connecting?"))
			{
				main.network.havingTroubleConnecting = true;
			}
		}
		else
		{
			// Display Port Forwarding

			if (GUILayout.Button("Create Port Forward"))
			{
				Network.CreatePortMap(main.network.server.port, main.network.server.address, main.network.internetAccess);
			}

			if (GUILayout.Button("Delete Port Mapping"))
			{
				Network.DeletePortMap(main.network.server.port, main.network.internetAccess);
			}

			GUILayout.Label("Status: " + main.network.internetAccess.openDotNatStatus);

			GUILayout.Label("Port Mapping:");

			if (main.network.internetAccess.openDotNatResult.Count == 0)
			{
				GUILayout.Label("None...");
			}

			for (int i = 0; i < main.network.internetAccess.openDotNatResult.Count; i++)
			{
				GUILayout.Label(main.network.internetAccess.openDotNatResult[i]);
			}
		}

		if (GUILayout.Button("Back"))
		{
			state = State.PeerToPeer;
		}

		GUILayout.EndVertical();
	}

	private void Options()
    {
		GUILayout.BeginVertical(menuLayoutOptions);

		if (GUILayout.Button("Back"))
        {
            state = State.MainMenu;
        }

		GUILayout.EndVertical();
	}

    private void InGame()
    {
		GUILayout.BeginVertical(menuLayoutOptions);

		if (GUILayout.Button("Forfeit"))
        {
			// CHANGE TO GO BACK TO THE LAST MENU NOT MAIN MENU

            state = State.MainMenu;
        }

		GUILayout.EndVertical();
	}

	// ============

	struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	/// <summary>
	/// The hotkey to show and hide the console window.
	/// </summary>
	public KeyCode toggleKey = KeyCode.Tab;

	List<Log> logs = new List<Log>();
	Vector2 scrollPosition;
	bool show;
	bool collapse;

	// Visual elements:

	static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

	const int margin = 20;

	Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
	Rect titleBarRect = new Rect(0, 0, 10000, 20);
	GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= null;
	}

	void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			show = !show;
		}
	}

	/// <summary>
	/// A window that displayss the recorded logs.
	/// </summary>
	/// <param name="windowID">Window ID.</param>
	void ConsoleWindow(int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		// Iterate through the recorded logs.
		for (int i = 0; i < logs.Count; i++)
		{
			var log = logs[i];

			// Combine identical messages if collapse option is chosen.
			if (collapse)
			{
				var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

				if (messageSameAsPrevious)
				{
					continue;
				}
			}

			GUI.contentColor = logTypeColors[log.type];
			GUILayout.Label(log.message);
		}

		GUILayout.EndScrollView();

		GUI.contentColor = Color.white;

		GUILayout.BeginHorizontal();

		if (GUILayout.Button(clearLabel))
		{
			logs.Clear();
		}

		collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

		GUILayout.EndHorizontal();

		// Allow the window to be dragged by its title bar.
		GUI.DragWindow(titleBarRect);
	}

	/// <summary>
	/// Records a log from the log callback.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="stackTrace">Trace of where the message came from.</param>
	/// <param name="type">Type of message (error, exception, warning, assert).</param>
	void HandleLog(string message, string stackTrace, LogType type)
	{
		logs.Add(new Log()
		{
			message = message,
			stackTrace = stackTrace,
			type = type,
		});
	}
}
