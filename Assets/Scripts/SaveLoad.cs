using SimpleFirebaseUnity;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    public LinesDrawer PhysicsLine, JustLine;
    public SpawnerManager StarManager,BoostManager,YellowFlagManager,RedFlagManager;
    public InputField mapName;
    public GameObject noticeText;
    public float NoticeShowTime = .8f;
    public float NoticeDisableAfterTime = 0;
    public bool DebugThis;
    private Firebase firebase;
    public static string loadMapName = "";
    private string lastMapName = "";
    public MapRowController mapRowController;
    private List<MapRowData> mapRows=new();
    public BikeControl bikeControl;
    public static string NoticeMsg = "";
    public FoodDiv foodDiv;
    public static bool ShowFoodDiv = false, showFoodNotice = false;
    public static bool LoadCompleted = false;
    public static string floatSeperator = "*", ItemSeperator = "###";
    public static bool isPublished=false;
    public Text PublishedText;
    string[] publishMod = new string[] {"Publish Map","Draft Map"};
    public static List<string> lightColors = new List<string>()
{
    "#D9D9D6",
    "#FF7276",
    "#F1EB9C",
    "#ADD8E6",
    "#90EE90",
    "#E6E6FA",
    "#FFB6C1",
    "#FFDAB9",
    "#CD853F",
    "#E0FFFF",
    "#E6E6FA",
    "#FFA07A"
};
    // Start is called before the first frame update
    private void Start()
    {
        firebase=Firebase.CreateNew(FirebaseConfig.url, FirebaseConfig.key);
        mapName.text = "Map-" + Random.Range(0, 100000000);
        isPublished = false;
    }
    private void FixedUpdate()
    {
        if (isPublished)
        {
            PublishedText.text = publishMod[1];
        }
        else
        {
            PublishedText.text = publishMod[0];
        }
        if(Time.time> NoticeDisableAfterTime)
        {
            noticeText.gameObject.SetActive(false);
        }
        if (loadMapName != "")
        {
            ShowNotice("Loading " + loadMapName);
            var MapFirebase = firebase.Child("Maps").Child(loadMapName);
            MapFirebase.OnGetFailed += GetFailHandler;
            MapFirebase.OnGetSuccess += GetMapDataHandler;
            MapFirebase.GetValue();
            lastMapName = loadMapName;
            BikeControl.lastMapName= lastMapName;
            loadMapName = "";
        }
        if (NoticeMsg != "")
        {
            ShowNotice(NoticeMsg);
            NoticeMsg = "";
        }
    }
    public void Publish()
    {
        isPublished = !isPublished;
        Save();
    }
    public void ClearMap()
    {
        ClearChildrens(JustLine.transform);
        ClearChildrens(PhysicsLine.transform);
        ClearChildrens(StarManager.transform);
        ClearChildrens(BoostManager.transform);
        ClearChildrens(RedFlagManager.transform);
        ClearChildrens(YellowFlagManager.transform);

        bikeControl.LineDrawerPhysics.Enable = true;
        bikeControl.LineDrawerPhysics.BeginDraw();
        bikeControl.LineDrawerPhysics.AddPoint(new(-6, 0));
        bikeControl.LineDrawerPhysics.AddPoint(new(-5.5f, 0));
        bikeControl.LineDrawerPhysics.AddPoint(new(-5, 0));
        bikeControl.LineDrawerPhysics.AddPoint(new(-4.5f, 0));
        bikeControl.LineDrawerPhysics.AddPoint(new(-4, 0));
        bikeControl.LineDrawerPhysics.AddPoint(new(-3.5f, 0));
        bikeControl.LineDrawerPhysics.AddPoint(new(-3, 0));
        bikeControl.LineDrawerPhysics.EndDraw();
        bikeControl.LineDrawerPhysics.Enable = false;
    }

    public void DeleteMap()
    {
        firebase.Child("MapNames").Child(mapName.text).Delete();
        firebase.Child("Maps").Child(mapName.text).Delete();
        bikeControl.NewMap();
    }
    [DllImport("__Internal")]
    private static extern void copyToClipboard(string text);
    [DllImport("__Internal")]
    private static extern void gotoBhaariWebsite();
    public void GotoMainWebsite()
    {
        gotoBhaariWebsite();
    }
    public void CopyLink()
    {
        copyToClipboard(AdminScript.WebURl + "?mapid=" + mapName.text);
        //GUIUtility.systemCopyBuffer = AdminScript.WebURl + "?mapid=" + mapName.text;
    }
    //public void ResetCachedMap()
    //{
    //    PlayerPrefs.SetString("physicsLine","");
    //    PlayerPrefs.SetString("justLine", "");
    //    PlayerPrefs.SetString("Star", "");
    //    PlayerPrefs.SetString("Boost", "");
    //    PlayerPrefs.SetString("YellowFlag", "");
    //    PlayerPrefs.SetString("RedFlag", "");
    //}
    public static void ClearChildrens(Transform transform)
    {
        for(var i = 0; i < transform.childCount; i++)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void Save()
    {
        //ResetCachedMap();
        Dictionary<string, object> mapItems = new();
        //string lineOutput = "";
        for (var i = 0; i < PhysicsLine.transform.childCount; i++)
        {
            Dictionary<string,object> line=new();
            var lineData = PhysicsLine.transform.GetChild(i).GetComponent<Line>();
            line["type"] = "physicsLine";
            Dictionary<string,object> points = new();
            
            for(var j=0;j<lineData.points.Count; j++)
            {
                points["point"+j] = "" + lineData.points[j].x + "," + lineData.points[j].y;
                //lineOutput = lineData.points[j].x + "," + lineData.points[j].y + floatSeperator;
            }
            if(points.Count > 0)
            {
                line["points"]=points;
                mapItems["PhysicsLine"+i]=line;
            }
            //lineOutput += ItemSeperator;
        }
        //PlayerPrefs.SetString("physicsLine", lineOutput);
        //lineOutput = "";
        for (var i = 0; i < JustLine.transform.childCount; i++)
        {
            Dictionary<string, object> line = new();
            var lineData = JustLine.transform.GetChild(i).GetComponent<Line>();
            line["type"] = "justLine";
            Dictionary<string, object> points = new();
            for (var j = 0; j < lineData.points.Count; j++)
            {
                points["point" + j] = ""+ lineData.points[j].x+","+ lineData.points[j].y;
                //lineOutput = lineData.points[j].x + "," + lineData.points[j].y + floatSeperator;
            }
            if (points.Count > 0)
            {
                line["points"] = points;
                mapItems["JustLine" + i] = line;
            }
            //lineOutput += ItemSeperator;
        }
        //PlayerPrefs.SetString("justLine", lineOutput);
        //lineOutput = "";
        for (var i = 0; i < StarManager.transform.childCount; i++)
        {
            Dictionary<string, object> line = new();
            line["type"] = "Star";
            line["position"] = StarManager.transform.GetChild(i).transform.position.x + "," + StarManager.transform.GetChild(i).transform.position.y;
            //lineOutput = StarManager.transform.GetChild(i).transform.position.x + "," + StarManager.transform.GetChild(i).transform.position.y + floatSeperator;
            mapItems["Star"+i]=line;
        }
        //PlayerPrefs.SetString("Star", lineOutput);
        //lineOutput = "";
        for (var i = 0; i < BoostManager.transform.childCount; i++)
        {
            Dictionary<string, object> line = new();
            line["type"] = "Boost";
            line["position"] = BoostManager.transform.GetChild(i).transform.position.x + "," + BoostManager.transform.GetChild(i).transform.position.y;
            //lineOutput = BoostManager.transform.GetChild(i).transform.position.x + "," + BoostManager.transform.GetChild(i).transform.position.y + floatSeperator;

            mapItems["Boost" + i] = line;
        }
        //PlayerPrefs.SetString("Boost", lineOutput);
        //lineOutput = "";
        for (var i = 0; i < YellowFlagManager.transform.childCount; i++)
        {
            Dictionary<string, object> line = new();
            line["type"] = "YellowFlag";
            line["position"] = YellowFlagManager.transform.GetChild(i).transform.position.x + "," + YellowFlagManager.transform.GetChild(i).transform.position.y;
            //lineOutput = YellowFlagManager.transform.GetChild(i).transform.position.x + "," + YellowFlagManager.transform.GetChild(i).transform.position.y + floatSeperator;

            mapItems["YellowFlag" + i] = line;
        }
        //PlayerPrefs.SetString("YellowFlag", lineOutput);
        //lineOutput = "";
        for (var i = 0; i < RedFlagManager.transform.childCount; i++)
        {
            Dictionary<string, object> line = new();
            line["type"] = "RedFlag";
            line["position"] = RedFlagManager.transform.GetChild(i).transform.position.x + "," + RedFlagManager.transform.GetChild(i).transform.position.y;
            //lineOutput = RedFlagManager.transform.GetChild(i).transform.position.x + "," + RedFlagManager.transform.GetChild(i).transform.position.y + floatSeperator;

            mapItems["RedFlag" + i] = line;
        }
        //PlayerPrefs.SetString("RedFlag", lineOutput);
        //lineOutput = "";
        var MapFirebase = firebase.Child("Maps").Child(mapName.text);
        MapFirebase.OnSetFailed += SetFailHandler;
        MapFirebase.OnSetSuccess += SetOKHandler;
        MapFirebase.SetValue(mapItems);
        var mapData= new Dictionary<string, object>();
        if (isPublished)
        {
            mapData["published"] = "yes";
        }
        else
        {
            mapData["published"] = "no";
        }
        mapData["lastModified"] = MMAR.SystemInfo.SystemDateTimeinMills;
        firebase.Child("MapNames").Child(mapName.text).SetValue(mapData);
        BikeControl.lastMapName = mapName.text;
    }
    public void Load()
    {
        if (BikeControl.EditGame)
        {
            bikeControl.OnEditButtonClicked(false);
        }
        if (mapRowController.gameObject.activeInHierarchy)
        {
            mapRows.Clear();
            mapRowController.setData(mapRows);
            mapRowController.gameObject.SetActive(false);
        }
        else
        {
            mapRowController.gameObject.SetActive(true);
            var mapNamesFirebase = firebase.Child("MapNames");
            mapNamesFirebase.OnGetFailed += GetFailHandler;
            mapNamesFirebase.OnGetSuccess += GetMapNamesHandler;
            mapNamesFirebase.GetValue();
        }
    }
    #region Firebase handlers
    void GetMapNamesHandler(Firebase sender, DataSnapshot snapshot)
    {
        if (BikeControl.EditGame)
        {
            bikeControl.OnEditButtonClicked(false);
        }
        DebugLog("[OK] Get from key: <" + sender.FullKey + ">");
        DebugLog("[OK] Raw Json: " + snapshot.RawJson);
        if (snapshot.RawJson == "null")
        {
            ShowNotice("There is no saved maps.");
            if (mapRowController.gameObject.activeInHierarchy)
            {
                Load();
            }
        }
        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;
        //var mapData = new Dictionary<string, object>();
        //mapData["published"] = "no";
        //mapData["lastModified"] = MMAR.SystemInfo.SystemDateTimeinMills;
        if (keys != null && mapRowController.gameObject.activeInHierarchy)
        {
            foreach (string key in keys)
            {
                var data=(Dictionary<string, object>)dict[key];
                mapRows.Add(new(key, data["published"].ToString()));
                //DebugLog(key + " = " + dict[key].ToString());
                //dict[key] = mapData;
            }
            //firebase.Child("MapNames").SetValue(dict);
            mapRowController.setData(mapRows);
        }
    }
    void GetMapDataHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Get from key: <" + sender.FullKey + ">");
        DebugLog("[OK] Raw Json: " + snapshot.RawJson);

        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;

        if (keys != null)
        {
            bikeControl.LoadBike(new(-5.65f, 2.64f));
            Time.timeScale = 0;
            #region reseting data
            ClearChildrens(JustLine.transform);
            ClearChildrens(PhysicsLine.transform);
            ClearChildrens(StarManager.transform);
            ClearChildrens(BoostManager.transform);
            ClearChildrens(RedFlagManager.transform);
            ClearChildrens(YellowFlagManager.transform);
            mapName.text = lastMapName;

            #endregion


            //string physicsLineOutput = "", justLineOutPut = "", starOutput = "", boostOutput = "", yellowFlagOutput = "", redFlagOutput = "";
            foreach (string key in keys)
            {
                DebugLog(key + " = " + dict[key].ToString());
                var lineData = (Dictionary<string, object>)dict[key];
                
                switch (lineData["type"].ToString())
                {
                    case "physicsLine":
                        PhysicsLine.Enable = true;
                        PhysicsLine.BeginDraw();
                        var points = (Dictionary<string, object>)lineData["points"];

                        for(int i = 0; i < points.Keys.Count; i++)
                        {
                            var data = points["point"+i].ToString().Split(",");
                            PhysicsLine.AddPoint(new(float.Parse(data[0]), float.Parse(data[1])));
                            //physicsLineOutput+= data[0] + "," + data[1]+floatSeperator;
                        }
                        //physicsLineOutput += ItemSeperator;
                        PhysicsLine.EndDraw();
                        PhysicsLine.Enable = false;
                        break;
                    case "justLine":
                        JustLine.Enable = true;
                        JustLine.BeginDraw();
                        var jpoints = (Dictionary<string, object>)lineData["points"];
                        for (int i = 0; i < jpoints.Keys.Count; i++)
                        {
                            var data = jpoints["point" + i].ToString().Split(",");
                            //justLineOutPut += data[0] + "," + data[1] + floatSeperator;
                            JustLine.AddPoint(new(float.Parse(data[0]), float.Parse(data[1])));
                        }
                        //justLineOutPut += ItemSeperator;
                        JustLine.EndDraw();
                        JustLine.Enable = false;
                        break;
                    case "Star":
                        var sdata = lineData["position"].ToString().Split(",");
                        //starOutput += sdata[0] + "," + sdata[1] + floatSeperator;
                        StarManager.Create(new(float.Parse(sdata[0]), float.Parse(sdata[1])));
                        break;
                    case "Boost":
                        var bdata = lineData["position"].ToString().Split(",");
                        //boostOutput += bdata[0] + "," + bdata[1] + floatSeperator;
                        BoostManager.Create(new(float.Parse(bdata[0]), float.Parse(bdata[1])));
                        break;
                    case "YellowFlag":
                        var ydata = lineData["position"].ToString().Split(",");
                        //yellowFlagOutput+= ydata[0]+ "," + ydata[1]+ floatSeperator;
                        YellowFlagManager.Create(new(float.Parse(ydata[0]), float.Parse(ydata[1])));
                        break;
                    case "RedFlag":
                        var rdata = lineData["position"].ToString().Split(",");
                        //redFlagOutput += rdata[0] + "," + rdata[1] + floatSeperator;
                        RedFlagManager.Create(new(float.Parse(rdata[0]), float.Parse(rdata[1])));
                        break;
                }
            }
            //PlayerPrefs.SetString("physicsLine", physicsLineOutput);
            //PlayerPrefs.SetString("justLine", justLineOutPut);
            //PlayerPrefs.SetString("Star", starOutput);
            //PlayerPrefs.SetString("Boost", boostOutput);
            //PlayerPrefs.SetString("YellowFlag", yellowFlagOutput);
            //PlayerPrefs.SetString("RedFlag", redFlagOutput);
        }
        LoadCompleted = true;
        BikeControl.restartGame = true;
        bikeControl.GameOverMenu.SetActive(false);
        BikeControl.GameOver = false;
        BikeControl.PlayGame = false;
        bikeControl.SideMapButtonsDiv.SetActive(true);
        bikeControl.GameControl.SetActive(true);
        HideNotice();
        //BikePosition = new(-5.65f, 2.64f);

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        ShowNotice("Check your internet connection");
        DebugLog("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void SetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        ShowNotice("Map saved");
        DebugLog("[OK] Set from key: <" + sender.FullKey + ">");
    }

    void SetFailHandler(Firebase sender, FirebaseError err)
    {
        ShowNotice("Internet connection error.\nPlease check your connection.");
        DebugLog("[ERR] Set from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }
    public void DebugLog(string log)
    {
        if (DebugThis)
        {
            Debug.Log(log);
        }
        
    }
    public void ShowNotice(string msg)
    {

        noticeText.GetComponentInChildren<Text>().text = msg;
        Color color;
        if(ColorUtility.TryParseHtmlString(lightColors[Random.Range(0, lightColors.Count)], out color)){
            noticeText.GetComponent<RawImage>().color = color;
        }
        
        noticeText.gameObject.SetActive(true);
        if (showFoodNotice)
        {
            NoticeDisableAfterTime = Time.time + 3;
            showFoodNotice = false;
        }
        else
        {
            NoticeDisableAfterTime = Time.time + NoticeShowTime;
        }
    }
    public void HideNotice()
    {
        noticeText.gameObject.SetActive(false);
    }
    #endregion
}
