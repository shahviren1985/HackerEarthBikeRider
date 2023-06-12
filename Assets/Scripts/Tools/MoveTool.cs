using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTool : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 lastPosition = Vector3.zero;
    public CameraControl cameraControl;
    public bool moveOk=false;
    public float reduceSpeedBy = 4;
    public void SelfUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = Input.mousePosition;
            moveOk = true;
        }
        if (moveOk)
        {
            cameraControl.IncreasePosition((Input.mousePosition-lastPosition)/reduceSpeedBy);
            lastPosition = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            moveOk = false;
        }
    }
}
