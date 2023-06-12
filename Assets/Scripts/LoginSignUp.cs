using Defective.JSON;
using EnhancedScrollerDemos.JumpToDemo;
using SimpleFirebaseUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginSignUp : MonoBehaviour
{
    public Text UserName, UserStatus;
    public static bool LoggedIn = false, IsAdmin = false;
    public Texture2D selectedLeftTexture, selectedRightTexture, unselectedLeftTexture, unselectedRightTexture;
    public RawImage LoginHeader, RegistrationHeader;
    public GameObject LoginBody, RegistrationBody;
    [Space(20)]
    public InputField LoginUserEmailText;
    public InputField LoginPasswordText;
    public InputField RegistrationName, RegistrationEmail;
    public InputField RegistrationPasswordText;
    public Text LoginSignUpNotice;
    public float NoticeShowTime = 3;
    public float LastNoticeShowTime = 0;
    Firebase firebase;
    bool LoginCheck = false;
    public bool DebugThis = false;
    public BikeControl bikeControl;
    private JSONObject curJsonObject;
    // Start is called before the first frame update
    void Start()
    {
        firebase = Firebase.CreateNew(FirebaseConfig.url, FirebaseConfig.key);
        IsAdmin = false;
        //LoggedIn = true;
        var curUserName = PlayerPrefs.GetString("username");
        if (curUserName != "")
        {
            LoggedIn = true;
        }
        if (PlayerPrefs.GetString("userstatus") == "Admin")
        {
            IsAdmin = true;
        }
        UserName.text = PlayerPrefs.GetString("username");
        UserStatus.text = PlayerPrefs.GetString("userstatus");
    }
    public void ShowLogin()
    {
        LoginHeader.texture = selectedRightTexture;
        RegistrationHeader.texture = unselectedLeftTexture;
        LoginBody.SetActive(true);
        RegistrationBody.SetActive(false);
    }
    public void ShowRegistration()
    {
        LoginHeader.texture = unselectedRightTexture;
        RegistrationHeader.texture = selectedLeftTexture;
        LoginBody.SetActive(false);
        RegistrationBody.SetActive(true);
    }
    public void LogOut()
    {
        LoggedIn = false;
        PlayerPrefs.SetString("username", "");
        PlayerPrefs.SetString("userstatus", "");
        bikeControl.NewMap();
    }
    public void Login()
    {
        if (AdminScript.LoadedMapUrl != "")
        {
            if (AdminScript.MapLoggedIn)
            {
                ContinueLogin();
            }
            else
            {
                ShowNotice("Map id is not published or valid");
            }
        }
        else
        {
            ContinueLogin();
        }
    }
    private void ContinueLogin()
    {
        if (LoginUserEmailText.text == "")
        {
            ShowNotice("Please insert user name or email");
        }
        else if (LoginPasswordText.text == "")
        {
            ShowNotice("Please insert password");
        }
        else
        {
            var loginFirebase = firebase.Child("Users");
            LoginCheck = true;
            loginFirebase.OnGetSuccess += GetUserData;
            loginFirebase.OnGetFailed += GetUserError;
            loginFirebase.GetValue();
        }
    }
    public void SignUp()
    {
        if (AdminScript.LoadedMapUrl != "")
        {
            if (AdminScript.MapLoggedIn)
            {
                ContinueSignUp();
            }
            else
            {
                ShowNotice("Map id is not published or valid");
            }
        }
        else
        {
            ContinueSignUp();
        }
    }
    public void ContinueSignUp()
    {
        if (RegistrationName.text == "")
        {
            ShowNotice("Please insert user name");
        }
        else if (RegistrationEmail.text == "")
        {
            ShowNotice("Please insert email");
        }
        else if (RegistrationPasswordText.text == "")
        {
            ShowNotice("Please insert password");
        }
        else
        {
            var loginFirebase = firebase.Child("Users");
            LoginCheck = false;
            loginFirebase.OnGetSuccess += GetUserData;
            loginFirebase.OnGetFailed += GetUserError;
            loginFirebase.GetValue();
        }
    }
    public void ShowNotice(string notice)
    {
        LoginSignUpNotice.text = notice;
        LoginSignUpNotice.gameObject.SetActive(true);
        LastNoticeShowTime = Time.time;
    }
    public void FixedUpdate()
    {
        if (Time.time - LastNoticeShowTime > NoticeShowTime)
        {
            LoginSignUpNotice.gameObject.SetActive(false);
        }
    }
    #region FirebaseHandlers
    void SignUpOk(Firebase sender, DataSnapshot snapshot)
    {
        LoggedIn = true;
        UserName.text = RegistrationName.text;
        UserStatus.text = "User";
        PlayerPrefs.SetString("username", UserName.text);
        PlayerPrefs.SetString("userstatus",UserStatus.text);
        if (AdminScript.MapLoggedIn)
        {
            StartCoroutine(PostRequest(RegistrationName.text, RegistrationEmail.text));
        }
    }
    void SignUpfailed(Firebase sender, FirebaseError snapshot)
    {
        ShowNotice("Sign Up error. Please try again");
    }
    void sendSignupdata()
    {
        var signupFirebase= firebase.Child("Users").Child(RegistrationName.text);
        Dictionary<string,object> userData= new Dictionary<string,object>();
        userData["email"] = RegistrationEmail.text;
        userData["password"] = RegistrationPasswordText.text;
        userData["status"] = "User";
        signupFirebase.OnSetFailed += SignUpfailed;
        signupFirebase.OnSetSuccess += SignUpOk;
        signupFirebase.SetValue(userData);
    }
    IEnumerator PostRequest(string name,string email)
    {
        Debug.Log("Started");
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        string secondName = "";
        var nameParts = name.Split(" ", 2);
        if (nameParts.Length > 1)
        {
            secondName = nameParts[1];
        }
        //formData.Add(new MultipartFormDataSection("action", "open_ai"));
        //formData.Add(new MultipartFormDataSection("data", "{\r\n\"name\": \""+ nameParts[0] + "\",\r\n\"lastName\": \""+secondName+"\",\r\n\""+email+"\": \"shah.viren1985@gmail.com\",\r\n\"mapid\": \""+ AdminScript.LoadedMapUrl + "\"\r\n}"));
        formData.Add(new MultipartFormDataSection("name", nameParts[0]));
        formData.Add(new MultipartFormDataSection("lastName", secondName));
        formData.Add(new MultipartFormDataSection("email", email));
        formData.Add(new MultipartFormDataSection("mapid", AdminScript.LoadedMapUrl));
        UnityWebRequest www = UnityWebRequest.Post("https://square.bhaari.com/api/customers/create-customer", formData);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            yield return PostRequest(name,email);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            curJsonObject = new(www.downloadHandler.text);
            //Debug.Log(curJsonObject["choices"].list[0]["text"].stringValue.TrimStart('\n'));
            //Debug.Log("Form upload complete!");
        }
        Debug.Log("Finished");
    }
    void GetUserData(Firebase sender, DataSnapshot snapshot)
    {
        if (snapshot.RawJson == "null")
        {
            if (LoginCheck)
            {
                ShowNotice("Wrong username/email or password");
            }
            else
            {
                sendSignupdata();
            }
        }
        else
        {
            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
            if (LoginCheck)
            {
                foreach(var key in dict.Keys)
                {
                    if (!LoginUserEmailText.text.Contains("@"))
                    {
                        //Debug.Log(key);
                        //Debug.Log(LoginUserEmailText.text);
                        if (key == LoginUserEmailText.text)
                        {
                            //Debug.Log("Matched");
                            Dictionary<string, object> dataDict = (Dictionary<string, object>)dict[key];
                            //Debug.Log(dataDict["password"].ToString());
                            //Debug.Log(LoginPasswordText.text);
                            if (dataDict["password"].ToString() == LoginPasswordText.text)
                            {
                                UserName.text = key;
                                UserStatus.text = "User";

                                if (dataDict["status"].ToString() == "Admin")
                                {
                                    IsAdmin = true;
                                    UserStatus.text = "Admin";
                                }
                                PlayerPrefs.SetString("username", UserName.text);
                                PlayerPrefs.SetString("userstatus", UserStatus.text);
                                if (AdminScript.MapLoggedIn)
                                {
                                    StartCoroutine(PostRequest(key, dataDict["email"].ToString()));
                                }
                                //StartCoroutine(PostRequest(key, dataDict["email"].ToString()));
                                LoggedIn = true;
                            }
                        }
                        //DebugLog("not contains @");
                    }
                    
                    else
                    {
                        //DebugLog("contains @");
                        Dictionary<string,object> dataDict=(Dictionary<string, object>)dict[key];
                        if (dataDict["email"].ToString() == LoginUserEmailText.text)
                        {
                            if (dataDict["password"].ToString()==LoginPasswordText.text)
                            {
                                UserName.text = key;
                                UserStatus.text = "User";
                                if (dataDict["status"].ToString() == "Admin")
                                {
                                    IsAdmin = true;
                                    UserStatus.text = "Admin";
                                }
                                PlayerPrefs.SetString("username", UserName.text);
                                PlayerPrefs.SetString("userstatus", UserStatus.text);
                                LoggedIn = true;
                                if (AdminScript.MapLoggedIn)
                                {
                                    StartCoroutine(PostRequest(key, dataDict["email"].ToString()));
                                }
                            }
                        }
                    }
                }
                if (!LoggedIn)
                {
                    ShowNotice("Wrong username/email or password");
                }
            }
            else
            {
                var signUpFailed = false;
                foreach (var key in dict.Keys)
                {
                    if (key == RegistrationName.text)
                    {
                        signUpFailed = true;
                        ShowNotice("User name is already taken");
                        break;
                    }
                    else
                    {
                        Dictionary<string, object> dataDict = (Dictionary<string, object>)dict[key];
                        if (dataDict["email"].ToString() == RegistrationEmail.text)
                        {
                            ShowNotice("Email account is already exists");
                            signUpFailed=true;
                            break;
                        }
                    }
                }
                if (!signUpFailed)
                {
                    sendSignupdata();
                }
            }
        }
    }
    void GetUserError(Firebase sender, FirebaseError snapshot)
    {
        ShowNotice("Internet connection error. Try again");
    }
    #endregion
    #region InputField change
    public void LoginUserNameInputDone()
    {
        LoginPasswordText.Select();
    }
    public void SignUpUserNameInputDone()
    {
        RegistrationEmail.Select();
    }
    public void SignUpUserEmailInputDone()
    {
        RegistrationPasswordText.Select();
    }
    #endregion
    public void DebugLog(object msg)
    {
        if (DebugThis)
        {
            Debug.Log(msg);
        }
    }
}