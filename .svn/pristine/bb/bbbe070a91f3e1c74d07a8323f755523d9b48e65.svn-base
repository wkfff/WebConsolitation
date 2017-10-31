using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class FormChildTaskParams : Form
    {
        TaskParams _taskParams;

        public static bool ShowChildTaskParamsForm(TaskParams taskParams)
        {
            FormChildTaskParams frmParams = new FormChildTaskParams(taskParams);
            if (frmParams.ShowDialog() == DialogResult.OK)
                return true;
            return false;
        }

        public FormChildTaskParams(TaskParams taskParams)
        {
            InitializeComponent();
            _taskParams = taskParams;
        }

        private void cbBeginDate_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            switch (cb.Name)
            {
                case "cbBeginDate":
                    _taskParams.BeginDate = cb.Checked;
                    break;
                case "cbEndDate":
                    _taskParams.EndDate = cb.Checked;
                    break;
                case "cbOwner":
                    _taskParams.Owner = cb.Checked;
                    break;
                case "cbDoer":
                    _taskParams.Doer = cb.Checked;
                    break;
                case "cbCurator":
                    _taskParams.Curator = cb.Checked;
                    break;
                case "cbTaskKind":
                    _taskParams.TaskKind = cb.Checked;
                    break;
                case "cbGroups":
                    _taskParams.Groups = cb.Checked;
                    break;
                case "cbUsers":
                    _taskParams.Users = cb.Checked;
                    break;
            }
        }
    }

    public class TaskParams
    {
        private bool _beginDate;
        private bool _endDate;
        private bool _owner;
        private bool _doer;
        private bool _curator;
        private bool _taskKind;
        private bool _groups;
        private bool _users;

        public TaskParams()
        {
            _beginDate = false;
            _endDate = false;
            _owner = false;
            _doer = false;
            _curator = false;
            _taskKind = false;
            _groups = false;
            _users = false;
        }

        public bool BeginDate
        {
            get { return _beginDate; }
            set { _beginDate = value; }
        }

        public bool EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        public bool Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public bool Doer
        {
            get { return _doer; }
            set { _doer = value; }
        }

        public bool Curator
        {
            get { return _curator; }
            set { _curator = value; }
        }

        public bool TaskKind
        {
            get { return _taskKind; }
            set { _taskKind = value; }
        }

        public bool Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public bool Users
        {
            get { return _users; }
            set { _users = value; }
        }
    }
}