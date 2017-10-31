using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ProtocolsViewer
{
    public partial class ProtocolsViewForm : Form
    {

        private string TypeOfProtocol = String.Empty;

        public ProtocolsViewForm()
        {
            InitializeComponent();
            SetGridProperty();
        }

        private void SetGridProperty()
        {
            // устанавливаем видимости кнопок тулбара
            protocolsObject.GridEx.utmMain.Tools["Refresh"].SharedProps.Visible = false;
            protocolsObject.GridEx.utmMain.Tools["SaveChange"].SharedProps.Visible = false;
            protocolsObject.GridEx.utmMain.Tools["CancelChange"].SharedProps.Visible = false;
            protocolsObject.GridEx.utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
            protocolsObject.GridEx.utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;
            
            protocolsObject.GridEx.utmMain.Tools["menuSave"].SharedProps.Visible = false;

            protocolsObject.GridEx.utmMain.Tools["excelImport"].SharedProps.Visible = false; 
            protocolsObject.GridEx.utmMain.Tools["menuReports"].SharedProps.Visible = false;
            
            protocolsObject.GridEx.utmMain.Tools["menuLoad"].SharedProps.Enabled = true;
            protocolsObject.GridEx.utmMain.Tools["ImportFromXML"].SharedProps.Enabled = true;

            protocolsObject.GridEx.utmMain.Toolbars["utbColumns"].Visible = false;

            protocolsObject.GridEx.AllowImportFromXML = true;
            protocolsObject.GridEx.OnLoadFromXML += new Krista.FM.Client.Components.SaveLoadXML(GridEx_OnLoadFromXML);
            protocolsObject.GridEx.OnInitializeRow += new Krista.FM.Client.Components.InitializeRow(GridEx_OnInitializeRow);

            DataSet dsnew = new DataSet();
            dsnew.Tables.Add();
            protocolsObject.GridEx.DataSource = dsnew;
        }

      
        bool GridEx_OnLoadFromXML(object sender)
        {
            bool returnValue = true;
            try
            {
                FileDialog dlg;
                dlg = new OpenFileDialog();
                dlg.Filter = "XML документы *.xml|*.xml";
                DialogResult dlgRes = dlg.ShowDialog();

                if (dlgRes == DialogResult.OK)
                {
                    string tmpFileName = dlg.FileName;
                    DataSet dsprotocol = new DataSet();
                    
                    dsprotocol.ReadXml(tmpFileName, XmlReadMode.Auto);

                    if (dsprotocol.Tables.Count != 0)
                    {
                        TypeOfProtocol = dsprotocol.DataSetName;
                        SetTypeOfColumn(dsprotocol);
                        protocolsObject.GridEx.DataSource = dsprotocol;

                        //protocolsObject.GridEx.ugData.DisplayLayout.Bands[0].Columns["EVENTDATETIME"].Format = "dd.MM.yyyy HH:mm:ss";
                        //protocolsObject.GridEx.ugData.DisplayLayout.Bands[0].Columns["EVENTDATETIME"].Width = 120;
                        
                        statuslbFileName.Text = tmpFileName;
                        protocolsObject.GridEx.SaveLoadFileName = Path.GetFileNameWithoutExtension(tmpFileName);
                        
                        protocolsObject.GridEx.utmMain.Toolbars["utbColumns"].Visible = true;
                        protocolsObject.GridEx.utmMain.Tools["menuReports"].SharedProps.Visible = true;
                    }
                    else
                    {
                        string warnstr = String.Format("Файл '{0}' невозможно загрузить.", tmpFileName);
                        MessageBox.Show(warnstr, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        returnValue = false;
                    }
                   
                }
                dlg.Dispose();
            }
            catch (Exception exception)
            {

                if (exception.Message.Contains("is denied") || exception.Message.Contains("Отказано в доступе"))
                {
                    string errStr = "Приложение не может получить доступ к файлу. Возможно он используется другим процессом или защищен от записи.";
                    MessageBox.Show(errStr, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    returnValue = false;
                }
            }
            finally
            {

            }
            return returnValue;
        }

        
        
        Type GetTypeOfData(string ColumnName)
        {
            switch (ColumnName)
            { 
                case "ID":
                    return typeof(Decimal);
                case "EVENTDATETIME":
                    return typeof(DateTime);
                default:
                    return typeof(String);
            }
        }
        
        private void SetTypeOfColumn(DataSet dsprotocol)
        {
            string OldColumnName = string.Empty;
            int QuantityOfColums = dsprotocol.Tables[0].Columns.Count;

            for (int count = 0; count < QuantityOfColums; count++)
            {
                string NewColumnName = String.Format("{1}{0}", dsprotocol.Tables[0].Columns[count].ColumnName, "tmp");
                dsprotocol.Tables[0].Columns.Add(NewColumnName,GetTypeOfData(dsprotocol.Tables[0].Columns[count].ColumnName));
                
                foreach (DataRow DR in dsprotocol.Tables[0].Rows)
                {
                    DR[NewColumnName] = DR[count];
                }

                OldColumnName = dsprotocol.Tables[0].Columns[count].ColumnName;
                int ColumnPosition = dsprotocol.Tables[0].Columns[count].Ordinal;

                dsprotocol.Tables[0].Columns.Remove(OldColumnName);
                dsprotocol.Tables[0].Columns[NewColumnName].ColumnName = OldColumnName;
                dsprotocol.Tables[0].Columns[OldColumnName].SetOrdinal(ColumnPosition);
            }
        }

        void GridEx_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            UltraGridRow row = Krista.FM.Client.Components.UltraGridHelper.GetRowCells(e.Row);
            UltraGridCell cell = null;
            int? val = -1;

            if (TypeOfProtocol == "UpdateModule")
            {
                cell = row.Cells["ModificationType"];
                val = Convert.ToInt32(cell.Value);

                switch (val)
                {
                    case 0:
                        cell.Appearance.Image = this.protocolsObject.GridEx.il.Images[2];
                        cell.ToolTipText = "Создание нового объекта";
                        break;
                    case 1:
                        cell.Appearance.Image = this.protocolsObject.GridEx.il.Images[0];
                        cell.ToolTipText = "Изменение структуры";
                        break;
                    case 2:
                        cell.Appearance.Image = this.protocolsObject.GridEx.il.Images[1];
                        cell.ToolTipText = "Удаление существующего объекта";
                        break;
                }
            }
            if (TypeOfProtocol == "ClassifiersModule")
            {
                cell = row.Cells["OBJECTTYPE"];
                if (cell.Value != DBNull.Value && cell.Value != null)
                    val = Convert.ToInt32(cell.Value);
                switch (val)
                {
                    case 0:
                        cell.ToolTipText = "Тип объекта: 'Сопоставимый классификатор'";
                        break;
                    case 1:
                        cell.ToolTipText = "Тип объекта: 'Классификатор данных'";
                        break;
                    case 2:
                        cell.ToolTipText = "Тип объекта: 'Фиксированный классификатор'";
                        break;
                    case 3:
                        cell.ToolTipText = "Тип объекта: 'Таблица фактов'";
                        break;
                    case 4:
                        cell.ToolTipText = "Тип объекта: 'Системная таблица'";
                        break;
                }
            }
        }

       

    }

    }