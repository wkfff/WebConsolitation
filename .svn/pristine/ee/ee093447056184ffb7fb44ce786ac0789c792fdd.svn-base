using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client; // For Oracle

namespace Krista.FM.Utils.SQLEditor
{
    public partial class frmSqlWindow : Form
    {
        /// <summary>
        /// Запрос
        /// </summary>
        private string query;
        /// <summary>
        /// Строка подключения
        /// </summary>
        private string connectionString;
        // Const definition
        const int LINE_HEIGHT = 20;
        const int BASE_LINES = 5;
        const int BASE_HEIGHT = LINE_HEIGHT * BASE_LINES;
        const int SCROLL_HEIGHT = 34;
        const int SCROLL_WIDTH = 70;

        // Variable definition
        private String strQuery = null;
        private List<String> lSQLStrings = new List<String>();  // list for saving historic sql commands

        private int currSQLStringInd = 1;

        private OracleConnection con = null;
        private OracleDataReader reader = null;
        private OracleTransaction transaction = null;
        private bool isInTransaction = false;

        private OracleCommand cmd = null;
        private OracleDataAdapter datAdapt;
        private OracleCommandBuilder cmdBuilder;
        private DataSet ds;

        public String FileName = "";

        // Code Area
        private void frmSqlWindow_Load(object sender, EventArgs e)
        {
            dgvResults.Visible = true;

            txtSqlArea.Width = this.Width;
            txtSqlArea.Height = this.Height;
            dgvResults.Width = this.Width;

            con = OneTimeVars.Instance(connectionString).OracleConn;

            ToolTip ttp = new ToolTip();

            ttp.AutoPopDelay = 5000;
            ttp.InitialDelay = 1000;
            ttp.ReshowDelay = 500;
            ttp.ShowAlways = true;

            ttp.SetToolTip(this.btnPrevSql, "Previous Sql Command");
            ttp.SetToolTip(this.btnNextSql, "Next Sql Command");
            ttp.SetToolTip(this.btnSQLHist, "Sql History");
            ttp.SetToolTip(this.btnClearHist, "Clear History");
            ttp.SetToolTip(this.btnRun, "Run (F8)");
            ttp.SetToolTip(this.btnExplain, "Explain Plan (F5)");
            ttp.SetToolTip(this.btnCommit, "Commit (F9)");
            ttp.SetToolTip(this.btnRollback, "Rollback (F11)");

            btnNextSql.Enabled = false;
            btnPrevSql.Enabled = false;
            btnCommit.Enabled = false;
            btnRollback.Enabled = false;
        }

        public List<String> GetSQLList()
        {
            return lSQLStrings;
        }

        ///////////////////////////////////////////////////////
        // 
        ///////////////////////////////////////////////////////
        public void ExecuteQuery()
        {
            // local variables
            bool isSelect;
            String trimmedSQL;

            if (txtSqlArea.ReadOnly) // do nothing if in "Read only" mode
            {
                return;
            }
            if (!con.State.Equals(ConnectionState.Open))
            {
                MessageBox.Show("You are not connected", "No Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // clear the grids data source
            dgvResults.DataSource = null;
            if (datAdapt != null && ds != null)
            {
                datAdapt.Dispose();
                ds.Clear();
            }

            // Check if user selected text to run
            if (txtSqlArea.SelectedText == "")
            {
                strQuery = txtSqlArea.Text.TrimEnd(';');
            }
            else
            {
                strQuery = txtSqlArea.SelectedText.TrimEnd(';');
            }
            if (strQuery != "")
            {
                // checking type of SQL command
                trimmedSQL = strQuery.Trim().ToUpper();
                isSelect = trimmedSQL.StartsWith("SELECT");

                // Inserting the new text to the SQL's list (after reomving all occurances of that SQL
                lSQLStrings.RemoveAll(delegate(String s) { return s.Equals(strQuery); });
                lSQLStrings.Add(strQuery);
                currSQLStringInd = lSQLStrings.Count;
                btnNextSql.Enabled = true;

                if (currSQLStringInd != 1)
                {
                    btnPrevSql.Enabled = true;
                }

                try
                {
                    // Executing the Select command and showing the data grid 
                    if (isSelect)
                    {
                        // Create the OracleCommand object
                        //cmd.AddToStatementCache = false;
                        cmd = new OracleCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.AddToStatementCache = false;
                        cmd.Connection = con;
                        cmd.CommandText = strQuery;

                        this.Cursor = Cursors.WaitCursor;

                        // Create the adapter and command builder and data set 
                        datAdapt = new OracleDataAdapter(cmd);
                        //cmdBuilder = new OracleCommandBuilder(datAdapt);
                        ds = new DataSet();
                        // Fill the dataAccess
                        datAdapt.Fill(ds);
                        // Set the grid's data source
                        dgvResults.DataSource = ds.Tables[0];


                        this.Cursor = Cursors.Default;

                        // Set the sizes of the text area and grid view
                        txtSqlArea.Height = Math.Max((txtSqlArea.Lines.Length + 1) * LINE_HEIGHT, BASE_HEIGHT);
                        txtSqlArea.Width = this.Width - SCROLL_WIDTH;
                        dgvResults.Width = this.Width - SCROLL_WIDTH;
                        dgvResults.Height = this.Height - txtSqlArea.Height - SCROLL_HEIGHT;
                        dgvResults.Location = new Point(0, txtSqlArea.Height);
                        dgvResults.Visible = true;

                        con.PurgeStatementCache();

                    }
                    else // if not SELECT statement, execute the DML/DDL Command
                    {
                        // If its a DML command
                        if (trimmedSQL.StartsWith("DELETE") || trimmedSQL.StartsWith("UPDATE") || trimmedSQL.StartsWith("INSERT"))
                        {
                            // begin a new transaction if we are not in transaction
                            if (!isInTransaction)
                            {
                                isInTransaction = true;
                                transaction = con.BeginTransaction();
                                cmd = con.CreateCommand();
                            }

                            cmd.CommandText = strQuery;
                            btnCommit.Enabled = true;
                            btnRollback.Enabled = true;
                        }
                        else
                        {
                            cmd = new OracleCommand(strQuery, con);
                            cmd.CommandType = CommandType.Text;
                        }

                        // executing the command
                        this.Cursor = Cursors.WaitCursor;
                        int n = cmd.ExecuteNonQuery();
                        this.Cursor = Cursors.Default;

                        // displaying appropriate message at the status bar 
                        if (trimmedSQL.StartsWith("DELETE"))
                        {
                        }
                        else if (trimmedSQL.StartsWith("INSERT"))
                        {
                        }
                        else if (trimmedSQL.StartsWith("UPDATE"))
                        {
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            } // if SQL Area text is null
        }

        private void ExecuteExplainPlan()
        {
            // local variables
            String ExplainCmd = "";
            if (!con.State.Equals(ConnectionState.Open))
            {
                MessageBox.Show("You are not connected", "No Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // clear the grids data source
            dgvResults.DataSource = null;

            strQuery = txtSqlArea.Text.TrimEnd(';');
            if (strQuery != "")
            {
                // Executing the explain plan for current SQL
                ExplainCmd = "explain plan for " + strQuery;

                try
                {
                    // Create the OracleCommand object
                    cmd = new OracleCommand("delete from plan_table", con);
                    int n = cmd.ExecuteNonQuery();

                    OracleCommand cmdExplain = new OracleCommand(ExplainCmd, con);
                    cmdExplain.ExecuteNonQuery();

                    ExplainCmd = "select " +
                                    "substr (lpad(' ', (level-1) * 3) || operation || ' (' || options || ')',1,30 ) \"Operation\", " +
                                    "object_name   \"Object\", " +
                                    "cost, cardinality, bytes, access_predicates " +
                                 "from plan_table " +
                                 "start with id = 0 " +
                                 "connect by prior id=parent_id";

                    OracleCommand cmdExpSQL = new OracleCommand(ExplainCmd, con);
                    cmdExpSQL.CommandType = CommandType.Text;

                    // Create the adapter and command builder and data set 
                    datAdapt = new OracleDataAdapter(cmdExpSQL);
                    cmdBuilder = new OracleCommandBuilder(datAdapt);
                    ds = new DataSet();

                    // Fill the dataAccess
                    datAdapt.Fill(ds);

                    // Set the grid's data source
                    dgvResults.DataSource = ds.Tables[0];

                    // Set the sizes of the text area and grid view
                    txtSqlArea.Height = Math.Max((txtSqlArea.Lines.Length + 1) * LINE_HEIGHT, BASE_HEIGHT);
                    txtSqlArea.Width = this.Width - SCROLL_WIDTH;
                    dgvResults.Width = this.Width - SCROLL_WIDTH;
                    dgvResults.Height = this.Height - txtSqlArea.Height - SCROLL_HEIGHT;
                    dgvResults.Location = new Point(0, txtSqlArea.Height);
                    dgvResults.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void ExecuteCommit()
        {
            if (!con.State.Equals(ConnectionState.Open))
            {
                MessageBox.Show("You are not connected", "No Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Commiting the transaction
                this.Cursor = Cursors.WaitCursor;
                transaction.Commit();
                this.Cursor = Cursors.Default;

                isInTransaction = false;
                btnCommit.Enabled = false;
                btnRollback.Enabled = false;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ExecuteRollback()
        {
            if (!con.State.Equals(ConnectionState.Open))
            {
                MessageBox.Show("You are not connected", "No Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Rollbacking the transaction
                this.Cursor = Cursors.WaitCursor;
                transaction.Rollback();
                this.Cursor = Cursors.Default;

                isInTransaction = false;
                btnCommit.Enabled = false;
                btnRollback.Enabled = false;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    break;

                case Keys.F8:
                    ExecuteQuery();
                    break;

                case Keys.F9:
                    ExecuteCommit();
                    break;

                case Keys.F11:
                    ExecuteRollback();
                    break;

                case Keys.F5:
                    ExecuteExplainPlan();
                    break;

                default:
                    /// nothing to do here ... move along
                    break;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    break;

                case Keys.F8:
                    ExecuteQuery();
                    break;

                case Keys.F9:
                    ExecuteCommit();
                    break;

                case Keys.F11:
                    ExecuteRollback();
                    break;

                case Keys.F5:
                    ExecuteExplainPlan();
                    break;

                default:
                    /// nothing to do here ... move along
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public frmSqlWindow(string startQuery, string connectionString)
        {
            this.connectionString = connectionString;

            InitializeComponent();

            frmSqlWindow_Load(this, null);

            txtSqlArea.Text = startQuery;
            txtSqlArea.ArrangeSQL();

            ExecuteQuery();
        }


        private void frmSqlWindow_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessDialogKey(e.KeyCode);
        }

        private void frmSqlWindow_Resize(object sender, EventArgs e)
        {
            // Set the GUI commands on this form to adjust to its size
            txtSqlArea.Width = Width - SCROLL_WIDTH;
            dgvResults.Width = Width - SCROLL_WIDTH;
            txtSqlArea.Height = Math.Max((txtSqlArea.Lines.Length + 1) * LINE_HEIGHT, BASE_HEIGHT);
            dgvResults.Height = this.Height - txtSqlArea.Height - SCROLL_HEIGHT;
            txtSqlArea.Anchor = AnchorStyles.Top;
            dgvResults.Anchor = AnchorStyles.Bottom;
            dgvResults.Location = new Point(0, txtSqlArea.Height);
            txtSqlArea.Location = new Point(0, 0);
        }

        private void frmSqlWindow_LocationChanged(object sender, EventArgs e)
        {

        }

        public Krista.FM.Utils.SQLEditor.RichDDLBox SQLText
        {
            get
            {
                return this.txtSqlArea;
            }

            set
            {
                this.txtSqlArea = value;
            }
        }

        private void btnNextSql_Click(object sender, EventArgs e)
        {
            // Preparing for the next SQL statement
            txtSqlArea.Text = "";
            currSQLStringInd++;

            // If the current node is not the last one, fetch the next SQL lfrom the history ist
            if ((currSQLStringInd - 1) != lSQLStrings.Count)
            {
                txtSqlArea.ParseDDL(lSQLStrings[currSQLStringInd - 1]);
            }
            else
            {
                btnNextSql.Enabled = false;
            }

            btnPrevSql.Enabled = true;
        }

        private void btnPrevSql_Click(object sender, EventArgs e)
        {
            // Enabling the Next button
            btnNextSql.Enabled = true;

            // Get the previous SQL statement
            txtSqlArea.Text = "";
            currSQLStringInd--;
            txtSqlArea.ParseDDL(lSQLStrings[currSQLStringInd - 1]);

            // Disabling the Prev button if it is the first SQL in history
            if (currSQLStringInd == 1)
            {
                btnPrevSql.Enabled = false;
            }
        }

        private void btnClearHist_Click(object sender, EventArgs e)
        {
            // Displaying dialog window to confirm the clearing
            DialogResult reply = MessageBox.Show("Are you sure you want to clear all SQL history ?",
                    "Confirm clearing SQL history", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

            // Clear SQL history
            if (reply == DialogResult.Yes)
            {
                lSQLStrings.Clear();
                currSQLStringInd = 1;
                btnPrevSql.Enabled = false;
                btnNextSql.Enabled = false;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            ExecuteQuery();
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            // Here need to Commit DDL if needed. GuyH
            ExecuteCommit();
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            // Here need to rollback DDL if needed. GuyH
            ExecuteRollback();
        }

        private void btnExplain_Click(object sender, EventArgs e)
        {
            ExecuteExplainPlan();
        }

        private void arrangeSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtSqlArea.ArrangeSQL();
        }

        public void ShowViewReadOnly(String strOnwer, String strName)
        {
            // setting form to "read only" mode
            txtSqlArea.ReadOnly = true;
            btnClearHist.Enabled = false;
            btnCommit.Enabled = false;
            btnExplain.Enabled = false;
            btnNextSql.Enabled = false;
            btnPrevSql.Enabled = false;
            btnRollback.Enabled = false;
            btnRun.Enabled = false;
            btnSQLHist.Enabled = false;

            ShowView(strOnwer, strName);

        }
        public void ShowView(String strOnwer, String strName)
        {
            this.Cursor = Cursors.WaitCursor;
            con = OneTimeVars.Instance(connectionString).OracleConn;

            try
            {
                // getting the text of view
                String strQuery = "select text, text_length from all_views " +
                                  "where owner ='" + strOnwer + "' and view_name = '" + strName + "'";

                txtSqlArea.Text = "CREATE OR REPLACE VIEW " + strOnwer + "." + strName + " AS\n";

                cmd = new OracleCommand(strQuery, con);

                // setting off the limitation on fetching "LONG" oracle columns 
                cmd.InitialLONGFetchSize = -1;

                reader = cmd.ExecuteReader();

                // get view text
                if (reader.Read())
                {
                    int nViewLength = (int)reader.GetDecimal(1);
                    txtSqlArea.Text += reader.GetString(0) + ";";
                }

            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);
            }

            this.Cursor = Cursors.Default;
        }

        public void SetText(String str)
        {
            txtSqlArea.Text = "";
            txtSqlArea.ParseDDL(str);
        }

        private void dgvResults_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void frmSqlWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //con.Close();
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            query = txtSqlArea.Text.Trim(';');

            Close();
        }

        public string Query
        {
            get { return query; }
        }
    }
}