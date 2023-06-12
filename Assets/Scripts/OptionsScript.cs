using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsScript : MonoBehaviour
{
    public GameObject OptionButton,OptionsDiv;

    public void ShowOptions()
    {
        if (LoginSignUp.LoggedIn)
        {
            OptionsDiv.SetActive(!OptionsDiv.activeInHierarchy);
        }
    }
    private void FixedUpdate()
    {
        OptionButton.SetActive(LoginSignUp.IsAdmin&&!BikeControl.PlayGame&&!BikeControl.GameOver);
        if(BikeControl.PlayGame)
        {
            OptionsDiv.SetActive(false);
        }
    }
}
