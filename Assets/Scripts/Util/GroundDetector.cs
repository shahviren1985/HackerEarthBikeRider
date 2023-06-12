using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public bool Debugthis;
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name== "SuperMotoFrontWheel")
        {
            BikeControl.frontTireTouched = true;
        }
        else if (collision.gameObject.name == "SuperMotoRearWheel")
        {
            BikeControl.backTireTouched = true;
        }
        else if (collision.gameObject.name == "biker")
        {
            DebugLog("Biker crushed");
            BikeControl.GameOver = true;
        }
        else
        {
            DebugLog(collision.gameObject.name);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.name == "SuperMotoFrontWheel")
        {
            BikeControl.frontTireTouched = false;
        }
        else if (collision.gameObject.name == "SuperMotoRearWheel")
        {
            BikeControl.backTireTouched = false;
        }
        
    }
    public void DebugLog(string msg)
    {
        if (Debugthis)
        {
            Debug.Log(msg);
        }
    }
}

