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
    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        _color = 0;
    }
    bool canMove = true;
    public void SetPixel(int no)
    {
        mesh.material = GameManager.instance.tileMats[no];
        _color = no;
        if(no == 1)
        {
            GameManager.instance.border.Add(this);
        }
        else if(_color == 2)
        {
            if (canMove)
            {
                StartCoroutine(Animate());
            }
        }else
        {
            Vector3 upPos = transform.localPosition;
            upPos.y = 0.0f;
            transform.localPosition = upPos;
        }
    }
    IEnumerator Animate()
    {
        canMove = false;
        Vector3 upPos = transform.localPosition + Vector3.up * 0.25f;
        float progress = 0.0f;
        while (progress <= 1.0f)
        {
            progress += Time.deltaTime * 5;
            transform.localPosition = Vector3.Lerp(transform.localPosition, upPos, progress);
            yield return null;
        }
        transform.localPosition = upPos;
        if (obstracle)
        {
            Destroy(obstracle);
            GameObject g = Instantiate(GameManager.instance.cubeParticles, transform.position,Quaternion.identity);
            Destroy(g, 1f);
        }
        canMove = true;
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
