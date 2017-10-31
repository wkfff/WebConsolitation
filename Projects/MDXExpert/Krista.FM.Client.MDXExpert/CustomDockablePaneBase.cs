using System;
using System.Drawing;
using Infragistics.Win.UltraWinDock;

namespace Krista.FM.Client.MDXExpert
{
    // Содержит информацию о базовом докабельном окне
    [Serializable]
    public class CustomDockablePaneBase
    {
        #region Поля

        protected int paneId;

        protected int floatParentId;

        protected int dockParentId;

        protected DockedState state;

        protected ChildPaneStyle style;

        protected Size size;

        protected bool minimized;

        #endregion

        #region Свойства

        /// <summary>
        /// Id окна
        /// </summary>
        public int PaneId
        {
            get { return paneId; }
            set { paneId = value; }
        }

        /// <summary>
        /// Id родителя плавающего окна
        /// </summary>
        public int FloatPaneId
        {
            get { return floatParentId; }
            set { floatParentId = value; }
        }

        /// <summary>
        /// Id родителя докабельного окна
        /// </summary>
        public int DockPaneId
        {
            get { return dockParentId; }
            set { dockParentId = value; }
        }

        /// <summary>
        /// Состояние окна
        /// </summary>
        public DockedState State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Стиль окна
        /// </summary>
        public ChildPaneStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>
        /// Размер окна
        /// </summary>
        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Свернуто ли оно
        /// </summary>
        public bool Minimized
        {
            get { return minimized; }
            set { minimized = value; }
        }

        #endregion

        public CustomDockablePaneBase()
        {

        }

        /// <summary>
        /// Создание базового докабельного окна
        /// </summary>
        /// <param name="paneId">id окна</param>
        /// <param name="floatParentId">id плавающего родителя</param>
        /// <param name="dockParentId">id докабельного родителя</param>
        /// <param name="state">состояние окна</param>
        /// <param name="style">стиль окна</param>
        /// <param name="size">размер окна</param>
        /// <param name="minimized">свернуто ли окно</param>
        public CustomDockablePaneBase(int paneId, int floatParentId, int dockParentId, DockedState state, ChildPaneStyle style, Size size, bool minimized)
        {
            this.paneId = paneId;
            this.floatParentId = floatParentId;
            this.dockParentId = dockParentId;
            this.state = state;
            this.style = style;
            this.size = size;
            this.minimized = minimized;
        }
    }

}
