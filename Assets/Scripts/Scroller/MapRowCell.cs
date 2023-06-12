using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;

public class MapRowCell : EnhancedScrollerCellView
    {
        public Text cellText;
    private string published = "";
        public virtual void SetData(MapRowData data)
        {
            cellText.text = data.cellText;
            published= data.published;
        }
        public virtual void ClickListener()
        {
        if (published == "yes")
        {
            SaveLoad.isPublished = true;
        }
        else
        {
            SaveLoad.isPublished=false;
        }
            SaveLoad.loadMapName=cellText.text;
        }
    }
