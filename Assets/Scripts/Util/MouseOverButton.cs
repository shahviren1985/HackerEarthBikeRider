using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverButton : MonoBehaviour
{
    public bool Debugthis;
    public List<RectTransform> MainGameDivrectTransforms;
    public List<LinesDrawer> MainGameDivLinesDrawers;
    public MoveTool MainGameDivMoveTool;
    public EraserTool MainGameDivEraserTool;
    public List<RectTransform> ObjectDivrectTransforms;
    public List<LinesDrawer> ObjectDivLinesDrawers;
    public MoveTool ObjectDivMoveTool;
    public EraserTool ObjectDivEraserTool;
    private bool isMouseOverButton = false;
    public GameObject MainGameDiv, ObjectDiv;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&MainGameDiv.activeInHierarchy)
        {
            isMouseOverButton = false;
            for(var i=0; i<MainGameDivrectTransforms.Count; i++)
            {
                if (MainGameDivrectTransforms[i].gameObject.activeInHierarchy)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    if (RectTransformUtility.RectangleContainsScreenPoint(MainGameDivrectTransforms[i], mousePosition))
                    {
                        if (Debugthis)
                        {
                            Debug.Log("Mouser over rect");
                        }
                        isMouseOverButton = true;
                        break;
                    }
                }
            }            
        }
        if (!isMouseOverButton && MainGameDiv.activeInHierarchy)
        {
            foreach (var lineDrawer in MainGameDivLinesDrawers)
            {
                if (lineDrawer.Enable)
                {
                    lineDrawer.SelfUpdate();
                }
            }
            if (BikeControl.MoveInTheScene)
            {
                MainGameDivMoveTool.SelfUpdate();
            }
            //Eraser Tool
            if (BikeControl.EraserIsOn)
            {
                MainGameDivEraserTool.SelfUpdate();
            }
        }
        if (Input.GetMouseButtonDown(0) && ObjectDiv.activeInHierarchy)
        {
            isMouseOverButton = false;
            for(var i=0; i<ObjectDivrectTransforms.Count; i++)
            {
                if (ObjectDivrectTransforms[i].gameObject.activeInHierarchy)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    if (RectTransformUtility.RectangleContainsScreenPoint(ObjectDivrectTransforms[i], mousePosition))
                    {
                        if (Debugthis)
                        {
                            Debug.Log("Mouser over rect");
                        }
                        isMouseOverButton = true;
                        break;
                    }
                }
            }            
        }
        if (!isMouseOverButton&& ObjectDiv.activeInHierarchy)
        {
            foreach (var lineDrawer in ObjectDivLinesDrawers)
            {
                if (lineDrawer.Enable)
                {
                    lineDrawer.SelfUpdate();
                }
            }
            if (ObjectsEditor.MoveInTheScene)
            {
                ObjectDivMoveTool.SelfUpdate();
            }
            //Eraser Tool
            if (ObjectsEditor.EraserIsOn)
            {
                ObjectDivEraserTool.SelfUpdate();
            }
        }
    }
}
