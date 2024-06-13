using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject xPrefab;
    public GameObject oPrefab;

    public bool xTurn = true;
    public bool computerMove = false;
    public bool canClick = true;

    bool gameOver = false;
    bool onePlayerMode = false;

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
    GameObject resultPanel;

    [SerializeField]
    TextMeshProUGUI resultText;

    [SerializeField]
    GameObject buttonPanel;

    [SerializeField]
    Animator cameraAnimator;

    [SerializeField]
    AudioSource musicSource;

    [SerializeField]
    AudioSource sfxSource;

    [SerializeField]
    Color[] buttonColors;

    [SerializeField]
    GameObject muteMusicButton;

    [SerializeField]
    GameObject muteSFXButton;

    [SerializeField]
    GameObject blockingCollider;

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
        PlayQuickSound(sfxClips[Random.Range(1, 4)]);
        board[x, y] = player;
        movesMade++;

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
            StartCoroutine(DelayInstructionsText());
        }

        resultPanel.SetActive(true);
        resultText.text = "X moves first. Click an open space on the board.";
        PlayQuickSound(sfxClips[0]);
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
            StartCoroutine(DelayInstructionsText());
        }


        PlayQuickSound(sfxClips[0]);
    }

    IEnumerator DelayInstructionsText()
    {
        yield return new WaitForSeconds(2f);
        resultPanel.SetActive(true);
        resultText.text = "X moves first. Click an open space on the board.";
    }

    public void MuteMusicButton()
    {
        if (musicMuted)
        {
            muteMusicButton.GetComponent<Image>().color = buttonColors[0];
            muteMusicButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Mute Music";
            musicSource.volume = 1;
            musicMuted = false;
            PlayQuickSound(sfxClips[0]);
        }
        else
        {
            muteMusicButton.GetComponent<Image>().color = buttonColors[1];
            muteMusicButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Unmute Music";
            musicSource.volume = 0;
            musicMuted = true;
        }
    }

    public void MuteSFXButton()
    {
        if (sfxMuted)
        {
            muteSFXButton.GetComponent<Image>().color = buttonColors[0];
            muteSFXButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Mute SFX";
            sfxSource.volume = 1;
            sfxMuted = false;
            PlayQuickSound(sfxClips[0]);
        }
        else
        {
            muteSFXButton.GetComponent<Image>().color = buttonColors[1];
            muteSFXButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Unmute SFX";
            sfxSource.volume = 0;
            sfxMuted = true;
        }
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }

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
        blockingCollider.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);

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

            GameObject piece = Instantiate(oPrefab, gameBoardTriggers[moveIndex].transform.position, Quaternion.Euler(-90f, 0, 0));
            piece.GetComponent<MeshRenderer>().material = placedPieceMat;
            placedPieces.Add(piece);
            gameBoardTriggers[moveIndex].enabled = false;

            TogglePlayerTurn();
        }
        computerMove = false;
        
        yield return new WaitForSeconds(1.0f);
        blockingCollider.SetActive(false);
    }
}
