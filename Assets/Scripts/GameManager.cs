using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool xTurn = true;

    public GameObject xPrefab;
    public GameObject oPrefab;

    public Material placedPieceMat;

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
}
