using SimpleFirebaseUnity;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSaveLoad : MonoBehaviour
{
    public LinesDrawer PhysicsLine, JustLine;
    public SpawnerManager StarManager,BoostManager,YellowFlagManager;
    public InputField Objectame;
    public bool DebugThis;
    private Firebase firebase;
    public static string loadMapName = "";
    private string lastMapName = "";
    public ObjectRowController mapRowController;
    private List<MapRowData> mapRows=new();
    public static bool LoadCompleted = false;
    public static string floatSeperator = "*", ItemSeperator = "###";
    public StartingPoint startPoint;
    public CameraControl cameraControl;
    private Vector2 startPointDefaultPosition = new(-9.8f, -0.5f);
    public bool thisIsMainDiv = false;
    private string MainDivObjectName = "";
    public GameObject confirmDiv;
    public Text confirmDivText;
    // Start is called before the first frame update
    private void Start()
    {
        firebase=Firebase.CreateNew(FirebaseConfig.url, FirebaseConfig.key);
        GenerateObjectName();
    }
    public void GenerateObjectName()
    {
        Objectame.text = "Object-" + Random.Range(0, 100000000);
    }
    private void FixedUpdate()
    {
        if (loadMapName != "")
        {
            if (!thisIsMainDiv)
            {
                var MapFirebase = firebase.Child("Objects").Child(loadMapName);
                MapFirebase.OnGetFailed += GetFailHandler;
                MapFirebase.OnGetSuccess += GetMapDataHandler;
                MapFirebase.GetValue();
                lastMapName = loadMapName;
                loadMapName = "";
            }
            else
            {
                BikeControl.ResetAllDrawButtons();
                MainDivObjectName = loadMapName;
                loadMapName = "";
                var tempLocation = cameraControl.transform.position;
                tempLocation.z = 0;
                startPoint.transform.position = tempLocation;
                startPoint.gameObject.SetActive(true);
                confirmDiv.SetActive(true);
                confirmDivText.text = "Import " + MainDivObjectName + "?";
            }
        }
        if (thisIsMainDiv)
        {
            if (!BikeControl.EditGame)
            {
                confirmDiv.SetActive(false);
                startPoint.gameObject.SetActive(false);
            }
        }
    }
    public void CancelConfirmDiv()
    {
        startPoint.gameObject.SetActive(false);
        confirmDiv.SetActive(false);

    }
    public void OkConfirmDiv()
    {
        CancelConfirmDiv();
        var MapFirebase = firebase.Child("Objects").Child(MainDivObjectName);
        MapFirebase.OnGetFailed += GetFailHandler;
        MapFirebase.OnGetSuccess += GetMapDataHandler;
        MapFirebase.GetValue();
    }
    public void Publish()
    {
        Save();
    }
    public void ClearMap()
    {
        ClearChildrens(JustLine.transform);
        ClearChildrens(PhysicsLine.transform);
        ClearChildrens(StarManager.transform);
        ClearChildrens(BoostManager.transform);
        //ClearChildrens(RedFlagManager.transform);
        ClearChildrens(YellowFlagManager.transform);
        startPoint.transform.position = startPointDefaultPosition;
        cameraControl.transform.position = new(0, 0, -10);
    }

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
                points["point"+j] = "" + (lineData.points[j].x-startPoint.transform.position.x) + "," + (lineData.points[j].y-startPoint.transform.position.y);
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
                points["point" + j] = "" + (lineData.points[j].x - startPoint.transform.position.x) + "," + (lineData.points[j].y - startPoint.transform.position.y);
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
            line["position"] = (StarManager.transform.GetChild(i).transform.position.x-startPoint.transform.position.x) + "," + (StarManager.transform.GetChild(i).transform.position.y-startPoint.transform.position.y);
            //lineOutput = StarManager.transform.GetChild(i).transform.position.x + "," + StarManager.transform.GetChild(i).transform.position.y + floatSeperator;
            mapItems["Star"+i]=line;
        }
        //PlayerPrefs.SetString("Star", lineOutput);
        //lineOutput = "";
        for (var i = 0; i < BoostManager.transform.childCount; i++)
        {
            Dictionary<string, object> line = new();
            line["type"] = "Boost";
            line["position"] = (BoostManager.transform.GetChild(i).transform.position.x - startPoint.transform.position.x) + "," + (BoostManager.transform.GetChild(i).transform.position.y - startPoint.transform.position.y);
            //lineOutput = BoostManager.transform.GetChild(i).transform.position.x + "," + BoostManager.transform.GetChild(i).transform.position.y + floatSeperator;

            mapItems["Boost" + i] = line;
        }
        //PlayerPrefs.SetString("Boost", lineOutput);
        //lineOutput = "";
        for (var i = 0; i < YellowFlagManager.transform.childCount; i++)
        {
            Dictionary<string, object> line = new();
            line["type"] = "YellowFlag";
            line["position"] = (YellowFlagManager.transform.GetChild(i).transform.position.x - startPoint.transform.position.x) + "," + (YellowFlagManager.transform.GetChild(i).transform.position.y - startPoint.transform.position.y);
            //lineOutput = YellowFlagManager.transform.GetChild(i).transform.position.x + "," + YellowFlagManager.transform.GetChild(i).transform.position.y + floatSeperator;

            mapItems["YellowFlag" + i] = line;
        }
        //PlayerPrefs.SetString("YellowFlag", lineOutput);
        //lineOutput = "";
        //for (var i = 0; i < RedFlagManager.transform.childCount; i++)
        //{
        //    Dictionary<string, object> line = new();
        //    line["type"] = "RedFlag";
        //    line["position"] = RedFlagManager.transform.GetChild(i).transform.position.x + "," + RedFlagManager.transform.GetChild(i).transform.position.y;
        //    //lineOutput = RedFlagManager.transform.GetChild(i).transform.position.x + "," + RedFlagManager.transform.GetChild(i).transform.position.y + floatSeperator;

        //    mapItems["RedFlag" + i] = line;
        //}
        //PlayerPrefs.SetString("RedFlag", lineOutput);
        //lineOutput = "";
        var MapFirebase = firebase.Child("Objects").Child(Objectame.text);
        MapFirebase.OnSetFailed += SetFailHandler;
        MapFirebase.OnSetSuccess += SetOKHandler;
        MapFirebase.SetValue(mapItems);
        var mapData= new Dictionary<string, object>();
        mapData["lastModified"] = MMAR.SystemInfo.SystemDateTimeinMills;
        firebase.Child("ObjectNames").Child(Objectame.text).SetValue(mapData);
    }
    public void Load()
    {
        if (mapRowController.gameObject.activeInHierarchy)
        {
            mapRows.Clear();
            mapRowController.setData(mapRows);
            mapRowController.gameObject.SetActive(false);
        }
        else
        {
            mapRowController.gameObject.SetActive(true);
            var mapNamesFirebase = firebase.Child("ObjectNames");
            mapNamesFirebase.OnGetFailed += GetFailHandler;
            mapNamesFirebase.OnGetSuccess += GetMapNamesHandler;
            mapNamesFirebase.GetValue();
        }
    }
    #region Firebase handlers
    void GetMapNamesHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Get from key: <" + sender.FullKey + ">");
        DebugLog("[OK] Raw Json: " + snapshot.RawJson);
        if (snapshot.RawJson == "null")
        {
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
                mapRows.Add(new(key));
                //DebugLog(key + " = " + dict[key].ToString());
                //dict[key] = mapData;
            }
            //firebase.Child("ObjectNames").SetValue(dict);
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
            Time.timeScale = 0;
            #region reseting data
            if (!thisIsMainDiv)
            {
                ClearChildrens(JustLine.transform);
                ClearChildrens(PhysicsLine.transform);
                ClearChildrens(StarManager.transform);
                ClearChildrens(BoostManager.transform);
                //ClearChildrens(RedFlagManager.transform);
                ClearChildrens(YellowFlagManager.transform);
                Objectame.text = lastMapName;
            }

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
                            PhysicsLine.AddPoint(new(float.Parse(data[0])+startPoint.transform.position.x, float.Parse(data[1])+startPoint.transform.position.y));
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
                            JustLine.AddPoint(new(float.Parse(data[0]) + startPoint.transform.position.x, float.Parse(data[1]) + startPoint.transform.position.y));
                        }
                        //justLineOutPut += ItemSeperator;
                        JustLine.EndDraw();
                        JustLine.Enable = false;
                        break;
                    case "Star":
                        var sdata = lineData["position"].ToString().Split(",");
                        //starOutput += sdata[0] + "," + sdata[1] + floatSeperator;
                        StarManager.Create(new(float.Parse(sdata[0]) + startPoint.transform.position.x, float.Parse(sdata[1]) + startPoint.transform.position.y));
                        break;
                    case "Boost":
                        var bdata = lineData["position"].ToString().Split(",");
                        //boostOutput += bdata[0] + "," + bdata[1] + floatSeperator;
                        BoostManager.Create(new(float.Parse(bdata[0]) + startPoint.transform.position.x, float.Parse(bdata[1]) + startPoint.transform.position.y));
                        break;
                    case "YellowFlag":
                        var ydata = lineData["position"].ToString().Split(",");
                        //yellowFlagOutput+= ydata[0]+ "," + ydata[1]+ floatSeperator;
                        YellowFlagManager.Create(new(float.Parse(ydata[0]) + startPoint.transform.position.x, float.Parse(ydata[1]) + startPoint.transform.position.y));
                        break;
                    //case "RedFlag":
                    //    var rdata = lineData["position"].ToString().Split(",");
                    //    //redFlagOutput += rdata[0] + "," + rdata[1] + floatSeperator;
                    //    RedFlagManager.Create(new(float.Parse(rdata[0]), float.Parse(rdata[1])));
                    //    break;
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
        //BikePosition = new(-5.65f, 2.64f);

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        DebugLog("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void SetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Set from key: <" + sender.FullKey + ">");
    }

    void SetFailHandler(Firebase sender, FirebaseError err)
    {
        DebugLog("[ERR] Set from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }
    public void DebugLog(string log)
    {
        if (DebugThis)
        {
            Debug.Log(log);
        }
        
    }
   
    #endregion
}
