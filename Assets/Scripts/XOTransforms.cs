using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XOTransforms : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    bool hovering = false;

    GameObject hoveringGamePiece;
    GameObject placedGamePiece;

    private int x, y;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        //Creates coordinates from names of game objects
        string name = this.gameObject.name;
        x = name[0] - 'A';
        y = int.Parse(name[1].ToString()) - 1;
    }


    private void OnMouseOver()
    {
        if (!hovering && gameManager.computerMove == false)
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
    }

    private void OnMouseDown()
    {
        if (hovering && gameManager.computerMove == false)
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
        StartCoroutine(DelayChangeTurn());
    }

    IEnumerator DelayChangeTurn()
    {
        yield return new WaitForSeconds(0.1f);
        gameManager.TogglePlayerTurn();
    }     
}
