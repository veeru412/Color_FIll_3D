using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorFill.Core
{
    public class PlayerController : MonoBehaviour
    {
        public float walkSpeed;
        Transform mTransform;
        [HideInInspector]
        public Square mSquare;
        private void Awake()
        {
            mTransform = transform;
        }
 
        private void Update()
        {
            if( GameManager.instance.gameState != GameState.Playing)
            {
                return;
            }
            if (InputManager.dir != Vector2.zero && mSquare)
            {
                Vector3 targetPos = mSquare.transform.position;
                targetPos.y = 0;
                mTransform.localPosition = Vector3.MoveTowards(mTransform.localPosition, targetPos, Time.deltaTime * walkSpeed);
                if (Vector3.Distance(mTransform.localPosition, targetPos) < 0.05f)
                {
                    int wantedRow = mSquare.row + (int)InputManager.dir.y;
                    int wantedCol = mSquare.col + (int)InputManager.dir.x;
                    Square target = GameManager.instance.GetBlock(wantedRow, wantedCol);

                    if (mSquare._color == 0)
                    {
                        mSquare.SetPixel(1);
                        if (target != null && target._color == 2)
                        {
                            GameManager.instance.Fill();
                        }
                    }

                    if (target != null)
                    {
                        if (target._color == 1 || target.obstracle != null)
                        {
                            Destroy(gameObject);
                            GameManager.instance.gameState = GameState.GameOver;
                        }
                        mSquare = target;
                    }
                    else
                    {
                        InputManager.dir = Vector2.zero;
                        GameManager.instance.Fill();
                    }

                }
            }
            else if(mSquare == null && InputManager.dir != Vector2.zero)
            {
               // InputManager.dir = Vector2.zero;
            }
        }

        public void MoveToNextLevel()
        {
            StartCoroutine(ToNextLevel());
        }
        IEnumerator ToNextLevel()
        {
            yield return new WaitForSeconds(1.0f);
            Vector3 targetPos = mTransform.localPosition;
            targetPos.x = 0;
            while(Vector3.Distance(mTransform.localPosition,targetPos) > 0.05f)
            {
                mTransform.localPosition = Vector3.Lerp(mTransform.localPosition, targetPos, Time.deltaTime * 5);
                yield return null;
            }
            Square startTile = GameManager.instance.GetBlock(0, 4);
            mSquare = startTile;
            targetPos = mSquare.transform.position + Vector3.forward*19;
            GameManager.instance.lvl2.SetActive(true);
            Vector3 camPos = Camera.main.transform.position + Vector3.forward * 19;
            while (Vector3.Distance(mTransform.localPosition, targetPos) > 0.05f)
            {
                mTransform.localPosition = Vector3.Lerp(mTransform.localPosition, targetPos, Time.deltaTime * 3);
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, camPos, Time.deltaTime * 3);
                yield return null;
            }
            GameManager.instance.ChangeGridPos(); 
            startTile.SetPixel(2);
            yield return new WaitForEndOfFrame();
            GameManager.instance.gameState = GameState.Playing;
            
        }
    }
}
