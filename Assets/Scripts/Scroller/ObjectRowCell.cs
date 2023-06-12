using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class ObjectRowCell : EnhancedScrollerCellView
    {
        public Text cellText;
        public virtual void SetData(MapRowData data)
        {
            cellText.text = data.cellText;
        }
        public virtual void ClickListener()
        {
            ObjectSaveLoad.loadMapName=cellText.text;
        }
    }
