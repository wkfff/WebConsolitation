using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor
{
    public partial class EntityGridControl : UserControl
    {
        Krista.FM.ServerLibrary.IEntity entity;

        public EntityGridControl()
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeUltraGridParams(entityGrid._ugData);
        }

        private void RefreshData()
        {
            if (entity != null)
            {
                using (IDataUpdater du = entity.GetDataUpdater())
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    entityGrid.DataSource = dt;
                }
            }
        }

        internal Krista.FM.ServerLibrary.IEntity Entity
        {
            get { return entity; }
            set 
            {
                entity = value;
                if (entity != null)
                    entityGrid.IsReadOnly = (entity.ParentPackage.IsLocked) ? false : true;

                RefreshData();
            }
        }

        private bool ultraGridEx_OnRefreshData(object sender)
        {
            RefreshData();
            return default(bool);
        }

        private void ultraGridEx_OnCancelChanges(object sender)
        {
            RefreshData();
        }

        private bool ultraGridEx_OnSaveChanges(object sender)
        {
            return false;
        }
    }
}
