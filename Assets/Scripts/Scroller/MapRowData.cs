using UnityEngine;
using System.Collections;

public class MapRowData
{
    public string cellText="";
    public string published = ""; 
    public MapRowData(string cellText,string published)
    {
        this.cellText = cellText;
        this.published = published;
    }
    public MapRowData(string cellText)
    {
        this.cellText=cellText;
    }
    public MapRowData()
    {

    }
}
