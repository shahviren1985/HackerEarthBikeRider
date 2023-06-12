using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserTool : MonoBehaviour
{
    public LinesDrawer JustLineDrawer,PhysicsLineDrawer;
    public List<SpawnerManager> spawnerManager;
    public Camera cam;
    private bool EraserRun = false;
    public float Eraserwidth = 2;
    public Vector2 pointFixer;

    public bool DebugThis = false;
    public void SelfUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EraserRun=true;
            gameObject.SetActive(true);
        }
        if (EraserRun)
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
            StartCoroutine(getPoints(mousePosition +pointFixer));
            StartCoroutine(removePowerUps(mousePosition));
        }
        if (Input.GetMouseButtonUp(0))
        {
            gameObject.SetActive(false);
            EraserRun = false;
        }
        
    }

    private IEnumerator getPoints(Vector2 point)
    {
        
        for (var i = 0; i < PhysicsLineDrawer.transform.childCount; i++)
        {
            Dictionary<string, List<Vector2>> Lines = new();
            var DrawLines = false;
            var curLineNum=1;
            var curLineComponent = PhysicsLineDrawer.transform.GetChild(i).GetComponent<Line>();
            for (var j = 0; j < curLineComponent.points.Count; j++)
            {
                if (!Lines.ContainsKey("list" + curLineNum))
                {
                    Lines["list" + curLineNum] = new();
                }
                
                if (Vector2.Distance(point, curLineComponent.points[j]) < Eraserwidth)
                {
                    DebugLog(curLineComponent.points[j]);
                    DrawLines = true;
                    curLineNum++;
                }
                else
                {
                    Lines["list" + curLineNum].Add(curLineComponent.points[j]);
                }
            }
            if (DrawLines)
            {
                Object.Destroy(curLineComponent.gameObject);
                foreach(var key in Lines.Keys)
                {
                    if (Lines[key].Count > 1)
                    {
                        PhysicsLineDrawer.Enable = true;
                        PhysicsLineDrawer.BeginDraw();
                        for(var k=0;k< Lines[key].Count; k++)
                        {
                            PhysicsLineDrawer.AddPoint(Lines[key][k]);
                        }
                        PhysicsLineDrawer.EndDraw();
                        PhysicsLineDrawer.Enable = false;
                    }
                }
            }
        }
        for (var i = 0; i < JustLineDrawer.transform.childCount; i++)
        {
            Dictionary<string, List<Vector2>> Lines = new();
            var DrawLines = false;
            var curLineNum=1;
            var curLineComponent = JustLineDrawer.transform.GetChild(i).GetComponent<Line>();
            for (var j = 0; j < curLineComponent.points.Count; j++)
            {
                if (!Lines.ContainsKey("list" + curLineNum))
                {
                    Lines["list" + curLineNum] = new();
                }
                
                if (Vector2.Distance(point, curLineComponent.points[j]) < Eraserwidth)
                {
                    DebugLog(curLineComponent.points[j]);
                    DrawLines = true;
                    curLineNum++;
                }
                else
                {
                    Lines["list" + curLineNum].Add(curLineComponent.points[j]);
                }
            }
            if (DrawLines)
            {
                Object.Destroy(curLineComponent.gameObject);
                foreach(var key in Lines.Keys)
                {
                    if (Lines[key].Count > 1)
                    {
                        JustLineDrawer.Enable = true;
                        JustLineDrawer.BeginDraw();
                        for(var k=0;k< Lines[key].Count; k++)
                        {
                            JustLineDrawer.AddPoint(Lines[key][k]);
                        }
                        JustLineDrawer.EndDraw();
                        JustLineDrawer.Enable = false;
                    }
                }
            }
        }

        yield return null;
    }
    private IEnumerator removePowerUps(Vector2 point)
    {
        for(var i = 0; i < spawnerManager.Count; i++)
        {
            for(var j = 0; j < spawnerManager[i].transform.childCount; j++)
            {
                if (Vector2.Distance(point, spawnerManager[i].transform.GetChild(j).position) < Eraserwidth)
                {
                    GameObject.Destroy(spawnerManager[i].transform.GetChild(j).gameObject);
                };
            }
        }
        yield return null;
    }
    public void DebugLog(object msg)
    {
        if (DebugThis)
        {
            Debug.Log(msg);
        }
    }
}
