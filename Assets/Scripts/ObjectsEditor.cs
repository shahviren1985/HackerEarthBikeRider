using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectsEditor : MonoBehaviour
{
    public SpawnerManager StarManager, BoostManager, YelloFlagManager, RedFlagManager;
    public RawImage EraserButton, DrawPhysicsLineButton, DrawJustLineButton, MoveToolButton;
    public LinesDrawer LineDrawerPhysics, LineDrawerJust;
    public Transform startPoint;
    
    public static bool DrawPhysicsLine = false,
    DrawJustLine = false,
        EraserIsOn = false,
        MoveInTheScene = false;
    private bool lastBoolStorage;
    public SaveLoad saveLoad;
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

    private void Update()
    {
        LineDrawerPhysics.Enable = DrawPhysicsLine;
        LineDrawerJust.Enable = DrawJustLine;
    }
    private void FixedUpdate()
    {
        #region Drawing bool update
        //CHanging color based on selection
        if (DrawPhysicsLine)
        {
            DrawPhysicsLineButton.color = BikeControl.selectedColor;
        }
        else
        {
            DrawPhysicsLineButton.color = BikeControl.unselectedColor;
        }

        if (DrawJustLine)
        {
            DrawJustLineButton.color = BikeControl.selectedColor;
        }
        else
        {
            DrawJustLineButton.color = BikeControl.unselectedColor;
        }

        if (EraserIsOn)
        {
            EraserButton.color = BikeControl.selectedColor;
        }
        else
        {
            EraserButton.color = BikeControl.unselectedColor;
        }
        if (MoveInTheScene)
        {
            MoveToolButton.color = BikeControl.selectedColor;
        }
        else
        {
            MoveToolButton.color = BikeControl.unselectedColor;
        }
        #endregion
    }
}
