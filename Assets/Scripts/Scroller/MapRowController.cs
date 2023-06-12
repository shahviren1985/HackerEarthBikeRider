using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;


    /// <summary>
    /// This demo shows how to jump to an index in the scroller. You can jump to a position before
    /// or after the cell. You can also include the spacing before or after the cell.
    /// </summary>
    public class MapRowController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// In this example we are going to use a standard generic List. We could have used
        /// a SmallList for efficiency, but this is just a demonstration that other list
        /// types can be used.
        /// </summary>
        private List<MapRowData> data;

        /// <summary>
        /// Reference to the scrollers
        /// </summary>
        public EnhancedScroller hScroller;

        /// <summary>
        /// References to the UI elements
        /// </summary>
        public Toggle useSpacingToggle;
        public Slider scrollerOffsetSlider;
        public Slider cellOffsetSlider;

        /// <summary>
        /// Reference to the cell prefab
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;


        public EnhancedScroller.TweenType hScrollerTweenType = EnhancedScroller.TweenType.immediate;
        public float hScrollerTweenTime = 0f;


        void Start()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            // set up the scroller delegates
            hScroller.Delegate = this;
            data = new();
            /*
            // set up some simple data
            _data = new List<Data>();
            for (var i = 0; i < 100; i++)
                _data.Add(new Data() { cellText = "Cell Data Index " + i.ToString() });
            */
            // tell the scroller to reload now that we have the data
            //hScroller.ReloadData();
        }

        public void setData(List<MapRowData> curdata)
        {
            data = curdata;
            hScroller.ReloadData();
        }
        #region UI Handlers

        public void JumpButton_OnClick(int number)
        {
            //t jumpDataIndex;
            hScroller.JumpToDataIndex(number);

            // jump to the index
            //croller.JumpToDataIndex(jumpDataIndex, scrollerOffsetSlider.value, cellOffsetSlider.value, useSpacingToggle.isOn, hScrollerTweenType, hScrollerTweenTime);

        }

        #endregion

        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated. This should be the length of your data array.
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            // in this example, we just pass the number of our data elements
            return data.Count;
        }

        /// <summary>
        /// This tells the scroller what the size of a given cell will be. Cells can be any size and do not have
        /// to be uniform. For vertical scrollers the cell size will be the height. For horizontal scrollers the
        /// cell size will be the width.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell size</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <returns>The size of the cell</returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // in this example, even numbered cells are 30 pixels tall, odd numbered cells are 100 pixels tall for the vertical scroller
            // the horizontal scroller has a fixed cell size of 200 pixels

            return 60f;
        }

        /// <summary>
        /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping cells.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
        /// <returns>The cell for the scroller to use</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            MapRowCell cellView = scroller.GetCellView(cellViewPrefab) as MapRowCell;

            // set the name of the game object to the cell's data index.
            // this is optional, but it helps up debug the objects in
            // the scene hierarchy.
            cellView.name = "Cell " + dataIndex.ToString();

            // in this example, we just pass the data to our cell's view which will update its UI
            cellView.SetData(data[dataIndex]);

            // return the cell to the scroller
            return cellView;
        }

        #endregion
    }

