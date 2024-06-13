using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool xTurn = true;

    public GameObject xPrefab;
    public GameObject oPrefab;

    public Material placedPieceMat;

    public Animator cameraAnimator;

    public int[,] board = new int[3, 3]; // 0 = empty, 1 = X, 2 = O
    private int movesMade = 0;

    public TextMeshProUGUI resultText;
    public GameObject resultPanel;

    private void Start()
    {
        resultPanel.SetActive(false);
        ResetBoard();
    }
    public void TogglePlayerTurn()
    {
        if (xTurn)
        {
            xTurn = false;
        }
        else
        {
            xTurn = true;
        }
    }

    public void PlacePiece(int x, int y, int player)
    {
        board[x, y] = player;
        movesMade++;

        if (CheckWin(x, y, player))
        {
            EndGame(player == 1 ? "X Wins!" : "O Wins!");
        }
        else if (movesMade == 9)
        {
            EndGame("It's a Draw!");
        }
    }

    private bool CheckWin(int x, int y, int player)
    {
        // Check row
        if (board[x, 0] == player && board[x, 1] == player && board[x, 2] == player) return true;
        // Check column
        if (board[0, y] == player && board[1, y] == player && board[2, y] == player) return true;
        // Check diagonals
        if (x == y && board[0, 0] == player && board[1, 1] == player && board[2, 2] == player) return true;
        if (x + y == 2 && board[0, 2] == player && board[1, 1] == player && board[2, 0] == player) return true;

        return false;
    }

    private void EndGame(string result)
    {
        resultText.text = result;
        resultPanel.SetActive(true);
        // Disable further input or reset the game
    }

    private void ResetBoard()
    {
        board = new int[3, 3];
        movesMade = 0;
        xTurn = true;
    }

    public void OnePlayerButton()
    {
        cameraAnimator.SetTrigger("StartGame");
    }

    public void TwoPlayersButton()
    {
        cameraAnimator.SetTrigger("StartGame");
    }
}
