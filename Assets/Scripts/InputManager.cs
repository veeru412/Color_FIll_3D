
using UnityEngine;

namespace ColorFill.Core
{
    public class InputManager : MonoBehaviour
    {
        public static Vector2 dir;
        Vector3 mouseStartPos;
        bool isDragging = false;

        private void Start()
        {
            dir = Vector2.zero;
            isDragging = false;
            mouseStartPos = Vector2.zero;
        }

        Vector3 GetMousePosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
       
        private void Update()
        {
 
#if UNITY_EDITOR
            if (dir != Vector2.down && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
            {
                dir = Vector2.up;
            }
            else if (dir != Vector2.up && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
            {
                dir = Vector2.down;
            }
            else if (dir != Vector2.left && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)))
            {
                dir = Vector2.right;
            }
            else if (dir != Vector2.right && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
            {
                dir = Vector2.left;
            }

#else
            OnMobile();
           
#endif
        }
        bool swiping = false;
        Vector2 lastPosition = Vector2.zero;
        void OnMobile()
        {
            if (Input.touchCount == 0)
                return;

            if (Input.GetTouch(0).deltaPosition.sqrMagnitude != 0)
            {
                if (swiping == false)
                {
                    swiping = true;
                    lastPosition = Input.GetTouch(0).position;
                    return;
                }
                else
                {
                    Vector2 direction = Input.GetTouch(0).position - lastPosition;
                    if (Vector2.SqrMagnitude(direction) > 0.1f)
                    {
                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            if (direction.x > 0)
                            {
                                if(dir != Vector2.left)
                                    dir = Vector2.right;
                            }
                            else if(dir != Vector2.right)
                                dir = Vector2.left;
                        }
                        else
                        {
                            if (direction.y > 0)
                            {
                                if(dir != Vector2.down)
                                    dir = Vector2.up;
                            }
                            else if(dir != Vector2.up)
                                dir = Vector2.down;
                        }
                        swiping = false;
                    }
                }
            }
            else
            {
                swiping = false;
            }
        }
    }
}

