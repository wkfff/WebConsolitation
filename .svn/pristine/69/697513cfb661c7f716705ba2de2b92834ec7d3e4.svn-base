using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class ScrollListItem
    {
        const int enabledImageIndex = 0;
        const int disabledImageIndex = 1;
        #region Размеры

        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Top
        {
            get { return this.Y; }
            set { this.Y = value; }
        }

        public int Left
        {
            get { return this.X; }
            set { this.X = value; }
        }

        public int Right
        {
            get { return this.X + this.Width; }
        }

        public int Bottom
        {
            get { return this.Y + this.Height; }
        }

        public Point Location
        {
            get { return new Point(this.X, this.Y); }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle(this.X, this.Y, this.Width, this.Height); }
        }
        #endregion

        #region Поля
        private bool _visible;
        private string _text;
        private Font _font;
        private Color _backColor;
        private SolidBrush foreBrush;
        private Color _foreColor;
        private Image _itemImage;
        private bool _isEditMode;
        private Rectangle _imageBounds;
        private RectangleF _textBounds;
        private int _separateWidth;
        private bool _isChanged;
        private ImageList _imageList;
        private object _tag;
        private Rectangle _editBoxBounds;
        private Brush editBoxBrush;

        ImageAttributes imageAttribs;
        #endregion

        #region Свойства
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Font Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set 
            { 
                _foreColor = value;
                this.foreBrush = new SolidBrush(value);
            }
        }

        public Image ItemImage
        {
            get { return _itemImage; }
            set { _itemImage = value; }
        }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { _isEditMode = value; }
        }

        public Rectangle ImageBounds
        {
            get { return _imageBounds; }
            set { _imageBounds = value; }
        }

        public RectangleF TextBounds
        {
            get { return _textBounds; }
            set { _textBounds = value; }
        }

        public int SeparateWidth
        {
            get { return _separateWidth; }
            set { _separateWidth = value; }
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            set 
            { 
                _isChanged = value;
                int imageIndex = _isChanged ? enabledImageIndex : disabledImageIndex;
                if ((this.ImageList != null) && (this.ImageList.Images.Count > imageIndex))
                    this.ItemImage = this.ImageList.Images[imageIndex];
                else
                    this.ItemImage = null;
            }
        }

        public Rectangle EditBoxBounds
        {
            get { return _editBoxBounds; }
            set { _editBoxBounds = value; }
        }

        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        #endregion

        public ScrollListItem()
        {
            this.SetDefaultValue();
        }

        public void Paint(Graphics gr)
        {
            gr.FillRectangle(new SolidBrush(this.BackColor), this.Bounds);
            this.DrawItemImage(gr);
            this.DrawEditBox(gr);
            this.DrawText(gr);
        }

        /// <summary>
        /// если указаная точка принадлежит области редактирования элемента
        /// </summary>
        /// <param name="point"></param>
        /// <returns>принадлежит или нет</returns>
        public bool IsBelongEditBox(Point point)
        {
            bool result = false;
            if (this.IsEditMode)
            {
                int x = this.EditBoxBounds.X - this.SeparateWidth;
                int width = this.Width - x;
                Rectangle bounds = new Rectangle(x, this.Y, width, this.Height);
                result = bounds.Contains(point);
            }
            return result;
        }

        private void SetDefaultValue()
        {
            this.Visible = true;
            this.ResetBounds();
            this.ItemImage = null;
            this.Text = string.Empty;

            this.SeparateWidth = 10;

            this.editBoxBrush = new SolidBrush(Color.LightGray);
            this.Font = new Font("Tahoma", 10, FontStyle.Regular);
        }

        public void ResetBounds()
        {
            this.ImageBounds = Rectangle.Empty;
            this.TextBounds = Rectangle.Empty;
            this.EditBoxBounds = Rectangle.Empty;
        }

        private void DrawItemImage(Graphics gr)
        {
            if (this.ItemImage != null)
            {
                if (this.ImageBounds == Rectangle.Empty)
                {
                    //Если режим редактирования включен, то изображения располагается перед техстом, иначе
                    //за текстом
                    int x = this.IsEditMode ? this.SeparateWidth : this.Right - this.ItemImage.Width
                        - this.SeparateWidth;
                    int y = (int)((float)(this.Height - this.ItemImage.Height) / 2f);
                    this.ImageBounds = new Rectangle(x, y, this.ItemImage.Width, this.ItemImage.Height);
                }

                if (this.imageAttribs == null)
                {
                    this.imageAttribs = new ImageAttributes();
                    //this.imageAttribs.SetColorKey(Color.FromArgb(255, 192, 203), Color.FromArgb(255, 192, 203));
                    this.imageAttribs.SetColorKey(Color.Black, Color.Black);
                }

                Rectangle region = new Rectangle(this.ImageBounds.X, this.ImageBounds.Y + this.Top,
                    this.ImageBounds.Width, this.ImageBounds.Height);

                gr.DrawImage(this.ItemImage, region, 0, 0, region.Width, region.Height, GraphicsUnit.Pixel,
                    this.imageAttribs);
            }
        }

        private void DrawEditBox(Graphics gr)
        {
            if (this.IsEditMode)
            {
                if (this.EditBoxBounds == Rectangle.Empty)
                {
                    int height = (int)((float)this.Height / 2f);
                    int width = height;
                    int x = this.Right - width - this.SeparateWidth;
                    int y = (int)((float)(this.Height - height) / 2f);
                    this._editBoxBounds = new Rectangle(x, y, width, height);
                }

                int countBar = 5;
                int barHeight = this.EditBoxBounds.Height / countBar;
                for (int i = 0; i < countBar; i++)
                {
                    if ((i % 2) == 0)
                    {
                        gr.FillRectangle(this.editBoxBrush, new Rectangle(this._editBoxBounds.X,
                            this.EditBoxBounds.Y + barHeight * i + this.Top, this.EditBoxBounds.Width, barHeight));
                    }
                }
            }
        }

        private void DrawText(Graphics gr)
        {
            const string cuStringEnd = "...";

            if (!string.IsNullOrEmpty(this.Text))
            {
                if (this.TextBounds == Rectangle.Empty)
                {
                    //Если не режим редактироваиня, или изображение отсутствует, то текс рисуется 
                    //с самого начала ячейки
                    int x = (this.ItemImage == null) || !this.IsEditMode ? this.SeparateWidth : 
                        this.ImageBounds.X + this.ItemImage.Width + this.SeparateWidth;
                    SizeF textSize = Utils.GetStringSize(this.Text + " ", gr, this.Font);
                    int height = (int)textSize.Height;
                    int width = (int)textSize.Width;
                    //Если режим редактирования, значти "бруски" справа будут постоянно
                    //если не влезает текст подрежим его и оставим навсегда
                    if (this.IsEditMode)
                    {
                        width = this.EditBoxBounds.X - x;
                        this.Text = Utils.CutString(this.Text, textSize.Width, width, cuStringEnd);
                    }
                    int y = (int)(((float)this.Height - height) / 2f);
                    this.TextBounds = new RectangleF(x, y, width, height);
                }
                
                RectangleF offsetBounds = new RectangleF(this.TextBounds.X, this.TextBounds.Y + this.Top, 
                    this.TextBounds.Width, this.TextBounds.Height);

                string text = this.Text;
                //Если изображение размещается правее текста, то текс рисуем только до изображения, 
                //делаем это сейчас, т.к. картинка может исчезать и текст будем рисовать опять целиком
                if ((this.ItemImage != null) && (offsetBounds.X < this.ImageBounds.X))
                {
                    offsetBounds.Width -= offsetBounds.Width - this.ImageBounds.X;
                    text = Utils.CutString(text, this.TextBounds.Width, offsetBounds.Width, "...");
                }

                gr.DrawString(text, this.Font, this.foreBrush, offsetBounds);
            }
        }        
    }
}
