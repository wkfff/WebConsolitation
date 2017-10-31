using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    internal class DetailView : BaseView
    {
        internal Components.UltraGridEx ugeDetailData;

        private void InitializeComponent()
        {
            this.ugeDetailData = new Krista.FM.Client.Components.UltraGridEx();
            this.SuspendLayout();
            // 
            // ugeDetailData
            // 
            this.ugeDetailData.AllowAddNewRecords = true;
            this.ugeDetailData.AllowClearTable = true;
            this.ugeDetailData.Caption = "";
            this.ugeDetailData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeDetailData.InDebugMode = false;
            this.ugeDetailData.LoadMenuVisible = false;
            this.ugeDetailData.Location = new System.Drawing.Point(0, 0);
            this.ugeDetailData.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeDetailData.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeDetailData.Name = "ugeDetailData";
            this.ugeDetailData.SaveLoadFileName = "";
            this.ugeDetailData.SaveMenuVisible = false;
            this.ugeDetailData.ServerFilterEnabled = false;
            this.ugeDetailData.SingleBandLevelName = "Добавить запись...";
            this.ugeDetailData.Size = new System.Drawing.Size(588, 536);
            this.ugeDetailData.sortColumnName = "";
            this.ugeDetailData.StateRowEnable = false;
            this.ugeDetailData.TabIndex = 0;
            // 
            // DetailView
            // 
            this.Controls.Add(this.ugeDetailData);
            this.Name = "DetailView";
            this.ResumeLayout(false);

        }
    }
}
