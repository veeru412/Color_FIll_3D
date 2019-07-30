using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstracle : MonoBehaviour
{
    public int row;
    public int col;

    private void Start()
    {
        GameManager.instance.GetBlock(row, col).obstracle = gameObject;
    }
}
