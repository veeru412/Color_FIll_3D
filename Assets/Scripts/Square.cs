using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Square : MonoBehaviour
{
    public int row;
    public int col;
    public int _color;
    public MeshRenderer mesh;
    [HideInInspector]
    public GameObject obstracle;
    public SquareType squareType;
    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        _color = 0;
    }
    private void Start()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.1f);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].tag == "wall")
            {
                squareType = SquareType.None;
            }
        }
    }

    public void SetPixel(int no)
    {
        if (_color == no)
            return;
        mesh.material = GameManager.instance.tileMats[no];
        _color = no;
        if(no == 1)
        {
            GameManager.instance.border.Add(this);
        }
        else if(_color == 2)
        {
            StartCoroutine(Animate());
        }else
        {
            Vector3 upPos = transform.localPosition;
            upPos.y = 0.0f;
            transform.localPosition = upPos;
        }
    }
    IEnumerator Animate()
    {
        Vector3 upPos = transform.localPosition + Vector3.up * 0.25f;
        float progress = 0.0f;
        while (progress <= 1.0f)
        {
            progress += Time.deltaTime * 5;
            transform.localPosition = Vector3.Lerp(transform.localPosition, upPos, progress);
            yield return null;
        }
        transform.localPosition = upPos;
        GetComponent<Collider>().enabled = true;
    }
    public int GetMatches()
    {
        int matches = 0;
        if(_color == 0)
        {
            Square rightNeighb = GameManager.instance.GetBlock(row, col + 1);
            if (rightNeighb != null && rightNeighb._color == 1)
            {
                matches++;
            }

            Square leftNeighb = GameManager.instance.GetBlock(row, col - 1);
            if (leftNeighb != null && leftNeighb._color == 1)
            {
                matches++;
            }

            Square frontNeighb = GameManager.instance.GetBlock(row - 1, col);
            if (frontNeighb != null && frontNeighb._color == 1)
            {
                matches++;
            }

            Square backNeighb = GameManager.instance.GetBlock(row + 1, col);
            if (backNeighb != null && backNeighb._color == 1)
            {
                matches++;
            }
           
            if(row == 0 && col == 0 && backNeighb != null && backNeighb._color >= 1)
            {
               // matches += 2;
            }
            
            if(row == GameManager.instance.maxRows-1 && col == GameManager.instance.maxCols - 1 && frontNeighb != null && frontNeighb._color >= 1)
            {
              //  matches += 2;
            }
        }
        return matches;
    }    
}
public enum SquareType
{ 
    Empty,
    None,
    Obstracle,
}
