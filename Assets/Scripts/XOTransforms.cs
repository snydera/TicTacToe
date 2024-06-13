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


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
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

    }

    private void OnMouseExit()
    {
        Destroy(hoveringGamePiece);
        hovering = false;
    }

    private void OnMouseDown()
    {
        if (gameManager.xTurn && hovering)
        {
            PlaceGamePiece(gameManager.xPrefab);
        }
        else if (gameManager.xTurn == false && hovering)
        {
            PlaceGamePiece(gameManager.oPrefab);
        }
    }

    void PlaceGamePiece(GameObject gamePiecePrefab)
    {
        placedGamePiece = Instantiate(gamePiecePrefab, transform.position, Quaternion.Euler(-90f, 0, 0));
        Destroy(hoveringGamePiece);
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
