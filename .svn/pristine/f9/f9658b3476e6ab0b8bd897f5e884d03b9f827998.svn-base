using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class BatchDetailViewer : UserControl
    {
        private BindingSource biSourceBatch_Accumulator;

        public BatchDetailViewer()
        {
            InitializeComponent();
            gridBatchDetail.AutoGenerateColumns = false;
        }

        public void Connect(BindingSource _biSourceBatch_Accumulator, bool activateConnection)
        {
            biSourceBatch_Accumulator = _biSourceBatch_Accumulator;
            if (activateConnection)
            {
                Connect();
            }            
        }

        public void Connect()
        {
            if (biSourceBatch_Accumulator != null)
            {                
                gridBatchDetail.DataSource = biSourceBatch_Accumulator;                
            }
        }

        public void Disconnect()
        {
            gridBatchDetail.DataSource = null;
        }
    }
}
