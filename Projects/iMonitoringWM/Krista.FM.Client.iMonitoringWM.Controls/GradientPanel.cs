using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class GradientPanel : Control
    {
        private System.ComponentModel.IContainer components = null;

        private Color borderColor;
        private Rectangle _textBounds;
        private StringAlignment _textAlligment;
        private Bitmap _image;
        private Bitmap _vgaImage;
        private ImageAlignment _imageAlignment;

        public ImageAlignment ImageAlignment
        {
            get { return _imageAlignment; }
            set { _imageAlignment = value; }
        }

        public Bitmap Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public Bitmap VgaImage
        {
            get { return _vgaImage; }
            set { _vgaImage = value; }
        }

        /// <summary>
        /// Границы текста
        /// </summary>
        public Rectangle TextBounds
        {
            get { return _textBounds; }
            set { _textBounds = value; }
        }

        /// <summary>
        /// Цвет бордюра
        /// </summary>
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        /// <summary>
        /// Выравнивание текста
        /// </summary>
        public StringAlignment TextAlligment
        {
            get { return _textAlligment; }
            set { _textAlligment = value; }
        }

        public GradientPanel()
        {
            components = new System.ComponentModel.Container();
            this.Font = new Font(this.Font.Name, this.Font.Size, FontStyle.Bold);
            this.TextBounds = Rectangle.Empty;
            this.TextAlligment = StringAlignment.Near;
        }

        // Controls the direction in which the button is filled.
        public GradientFill.FillDirection FillDirection
        {
            get { return fillDirectionValue; }
            set
            {
                fillDirectionValue = value;
                Invalidate();
            }
        }
        private GradientFill.FillDirection fillDirectionValue;

        // The start color for the GradientFill. This is the color
        // at the left or top of the control depeneding on the value
        // of the FillDirection property.
        public Color StartColor
        {
            get { return startColorValue; }
            set
            {
                startColorValue = value;
                Invalidate();
            }
        }
        private Color startColorValue = Color.Red;

        // The end color for the GradientFill. This is the color
        // at the right or bottom of the control depending on the value
        // of the FillDirection property
        public Color EndColor
        {
            get { return endColorValue; }
            set
            {
                endColorValue = value;
                Invalidate();
            }
        }
        private Color endColorValue = Color.Blue;

        // This is the offset from the left or top edge
        //  of the button to start the gradient fill.
        public int StartOffset
        {
            get { return startOffsetValue; }
            set
            {
                startOffsetValue = value;
                Invalidate();
            }
        }
        private int startOffsetValue;

        // This is the offset from the right or bottom edge
        //  of the button to end the gradient fill.
        public int EndOffset
        {
            get { return endOffsetValue; }
            set
            {
                endOffsetValue = value;
                Invalidate();
            }
        }
        private int endOffsetValue;

        // Used to double-buffer our drawing to avoid flicker
        // between painting the background, border, focus-rect
        // and the text of the control.
        private Bitmap DoubleBufferImage
        {
            get
            {
                if (bmDoubleBuffer == null)
                    bmDoubleBuffer = new Bitmap(
                        this.ClientSize.Width,
                        this.ClientSize.Height);
                return bmDoubleBuffer;
            }
            set
            {
                if (bmDoubleBuffer != null)
                    bmDoubleBuffer.Dispose();
                bmDoubleBuffer = value;
            }
        }
        private Bitmap bmDoubleBuffer;

        // Called when the control is resized. When that happens,
        // recreate the bitmap used for double-buffering.
        protected override void OnResize(EventArgs e)
        {
            DoubleBufferImage = new Bitmap(
                this.ClientSize.Width,
                this.ClientSize.Height);
            base.OnResize(e);
        }

        // Called when the control gets focus. Need to repaint
        // the control to ensure the focus rectangle is drawn correctly.
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }
        //
        // Called when the control loses focus. Need to repaint
        // the control to ensure the focus rectangle is removed.
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate();
        }


        // Override this method with no code to avoid flicker.
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawButton(e.Graphics);
        }

        // Draws the button on the specified Grapics
        // in the specified state.
        //
        // Parameters:
        //  gr - The Graphics object on which to draw the button.
        void DrawButton(Graphics gr)
        {
            // Get a Graphics object from the background image.
            Graphics gr2 = Graphics.FromImage(DoubleBufferImage);

            // Fill solid up until where the gradient fill starts.
            if (startOffsetValue > 0)
            {
                if (fillDirectionValue ==
                    GradientFill.FillDirection.LeftToRight)
                {
                    gr2.FillRectangle(
                        new SolidBrush(StartColor),
                        0, 0, startOffsetValue, Height);
                }
                else
                {
                    gr2.FillRectangle(
                        new SolidBrush(StartColor),
                        0, 0, Width, startOffsetValue);
                }
            }

            // Draw the gradient fill.
            Rectangle rc = this.ClientRectangle;
            if (fillDirectionValue == GradientFill.FillDirection.LeftToRight)
            {
                rc.X = startOffsetValue;
                rc.Width = rc.Width - startOffsetValue - endOffsetValue;
            }
            else
            {
                rc.Y = startOffsetValue;
                rc.Height = rc.Height - startOffsetValue - endOffsetValue;
            }
            GradientFill.Fill(
                gr2,
                rc,
                startColorValue,
                endColorValue,
                fillDirectionValue);

            // Fill solid from the end of the gradient fill
            // to the edge of the button.
            if (endOffsetValue > 0)
            {
                if (fillDirectionValue ==
                    GradientFill.FillDirection.LeftToRight)
                {
                    gr2.FillRectangle(
                        new SolidBrush(EndColor),
                        rc.X + rc.Width, 0, endOffsetValue, Height);
                }
                else
                {
                    gr2.FillRectangle(
                        new SolidBrush(EndColor),
                        0, rc.Y + rc.Height, Width, endOffsetValue);
                }
            }

            // Draw the text.
            StringFormat sf = new StringFormat();
            sf.Alignment = this.TextAlligment;
            sf.LineAlignment = StringAlignment.Center;
            if (this.TextBounds == Rectangle.Empty)
                this.TextBounds = this.ClientRectangle;
            gr2.DrawString(this.Text, this.Font,
                new SolidBrush(this.ForeColor),
                this.TextBounds, sf);

            if (this.BorderColor != Color.Empty)
            {
                // Draw the border.
                // Need to shrink the width and height by 1 otherwise
                // there will be no border on the right or bottom.
                rc = this.ClientRectangle;
                rc.Width--;
                rc.Height--;
                Pen pen = new Pen(this.BorderColor);
                gr2.DrawRectangle(pen, rc);
            }

            Bitmap image = Utils.ScreenSize == ScreenSizeMode.s240x320 ? this.Image : this.VgaImage;
            if (image != null)
            {
                Point imageLocation = this.GetImageLocation(this.ImageAlignment, new Rectangle(0, 0, image.Width, image.Height),
                    this.Bounds);
                gr2.DrawImage(image, imageLocation.X, imageLocation.Y);
            }

            // Draw from the background image onto the screen.
            gr.DrawImage(DoubleBufferImage, 0, 0);
            gr2.Dispose();
        }

        private Point GetImageLocation(ImageAlignment alignment, Rectangle imageBounds, 
            Rectangle imagePlaceBounds)
        {
            int x = 0;
            int y = 0;
            switch (alignment)
            {
                case ImageAlignment.Middle:
                    {
                        x = (int)((float)(imagePlaceBounds.Width - imageBounds.Width) / 2f);
                        break;
                    }
                case ImageAlignment.Right:
                    {
                        x = imagePlaceBounds.Width - imageBounds.Width;
                        break;
                    }
            }
            return new Point(x, y);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public enum ImageAlignment
    {
        Left,
        Middle,
        Right
    }
}
