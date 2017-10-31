using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Krista.FM.Common;

namespace Krista.FM.Client.OlapAdmin.Connection
{
    public partial class ConnectionForm : Form
    {
        /// <summary>
        /// имя текущего подключения
        /// </summary>
        public string ConnectionName { get; set; }

        public string ConnectionPath { get; set; }

        public string ConnectionString { get; set; }

        public ConnectionForm(string connectionName, string connectionPath, string connectionString)
        {
            InitializeComponent();
            ConnectionPath = connectionPath;
            ConnectionName = connectionName.Trim();
            ConnectionString = connectionString;

            InitConnectionList();
        }

        /// <summary>
        /// Инициализируем список подключений
        /// </summary>
        public void InitConnectionList()
        {
            if (!Directory.Exists(ConnectionPath))
            {
                Directory.CreateDirectory(ConnectionPath);
            }

            lvConnections.Items.Clear();
            string[] connectionList = Directory.GetFiles(ConnectionPath, "*.udl");

            foreach (string connection in connectionList)
            {
                string connName = Path.GetFileNameWithoutExtension(connection);
                UltraListViewItem item = lvConnections.Items.Add(null, connName);
                item.Appearance.Image = 0;

                if (String.Format("{0}.udl", connName).ToUpper() == this.ConnectionName.ToUpper())
                {
                    lvConnections.SelectedItems.Add(item);
                }
            }

            propertyGrid.Refresh();
            //lvConnections.Focus();
            //InitConnectionProperties();
        }

        /// <summary>
        /// Получить имя файла для нового подключения
        /// </summary>
        /// <returns></returns>
        private string GetNameForConnection()
        {
            string connFileName = String.Format("{0}\\{1}.udl", ConnectionPath, ConnectionName);
            int i = 1;
            while (File.Exists(connFileName))
            {
                i++;
                connFileName = String.Format("{0}\\{1}({2}).udl", ConnectionPath, ConnectionName, i);
            }

            return connFileName; // Path.GetFileName(connFileName);
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            string connFileName = GetNameForConnection();
            StreamWriter sw = new StreamWriter(connFileName, true, Encoding.Unicode);
            sw.WriteLine("[oledb]");
            sw.WriteLine("; Everything after this line is an OLE DB initstring");
            sw.WriteLine(ConnectionString);
            sw.Close();


            Process.Start(connFileName);
            this.ConnectionName = Path.GetFileName(connFileName);
            InitConnectionList();
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    string destFileName = String.Format("{0}\\{1}", ConnectionPath, Path.GetFileName(fileName));
                    File.Copy(fileName, destFileName, true);
                }

                InitConnectionList();
            }
        }

        private void InitConnectionProperties()
        {
            propertyGrid.SelectedObject = null;
            if (lvConnections.SelectedItems.Count > 0)
            {
                ConnectionString cs = new ConnectionString();
                string fileName = String.Empty;
                try
                {
                    fileName = String.Format("{0}\\{1}.udl", ConnectionPath,
                                             lvConnections.SelectedItems[0].Value);
                    cs.ReadConnectionString(fileName);
                }
                catch
                {
                    return;
                }

                propertyGrid.SelectedObject = new ConnectionStringBrowse(cs, fileName, this);
            }
        }

        private void lvConnections_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
        {
            InitConnectionProperties();
        }

        private void ChangeConnectionProperties()
        {
            if (lvConnections.SelectedItems.Count > 0)
            {
                string selectedConnection = lvConnections.SelectedItems[0].Value.ToString() + ".udl";
                Process.Start(String.Format("{0}\\{1}", ConnectionPath, selectedConnection));
                InitConnectionProperties();
            }

        }

        private void btChange_Click(object sender, EventArgs e)
        {
            ChangeConnectionProperties();
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (lvConnections.SelectedItems.Count > 0)
            {
                string selectedConnection = lvConnections.SelectedItems[0].Value.ToString() + ".udl";
                File.Delete(String.Format("{0}\\{1}", ConnectionPath, selectedConnection));
                InitConnectionList();
            }

        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.ConnectionName = String.Empty;
            if (lvConnections.SelectedItems.Count > 0)
            {
                this.ConnectionName = lvConnections.SelectedItems[0].Value.ToString() + ".udl";
            }
        }

        private void ConnectionForm_Activated(object sender, EventArgs e)
        {
            InitConnectionProperties();
        }

        private void lvConnections_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
        {
            ChangeConnectionProperties();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
        }

        private static void btCancel_Click(object sender, EventArgs e)
        {
        }

    }

    public sealed class ConnectionStringBrowse
    {
        private readonly ConnectionString _connStr;
        private string _connFileName;
        private readonly ConnectionForm _connectionForm;

        public ConnectionStringBrowse(ConnectionString cs, string connectionFileName, ConnectionForm cForm)
        {
            this._connStr = cs;
            this._connFileName = connectionFileName.Trim();
            this._connectionForm = cForm;
        }

        [DisplayName("Имя подключения")]
        public string ConnectionName
        {
            get { return GetConnectionName(); }
            set { SetConnectionName(value); }
        }

        public string Provider
        {
            get { return this._connStr.Provider; }
        }

        public string DataSource
        {
            get { return this._connStr.DataSource; }
        }

        public string InitialCatalog
        {
            get { return this._connStr.InitialCatalog; }
        }

        public string ConnectTo
        {
            get { return this._connStr.ConnectTo; }
        }

        public int? ConnectTimeOut
        {
            get { return this._connStr.ConnectTimeout; }
        }

        public string IntegratedSecurity
        {
            get { return this._connStr.IntegratedSecurity; }
        }

        public bool PersistSecurityInfo
        {
            get { return this._connStr.PersistSecurityInfo; }
        }


        public string Timeout
        {
            get { return this._connStr.Timeout; }
        }

        public string UserID
        {
            get { return this._connStr.UserID; }
        }

        private void SetConnectionName(string value)
        {
            value = value.Trim();

            if (String.IsNullOrEmpty(this._connFileName) || String.IsNullOrEmpty(value))
                return;

            string destFileName = String.Format("{0}\\{1}.udl", _connectionForm.ConnectionPath, value);
            if (this._connFileName == destFileName)
                return;

            if (File.Exists(destFileName))
            {
                if (MessageBox.Show(String.Format("Подключение '{0}' уже существует. Заменить его?", value),
                    "OLAPAdmin", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
                File.Delete(destFileName);
            }

            File.Move(this._connFileName, destFileName);
            if (this._connectionForm != null)
            {
                this._connectionForm.ConnectionName = String.Format("{0}.udl", value);
                this._connectionForm.InitConnectionList();
            }
            this._connFileName = destFileName;
        }

        private string GetConnectionName()
        {
            return Path.GetFileNameWithoutExtension(this._connFileName);
        }
    }
}
