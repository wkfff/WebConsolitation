using System;
using System.Collections.Generic;
using System.Drawing;
using Infragistics.Win.UltraWinDock;

namespace Krista.FM.Client.MDXExpert
{

    // Содержит информацию о докабельной области
    [Serializable]
    public class CustomDockAreaPane
    {
        #region Поля

        public List<CustomDockablePaneBase> customDockableBaseCollection;

        private int paneId;

        private DockedLocation dockLocation;

        private Size size;

        private Point locationPoint;

        private ChildPaneStyle childPaneStyle;

        private int selectedTabIndex;

        #endregion

        #region Свойства

        /// <summary>
        /// Id области
        /// </summary>
        public int PaneId
        {
            get { return paneId; }
            set { paneId = value; }
        }

        /// <summary>
        /// Расположение области
        /// </summary>
        public DockedLocation DockLocation
        {
            get { return dockLocation; }
            set { dockLocation = value; }
        }

        /// <summary>
        /// Размер области
        /// </summary>
        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Положение вложенных окон
        /// </summary>
        public ChildPaneStyle ChildPaneStyle
        {
            get { return childPaneStyle; }
            set { childPaneStyle = value; }
        }

        /// <summary>
        /// Положение верхнего левого угла области
        /// </summary>
        public Point LocationPoint
        {
            get { return locationPoint; }
            set { locationPoint = value; }
        }

        /// <summary>
        /// Номер активновного окна в группе
        /// </summary>
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set { selectedTabIndex = value; }
        }

        #endregion

        public CustomDockAreaPane()
        {
            customDockableBaseCollection = new List<CustomDockablePaneBase>();
        }

        /// <summary>
        /// Создание докабельной области
        /// </summary>
        /// <param name="paneId">id области</param>
        /// <param name="dockLocation">стиль привязки области</param>
        /// <param name="locationPoint">положение левого верхнего угла области</param>
        /// <param name="size">размер облас</param>
        /// <param name="childPaneStyle">стиль группировки потомков</param>
        /// <param name="selectedTabIndex">Номер активного окна в группе</param>>
        public CustomDockAreaPane(int paneId, DockedLocation dockLocation, Point locationPoint, Size size, ChildPaneStyle childPaneStyle, int selectedTabIndex)
        {
            customDockableBaseCollection = new List<CustomDockablePaneBase>();
            this.paneId = paneId;
            this.dockLocation = dockLocation;
            this.locationPoint = locationPoint;
            this.size = size;
            this.childPaneStyle = childPaneStyle;
            this.selectedTabIndex = selectedTabIndex;
        }
    }
}
