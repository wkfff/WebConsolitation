using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor.Tools
{
    public partial class ZoomControl : UserControl
    {
        /// <summary>
        ///  ���� ���������...
        /// </summary>
        public class ScaleEventArgs : EventArgs
        {
            public readonly int ScaleFactor;

            public ScaleEventArgs(int scaleFactor)
            {
                this.ScaleFactor = scaleFactor;
            }
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ScaleEventHandler(object sender, ScaleEventArgs args);

        /// <summary>
        /// ������� ��� ��������� �������
        /// </summary>
        public event ScaleEventHandler ScaleChangeEvent;

        /// <summary>
        /// ������� ������� ���������
        /// </summary>
        private int scaleFactor;

        /// <summary>
        /// ������,����������� ��� ������� �� ������ +/-
        /// </summary>
        Timer timer = new Timer();

        /// <summary>
        /// ������
        /// </summary>
        bool downbuttonDown = false;

        bool upbuttonDown = false;

        public int ScaleFactor
        {
            get { return scaleFactor; }
            set
            {
                scaleFactor = value;

                DisplayScale();
                CallEventHandler();
            }
        }

        /// <summary>
        /// ��������� ������ ��������
        /// </summary>
        private void DisplayScale()
        {
            this.ComboZoom.Text = String.Format("{0}%", scaleFactor);
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="site"></param>
        public ZoomControl()
        {
            InitializeComponent();

            this.ComboZoom.SelectionChanged += new EventHandler(SelectionChanged);
            this.ComboZoom.KeyDown += new KeyEventHandler(ComboZoom_KeyDown);

            timer.Enabled = false;
            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);

            DisplayScale();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (downbuttonDown)
                    this.ScaleChange(-10);
                else if (upbuttonDown)
                    this.ScaleChange(10);
            }
            // ������������� ����������.. � ������
            catch
            {
                FinalizeOperation();
            }
        }

        /// <summary>
        /// ���������� ��������
        /// </summary>
        public void FinalizeOperation()
        {
            downbuttonDown = false;
            upbuttonDown = false;
            timer.Enabled = false;
        }

        /// <summary>
        /// ����� �������� ���������� ��������
        /// </summary>
        public void StartUpOperation()
        {
            timer.Enabled = true;
            upbuttonDown = true;
        }

        /// <summary>
        /// ����� �������� ���������� ��������
        /// </summary>
        public void StartDownOperation()
        {
            timer.Enabled = true;
            downbuttonDown = true;
        }

        /// <summary>
        /// ��������� ������� ����������
        /// </summary>
        /// <param name="args"></param>
        private void OnChangeScale(ScaleEventArgs args)
        {
            if (ScaleChangeEvent != null)
            {
                // ���������� ���� ���������
                ScaleChangeEvent(this, args);
            }
        }

        /// <summary>
        /// �������� ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboZoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidateText(ComboZoom.Text);
            }
        }

        /// <summary>
        /// ������ �� �������
        /// </summary>
        private void CallEventHandler()
        {
            ScaleEventArgs args = new ScaleEventArgs(scaleFactor);

            // ��� � ��!
            OnChangeScale(args);
        }

        /// <summary>
        /// �������� �������� ��������
        /// </summary>
        /// <param name="p"></param>
        private void ValidateText(string p)
        {
            if (String.IsNullOrEmpty(p))
                return;

            int z = 0;

            if (p[p.Length - 1] == '%')
                p = p.Substring(0, p.Length - 1);

            try
            {
                z = int.Parse(p);
            }
            catch (FormatException e)
            {
                
                DisplayScale();

                //���������� ����, �� �� �������������
                throw new Exception("������������ ������ ��������");
            }

            if (z < 10 || z > 400)
            {
                DisplayScale();

                //���������� ����, �� �� �������������
                throw new Exception("����� ������ ���� ����� 10 � 400");
            }

            scaleFactor = int.Parse(p);

            DisplayScale();
            CallEventHandler();
        }

        /// <summary>
        /// ��������� ������ ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectionChanged(object sender, EventArgs e)
        {
            switch(ComboZoom.Text)
            {
                case "200%":
                    scaleFactor = 200;
                    break;
                case "150%":
                    scaleFactor = 150;
                    break;
                case "100%":
                    scaleFactor = 100;
                    break;
                case "75%":
                    scaleFactor = 75;
                    break;
                case "50%":
                    scaleFactor = 50;
                    break;
                case "25%":
                    scaleFactor = 25;
                    break;
                case "��� ���������":
                    //��� ������� ������������ �������� ����� ���������� -1 � �������� ���������
                    ScaleEventArgs args = new ScaleEventArgs(-1);
                    OnChangeScale(args);
                    // ��������� ������
                    
                    return;
            }

            CallEventHandler();
        }

        public void ScaleChange(int index)
        {
            try
            {
                ValidateText(string.Format("{0}", scaleFactor + index));
            }
                // ..������ ����������
            catch
            {
                FinalizeOperation();
            }
        }
    }
}
