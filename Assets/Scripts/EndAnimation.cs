using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimation : MonoBehaviour
{
    public Transform gameViewTransform;
    public Collider[] gameBoardTriggers;
    public void EndAnimations()
    {
        transform.position = gameViewTransform.position;
        transform.rotation = gameViewTransform.rotation;

        foreach (Collider trigger in gameBoardTriggers)
        {
            trigger.enabled = true;
        }

        GetComponent<Animator>().enabled = false;
    }
}
