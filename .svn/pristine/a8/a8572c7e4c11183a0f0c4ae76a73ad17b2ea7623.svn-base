using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Workplace.Gui;


namespace Krista.FM.Client.Workplace.Services
{
    public class DockManagerService
    {
        private static UltraDockManager dockManager;

        public static void Attach(Form parentForm)
        {
            dockManager = new UltraDockManager(parentForm.Container);

            ((System.ComponentModel.ISupportInitialize)(dockManager)).BeginInit();
            
            dockManager.HostControl = parentForm;

            UnpinnedTabArea unpinnedTabAreaLeft = new UnpinnedTabArea();
            UnpinnedTabArea unpinnedTabAreaRight = new UnpinnedTabArea();
            UnpinnedTabArea unpinnedTabAreaTop = new UnpinnedTabArea();
            UnpinnedTabArea unpinnedTabAreaBottom = new UnpinnedTabArea();
            AutoHideControl autoHideControl = new AutoHideControl();

            // 
            // unpinnedTabAreaLeft
            // 
            unpinnedTabAreaLeft.Dock = System.Windows.Forms.DockStyle.Left;
            unpinnedTabAreaLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            unpinnedTabAreaLeft.Location = new System.Drawing.Point(0, 25);
            unpinnedTabAreaLeft.Name = "unpinnedTabAreaLeft";
            unpinnedTabAreaLeft.Owner = dockManager;
            unpinnedTabAreaLeft.Size = new System.Drawing.Size(0, 525);
            unpinnedTabAreaLeft.TabIndex = 31;
            // 
            // unpinnedTabAreaRight
            // 
            unpinnedTabAreaRight.Dock = System.Windows.Forms.DockStyle.Right;
            unpinnedTabAreaRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            unpinnedTabAreaRight.Location = new System.Drawing.Point(931, 25);
            unpinnedTabAreaRight.Name = "unpinnedTabAreaRight";
            unpinnedTabAreaRight.Owner = dockManager;
            unpinnedTabAreaRight.Size = new System.Drawing.Size(0, 525);
            unpinnedTabAreaRight.TabIndex = 32;
            // 
            // unpinnedTabAreaTop
            // 
            unpinnedTabAreaTop.Dock = System.Windows.Forms.DockStyle.Top;
            unpinnedTabAreaTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            unpinnedTabAreaTop.Location = new System.Drawing.Point(0, 25);
            unpinnedTabAreaTop.Name = "unpinnedTabAreaTop";
            unpinnedTabAreaTop.Owner = dockManager;
            unpinnedTabAreaTop.Size = new System.Drawing.Size(931, 0);
            unpinnedTabAreaTop.TabIndex = 33;
            // 
            // unpinnedTabAreaBottom
            // 
            unpinnedTabAreaBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            unpinnedTabAreaBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            unpinnedTabAreaBottom.Location = new System.Drawing.Point(0, 550);
            unpinnedTabAreaBottom.Name = "unpinnedTabAreaBottom";
            unpinnedTabAreaBottom.Owner = dockManager;
            unpinnedTabAreaBottom.Size = new System.Drawing.Size(931, 0);
            unpinnedTabAreaBottom.TabIndex = 34;
            // 
            // autoHideControl
            // 
            autoHideControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            autoHideControl.Location = new System.Drawing.Point(0, 0);
            autoHideControl.Name = "autoHideControl";
            autoHideControl.Owner = dockManager;
            autoHideControl.Size = new System.Drawing.Size(0, 0);
            autoHideControl.TabIndex = 35;

            parentForm.Controls.Add(autoHideControl);
            parentForm.Controls.Add(unpinnedTabAreaTop);
            parentForm.Controls.Add(unpinnedTabAreaBottom);
            parentForm.Controls.Add(unpinnedTabAreaLeft);
            parentForm.Controls.Add(unpinnedTabAreaRight);

            ((System.ComponentModel.ISupportInitialize)(dockManager)).EndInit();
        }

        public static DockableControlPane AttachControl(string key/*, Control control*/)
        {
            DockableControlPane dcp;
            if (dockManager.ControlPanes.Exists(key))
            {
                dcp = dockManager.ControlPanes[key];
            }
            else
            {
                dcp = new DockableControlPane(key/*, control*/);
                DockAreaPane dap = new DockAreaPane(DockedLocation.DockedLeft);
                dap.Panes.Add(dcp);

                dockManager.DockAreas.Add(dap);
            }
            dcp.Closed = false;
            dcp.Activate();
            return dcp;
        }

        public static UltraDockManager Control
        {
            get
            {
                //System.Diagnostics.Debug.Assert(dockManager != null);
                return dockManager;
            }
        }
    }
}
