using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ProtocolsUI
{
    public partial class ProtocolsViewObject : BaseViewObj, IInplaceProtocolView
    {
        #region объекты для хранения значений и названия параметров фильтрации внедренного лога
        private class CashedDBParam
        {
            public string ParamName;
            public object ParamValue;
            public DbType ParamType;
            
            public CashedDBParam(string paramName, object paramValue, DbType paramType)
            {
                ParamName = paramName;
                ParamValue = paramValue;
                ParamType = paramType;
            }
        }

        private class CashedDBParameters
        {
            private Dictionary<int, CashedDBParam> parameters = new Dictionary<int,CashedDBParam>();

            public void Init(IDbDataParameter[] proxyParams)
            {
                parameters.Clear();
                for (int i = 0; i < proxyParams.Length; i++)
                {
                    IDbDataParameter curParam = proxyParams[i];
                    CashedDBParam cashedParam = new CashedDBParam(curParam.ParameterName, curParam.Value, curParam.DbType);
                    parameters.Add(i, cashedParam);
                }
            }

            public IDbDataParameter[] ToParamsArray()
            {
                if (parameters.Count == 0)
                    return null;

                IDbDataParameter[] res = new IDbDataParameter[parameters.Count];
                int i = 0;
                foreach (CashedDBParam prm in parameters.Values)
                {
                    res[i] = new DbParameterDescriptor(prm.ParamName, prm.ParamValue);
                    i++;
                }
                return res;
            }
        }
        #endregion

        private CashedDBParameters AttachFilterParams = new CashedDBParameters();
        private string AttachFilter = string.Empty;
        private DataTable AttachLogDataTable = new DataTable();
        //private ModulesTypes ActiveViewLog = ModulesTypes.DataPumpModule;
        private ModulesTypes AttachModuleType;
        bool inInplaceMode = false;
        string ProtocolName = string.Empty;
        /// <summary>
        /// свойство, указывающее на то, что протокол внедрен куда то
        /// </summary>
        public bool InInplaceMode
        {
            get { return inInplaceMode; }
            set { inInplaceMode = value; }
        }

        public UltraGridEx GridComponent
        {
            get { return this.pView.ugex1; }
        }

        /// <summary>
        /// Вставка лога в другой компонент
        /// </summary>
        public void AttachViewObject(ModulesTypes mt, Control ParentArea, string Filter, params IDbDataParameter[] FilterParams)
        {
            AttachViewObject(mt, ParentArea, String.Empty, Filter, FilterParams);
        }

        /// <summary>
        /// Вставка лога в другой компонент
        /// </summary>
        public void AttachViewObject(ModulesTypes mt, Control ParentArea, string protocolFileName, string Filter, params IDbDataParameter[] FilterParams)
        {
            InInplaceMode = true;
            AttachModuleType = mt;
            currentProtocol = mt;
            //if (protocolFileName == String.Empty)
            
                switch (mt)
                {
                    case ModulesTypes.BridgeOperationsModule:
                        ProtocolName = "Протокол сопоставления классификаторов";
                        break;
                    case ModulesTypes.DataPumpModule:
                        ProtocolName = "Протокол закачки данных";
                        break;
                    case ModulesTypes.DeleteDataModule:
                        ProtocolName = "Протокол удаления данных";
                        break;
                    case ModulesTypes.MDProcessingModule:
                        ProtocolName = "Протокол Обработка многомерных хранилищ";
                        break;
                    case ModulesTypes.ProcessDataModule:
                        ProtocolName = "Протокол обработки данных";
                        break;
                    case ModulesTypes.ReviseDataModule:
                        ProtocolName = "Протокол сверки данных";
                        break;
                    case ModulesTypes.PreviewDataModule:
                        ProtocolName = "Протокол предпросмотра данных";
                        break;
                    case ModulesTypes.ClassifiersModule:
                        ProtocolName = "Протокол классификаторов";
                        break;
                    case ModulesTypes.AuditModule:
                        ProtocolName = "Аудит";
                        break;
                    case ModulesTypes.DataSourceModule:
                        ProtocolName = "Протокол источников данных";
                        break;
                }
            
            /*else
            {
                pView.ugex1.SaveLoadFileName = protocolFileName;
            }*/

            pView.ugex1.Parent = ParentArea;
            pView.ugex1.Dock = DockStyle.Fill;
            RefreshAttachData(protocolFileName, Filter, FilterParams);
        }

		/// <summary>
		/// Обновление данных в гриде
		/// </summary>
		public void RefreshAttachData()
		{
            RefreshAttachData( String.Empty, String.Empty, null);
		}

		/// <summary>
		/// Обновление данных в гриде с изменением фильтром на данные
		/// </summary>
        public void RefreshAttachData(string protocolFileName, string Filter, params IDbDataParameter[] FilterParams)
		{
            IBaseProtocol viewProtocol = null;
            IDatabase db = null;
            Workplace.OperationObj.Text = "Запрос данных";
            Workplace.OperationObj.StartOperation();
            
            try
            {
                viewProtocol = Workplace.ActiveScheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
                db = Workplace.ActiveScheme.SchemeDWH.DB;

                AttachLogDataTable.Clear();

                if (Filter != String.Empty)
                    AttachFilter = Filter;
                if (FilterParams != null)
                {
                    AttachFilterParams.Init(FilterParams);
                }

                viewProtocol.GetProtocolData(AttachModuleType, ref AttachLogDataTable, AttachFilter, AttachFilterParams.ToParamsArray());

                pView.ugex1.DataSource = AttachLogDataTable;
                CustomizeColumns();
                SetSortDirection(pView.ugex1.ugData);
                
                pView.ugex1.SaveLoadFileName = String.Format("{0}_{1}", ProtocolName, protocolFileName);
            }
            finally
            {
                if (viewProtocol != null)
                    viewProtocol.Dispose();
                
                if (db != null)
                    db.Dispose();

                Workplace.OperationObj.StopOperation();
            }
		}
    }
}