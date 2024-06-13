using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XOTransforms : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    bool hovering = false;

    GameObject hoveringGamePiece;
    GameObject placedGamePiece;

    private int x, y;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        string name = this.gameObject.name;

        // Map letters A, B, C to indices 0, 1, 2
        x = name[0] - 'A';

        // Map numbers 1, 2, 3 to indices 0, 1, 2
        y = int.Parse(name[1].ToString()) - 1;
    }


    private void OnMouseOver()
    {
        /*
        Debug.Log("Mouse over " + this.gameObject.name);

        if (gameManager.xTurn && !hovering)
        {
            hoveringGamePiece = Instantiate(gameManager.xPrefab, transform);
            hovering = true;
        }
        else if (gameManager.xTurn == false && !hovering)
        {
            hoveringGamePiece = Instantiate(gameManager.oPrefab, transform);
            hovering = true;
        }
        */

        if (!hovering && gameManager.board[x, y] == 0 && gameManager.computerMove == false)
        {
            hoveringGamePiece = Instantiate(gameManager.xTurn ? gameManager.xPrefab : gameManager.oPrefab, transform);
            hovering = true;
        }
    }

    private void OnMouseExit()
    {
        if (hoveringGamePiece)
        {
            Destroy(hoveringGamePiece);
            hovering = false;
        }

        /*
        Destroy(hoveringGamePiece);
        hovering = false;
        */
    }

    private void OnMouseDown()
    {
        /*
        if (gameManager.xTurn && hovering)
        {
            PlaceGamePiece(gameManager.xPrefab);
        }
        else if (gameManager.xTurn == false && hovering)
        {
            PlaceGamePiece(gameManager.oPrefab);
        }
        */

        if (hovering && gameManager.board[x, y] == 0 && gameManager.computerMove == false)
        {
            PlaceGamePiece(gameManager.xTurn ? gameManager.xPrefab : gameManager.oPrefab);
            gameManager.PlacePiece(x, y, gameManager.xTurn ? 1 : 2);
        }
    }

    void PlaceGamePiece(GameObject gamePiecePrefab)
    {
        placedGamePiece = Instantiate(gamePiecePrefab, transform.position, Quaternion.Euler(-90f, 0, 0));
        Destroy(hoveringGamePiece);
        gameManager.placedPieces.Add(placedGamePiece);
        placedGamePiece.GetComponent<MeshRenderer>().material = gameManager.placedPieceMat;
        GetComponent<BoxCollider>().enabled = false;
        hovering = false;
        //gameManager.PlayQuickSound(gameManager.sfxClips[Random.Range(1, 4)]);
        StartCoroutine(DelayChangeTurn());
    }

    IEnumerator DelayChangeTurn()
    {
        yield return new WaitForSeconds(0.1f);
        gameManager.TogglePlayerTurn();
    }
        
}
