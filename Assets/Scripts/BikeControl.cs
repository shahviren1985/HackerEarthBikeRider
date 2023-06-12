
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System;

public class BikeControl : MonoBehaviour
{
    private Rigidbody2D frontWheel,backWheel;
    Transform Bike;
    public GameObject BikePrefab,GameOverMenu,GameButtonDivs,GameControl,TopMapEditDiv,ScrollViewDiv,SideMapButtonsDiv,MapDivs;
    public TextMeshProUGUI PlayButtonText;
    public Text MapEditorButtonText;

    private bool moveForward = false, breakBike = false, rotateUp = false, rotateDown = false, lastBoolStorage = false;
    public float BikeSpeed=200, RotateSpeed=1000,MaxTorque=4000;
    public static bool frontTireTouched = true,backTireTouched=true,pickedOne=false;
    public static bool GameOver = false,PlayGame=false,EditGame=false;
    //Static bool for editing map
    public static bool DrawPhysicsLine = false,DrawJustLine = false,EraserIsOn=false, MoveInTheScene=false;
    //Edit Map Buttons
    public RawImage EraserButton, DrawPhysicsLineButton, DrawJustLineButton,MoveToolButton;
    public LinesDrawer LineDrawerPhysics, LineDrawerJust;
    public CameraControl cameraControl;
    public SaveLoad saveLoad;
    public static UnityEngine.Color selectedColor = new UnityEngine.Color(),unselectedColor=new(1,1,1);
    public static Vector2 Position = Vector2.zero,BikePosition=new(-5.65f,2.64f);
    public static string lastMapName = "";
    [Space(30f)]
    public SpawnerManager StarManager,BoostManager,YelloFlagManager,RedFlagManager;
    public FoodDiv foodDiv;
    public static bool ShowFoodDiv = false, RedFlag = false, restartGame = false;
    public static float BoostPower = 0;
    public float BoostPowerMultifier = 1.5f;
    public Button RedFlagManagerButton;
    public static bool ShowGameButtons=false;
    public GameObject LoginSignUpDiv,MenuBackground;
    public static List<string> foodItemNames = new();
    public static List<string> CollectedItems = new();
    public static List<string> SavedCollecttedItems = new();
    public float TimerStartingTime = 0;
    public Text TimerText;
    public GameObject MainGameDiv, ObjectDrawingDiv;
    private bool EditGameLog = false;
    private Vector3 MainGameDivCameraPosition = new Vector3(0, 0, -10),ObjectDrawingDivCameraPosition= new Vector3(0, 0, -10);
    public GameObject objectDivButton;
    public RecipeDiv recipeDiv;
    private void Start()
    {

        LoadBike(new(-5.65f, 2.64f));
        BoostPower = 0;
        ColorUtility.TryParseHtmlString("#A68787", out selectedColor);
        PlayGame = false;
        GameOver = false;
        Time.timeScale = 1;
        RedFlag = false;
        //Drawing starting straight line
        LineDrawerPhysics.Enable = true;
        LineDrawerPhysics.BeginDraw();
        LineDrawerPhysics.AddPoint(new(-6, 0));
        LineDrawerPhysics.AddPoint(new(-5.5f, 0));
        LineDrawerPhysics.AddPoint(new(-5, 0));
        LineDrawerPhysics.AddPoint(new(-4.5f, 0));
        LineDrawerPhysics.AddPoint(new(-4, 0));
        LineDrawerPhysics.AddPoint(new(-3.5f, 0));
        LineDrawerPhysics.AddPoint(new(-3, 0));
        LineDrawerPhysics.EndDraw();
        LineDrawerPhysics.Enable = false;
        ResetFoodItems();
        //if (lastMapName != "")
        //{
        //    LoadSavedMap();
        //    if (!restartGame)
        //    {
        //        this.transform.position = BikePosition + new Vector2(0, 2);
        //        Play();
        //    }
        //}
        
    }

    public void ResetFoodItems()
    {
        foodItemNames.Clear();
        CollectedItems.Clear();
        SavedCollecttedItems.Clear();
        StartCoroutine(getAllData());
    }
    IEnumerator getAllData()
    {
        if (AdminScript.LoadedMapUrl != "")
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://bhaari.s3.ap-south-1.amazonaws.com/groceries/"+AdminScript.LoadedMapUrl+".csv"))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {

                    var data = Resources.Load<TextAsset>("Groceries").text.Split("\n");
                    for (var i = 1; i < data.Length - 1; i++)
                    {
                        foodItemNames.Add(data[i]);
                    }
                    yield return null;
                }
                else
                {
                    var data = webRequest.downloadHandler.text.Split("\n");
                    for (var i = 1; i < data.Length - 1; i++)
                    {
                        foodItemNames.Add(data[i]);
                    }
                }
            }

        }
        else
        {
            var data = Resources.Load<TextAsset>("Groceries").text.Split("\n");
            for (var i = 1; i < data.Length - 1; i++)
            {
                foodItemNames.Add(data[i]);
            }
        }
        yield return null;
    }
    IEnumerator ReturnExtraFoods()
    {
        if (SavedCollecttedItems.Count != CollectedItems.Count)
        {
            for (var i = 0; i < CollectedItems.Count; i++)
            {
                var macthedNotFound = true;
                for(var j=0; j < SavedCollecttedItems.Count; j++)
                {
                    if (CollectedItems[i] == SavedCollecttedItems[j])
                    {
                        macthedNotFound = false;
                        break;
                    }
                }
                if (macthedNotFound)
                {
                    foodItemNames.Add(CollectedItems[i]);
                    CollectedItems.RemoveAt(i);
                    i--;
                }
            }
        }
        
        yield return null;
    }
    public void LoadBike(Vector2 position)
    {
        BoostPower = 0;
        if(Bike!=null)
        {
            UnityEngine.Object.Destroy(Bike.gameObject);
        }
        for(var i = 0; i < StarManager.transform.childCount; i++)
        {
            StarManager.transform.GetChild(i).gameObject.SetActive(true);
        }
        Bike = Instantiate(BikePrefab,MainGameDiv.transform).transform;
        Bike.position = position;
        cameraControl.target=Bike.gameObject;
        cameraControl.followPosition = false;
        cameraControl.followTarget = true;
        cameraControl.FollowTargetSmooth = false;
        frontWheel = Bike.GetChild(0).GetComponent<Rigidbody2D>();
        backWheel = Bike.GetChild(1).GetComponent<Rigidbody2D>();
    }
    //public void LoadSavedMap()
    //{
    //    var outputString = PlayerPrefs.GetString("physicsLine");
    //    if(outputString != "")
    //    {
    //        var data = outputString.Split(SaveLoad.ItemSeperator);
    //        for(var i=0;i<data.Length; i++)
    //        {
    //            var linePoints = data[i].Split(SaveLoad.floatSeperator);
    //            if(linePoints.Length > 1)
    //            {
    //                LineDrawerPhysics.Enable = true;
    //                LineDrawerPhysics.BeginDraw();
    //                for(var j=0;j<linePoints.Length;j++)
    //                {
    //                    if (linePoints[j] != "")
    //                    {
    //                        var point = linePoints[j].Split(",");
    //                        LineDrawerPhysics.AddPoint(new(float.Parse(point[0]), float.Parse(point[1])));
    //                    }
    //                }
    //                LineDrawerPhysics.EndDraw();
    //                LineDrawerPhysics.Enable = false;
    //            }
    //        }
    //    }
    //    outputString = PlayerPrefs.GetString("justLine");
    //    if (outputString != "")
    //    {
    //        var data = outputString.Split(SaveLoad.ItemSeperator);
    //        for (var i = 0; i < data.Length; i++)
    //        {
    //            var linePoints = data[i].Split(SaveLoad.floatSeperator);
    //            if (linePoints.Length > 1)
    //            {
    //                LineDrawerJust.Enable = true;
    //                LineDrawerJust.BeginDraw();
    //                for (var j = 0; j < linePoints.Length; j++)
    //                {
    //                    if (linePoints[j] != "")
    //                    {
    //                        var point = linePoints[j].Split(",");
    //                        LineDrawerJust.AddPoint(new(float.Parse(point[0]), float.Parse(point[1])));
    //                    }
    //                }
    //                LineDrawerJust.EndDraw();
    //                LineDrawerJust.Enable = false;
    //            }
    //        }
    //    }
    //    outputString = PlayerPrefs.GetString("Star");
    //    if (outputString != "")
    //    {
    //        var data=outputString.Split(SaveLoad.floatSeperator);
    //        for(var i=0;i < data.Length; i++)
    //        {
    //            if (data[i] != "")
    //            {
    //                var pos = data[i].Split(",");
    //                StarManager.Create(new(float.Parse(pos[0]), float.Parse(pos[1])));
    //            }
    //        }
    //    }
    //    outputString = PlayerPrefs.GetString("Boost");
    //    if (outputString != "")
    //    {
    //        var data = outputString.Split(SaveLoad.floatSeperator);
    //        for (var i = 0; i < data.Length; i++)
    //        {
    //            if (data[i] != "")
    //            {
    //                var pos = data[i].Split(",");
    //                BoostManager.Create(new(float.Parse(pos[0]), float.Parse(pos[1])));
    //            }
    //        }
    //    }
    //    outputString = PlayerPrefs.GetString("YellowFlag");
    //    if (outputString != "")
    //    {
    //        var data = outputString.Split(SaveLoad.floatSeperator);
    //        for (var i = 0; i < data.Length; i++)
    //        {
    //            if (data[i] != "")
    //            {
    //                var pos = data[i].Split(",");
    //                YelloFlagManager.Create(new(float.Parse(pos[0]), float.Parse(pos[1])));
    //            }
    //        }
    //    }
    //    outputString = PlayerPrefs.GetString("RedFlag");
    //    if (outputString != "")
    //    {
    //        var data = outputString.Split(SaveLoad.floatSeperator);
    //        for (var i = 0; i < data.Length; i++)
    //        {
    //            if (data[i] != "")
    //            {
    //                var pos = data[i].Split(",");
    //                RedFlagManager.Create(new(float.Parse(pos[0]), float.Parse(pos[1])));
    //            }
    //        }
    //    }
    //}
    #region TakeInput
    public void SpeedUpBike(InputAction.CallbackContext callbackContext)
    {
        var inputValue = callbackContext.ReadValue<float>();
        if (!GameOver&&PlayGame)
        {
            switch (inputValue)
            {
                case 1:
                    moveForward = true;
                    breakBike = false;
                    break;
                case -1:
                    breakBike = true;
                    moveForward = false;
                    break;
                case 0:
                    breakBike = false;
                    moveForward = false;
                    break;
            }
        }
    }
    public void RotateBike(InputAction.CallbackContext callbackContext)
    {
        var inputValue = callbackContext.ReadValue<float>();
        //Debug.Log("Rotating");
        if (!GameOver&&PlayGame)
        {
            switch (inputValue)
            {
                case 1:
                    rotateUp = true;
                    rotateDown = false;
                    break;
                case -1:
                    rotateDown = true;
                    rotateUp = false;
                    break;
                case 0:
                    rotateUp = false;
                    rotateDown = false;
                    break;
            }
        }
    }

    #region Top Graphics Edit Button Input
    public static void ResetAllDrawButtons()
    {
        DrawPhysicsLine = false;
        DrawJustLine = false;
        EraserIsOn = false;
        MoveInTheScene = false;
    }
    public void OnSelectDrawPhysicsButton()
    {
        lastBoolStorage = DrawPhysicsLine;
        ResetAllDrawButtons();
        DrawPhysicsLine = !lastBoolStorage;
        if (DrawPhysicsLine)
        {
            saveLoad.ShowNotice("Physics line tool selected");
        }
    }
    public void OnSelectJustDrawButton()
    {
        lastBoolStorage = DrawJustLine;
        ResetAllDrawButtons();
        DrawJustLine = !lastBoolStorage;
        if (DrawJustLine)
        {
            saveLoad.ShowNotice("Just line tool selected");
        }
    }
    public void OnSelectEraserButton()
    {
        lastBoolStorage = EraserIsOn;
        ResetAllDrawButtons();
        EraserIsOn = !lastBoolStorage;
        if (EraserIsOn)
        {
            saveLoad.ShowNotice("Eraser tool selected");
        }
    }
    public void OnClickOnStarButton()
    {
        ResetAllDrawButtons();
        StarManager.Create();
    }
    public void OnClickOnBoosButton()
    {
        ResetAllDrawButtons();
        BoostManager.Create();
    }
    public void OnClickOnYellowFlagButton()
    {
        ResetAllDrawButtons();
        YelloFlagManager.Create();
    }
    public void OnClickOnRedFlagButton()
    {
        ResetAllDrawButtons();
        RedFlagManager.Create();
    }
    public void OnSelectMoveButton()
    {
        lastBoolStorage = MoveInTheScene;
        ResetAllDrawButtons();
        MoveInTheScene = !lastBoolStorage;
        if (MoveInTheScene)
        {
            saveLoad.ShowNotice("Move tool selected");
        }
    }
    #endregion
    #region Left side buttons
    public void OnEditButtonClicked(bool save=true)
    {
        if (EditGame)
        {
            if (save)
            {
                saveLoad.Save();
            }
            
            EditGame = false;
            ResetAllDrawButtons();
            TopMapEditDiv.SetActive(false);
            MapEditorButtonText.text = "Edit Map";
            GameControl.SetActive(true);
        }
        else
        {
            DrawPhysicsLine = true;
            TopMapEditDiv.SetActive(true);
            EditGame = true;
            MapEditorButtonText.text = "Save Map";
            GameControl.SetActive(false);
            MapDivs.SetActive(false);
        }
    }
    #endregion

    #endregion

    public void CheckMaxTorque(Rigidbody2D wheel)
    {
        if (wheel.angularVelocity < -MaxTorque)
        {
            wheel.angularVelocity = -MaxTorque;
        }
        else if (wheel.angularVelocity > MaxTorque/2)
        {
            wheel.angularVelocity = MaxTorque/2;
        }
    }
    private void Update()
    {

        LineDrawerPhysics.Enable = DrawPhysicsLine;
        LineDrawerJust.Enable = DrawJustLine;
        objectDivButton.SetActive(EditGame);
        Position = new(Bike.position.x, Bike.position.y);
        
    }
    private void FixedUpdate()
    {
        if (RedFlag)
        {
            recipeDiv.show();
            Time.timeScale = 0;
            RedFlag = false;
        }
        if (ShowFoodDiv)
        {
            foodDiv.Show();
            ShowFoodDiv = false;
        }
        if (EditGame)
        {
            if (RedFlagManager.transform.childCount > 0)
            {
                RedFlagManagerButton.interactable = false;
            }
            else
            {
                RedFlagManagerButton.interactable = true;
            }
        }

        LoginSignUpDiv.SetActive(!LoginSignUp.LoggedIn);
        MenuBackground.SetActive(!LoginSignUp.LoggedIn);
        //Vector2 fwd = frontWheel.transform.TransformDirection(Vector2.down);
        //Debug.DrawRay(frontWheel.transform.position, fwd * -0.55f, Color.red);
        //RaycastHit2D hit = Physics2D.Raycast(frontWheel.transform.position, fwd * -0.55f);
        //if (hit.collider != null)
        //{
        //    Debug.Log(hit.collider.gameObject.name);
        //}
        #region Drawing bool update
        //CHanging color based on selection
        if (DrawPhysicsLine)
        {
            DrawPhysicsLineButton.color = selectedColor;
        }
        else
        {
            DrawPhysicsLineButton.color = unselectedColor;
        }

        if (DrawJustLine)
        {
            DrawJustLineButton.color = selectedColor;
        }
        else
        {
            DrawJustLineButton.color = unselectedColor;
        }

        if (EraserIsOn)
        {
            EraserButton.color = selectedColor;
        }
        else
        {
            EraserButton.color = unselectedColor;
        }
        if (MoveInTheScene)
        {
            MoveToolButton.color = selectedColor;
        }
        else
        {
            MoveToolButton.color = unselectedColor;
        }
        #endregion
        if (GameOver)
        {
            GameOverMenu.SetActive(true);
            Time.timeScale = 0;
        }
        CheckMaxTorque(frontWheel);
        CheckMaxTorque(backWheel);
        if (moveForward)
        {
            AddForce(Bike.right * Mathf.Abs(frontWheel.angularVelocity + BoostPower * BoostPowerMultifier) / 100);
            backWheel.AddTorque(-BikeSpeed * Time.deltaTime);
            frontWheel.AddTorque(-BikeSpeed * Time.deltaTime);
            //Debug.Log("Adding boost");

        }
        else if (breakBike)
        {
            if (backWheel.angularVelocity < 0)
            {

                AddForce(-Bike.right * Mathf.Abs(frontWheel.angularVelocity) / 100);
                backWheel.AddTorque(BikeSpeed * Time.deltaTime);
                frontWheel.AddTorque(BikeSpeed * Time.deltaTime);
            }
            else
            {
                backWheel.AddTorque(BikeSpeed * Time.deltaTime / 3);
                frontWheel.AddTorque(BikeSpeed * Time.deltaTime / 3);
                AddForce(-Bike.right * Mathf.Abs(frontWheel.angularVelocity) / 300);
            }
        }
        else
        {
            if (frontTireTouched || backTireTouched)
            {
                if (BoostPower > 0)
                {
                    AddForce(Bike.right * Mathf.Abs(frontWheel.angularVelocity + BoostPower * BoostPowerMultifier) / 100);
                }
            }
            ReduceWheelSpeed(frontWheel);
            ReduceWheelSpeed(backWheel);
            
        }
        //Debug.Log(backWheel.angularVelocity);
        //Rotating Bike
        if (rotateUp)
        {
            Vector2 direction = (Bike.position-frontWheel.transform.position).normalized;
            //Debug.Log(transform.forward);
            Bike.GetComponent<Rigidbody2D>().angularVelocity=RotateSpeed;
            //Debug.Log("Rotate up");
            //transform.Rotate(Vector3.forward * RotateSpeed * Time.deltaTime);
        }
        else if (rotateDown)
        {
            Vector2 direction = (Bike.position - frontWheel.transform.position).normalized;
            //Debug.Log(transform.forward);
            Bike.GetComponent<Rigidbody2D>().angularVelocity = -RotateSpeed;
            //transform.Rotate(Vector3.back * RotateSpeed * Time.deltaTime);
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1) * RotateSpeed);
        }
        if (PlayGame)
        {
            var totalTime = Mathf.Floor(Time.time - TimerStartingTime);
            var minute=Mathf.Floor(totalTime/60);
            var seconds=totalTime-minute*60;
            TimerText.text = MakeNumberDouble(minute) + ":" + MakeNumberDouble(seconds);
        }
    }
    private string MakeNumberDouble(float number)
    {
        if (number < 10)
        {
            return "0" + number;
        }
        else
        {
            return number + "";
        }
    }
    //For reducing speed when we are not accelarating
    private void ReduceWheelSpeed(Rigidbody2D wheel)
    {
        var reducingSpeed = 1 + Mathf.Abs(wheel.angularVelocity)/10*Time.deltaTime;
        if (wheel.angularVelocity > 0)
        {
            
            wheel.angularVelocity -= reducingSpeed;
            if(wheel.angularVelocity < 0)
            {
                wheel.angularVelocity = 0;
            }
            else
            {
                AddForce(Bike.right * Mathf.Abs(reducingSpeed) / 200);
            }
        }
        else
        {
            if (BoostPower > 0)
            {
                BoostPower -= 2*reducingSpeed;
            }
            else
            {
                wheel.angularVelocity += reducingSpeed;
                BoostPower = 0;
            }
            
            if (wheel.angularVelocity > 0)
            {
                wheel.angularVelocity = 0;
            }
            AddForce(-Bike.right * Mathf.Abs(reducingSpeed) / 200);
        }
    }
    private void AddForce(Vector2 force)
    {
        if (frontTireTouched || backTireTouched)
        {
            Bike.GetComponent<Rigidbody2D>().AddForce(force);
        }
    }
    public void TryAgain()
    {
        restartGame = false;
        GameOverMenu.SetActive(false);
        GameOver = false;
        PlayGame = true;
        saveLoad.HideNotice();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(ReturnExtraFoods());
        LoadBike(BikePosition + new Vector2(0, 2));
        Time.timeScale = 1;
    }
    public void Restart()
    {
        restartGame = true;
        GameOverMenu.SetActive(false);
        GameOver = false;
        PlayGame = false;
        SideMapButtonsDiv.SetActive(true);
        GameControl.SetActive(true);
        saveLoad.HideNotice();
        //BikePosition = new(-5.65f, 2.64f);
        ResetFoodItems();
        LoadBike(new(-5.65f, 2.64f));
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        TimerStartingTime = Time.time;
    }
    public void NewMap()
    {
        lastMapName = "";
        BikePosition = new(-5.65f, 2.64f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;

    }
    public void ShowHideDrawingObject()
    {
        MainGameDiv.SetActive(!MainGameDiv.activeInHierarchy);
        ObjectDrawingDiv.SetActive(!ObjectDrawingDiv.activeInHierarchy);
        if (MainGameDiv.activeInHierarchy)
        {
            ObjectsEditor.ResetAllDrawButtons();
            EditGame = EditGameLog;
            ObjectDrawingDivCameraPosition = cameraControl.transform.position;
            cameraControl.ResetCamera();
            cameraControl.transform.position = MainGameDivCameraPosition;
        }
        else
        {
            EditGameLog = EditGame;
            MainGameDivCameraPosition = cameraControl.transform.position;
            cameraControl.ResetCamera();
            cameraControl.transform.position = ObjectDrawingDivCameraPosition;
            EditGame = true;
            
        }
    }
    public void RandomMap()
    {

    }
    
    public void Play()
    {
        
        PlayGame = true;
        cameraControl.followPosition = false;
        cameraControl.followTarget = true;
        if (ShowGameButtons)
        {
            GameButtonDivs.SetActive(true);
        }
        ScrollViewDiv.SetActive(false);
        SideMapButtonsDiv.SetActive(false);
        TimerStartingTime = Time.time;
    }
    public void showGameButtons(string msg)
    {
        ShowGameButtons= true;
    }
}
