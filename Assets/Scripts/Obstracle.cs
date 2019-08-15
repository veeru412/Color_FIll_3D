using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstracle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.instance.gameState = GameState.GameOver;
        }
        else if(other.tag == "cube")
        {
            GameManager.instance.DestroyObstracle(other.transform);
            Destroy(gameObject);
        }
    }
}
