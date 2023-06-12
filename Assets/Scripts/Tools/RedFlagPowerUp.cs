using UnityEngine;

public class RedFlagPowerUp : MonoBehaviour
{
    public bool DebugThis;
    public bool Picked = false;
    public Vector2 DistanceFixer= Vector2.zero;
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Picked = false;
            BikeControl.pickedOne = Picked;
        }
        if (Picked)
        {
            if (BikeControl.EditGame)
            {
                var objectPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                objectPosition.z = 0;
                this.transform.position = objectPosition;
                BikeControl.ResetAllDrawButtons();
            }
        }
    }
    private void FixedUpdate()
    {
        if(!BikeControl.EditGame)
        {
            if(Vector2.Distance(BikeControl.Position, new Vector2(this.transform.position.x, this.transform.position.y) + DistanceFixer) < 1f)
            {
                BikeControl.RedFlag=true;
            }
        }
    }
    private void OnMouseDown()
    {
        if (!BikeControl.pickedOne && !BikeControl.EraserIsOn)
        {
            Picked=true;
            BikeControl.pickedOne = true;
        }
    }
    public void DebugLog(object msg)
    {
        if (DebugThis)
        {
            Debug.Log(msg);
        }
    }
}
