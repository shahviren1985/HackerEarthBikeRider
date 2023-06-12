using UnityEngine;

public class StarPowerUp : MonoBehaviour
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
            if(Vector2.Distance(BikeControl.Position, new Vector2(this.transform.position.x, this.transform.position.y) + DistanceFixer) < 1.32)
            {
                Debug.Log(BikeControl.foodItemNames.Count);
                var randomNumber = Random.Range(0, BikeControl.foodItemNames.Count - 1);
                SaveLoad.showFoodNotice = true;
                SaveLoad.NoticeMsg = "Got " + BikeControl.foodItemNames[randomNumber];
                BikeControl.CollectedItems.Add(BikeControl.foodItemNames[randomNumber]);
                BikeControl.foodItemNames.RemoveAt(randomNumber);
                Debug.Log(BikeControl.foodItemNames.Count);
                Debug.Log(BikeControl.CollectedItems.Count);
                gameObject.SetActive(false);
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
