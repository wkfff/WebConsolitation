using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.DetailUI
{
    public class DetailViewObject : BaseViewObject.BaseView
    {
        private Components.UltraGridEx ugeDetailGrid;
    
        public override void Customize()
        {
            ComponentCustomizer.CustomizeInfragisticsComponents(this.Container);
            base.Customize();
        }

        public UltraGridEx DetailGrid
        {
            get { return ugeDetailGrid; }
        }

        private void InitializeComponent()
        {
            this.ugeDetailGrid = new Krista.FM.Client.Components.UltraGridEx();
            this.SuspendLayout();
            // 
            // ugeDetailGrid
            // 
            this.ugeDetailGrid.AllowAddNewRecords = true;
            this.ugeDetailGrid.AllowClearTable = true;
            this.ugeDetailGrid.Caption = "";
            this.ugeDetailGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeDetailGrid.EnableGroups = false;
            this.ugeDetailGrid.InDebugMode = false;
            this.ugeDetailGrid.LoadMenuVisible = false;
            this.ugeDetailGrid.Location = new System.Drawing.Point(0, 0);
            this.ugeDetailGrid.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeDetailGrid.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeDetailGrid.Name = "ugeDetailGrid";
            this.ugeDetailGrid.SaveLoadFileName = "";
            this.ugeDetailGrid.SaveMenuVisible = false;
            this.ugeDetailGrid.ServerFilterEnabled = false;
            this.ugeDetailGrid.SingleBandLevelName = "Добавить запись...";
            this.ugeDetailGrid.Size = new System.Drawing.Size(702, 454);
            this.ugeDetailGrid.sortColumnName = "";
            this.ugeDetailGrid.StateRowEnable = false;
            this.ugeDetailGrid.TabIndex = 0;
            // 
            // DetailViewObject
            // 
            this.Controls.Add(this.ugeDetailGrid);
            this.Name = "DetailViewObject";
            this.Size = new System.Drawing.Size(702, 454);
            this.ResumeLayout(false);

        }
    }
}
