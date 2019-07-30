using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform mTransform;

    IEnumerator Start()
    {
        mTransform = transform;
        Camera cam = GetComponent<Camera>();
        while(cam.fieldOfView > 61)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60.0f, Time.deltaTime*5);
            yield return null;
        }
        cam.fieldOfView = 60;
        GameManager.instance.gameState = GameState.Playing;
    }
}
