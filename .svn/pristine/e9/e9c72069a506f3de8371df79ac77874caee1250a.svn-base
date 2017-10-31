using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Client.ProcessManager
{
    public partial class ProcessOptionControl : UserControl
    {
        private IProcessManager manager;

        public ProcessOptionControl()
        {
            InitializeComponent();

            rbParallelMode.Click += new EventHandler(rbModeClick);
            rbSequentialMode.Click += new EventHandler(rbModeClick);
            rbOneTransaction.Click += new EventHandler(rbModeClick);
            rbSeparateTansaction.Click += new EventHandler(rbModeClick);
        }

        void rbModeClick(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            switch (rb.Name)
            {
                case "rbParallelMode":
                    {
                        manager.ProcessMode = ProcessMode.ParallelMode;
                        gbTransaction.Enabled = false;
                        break;
                    }
                case "rbSequentialMode":
                    {
                        manager.ProcessMode = ProcessMode.SequentialMode;
                        gbTransaction.Enabled = true;
                        break;
                    }
                case "rbOneTransaction":
                    {
                        manager.TransactionMode = TransactionMode.OneTransaction;
                        break;
                    }
                case "rbSeparateTansaction":
                    {
                        manager.TransactionMode = TransactionMode.SeparateTransaction;
                        break;
                    }
            }
        }

        private void Initialize()
        {
            switch (manager.ProcessMode)
            {
                case ProcessMode.ParallelMode:
                    rbParallelMode.Checked = true;
                    gbTransaction.Enabled = false;
                    break;
                case ProcessMode.SequentialMode:
                    rbSequentialMode.Checked = true;
                    switch (manager.TransactionMode)
                    {
                        case TransactionMode.OneTransaction:
                            rbOneTransaction.Checked = true;
                            break;
                        case TransactionMode.SeparateTransaction:
                            rbSeparateTansaction.Checked = true;
                            break;
                    }
                    break;
            }
        }

        public void Connect(IProcessManager manager)
        {
            this.manager = manager;
            Initialize();
        }


    }
}
