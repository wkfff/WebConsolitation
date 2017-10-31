using System;
using System.Collections.Generic;
using System.Drawing;
using Infragistics.Win.UltraWinDock;

namespace Krista.FM.Client.MDXExpert
{

    // Содержит информацию о групповом докабельном окне
    [Serializable]
    public class CustomDockableGroupPane : CustomDockablePaneBase
    {
        #region Поля

        public List<CustomDockablePaneBase> customDockableBaseCollection;

        private int selectedTabIndex;

        #endregion

        #region Свойства

        /// <summary>
        /// Номер активновного окна в группе
        /// </summary>
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set { selectedTabIndex = value; }
        }

        #endregion

        public CustomDockableGroupPane()
        {
            customDockableBaseCollection = new List<CustomDockablePaneBase>();
        }

        /// <summary>
        /// Создание группового окна
        /// </summary>
        /// <param name="paneId">id окна</param>
        /// <param name="floatParentId">id плавающего родителя</param>
        /// <param name="dockParentId">id докабельного родителя</param>
        /// <param name="state">состояние окна</param>
        /// <param name="style">стиль окна</param>
        /// <param name="size">размер окна</param>
        /// <param name="selectedTabIndex">Номер активного окна в группе</param>>
        /// <param name="minimized">свернуто ли окно</param>
        public CustomDockableGroupPane(int paneId, int floatParentId, int dockParentId, DockedState state, ChildPaneStyle style, Size size, int selectedTabIndex, bool minimized)
        {
            customDockableBaseCollection = new List<CustomDockablePaneBase>();
            this.paneId = paneId;
            this.floatParentId = floatParentId;
            this.dockParentId = dockParentId;
            this.state = state;
            this.style = style;
            this.size = size;
            this.minimized = minimized;
            this.selectedTabIndex = selectedTabIndex;
        }
    }

}
