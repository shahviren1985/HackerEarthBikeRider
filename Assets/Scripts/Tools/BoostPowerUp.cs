using UnityEngine;

public class BoostPowerUp : MonoBehaviour
{
    public bool DebugThis;
    public bool Picked = false;
    public Vector2 DistanceFixer= new(.5f,-.3f);
    public float MaxBoostPower = 3000;
    public float CurBoostPower = 600;
    public float DeactiveTime = 0;
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
            if(Vector2.Distance(BikeControl.Position, new Vector2(this.transform.position.x, this.transform.position.y) + DistanceFixer) < 1.32)
            {
                if (Time.time > DeactiveTime)
                {
                    DeactiveTime += Time.time+1;
                    BikeControl.BoostPower += CurBoostPower;
                    DebugLog(BikeControl.BoostPower);
                    if (BikeControl.BoostPower > MaxBoostPower)
                    {
                        BikeControl.BoostPower = MaxBoostPower;
                        
                    }
                }
            }
        }
    }
    private void OnMouseDown()
    {
        if (!BikeControl.pickedOne&&!BikeControl.EraserIsOn)
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
