using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    class CellComments
    {
        private ExpertGrid _grid;
        private CellStyle _style; //стиль комеентария
        private bool _isDisplayingComment = false; //отображается ли в данный момент комментарий
        private bool _isDisplayComments = true;    //отображать комментарии
        private Rectangle commentBounds = Rectangle.Empty;
        private Image commentPlace; //снимок места, которое будет занимать комментарий
        private int _maxWidth = 500;//максимальная ширина комментария
        private int _timerInterval = 500; //интервал, через который будет отоброжаться комментарий
        private Timer timer;
        private GridControl currentControl = null;//контрол к которому отображается комментарий
        //отображать показанный комментарий, до сменны ячейки к которой он принадлежит
        private bool _displayUntillControlChange;

        //эти поля инициализируются при загрузке шаблона
        private int templateMaxWidth;
        private int templateTimerInterval;
        private bool templateDisplayComments;
        private bool templateDispalyUntilControlChange;

        public CellComments(ExpertGrid grid)
        {
            this.Grid = grid;
            this.Style = new CellStyle(null, SystemColors.Info, SystemColors.Info, 
                SystemColors.InfoText, Color.Black);
            this.Style.Font = new Font("Arial", 9f);
            this.Style.StringFormat.LineAlignment = StringAlignment.Center;

            // timer
            this.timer = new System.Windows.Forms.Timer();
            this.timer.Interval = this.TimerInterval;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
        }

        /// <summary>
        /// Начало отсчета, до показа комментария
        /// </summary>
        public void StartTimer()
        {
            if (this.IsDisplayComments)
            {
                //Начало отсчета таймера на отображение комментария
                this.timer.Stop();
                this.timer.Start();
            }
        }

        /// <summary>
        /// Сработал таймер, если в этот момент ничего не перетаскиваем и приложение находится в фокусе, 
        /// то рисуем комментарий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.Grid.ParentForm == null)
                return;

            if ((this.Grid.ParentForm.ContainsFocus) && (!this.Grid.Splitter.IsDrag) && (!this.Grid.SelectionFrame.IsDrag)
                && (this.Grid.GridBounds.Contains(this.Grid.CurrentMousePosition)))
                this.DisplayComment(this.Grid.CurrentMousePosition);
            this.timer.Stop();
        }

        /// <summary>
        /// Если изменился контрол к которому отображали комментарий, очищаем комментарий, можно очистить 
        /// принудительно, выставив признак forceClean
        /// </summary>
        public void CleanComment(bool forceClean)
        {
            this.timer.Stop();
            if (!forceClean && (this.currentControl == this.Grid.LocationHelper.SelectedControl))
                return;

            if (this.IsDisplayingComment)
            {
                using (Graphics graphics = this.Grid.GetGridGraphics())
                {
                    graphics.DrawImage(this.commentPlace, this.commentBounds.Location);
                }
                this.IsDisplayingComment = false;
                this.currentControl = null;
            }
        }

        private Rectangle GetCommentBounds(Graphics graphics, string comment, int maxWidth, Point commentStart)
        {
            string[] separator = new string[]{"\n"};
            int width = CommonUtils.GetStringMaxWidth(graphics, comment.Split(separator,
                StringSplitOptions.RemoveEmptyEntries), this.Style.Font);

            //корректируем координаты так, что бы комментарий не был за границей экрана
            
            //прибавляем дополнительные пиксели к ширине и высоте, с ними смотриться на много красивее
            int ancillaryWidth = 8;
            width = Math.Min(width, maxWidth) + ancillaryWidth;
            int height = CommonUtils.GetStringHeight(graphics, comment, this.Style.Font, width - ancillaryWidth) + 4;

            Rectangle screenBounds = this.Grid.GridBounds;
            int x = (commentStart.X + width) > screenBounds.Right ?
                commentStart.X - ((commentStart.X + width) - screenBounds.Right) :
                commentStart.X;
            int y = (commentStart.Y + height) > screenBounds.Bottom ?
                commentStart.Y - ((commentStart.Y + height) - screenBounds.Bottom) :
                commentStart.Y;

            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Отображает комментарий к элементу грида, на который указывает мышка
        /// </summary>
        /// <param name="mousePosition"></param>
        public void DisplayComment(Point mousePosition)
        {
            this.currentControl = this.Grid.LocationHelper.SelectedControl;
            if ((this.currentControl != null) && !this.IsDisplayingComment && (Cursor.Current != null))
            {
                string commentText = this.currentControl.GetComment();

                if (commentText == string.Empty)
                {
                    return;
                }

                this.IsDisplayingComment = true;

                using (Graphics graphics = this.Grid.GetGridGraphics())
                {
                    mousePosition.Y += Cursor.Current.Size.Height - 7;
                    mousePosition.X += Cursor.Current.Size.Width - 15;
                    this.commentBounds = this.GetCommentBounds(graphics, commentText, this.MaxWidth, mousePosition);

                    this.commentPlace = CommonUtils.GetBitMap(this.Grid.GridPlace.RectangleToScreen(this.commentBounds));
                    this.Grid.Painter.DrawComment(graphics, commentBounds, this.Style, commentText);
                }
            }
        }

        /// <summary>
        /// Загружаем настройки комметариев
        /// </summary>
        /// <param name="commentsNode"></param>
        public void Load(XmlNode commentsNode, bool isLoadTemplate)
        {
            if (commentsNode == null)
                return;
            this.Style.Load(commentsNode.SelectSingleNode(GridConsts.style), isLoadTemplate);
            this.LoadPropertys(commentsNode.SelectSingleNode(GridConsts.propertys), isLoadTemplate);
        }

        /// <summary>
        /// Сохраняем настройки комментариев
        /// </summary>
        /// <param name="commentsNode"></param>
        public void Save(XmlNode commentsNode)
        {
            if (commentsNode == null)
                return;
            this.Style.Save(XmlHelper.AddChildNode(commentsNode, GridConsts.style));
            this.SavePropertys(XmlHelper.AddChildNode(commentsNode, GridConsts.propertys));
        }

        /// <summary>
        /// Загрузка свойств комментариев
        /// </summary>
        /// <param name="nodePropertys"></param>
        /// <param name="isLoadTemplate"></param>
        private void LoadPropertys(XmlNode nodePropertys, bool isLoadTemplate)
        {
            if (nodePropertys == null)
                return;

            this.TimerInterval = XmlHelper.GetIntAttrValue(nodePropertys, GridConsts.timerInterval, 500);
            this.MaxWidth = XmlHelper.GetIntAttrValue(nodePropertys, GridConsts.maxWidth, 500);
            this.IsDisplayComments = XmlHelper.GetBoolAttrValue(nodePropertys, GridConsts.isDisplay, true);
            this.DisplayUntillControlChange = XmlHelper.GetBoolAttrValue(nodePropertys, 
                GridConsts.displayUntilConrolChange, false);

            if (isLoadTemplate)
            {
                this.templateTimerInterval = this.TimerInterval;
                this.templateMaxWidth = this.MaxWidth;
                this.templateDisplayComments = this.IsDisplayComments;
                this.templateDispalyUntilControlChange = this.DisplayUntillControlChange;
            }
        }

        /// <summary>
        /// Сохранение свойств комментариев
        /// </summary>
        /// <param name="propertysNode"></param>
        private void SavePropertys(XmlNode propertysNode)
        {
            if (propertysNode == null)
                return;

            if (this.templateTimerInterval != this.TimerInterval)
            {
                XmlHelper.SetAttribute(propertysNode, GridConsts.timerInterval, this.TimerInterval.ToString());
            }
            if (this.templateMaxWidth != this.MaxWidth)
            {
                XmlHelper.SetAttribute(propertysNode, GridConsts.maxWidth, this.MaxWidth.ToString());
            }
            if (this.templateDisplayComments != this.IsDisplayComments)
            {
                XmlHelper.SetAttribute(propertysNode, GridConsts.isDisplay, this.IsDisplayComments.ToString());
            }
            if (this.templateDispalyUntilControlChange != this.DisplayUntillControlChange)
            {
                XmlHelper.SetAttribute(propertysNode, GridConsts.displayUntilConrolChange,
                    this.DisplayUntillControlChange.ToString());
            }
        }

        public ExpertGrid Grid
        {
            get { return this._grid; }
            set { this._grid = value; }
        }

        public CellStyle Style
        {
            get { return _style; }
            set 
            {
                if (_style != null)
                    _style.CopyDefaultStyle(value);
                _style = value; 
            }
        }

        /// <summary>
        /// Максимальная ширина комметария
        /// </summary>
        public int MaxWidth
        {
            get { return this._maxWidth; }
            set 
            {
                if (value > 0)
                    this._maxWidth = value; 
            }
        }

        /// <summary>
        /// Отображется ли в данный момент комментарий
        /// </summary>
        public bool IsDisplayingComment
        {
            get { return this._isDisplayingComment; }
            set { this._isDisplayingComment = value; }
        }

        /// <summary>
        /// Отображать комментарии к ячейкам
        /// </summary>
        public bool IsDisplayComments
        {
            get { return this._isDisplayComments; }
            set { this._isDisplayComments = value; }
        }

        /// <summary>
        /// Интервал времени (мс), через который отображается комментарий
        /// </summary>
        public int TimerInterval
        {
            get { return this._timerInterval; }
            set 
            {
                if (value > 0)
                {
                    this._timerInterval = value;
                    this.timer.Interval = this._timerInterval;
                }
            }
        }

        /// <summary>
        /// Отображать показанный комментарий, до сменны ячейки к которой он принадлежит
        /// </summary>
        public bool DisplayUntillControlChange
        {
            get { return _displayUntillControlChange; }
            set { _displayUntillControlChange = value; }
        }
    }
}
