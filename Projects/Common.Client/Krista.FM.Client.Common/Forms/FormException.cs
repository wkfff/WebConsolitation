using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Windows.Forms;
using Krista.Diagnostics;
using Krista.FM.Client.Common.Properties;
using Krista.FM.Common.Exceptions;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.Common.Forms
{
	public enum ErrorFormResult {efrContinue, efrRestart, efrClose};
	
    [Flags]
    public enum ErrorFormButtons
    { 
        Exit = 1, 
        Continue = 2, 
        Report = 8, 
        DetailInfo = 16,
        WithoutTerminate = Continue | Report | DetailInfo,
        All = Exit | Continue | Report | DetailInfo
    };

    public partial class FormException : Form
    {
        public FormException()
        {
            InitializeComponent();
        }

        private ErrorFormResult Result;

        /// <summary>
        /// ������ ����������� �����.
        /// </summary>
        private int FormExceptionExpandedHeight = 508;

        /// <summary>
        /// ������ ������.
        /// </summary>
        private int tbErrorExpandedHeight = 360;

        public static ErrorFormResult ShowErrorForm(Exception e)
        {
            return ShowErrorForm(e, ErrorFormButtons.All);
        }

        public static ErrorFormResult ShowErrorForm(Exception e, ErrorFormButtons buttons)
        {
            FormException tmpFrm = new FormException();
            tmpFrm.Result = ErrorFormResult.efrContinue;
            FrMessage friendlyMessage = FriendlyExceptionService.GetFriendlyMessage(e);

            // ����������� �� ������.
            tmpFrm.CenterToScreen();

            // ��������� �������� �� ����� ������, ����� ����������� ����� ���� �� ������.
            int formOffset = (Screen.PrimaryScreen.Bounds.Height - tmpFrm.FormExceptionExpandedHeight) / 2;
            // ��������� ����� � �������� ����.
            tmpFrm.Location = new Point(tmpFrm.Location.X, formOffset);

            // ���� ������ ����������, ������ ��� �����.
            if (friendlyMessage.erType == ErrType.Application)
            {
                tmpFrm.Text = "������ ����������";
                tmpFrm.pbErrIcon.Image = ResourceService.GetBitmap("Error");
            }

            tmpFrm.tbFriendlyMessage.Text = friendlyMessage.Message;

            //string[] errMsg = { e.Message, "", "���������� �����:", "", e.StackTrace };
            List<string> errMsg = new List<string>(new string[] { e.Message, "", "��������� ����������:", "" });
            errMsg.AddRange(KristaDiagnostics.ExpandException(e).Split(new string[] { "\r\n" }, StringSplitOptions.None));
            tmpFrm.tbErrorText.Lines = errMsg.ToArray();
            tmpFrm.tbErrorText.SelectionStart = 0;
            tmpFrm.tbErrorText.SelectionLength = 0;
            tmpFrm.SetButtonsAccessibility(buttons);
            tmpFrm.ShowDialog();
            return tmpFrm.Result;
        }

        /// <summary>
        /// ������������� ����������� ������ �� �����.
        /// </summary>
        /// <param name="buttons">��������� ������.</param>
        private void SetButtonsAccessibility(ErrorFormButtons buttons)
        {
            this.btnClose.Enabled = (buttons & ErrorFormButtons.Exit) > 0;
            this.btnContinue.Enabled = (buttons & ErrorFormButtons.Continue) > 0;
            this.btnReport.Enabled = (buttons & ErrorFormButtons.Report) > 0;
            this.btnDetails.Enabled = (buttons & ErrorFormButtons.DetailInfo) > 0;
        }

        private void btnClick(object sender, EventArgs e)
        {
            Control senderButton = (Control)sender;
            switch (senderButton.Name)
            {
                case "btnContinue":
                    Result = ErrorFormResult.efrContinue;
                    break;
                case "btnClose":
                    Result = ErrorFormResult.efrClose;
                    break;
            }
            this.Close();
        }

        /// <summary>
        /// ��������� ����� �� ������ � ����.
        /// </summary>
        private void SaveReport()
        {
            saveFileDialog.FileName = string.Format("{0:ddMMyyyy}_����� �� ������", DateTime.Now);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream Report = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter ReportText = new StreamWriter(Report);
                foreach (string line in tbErrorText.Lines)
                {
                    ReportText.WriteLine(line);
                }
                ReportText.WriteLine("���� � ����� ������������� ������: {0}", DateTime.Now);
                ReportText.Close();
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            ShowHideDetails();
        }

        /// <summary>
        /// �������� ��� ���������� ������ ������.
        /// </summary>
        private void ShowHideDetails()
        {
            if (tbErrorText.Visible)
            {
                btnDetails.Text = "�������� ������ >>";
                this.Height = this.MinimumSize.Height;
            }
            else
            {
                btnDetails.Text = "������ ������ <<";
                this.Height = FormExceptionExpandedHeight;
                tbErrorText.Height = tbErrorExpandedHeight;
            }
            tbErrorText.Visible = !tbErrorText.Visible;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            SaveReport();
        }
    }
}