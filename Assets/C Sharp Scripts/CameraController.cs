using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float horizontalResolution = 1920;

    /*public void Update()
    {
        // if the aspect ratio is set to free.
        //SetOrthographicSize();
    }*/

    public void SetOrthographicSize()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;
        Camera.main.orthographicSize = horizontalResolution / currentAspect / 200; // divide by 2 - 200 due to orthographic scale 1/2?
    }
}
