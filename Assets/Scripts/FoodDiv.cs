using TMPro;
using UnityEngine;

public class FoodDiv : MonoBehaviour
{
    public string lastTitle = "";
    // Start is called before the first frame update
    public TextMeshProUGUI Label, Description;
    public string[] Titles = new string[] {"Pizza","Burger","Sandwitch","HotDog","SandWitch" };
    public string[] Details = new string[] { "Very hot and lovely. Most people love it for it's texture", "Very delicious. We have 100% satification feed back with this food", "Customers loves this to buy. So hurry and buy", "It's very crispy and tasty. Don't wait, just order." };
    private void FixedUpdate()
    {
        
    }
    public void Buy()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        SaveLoad.NoticeMsg = "Thanks for buying " + lastTitle;
    }
    public void Cancel()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        SaveLoad.NoticeMsg = "Hope you buy next time";
    }
    public void Show()
    {
        var titleint = Random.Range(0, Titles.Length);
        var detailsint = Random.Range(0, Details.Length);
        lastTitle = Titles[titleint];
        Label.text = lastTitle;
        Description.text = Details[detailsint];
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}
