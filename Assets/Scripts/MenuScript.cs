using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public float Speed = 50;
    public RectTransform menuHolderRect;
    public bool MenuOpen = false;
    private float startX = 0;
    public Texture2D menuCloseTexture, menuOpenTexture;
    public Image MenuIcon;
    public float MenusWidth = 280;
    // Start is called before the first frame update
    void Start()
    {
        startX = menuHolderRect.localPosition.x+MenusWidth;
    }

    public void ShowMenu()
    {
        MenuOpen = !MenuOpen;
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        
        var tempPosition=menuHolderRect.localPosition;
        if (MenuOpen)
        {
            if (tempPosition.x < startX)
            {
                tempPosition.x += Time.deltaTime * Speed;
                if(tempPosition.x > startX)
                {
                    tempPosition.x = startX;
                }
                menuHolderRect.localPosition = tempPosition;
                //Debug.Log(menuHolderRect.localPosition.x + "," + menuHolderRect.localPosition.y + "," + menuHolderRect.localPosition.z);
            }
        }
        else
        {
            if (tempPosition.x > startX- MenusWidth)
            {
                tempPosition.x -= Time.deltaTime * Speed;
                if (tempPosition.x < startX - MenusWidth)
                {
                    tempPosition.x = startX - MenusWidth;
                }
                menuHolderRect.localPosition = tempPosition;
                //Debug.Log(menuHolderRect.localPosition.x+","+ menuHolderRect.localPosition.y + "," + menuHolderRect.localPosition.z);
            }
        }
    }
}
