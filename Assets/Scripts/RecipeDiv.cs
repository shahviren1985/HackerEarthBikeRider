using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecipeDiv : MonoBehaviour
{
    public Text RecipeTitle, RecipeDescription;
    public GameObject NoticeText;
    private JSONObject curJsonObject;
    private long lastTimeShown = 0, noticeDuration = 30000000;
    public RawImage FoodImage;
    public void show()
    {
        gameObject.SetActive(true);
        StartCoroutine(GetRecipeData());
        StartCoroutine(keepNoticeRunning());
    }
    public void ShowNotice(string text)
    {
        NoticeText.GetComponentInChildren<Text>().text = text;
        Color color;
        if (ColorUtility.TryParseHtmlString(SaveLoad.lightColors[UnityEngine.Random.Range(0, SaveLoad.lightColors.Count)], out color))
        {
            NoticeText.GetComponent<RawImage>().color = color;
        }
        NoticeText.SetActive(true);
        lastTimeShown = DateTime.Now.Ticks;
        //StopCoroutine(keepNoticeRunning());
        StartCoroutine(keepNoticeRunning());
    }
    IEnumerator keepNoticeRunning()
    {
        while (NoticeText.gameObject.activeInHierarchy)
        {
            Debug.Log(DateTime.Now.Ticks);
            Debug.Log(lastTimeShown);
            Debug.Log(DateTime.Now.Ticks - lastTimeShown);
            Debug.Log(noticeDuration);
            Debug.Log(DateTime.Now.Ticks - lastTimeShown > noticeDuration);
            if (DateTime.Now.Ticks - lastTimeShown > noticeDuration)
            {
                NoticeText.gameObject.SetActive(false);
            }
            yield return new WaitForSecondsRealtime(.5f);
        }
        yield return null;
    }
    public void Order()
    {
        ShowNotice("Your order is done");
    }
    public void Share()
    {
        ShowNotice("Recipe is sharing");
    }
    [DllImport("__Internal")]
    private static extern void getRecipeData(string text,string mapid);
    public void PutRecipeData(string data)
    {
        Debug.Log(data);
        var jsondata = new JSONObject(data);
        RecipeTitle.text = jsondata["name"].stringValue.TrimStart('\n');
        RecipeDescription.text= jsondata["recipe"].stringValue.TrimStart('\n');
        try
        {
            if (jsondata["images"] != null)
            {
                StartCoroutine(SetFoodImage(jsondata["images"].list[0].stringValue));
            }
        }
        catch
        {

        }
    }
    IEnumerator SetFoodImage(string Url)
    {
        using (WWW www = new WWW(Url))
        {
            yield return www;

            Texture2D texture = new Texture2D(www.texture.width, www.texture.height);
            www.LoadImageIntoTexture(texture);

            FoodImage.texture = texture;
        }
    }
    IEnumerator GetRecipeData()
    {
        var collectedItems = "";
        if(BikeControl.CollectedItems.Count > 1 ) {
            for(int i = 0;i<BikeControl.CollectedItems.Count;i++)
            {
                if(i==BikeControl.CollectedItems.Count-2)
                {
                    collectedItems += BikeControl.CollectedItems[i] + " and ";
                }
                else if (i == BikeControl.CollectedItems.Count - 1)
                {
                    collectedItems += BikeControl.CollectedItems[i];
                }
                else
                {
                    collectedItems += BikeControl.CollectedItems[i] + ",";
                }
            }
        }else if(BikeControl.CollectedItems.Count == 1)
        {
            collectedItems = BikeControl.CollectedItems[0];
        }
        getRecipeData(collectedItems,BikeControl.lastMapName);
        //yield return PostRequest("Write a recipe that only uses following ingredients:olive oil,egg plant and baby tomato. First 24 characters should be disH name");
        //yield return PostRequest("Write a recipe that only uses following ingredients:" + collectedItems + ". First 24 characters should be disH name");
        //if (curJsonObject != null)
        //{
        //    RecipeTitle.text = curJsonObject["name"].stringValue;
        //    //yield return PostRequest("Write a recipe that only uses the following ingredients:"+collectedItems);
        //    RecipeDescription.text = curJsonObject["recipe"].stringValue;
        //}
        yield return null;
    }
    IEnumerator PostRequest(string data)
    {

        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        //formData.Add(new MultipartFormDataSection("data", data));
        //formData.Add(new MultipartFormDataSection("nameData", "-"));
        //formData.Add(new MultipartFormDataSection("mapid", BikeControl.lastMapName));
        //UnityWebRequest www = UnityWebRequest.Post("https://square.bhaari.com/api/recipes/create-recipe", formData);
        var jsonData = "{\r\n\"data\":\"" + data + "\",\r\n\"nameData\": \"-\",\r\n\"mapid\":\"" + BikeControl.lastMapName + "\"\r\n}";
        UnityWebRequest www = new UnityWebRequest("https://square.bhaari.com/api/recipes/create-recipe", "POST");
        byte[] bytedata = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bytedata); // important
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Started");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            yield return PostRequest(data);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            curJsonObject = new(www.downloadHandler.text);
            //Debug.Log(curJsonObject["choices"].list[0]["text"].stringValue.TrimStart('\n'));
            //Debug.Log("Form upload complete!");
        }
        Debug.Log("Finished");
    }
}
