using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Krista.FM.Update.Framework.Controls
{
    public partial class PatchListControl : UserControl
    {
        /// <summary>
        /// Список обновлений
        /// </summary>
        private IList<IUpdatePatch> patches = new List<IUpdatePatch>();
        /// <summary>
        /// true - режим, когда применяем обновления
        /// false - когда просматриваем установленные
        /// </summary>
        private bool readOnlyMode;
        /// <summary>
        /// Работаем со службой обновления или с клиентским приложением
        /// </summary>
        private bool isServerMode;

        public PatchListControl()
        {
            InitializeComponent();
            InitializeDataGridView();

            dataGridView.CellFormatting += dataGridView_CellFormatting;
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == dataGridView.Columns["Description"].Index)
                    && e.Value != null)
            {
                DataGridViewCell cell =
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (!String.IsNullOrEmpty(((UpdateItemView)dataGridView.Rows[e.RowIndex].Tag).Patch.DescriptionDetail))
                {
                    cell.ToolTipText = ((UpdateItemView)dataGridView.Rows[e.RowIndex].Tag).Patch.DescriptionDetail;
                }
                else
                {
                    cell.ToolTipText = ((UpdateItemView)dataGridView.Rows[e.RowIndex].Tag).Patch.Description;
                }
            }
        }

        private void InitializeDataGridView()
        {
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.RowHeadersWidth = 15;

            dataGridView.ColumnCount = ColumnCount();

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            dataGridView.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            DataGridViewColumnCollection dgvcColumns = dataGridView.Columns;

            dgvcColumns.Clear();

            InitializeColumn(dgvcColumns);
            
            dataGridView.AutoGenerateColumns = false;

            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.RowHeadersWidthSizeMode =
                DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        protected virtual int ColumnCount()
        {
            return 2;
        }

        protected virtual void InitializeColumn(DataGridViewColumnCollection dgvcColumns)
        {
            dgvcColumns.Add(DataGridViewColumnFactory.BuildTextColumnStyle("Name", "Имя патча", 100, 100, 100, true));
            dgvcColumns.Add(DataGridViewColumnFactory.BuildTextColumnStyle("Description", "Описание патча", 300, 300, 300, false));
        }

        public DataGridView DataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        public IList<IUpdatePatch> Patches
        {
            get { return patches; }
            set
            {
                if (value != null)
                {
                    patches = value;
                    Initialize();
                }
            }
        }
        
        public bool IsServerMode
        {
            get { return isServerMode; }
            set { isServerMode = value;
                InitializeReadsMode();
            }
        }

        public bool ReadOnlyMode
        {
            get { return readOnlyMode; }
            set { readOnlyMode = value;
            InitializeReadsMode();}
        }

        public void Initialize()
        {
            InitializeUpdatesTable();
            dataGridView.Sort(dataGridView.Columns[0], ListSortDirection.Ascending);
        }

        private void InitializeReadsMode()
        {
            if (ReadOnlyMode)
            {
                btReport.Visible = false;
                btnRollback.Visible = false;
                btApplay.Visible = true;
            }
            else
            {
                btReport.Visible = true;
                btnRollback.Visible = !isServerMode;
                btApplay.Visible = false;
            }
        }

        private void InitializeUpdatesTable()
        {
            foreach (var updatePatch in patches)
            {
                UpdateItemView itemView = new UpdateItemView(updatePatch);
                DataGridViewRow row =  new DataGridViewRow {Tag = itemView};

                if (FillRow(row, updatePatch))
                {
                    dataGridView.Rows.Add(row);
                }
                    
                switch (updatePatch.Use)
                {
                        case Use.Optional:
                            row.DefaultCellStyle.BackColor = Color.LemonChiffon;
                            break;
                        case Use.Required:
                            row.DefaultCellStyle.BackColor = Color.Orange;
                            break;
                        case Use.Prohibited:
                            row.DefaultCellStyle.BackColor = Color.Red;
                            break;
                }
            }
        }

        protected virtual bool FillRow(DataGridViewRow row, IUpdatePatch updatePatch)
        {
            row.Cells.Add(new DataGridViewTextBoxCell());
            row.Cells.Add(new DataGridViewTextBoxCell());

            row.Cells[0].Value = updatePatch.Name;
            row.Cells[1].Value = updatePatch.Description;

            return true;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            if (ParentForm != null)
            {
                ParentForm.Close();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    public class UpdateItemView
    {
        public UpdateItemView(IUpdatePatch patch)
        {
            Patch = patch;
        }

        public IUpdatePatch Patch { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Patch.Name, Patch.Description);
        }
    }
}
