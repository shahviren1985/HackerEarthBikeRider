using UnityEngine;

public class StartingPoint : MonoBehaviour
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
    private void OnMouseDown()
    {
        Debug.Log(BikeControl.pickedOne);
        Debug.Log(BikeControl.EraserIsOn);
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
