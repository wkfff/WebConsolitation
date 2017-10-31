using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor.Tools
{
    /// <summary>
    /// Форма задания ширины линии
    /// </summary>
    public partial class LineWidthModalForm : Form
    {
        #region Поля

        /// <summary>
        /// Толщина линий
        /// </summary>
        private int lineWidth;

        #endregion

        #region Конструкторы

        public LineWidthModalForm()
        {
            InitializeComponent();
        }

        public LineWidthModalForm(int lineWidth)
        {
            InitializeComponent();
            this.lineWidth = lineWidth;

            this.lineWidthUpDown.Value = lineWidth;
            this.lineWidthUpDown.ValueChanged += new EventHandler(LineWidthUpDown_ValueChanged);
        }
                              
        #endregion

        #region Properties

        public int LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Изменнение ширины
        /// </summary>
        private void LineWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.LineWidth = (int)lineWidthUpDown.Value;
        }

        #endregion
    }
}