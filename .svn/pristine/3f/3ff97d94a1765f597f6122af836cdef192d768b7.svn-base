using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.SchemeEditor.ControlObjects;


namespace Krista.FM.Client.SchemeEditor
{
    /// <summary>
    /// Дерево изменений
    /// </summary>
    internal class ModificationTreeView : CustomTreeView
    {
        /// <summary>
        /// Определяет, что дерево изменений доступно только для просмотра
        /// </summary>
        private bool readOnly = false;


        /// <summary>
        /// Инициализация экземпляра дерева изменений
        /// </summary>
        public ModificationTreeView() 
            : base()
        {
            this.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
        }


        /// <summary>
        /// Определяет, что дерево изменений доступно только для просмотра
        /// </summary>
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }
    }
}
