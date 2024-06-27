using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject xPrefab;
    public GameObject oPrefab;

    public bool xTurn = true;
    public bool computerMove = false;

    bool gameOver = false;
    bool onePlayerMode = false;
    bool easyMode = false;

    bool musicMuted = false;
    bool sfxMuted = false;

    public AudioClip[] sfxClips;

    public Material placedPieceMat;

    public int[,] board = new int[3, 3];
    private int movesMade = 0;

    public List<GameObject> placedPieces = new List<GameObject>();

    [SerializeField]
    Collider[] gameBoardTriggers;

    [SerializeField]
    Animator cameraAnimator;

    [SerializeField]
    GameObject resultPanel;

    TextMeshProUGUI resultText;

    [SerializeField]
    GameObject buttonPanel;

    Button onePlayerButton;
    Button twoPlayersButton;

    GameObject muteMusicButtonGO;
    GameObject muteSFXButtonGO;


    [SerializeField]
    AudioSource musicSource;

    [SerializeField]
    AudioSource sfxSource;

    [SerializeField]
    Color[] buttonColors;


    private void Awake()
    {
        resultText = resultPanel.transform.Find("Result Text").GetComponent<TextMeshProUGUI>();
        onePlayerButton = buttonPanel.transform.Find("One Player Button").GetComponent<Button>();
        twoPlayersButton = buttonPanel.transform.Find("Two Players Button").GetComponent <Button>();
        muteMusicButtonGO = buttonPanel.transform.Find("Mute Music Button").gameObject;
        muteSFXButtonGO = buttonPanel.transform.Find("Mute SFX Button").gameObject;
    }

    private void Start()
    {
        resultPanel.SetActive(false);
        ResetBoard();
        onePlayerButton.onClick.AddListener(OnePlayerButtonPressed);
        twoPlayersButton.onClick.AddListener(TwoPlayersButtonPressed);

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
        PlayQuickSound(sfxClips[Random.Range(1, 4)]);
        board[x, y] = player;
        movesMade++;

        //Debug.Log("Piece placed at (" + x + ", " + y + ") by " + (player == 1 ? "X" : "O"));


        if (CheckWin(x, y, player))
        {
            EndGame(player == 1 ? "X Wins!" : "O Wins!");
            PlayQuickSound(sfxClips[5]);
        }
        else if (movesMade == 9)
        {
            EndGame("It's a Draw!");
            PlayQuickSound(sfxClips[6]);
        }
        else if (movesMade == 1)
        {
            resultPanel.SetActive(false);
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

        RemoveButtonListeners();
        onePlayerButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "One\nPlayer";
        onePlayerButton.onClick.AddListener(OnePlayerButtonPressed);
        twoPlayersButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Two\nPlayers";
        twoPlayersButton.onClick.AddListener(TwoPlayersButtonPressed);

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

    #region Buttons

    public void OnePlayerButtonPressed()
    {
        onePlayerMode = true;
        RemoveButtonListeners();
        onePlayerButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Easy Mode";
        onePlayerButton.onClick.AddListener(EasyModeButton);
        twoPlayersButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Hard Mode";
        twoPlayersButton.onClick.AddListener(HardModeButton);
    }

    void EasyModeButton()
    {
        easyMode = true;
        StartGame();
    }

    void HardModeButton()
    {
        easyMode = false;
        StartGame();
    }

    public void TwoPlayersButtonPressed()
    {
        onePlayerMode = false;
        StartGame();
    }

    void RemoveButtonListeners()
    {
        onePlayerButton.onClick.RemoveAllListeners();
        twoPlayersButton.onClick.RemoveAllListeners();
    }

    public void StartGame()
    {
        resultText.text = "X moves first. Click an open space on the board.";

        if (gameOver)
        {
            ResetBoard();
        }
        else
        {
            cameraAnimator.SetTrigger("StartGame");
        }

        PlayQuickSound(sfxClips[0]);
    }

    public void MuteMusicButtonPressed()
    {
        if (musicMuted)
        {
            muteMusicButtonGO.GetComponent<Image>().color = buttonColors[0];
            muteMusicButtonGO.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Mute Music";
            musicSource.volume = 1;
            musicMuted = false;
            PlayQuickSound(sfxClips[0]);
        }
        else
        {
            muteMusicButtonGO.GetComponent<Image>().color = buttonColors[1];
            muteMusicButtonGO.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Unmute Music";
            musicSource.volume = 0;
            musicMuted = true;
        }
    }

    public void MuteSFXButtonPressed()
    {
        if (sfxMuted)
        {
            muteSFXButtonGO.GetComponent<Image>().color = buttonColors[0];
            muteSFXButtonGO.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Mute SFX";
            sfxSource.volume = 1;
            sfxMuted = false;
            PlayQuickSound(sfxClips[0]);
        }
        else
        {
            muteSFXButtonGO.GetComponent<Image>().color = buttonColors[1];
            muteSFXButtonGO.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Unmute SFX";
            sfxSource.volume = 0;
            sfxMuted = true;
        }
    }

    public void ExitGameButtonPressed()
    {
        Application.Quit();
    }

    #endregion

    void PlayQuickSound(AudioClip clip)
    {
        if (!sfxMuted)
        {
            sfxSource.clip = clip;
            sfxSource.Play();
        }
    }

    IEnumerator ComputerMove()
    {
        yield return new WaitForSeconds(1f);

        if (easyMode)
        {
            RandomMove();
        }
        else
        {
            CompetitiveMove();
        }

        computerMove = false;
    }

    void RandomMove()
    {
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
            int x = moveIndex % 3;
            int y = moveIndex / 3;
            PlacePiece(x, y, 2);

            GameObject piece = Instantiate(oPrefab, gameBoardTriggers[moveIndex].transform.position, Quaternion.Euler(-90f, 0, 0));
            piece.GetComponent<MeshRenderer>().material = placedPieceMat;
            placedPieces.Add(piece);
            gameBoardTriggers[moveIndex].enabled = false;

            TogglePlayerTurn();
        }
    }

    void CompetitiveMove()
    {
        Vector2Int move = GetBestMove();
        int x = move.x;
        int y = move.y;
        PlacePiece(x, y, 2);

        GameObject piece = Instantiate(oPrefab, gameBoardTriggers[y * 3 + x].transform.position, Quaternion.Euler(-90f, 0, 0));
        piece.GetComponent<MeshRenderer>().material = placedPieceMat;
        placedPieces.Add(piece);
        gameBoardTriggers[y * 3 + x].enabled = false;

        TogglePlayerTurn();
    }

    private Vector2Int GetBestMove()
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (board[x, y] == 0)
                {
                    availableMoves.Add(new Vector2Int(x, y));
                }
            }
        }

        // Randomize the computer's first move
        if (movesMade == 1)
        {
            return availableMoves[Random.Range(0, availableMoves.Count)];
        }

        // Check for winning move or blocking move
        foreach (Vector2Int move in availableMoves)
        {
            int x = move.x;
            int y = move.y;

            // Win: If the computer can win in the next move, take that move
            board[x, y] = 2;
            if (CheckWin(x, y, 2))
            {
                board[x, y] = 0;
                return move;
            }
            board[x, y] = 0;

            // Block: If the player can win in the next move, block that move
            board[x, y] = 1;
            if (CheckWin(x, y, 1))
            {
                board[x, y] = 0;
                return move;
            }
            board[x, y] = 0;
        }

        // Checks available corners
        foreach (Vector2Int move in new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(2, 2), new Vector2Int(0, 2), new Vector2Int(2, 0) })
        {
            if (board[move.x, move.y] == 0)
            {
                return move;
            }
        }

        // Checks available sides
        foreach (Vector2Int move in new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 2), new Vector2Int(2, 1) })
        {
            if (board[move.x, move.y] == 0)
            {
                return move;
            }
        }
        
        // return an invalid move
        return new Vector2Int(-1, -1);
        
    }

}
