using SimpleFirebaseUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminScript : MonoBehaviour
{
    public List<GameObject> showObjectsIfAdmin;
    public List<GameObject> showObjectsIfUser;
    public bool run = true;
    public static string WebURl = "";
    public static string TempMapString="",LoadedMapUrl="";
    private float mapSetTime = 0;
    public static bool MapLoggedIn = false;
    public List<GameObject> HideObjectListOnMapLoad;
    public Text HotelNameText;
    public BikeControl bikeControl;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(LoginSignUp.LoggedIn&&run)
        {
            if (LoginSignUp.IsAdmin)
            {
                SetVisibilityToObjects(showObjectsIfAdmin, true);
                SetVisibilityToObjects(showObjectsIfUser, false);
            }
            else
            {
                SetVisibilityToObjects(showObjectsIfAdmin, false);
                SetVisibilityToObjects(showObjectsIfUser, true);
            }
            run=false;
        }
        if (TempMapString != ""&&Time.time-mapSetTime>.5)
        {
            LoadUrlFirebase();
            TempMapString = "";
        }
    }
    void SetVisibilityToObjects(List<GameObject> showObjects,bool visibility)
    {
        foreach(var gobject in showObjects)
        {
            gobject.SetActive(visibility);
        }
    }
    public void SetWebURL(string url)
    {
        WebURl=url;
    }


    public void SetCurrentMapURl(string url)
    {
        LoadedMapUrl = url;
        TempMapString = url;
        bikeControl.ResetFoodItems();
        mapSetTime = Time.time;
        foreach(var gobject in HideObjectListOnMapLoad)
        {
            gobject.SetActive(false);
        }
        Debug.Log("Started loading");
        
    }
    public void LoadUrlFirebase()
    {
        var firebase = Firebase.CreateNew(FirebaseConfig.url, FirebaseConfig.key).Child("MapNames").Child(LoadedMapUrl);
        firebase.OnGetFailed += GetMapPublishedFailUrl;
        firebase.OnGetSuccess += GetMapPublishedOkURl;
        firebase.GetValue();
    }
    #region Firebase Handlers
    public void GetMapPublishedOkURl(Firebase sender,DataSnapshot snapshot)
    {
        Debug.Log("loading started");
        if (snapshot.RawJson == "null")
        {
            MapLoggedIn = false;
            SaveLoad.NoticeMsg = "Map doesn't exists";
        }
        else
        {
            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
            if (dict["published"].ToString() == "yes")
            {
                SaveLoad.loadMapName = LoadedMapUrl;
                MapLoggedIn = true;
                Debug.Log("Map name set");
            }
            else
            {
                //Debug.Log(snapshot.RawJson);
                SaveLoad.NoticeMsg = "Map doesn't published";
                MapLoggedIn = false;
            }
            if (dict.ContainsKey("restaurantName"))
            {
                HotelNameText.text = dict.ContainsKey("restaurantName").ToString();
            }
        }
    }
    public void GetMapPublishedFailUrl(Firebase sender,FirebaseError error)
    {
        SaveLoad.NoticeMsg = "Connection Error. Trying again to load";
        LoadUrlFirebase();
        Debug.Log("loading failed");
    }
    #endregion
}
