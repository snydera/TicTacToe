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
    public Transform mainCameraTransform;
    public Transform gameViewTransform;

    public int[,] board = new int[3, 3]; // 0 = empty, 1 = X, 2 = O
    private int movesMade = 0;

    public TextMeshProUGUI resultText;
    public GameObject resultPanel;
    public GameObject buttonPanel;

    public List<GameObject> placedPieces = new List<GameObject>();
    public Collider[] gameBoardTriggers;

    bool gameOver = false;
    bool onePlayerMode = false;
    public bool computerMove = false;

    private void Start()
    {
        resultPanel.SetActive(false);
        ResetBoard();

        foreach (Collider trigger in gameBoardTriggers)
        {
            trigger.enabled = false;
        }
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

        if (!xTurn && onePlayerMode && !gameOver)
        {
            computerMove = true;
            StartCoroutine(ComputerMove());
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
        gameOver = true;
        resultText.text = result;
        resultPanel.SetActive(true);
        buttonPanel.SetActive(true);
        buttonPanel.GetComponent<CanvasGroup>().alpha = 1.0f;
        
        foreach (Collider trigger in gameBoardTriggers)
        {
            trigger.enabled = false;
        }
    }

    private void ResetBoard()
    {
        board = new int[3, 3];
        movesMade = 0;
        xTurn = true;

        if (gameOver)
        {
            resultPanel.SetActive(false);
            buttonPanel.SetActive(false);
        }

        foreach (GameObject go in placedPieces)
        {
            Destroy(go);
        }
      
        foreach (Collider trigger in gameBoardTriggers)
        {
            trigger.enabled = true;
        }

        placedPieces.Clear();
        gameOver = false;
    }

    public void OnePlayerButton()
    {
        onePlayerMode = true;

        if (gameOver)
        {
            ResetBoard();

        }
        else
        {
            cameraAnimator.SetTrigger("StartGame");
        }
        
    }

    public void TwoPlayersButton()
    {
        onePlayerMode = false;

        if (gameOver)
        {
            ResetBoard();
           
        }
        else
        {
            cameraAnimator.SetTrigger("StartGame");
        }
        
    }

    IEnumerator ComputerMove()
    {
        yield return new WaitForSeconds(1.0f); // Small delay to simulate thinking time

        // Find a random empty spot
        List<int> emptySpots = new List<int>();
        for (int i = 0; i < gameBoardTriggers.Length; i++)
        {
            if (gameBoardTriggers[i].enabled)
            {
                emptySpots.Add(i);
            }
        }

        if (emptySpots.Count > 0)
        {
            int moveIndex = emptySpots[Random.Range(0, emptySpots.Count)];
            int x = moveIndex / 3;
            int y = moveIndex % 3;
            PlacePiece(x, y, 2);

            // Instantiate and place the piece visually
            GameObject piece = Instantiate(oPrefab, gameBoardTriggers[moveIndex].transform.position, Quaternion.Euler(-90f, 0, 0));
            piece.GetComponent<MeshRenderer>().material = placedPieceMat;
            placedPieces.Add(piece);
            gameBoardTriggers[moveIndex].enabled = false;

            TogglePlayerTurn();
        }

        computerMove = false;
    }
}
