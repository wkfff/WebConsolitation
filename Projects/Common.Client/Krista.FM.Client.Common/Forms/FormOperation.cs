using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.Misc;


// Организация модуля аналогична FormProgress.cs. За подробными комментариями - туда.
namespace Krista.FM.Client.Common.Forms
{
	internal class frmOperation : System.Windows.Forms.Form
	{
		private UltraLabel laInfo;
		private PictureBox pbAnimation;
		private System.ComponentModel.Container components = null;

		public frmOperation()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
            this.laInfo = new UltraLabel();
            this.pbAnimation = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // laInfo
            // 
            this.laInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.laInfo.ForeColor = System.Drawing.Color.MediumBlue;
            this.laInfo.Appearance.ImageHAlign = Infragistics.Win.HAlign.Left;
            this.laInfo.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle; 
            this.laInfo.Location = new System.Drawing.Point(105, 10);
            this.laInfo.Name = "laInfo";
            this.laInfo.Size = new System.Drawing.Size(291, 59);
            this.laInfo.TabIndex = 2;
            this.laInfo.Text = "Инициализация";
            this.laInfo.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
            this.laInfo.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            // 
            // pbAnimation
            // 
            this.pbAnimation.Image = global::Krista.FM.Client.Common.Properties.Resources.Шестеренки;
            this.pbAnimation.InitialImage = null;
            this.pbAnimation.Location = new System.Drawing.Point(3, 6);
            this.pbAnimation.Name = "pbAnimation";
            this.pbAnimation.Size = new System.Drawing.Size(87, 66);
            this.pbAnimation.TabIndex = 3;
            this.pbAnimation.TabStop = false;
            // 
            // frmOperation
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(408, 78);
            this.ControlBox = false;
            this.Controls.Add(this.pbAnimation);
            this.Controls.Add(this.laInfo);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "frmOperation";
            this.Opacity = 0.8;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmOperation_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimation)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetText(string Val)
		{
			laInfo.Text = Val;
		}

		public void StartOperation()
		{
			if (!this.Visible)
                this.Show();
            this.TopMost = true;
            this.TopMost = false;
            Application.DoEvents();
		}

		public void StopOperation()
		{
			this.Hide();
		}

		public void ReleaseThread()
		{
			Application.ExitThread();
        }

        public void SetTopIsVisible()
        {
            if (this.Visible)
            {
                this.TopMost = true;
                this.TopMost = false;
                this.pbAnimation.Invalidate();
                Application.DoEvents();
            }
        }

        public bool GetVisible()
        {
            return this.Visible;
        }

		private void frmOperation_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
		}
	}

	public class Operation
	{
		private Thread OperationThread;
		private frmOperation OperationForm;
		private VoidDelegate StartOperationDelegate;
		private VoidDelegate StopOperationDelegate;
		private VoidDelegate ReleaseThreadDelegate;
        private VoidDelegate SetTopIsVisibleDelegate;
		private SetStringDelegate SetTextDelegate;
        private GetBoolDelegate GetVisibleDelegate;

		public Operation()
		{
			OperationThread = new Thread(new ThreadStart(this.ThreadEntryPoint));
            OperationThread.Priority = ThreadPriority.Highest;
			OperationThread.Start();
			while (OperationThread.ThreadState != System.Threading.ThreadState.Running) Thread.Sleep(50);
			while (OperationForm == null)Thread.Sleep(50);
			while ((!OperationForm.IsHandleCreated))Thread.Sleep(10);
			StartOperationDelegate = new VoidDelegate(OperationForm.StartOperation);
			StopOperationDelegate = new VoidDelegate(OperationForm.StopOperation);
			SetTextDelegate = new SetStringDelegate(OperationForm.SetText);
			ReleaseThreadDelegate = new VoidDelegate(OperationForm.ReleaseThread);
            SetTopIsVisibleDelegate = new VoidDelegate(OperationForm.SetTopIsVisible);
            GetVisibleDelegate = new GetBoolDelegate(OperationForm.GetVisible);
			StopOperation();
		}

		private void CheckThread()
		{
			if (OperationThread == null) throw(new Exception("Operation thread has been released"));
		}

		private void ThreadEntryPoint()
		{
			OperationForm = new frmOperation();
			Application.Run(OperationForm);						
		}

		public void StartOperation()
		{
			CheckThread();
			OperationForm.Invoke(StartOperationDelegate);
		}

		public void StopOperation()
		{
			CheckThread();
			OperationForm.Invoke(StopOperationDelegate);
		}

		public string Text
		{
			set
			{
				CheckThread();
				OperationForm.Invoke(SetTextDelegate, new object[] {value});
			}
		}

        public void SetTopIsVisible()
        {
            CheckThread();
            OperationForm.Invoke(SetTopIsVisibleDelegate);
        }

        public bool Visible
        {
            get
            {
                CheckThread();
                return (bool)OperationForm.Invoke(GetVisibleDelegate);
            }
        }

		public void ReleaseThread()
		{
			CheckThread();
			//OperationThread.Abort();
			OperationForm.Invoke(ReleaseThreadDelegate);
			OperationThread = null;
			//Application.ExitThread();
		}
	}
}
