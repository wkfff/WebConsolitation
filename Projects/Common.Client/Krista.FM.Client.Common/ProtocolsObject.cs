using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

using System.Collections.Generic;

namespace Krista.FM.Client.Common
{
    /// <summary>
    /// компонент с гридом для отображения протоколов
    /// </summary>
    public partial class ProtocolsObject : UserControl
    {
        private UltraGridEx ugeProtocolGrid;
        // объект схемы из которой получим все необходимые данные
        //private IScheme _scheme;
        private System.ComponentModel.IContainer components;
        internal ImageList ilColumns;
        internal ImageList ilObjectType;
        private ModulesTypes currentProtocol;

        private DataTable protocolData;

        private IScheme _scheme;
        IBaseProtocol protocolObject;

        private string _filter;
        private CashedDBParameters filterParam;
        /// <summary>
        /// инициализация объекта протоколов
        /// </summary>
        public ProtocolsObject()
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeUltraGridParams(this.ugeProtocolGrid._ugData);
            // настройка грида протоколов
            this.ugeProtocolGrid.AllowAddNewRecords = false;
            this.ugeProtocolGrid.AllowClearTable = false;
            this.ugeProtocolGrid.AllowDeleteRows = false;
            this.ugeProtocolGrid.AllowEditRows = false;
            this.ugeProtocolGrid.AllowImportFromXML = false;
            this.ugeProtocolGrid.StateRowEnable = true;

            this.ugeProtocolGrid.OnGridInitializeLayout += new GridInitializeLayout(ugeProtocolGrid_OnGridInitializeLayout);
            this.ugeProtocolGrid.OnGetGridColumnsState += new GetGridColumnsState(ugeProtocolGrid_OnGetGridColumnsState);
            this.ugeProtocolGrid.OnInitializeRow += new InitializeRow(ugeProtocolGrid_OnInitializeRow);
            this.ugeProtocolGrid.OnRefreshData += new RefreshData(ugeProtocolGrid_OnRefreshData);
        }

        public void ShowProtocol(ModulesTypes protocolType, IScheme scheme, string filter, params IDbDataParameter[] filterParams)
        {
            _scheme = scheme;
            protocolObject = _scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
            ShowProtocol(protocolType, filter, filterParams);
            // запоминаем парамерты фильтра для рефреша
            _filter = filter;
            filterParam = new CashedDBParameters();
            filterParam.Init(filterParams);
        }

        private void ShowProtocol(ModulesTypes protocolType, string filter, params IDbDataParameter[] filterParams)
        {
            // получаем данные по конкретному протоколу
            protocolObject.GetProtocolData(protocolType, ref protocolData, filter, filterParams);
            this.ugeProtocolGrid.DataSource = protocolData;
            currentProtocol = protocolType;
        }

        /// <summary>
        /// компонент с гридом
        /// </summary>
        public UltraGridEx GridEx
        {
            get { return this.ugeProtocolGrid; }
        }

        #region внутренние события грида

        bool ugeProtocolGrid_OnRefreshData(object sender)
        {
            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                ShowProtocol(currentProtocol, _filter, filterParam.ToParamsArray(db));

				foreach (UltraGridBand band in GridEx._ugData.DisplayLayout.Bands)
				{
					band.Columns["ID"].SortIndicator = SortIndicator.Descending;
				}
				return true;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        void ugeProtocolGrid_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridCell iniCell = e.Row.Cells["pic"];
            iniCell.Column.AutoSizeMode = ColumnAutoSizeMode.None;
            iniCell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            string message = string.Empty;
            switch (System.Convert.ToInt32(e.Row.Cells["KindsOfEvents"].Value))
            {
                #region DataPumpEventKind
                // Расшифровка сообщений для протокола закачки данных
                case (int)DataPumpEventKind.dpeStart:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; // Индкс картинки в листе
                    iniCell.Value = "Начало операции закачки";
                    iniCell.ToolTipText = "Начало операции закачки";
                    break;
                case (int)DataPumpEventKind.dpeInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)DataPumpEventKind.dpeWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)DataPumpEventKind.dpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции закачки";
                    iniCell.ToolTipText = "Успешное окончание операции закачки";
                    break;
                case (int)DataPumpEventKind.dpeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции закачки с ошибкой";
                    iniCell.ToolTipText = "Окончание операции закачки с ошибкой";
                    break;
                case (int)DataPumpEventKind.dpeError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе закачки";
                    iniCell.ToolTipText = "Ошибка в процессе закачки";
                    break;
                case (int)DataPumpEventKind.dpeCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе закачки";
                    iniCell.ToolTipText = "Критическая ошибка в процессе закачки";
                    break;
                case (int)DataPumpEventKind.dpeStartFilePumping:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало закачки файла";
                    iniCell.ToolTipText = "Начало закачки файла";
                    break;
                case (int)DataPumpEventKind.dpeSuccessfullFinishFilePump:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение закачки файла";
                    iniCell.ToolTipText = "Успешное завершение закачки файла";
                    break;
                case (int)DataPumpEventKind.dpeFinishFilePumpWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение закачки файла с ошибкой";
                    iniCell.ToolTipText = "Завершение закачки файла с ошибкой";
                    break;
                case (int)DataPumpEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)DataPumpEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)DataPumpEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region BridgeOperationsEventKind
                // Расшифровка сообщений для протокола сопоставления данных
                case (int)BridgeOperationsEventKind.boeStart:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции сопоставления";
                    iniCell.ToolTipText = "Начало операции сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.boeInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)BridgeOperationsEventKind.boeWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)BridgeOperationsEventKind.boeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции сопоставления";
                    iniCell.ToolTipText = "Успешное окончание операции сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.boeFinishedWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции сопоставления с ошибкой";
                    iniCell.ToolTipText = "Окончание операции сопоставления с ошибкой";
                    break;
                case (int)BridgeOperationsEventKind.boeError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе сопоставления";
                    iniCell.ToolTipText = "Ошибка в процессе сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.boeCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе сопоставления";
                    iniCell.ToolTipText = "Критическая ошибка в процессе сопоставления";
                    break;
                case (int)BridgeOperationsEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)BridgeOperationsEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)BridgeOperationsEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region UsersOperationEventKind
                // Расшифровка сообщений для протокола действий пользователей
                case (int)UsersOperationEventKind.uoeUserConnectToScheme:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[6]; //6;
                    iniCell.Value = "Пользователь подключился к схеме";
                    iniCell.ToolTipText = "Пользователь подключился к схеме";
                    break;
                case (int)UsersOperationEventKind.uoeUserDisconnectFromScheme:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[7]; //7;
                    iniCell.Value = "Пользователь отключился от схемы";
                    iniCell.ToolTipText = "Пользователь отключился от схемы";
                    break;
                case (int)UsersOperationEventKind.uoeChangeUsersTable:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[12]; //7;
                    iniCell.Value = "Изменение таблицы пользователей";
                    iniCell.ToolTipText = "Изменение таблицы пользователей";
                    break;
                case (int)UsersOperationEventKind.uoeChangeGroupsTable:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[13]; //7;
                    iniCell.Value = "Изменение таблицы групп пользователей";
                    iniCell.ToolTipText = "Изменение таблицы групп пользователей";
                    break;
                case (int)UsersOperationEventKind.uoeChangeDepartmentsTable:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[15]; //7;
                    iniCell.Value = "Изменение таблицы отделов";
                    iniCell.ToolTipText = "Изменение таблицы отделов";
                    break;
                case (int)UsersOperationEventKind.uoeChangeOrganizationsTable:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[15]; //7;
                    iniCell.Value = "Изменение таблицы организаций";
                    iniCell.ToolTipText = "Изменение таблицы организаций";
                    break;
                case (int)UsersOperationEventKind.uoeChangeTasksTypes:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[15]; //7;
                    iniCell.Value = "Изменение таблицы типов задач";
                    iniCell.ToolTipText = "Изменение таблицы типов задач";
                    break;
                case (int)UsersOperationEventKind.uoeChangeMembershipsTable:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[16]; //7;
                    iniCell.Value = "Изменение вхождения пользователя в группу";
                    iniCell.ToolTipText = "Изменение вхождения пользователя в группу";
                    break;
                case (int)UsersOperationEventKind.uoeUntilledExceptionsEvent:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //7;
                    iniCell.Value = "Необработанная ошибка";
                    iniCell.ToolTipText = "Необработанная ошибка";
                    break;
                case (int)UsersOperationEventKind.uoeSchemeUpdate:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[21]; //7;
                    iniCell.Value = "Обновление схемы";
                    iniCell.ToolTipText = "Обновление схемы";
                    break;
                case (int)UsersOperationEventKind.uoeProtocolsToArchive:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //7;
                    iniCell.Value = "Протоколы заархивированы";
                    iniCell.ToolTipText = "Протоколы заархивированы";
                    break;
                #endregion

                #region SystemEventKind
                case (int)SystemEventKind.seInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация";
                    iniCell.ToolTipText = "Информация";
                    break;
                case (int)SystemEventKind.seError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Ошибка";
                    iniCell.ToolTipText = "Ошибка";
                    break;
                case (int)SystemEventKind.seCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка";
                    iniCell.ToolTipText = "Критическая ошибка";
                    break;
                case (int)SystemEventKind.seWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                #endregion

                #region ProcessDataEventKind
                case (int)ProcessDataEventKind.pdeStart:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции обработки данных";
                    iniCell.ToolTipText = "Начало операции обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции обработки данных";
                    iniCell.ToolTipText = "Успешное окончание операции обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции обработки данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции обработки данных с ошибкой";
                    break;
                case (int)ProcessDataEventKind.pdeCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе обработки данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе обработки данных";
                    iniCell.ToolTipText = "Ошибка в процессе обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе обработки данных";
                    iniCell.ToolTipText = "Информация в процессе обработки данных";
                    break;
                case (int)ProcessDataEventKind.pdeWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)ProcessDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)ProcessDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)ProcessDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region DeleteDataEventKind
                case (int)DeleteDataEventKind.ddeStart:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции удаления данных";
                    iniCell.ToolTipText = "Начало операции удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции удаления данных";
                    iniCell.ToolTipText = "Успешное окончание операции удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции удаления данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции удаления данных с ошибкой";
                    break;
                case (int)DeleteDataEventKind.ddeCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе удаления данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе удаления данных";
                    iniCell.ToolTipText = "Ошибка в процессе удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе удаления данных";
                    iniCell.ToolTipText = "Информация в процессе удаления данных";
                    break;
                case (int)DeleteDataEventKind.ddeWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)DeleteDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)DeleteDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)DeleteDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region MDProcessingEventKind
                case (int)MDProcessingEventKind.mdpeStart:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Начало операции обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Успешное окончание операции обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeFinishedWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции обработки многомерных хранилищ с ошибкой";
                    iniCell.ToolTipText = "Окончание операции обработки многомерных хранилищ с ошибкой";
                    break;
                case (int)MDProcessingEventKind.mdpeCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Критическая ошибка в процессе обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Ошибка в процессе обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе обработки многомерных хранилищ";
                    iniCell.ToolTipText = "Информация в процессе обработки многомерных хранилищ";
                    break;
                case (int)MDProcessingEventKind.mdpeWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)MDProcessingEventKind.InvalidateObject:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //8;
                    iniCell.Value = "Требование на расчет";
                    iniCell.ToolTipText = "Требование на расчет";
                    break;
                #endregion

                #region ReviseDataEventKind
                case (int)ReviseDataEventKind.pdeStart:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции проверки данных";
                    iniCell.ToolTipText = "Начало операции проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции проверки данных";
                    iniCell.ToolTipText = "Успешное окончание операции проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции проверки данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции проверки данных с ошибкой";
                    break;
                case (int)ReviseDataEventKind.pdeCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе проверки данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе проверки данныхщ";
                    iniCell.ToolTipText = "Ошибка в процессе проверки данныхщ";
                    break;
                case (int)ReviseDataEventKind.pdeInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе проверки данных";
                    iniCell.ToolTipText = "Информация в процессе проверки данных";
                    break;
                case (int)ReviseDataEventKind.pdeWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)ReviseDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)ReviseDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)ReviseDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;

                #endregion

                #region предпросмотр данных
                case (int)PreviewDataEventKind.dpeStart:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; // Индкс картинки в листе
                    iniCell.Value = "Начало операции предпросмотра данных";
                    iniCell.ToolTipText = "Начало операции предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)PreviewDataEventKind.dpeWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)PreviewDataEventKind.dpeSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции предпросмотра данных";
                    iniCell.ToolTipText = "Успешное окончание операции предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeFinishedWithErrors:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции предпросмотра данных с ошибкой";
                    iniCell.ToolTipText = "Окончание операции предпросмотра данных с ошибкой";
                    break;
                case (int)PreviewDataEventKind.dpeError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка в процессе предпросмотра данных";
                    iniCell.ToolTipText = "Ошибка в процессе предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка в процессе предпросмотра данных";
                    iniCell.ToolTipText = "Критическая ошибка в процессе предпросмотра данных";
                    break;
                case (int)PreviewDataEventKind.dpeStartFilePumping:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки файла";
                    iniCell.ToolTipText = "Начало обработки файла";
                    break;
                case (int)PreviewDataEventKind.dpeSuccessfullFinishFilePump:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)PreviewDataEventKind.dpeFinishFilePumpWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки файла с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки файла с ошибкой";
                    break;
                case (int)PreviewDataEventKind.dpeStartDataSourceProcessing:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[8]; //8;
                    iniCell.Value = "Начало обработки источника данных";
                    iniCell.ToolTipText = "Начало обработки источника данных";
                    break;
                case (int)PreviewDataEventKind.dpeSuccessfullFinishDataSourceProcess:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[9]; //9;
                    iniCell.Value = "Успешное завершение обработки файла";
                    iniCell.ToolTipText = "Успешное завершение обработки файла";
                    break;
                case (int)PreviewDataEventKind.dpeFinishDataSourceProcessingWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[10]; //10;
                    iniCell.Value = "Завершение обработки источника данных с ошибкой";
                    iniCell.ToolTipText = "Завершение обработки источника данных с ошибкой";
                    break;
                #endregion

                #region BridgeOperationsEventKind
                // Расшифровка сообщений для протокола сопоставления данных
                case (int)ClassifiersEventKind.ceInformation:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)ClassifiersEventKind.ceWarning:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[2]; //2;
                    iniCell.Value = "Предупреждение";
                    iniCell.ToolTipText = "Предупреждение";
                    break;
                case (int)ClassifiersEventKind.ceSuccefullFinished:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции";
                    iniCell.ToolTipText = "Успешное окончание операции";
                    break;
                case (int)ClassifiersEventKind.ceStartHierarchySet:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //10;
                    iniCell.Value = "Начало операции установки иерархии";
                    iniCell.ToolTipText = "Начало операции установки иерархии";
                    break;
                case (int)ClassifiersEventKind.ceSuccessfullFinishHierarchySet:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //4;
                    iniCell.Value = "Успешное окончание операции установки иерархии";
                    iniCell.ToolTipText = "Успешное окончание операции установки иерархии";
                    break;
                case (int)ClassifiersEventKind.ceFinishHierarchySetWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции установки иерархии с ошибкой";
                    iniCell.ToolTipText = "Окончание операции установки иерархии с ошибкой";
                    break;
                case (int)ClassifiersEventKind.ceError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Ошибка";
                    iniCell.ToolTipText = "Ошибка";
                    break;
                case (int)ClassifiersEventKind.ceCriticalError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[5]; //5;
                    iniCell.Value = "Критическая ошибка";
                    iniCell.ToolTipText = "Критическая ошибка";
                    break;
                case (int)ClassifiersEventKind.ceClearClassifierData:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //8;
                    iniCell.Value = "Начало операции очистки данных";
                    iniCell.ToolTipText = "Начало операции очистки данных";
                    break;
                case (int)ClassifiersEventKind.ceImportClassifierData:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //9;
                    iniCell.Value = "Начало операции импорта";
                    iniCell.ToolTipText = "Начало операции импорта";
                    break;
                case (int)ClassifiersEventKind.ceCreateBridge:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //0;
                    iniCell.Value = "Начало операции формирования сопоставимого классификатора";
                    iniCell.ToolTipText = "Начало операции формирования сопоставимого классификатора";
                    break;
                case (int)ClassifiersEventKind.ceVariantCopy:
                    message = "Вариант скопирован";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[17];
                    break;
                case (int)ClassifiersEventKind.ceVariantDelete:
                    message = "Вариант удален";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[20];
                    break;
                case (int)ClassifiersEventKind.ceVariantLock:
                    message = "Вариант закрыт от изменений";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[19];
                    break;
                case (int)ClassifiersEventKind.ceVariantUnlok:
                    message = "Вариант открыт для изменений";
                    iniCell.Value = message;
                    iniCell.ToolTipText = message;
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[18];
                    break;

                #endregion

                #region обновления схемы
                case (int)UpdateSchemeEventKind.BeginUpdate:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[0]; //10;
                    iniCell.Value = "Начало операции обновления схемы";
                    iniCell.ToolTipText = "Начало операции обновления схемы";
                    break;
                case (int)UpdateSchemeEventKind.Information:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[1]; //1;
                    iniCell.Value = "Информация в процессе";
                    iniCell.ToolTipText = "Информация в процессе";
                    break;
                case (int)UpdateSchemeEventKind.EndUpdateSuccess:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[3]; //3;
                    iniCell.Value = "Успешное окончание операции обновления схемы";
                    iniCell.ToolTipText = "Успешное окончание операции обновления схемы";
                    break;
                case (int)UpdateSchemeEventKind.EndUpdateWithError:
                    iniCell.Appearance.ImageBackground = this.ilColumns.Images[4]; //4;
                    iniCell.Value = "Окончание операции обновления схемы с ошибкой";
                    iniCell.ToolTipText = "Окончание операции обновления схемы с ошибкой";
                    break;
                #endregion
            }

            UltraGridRow row = UltraGridHelper.GetRowCells(e.Row);
            UltraGridCell cell = null;
            int? val = -1;

            if (currentProtocol == ModulesTypes.UpdateModule)
            {
                cell = row.Cells["ModificationType"];
                val = Convert.ToInt32(cell.Value);

                switch (val)
                {
                    case 0:
                        cell.Appearance.Image = this.ugeProtocolGrid.il.Images[2];
                        cell.ToolTipText = "Создание нового объекта";
                        break;
                    case 1:
                        cell.Appearance.Image = this.ugeProtocolGrid.il.Images[0];
                        cell.ToolTipText = "Изменение структуры";
                        break;
                    case 2:
                        cell.Appearance.Image = this.ugeProtocolGrid.il.Images[1];
                        cell.ToolTipText = "Удаление существующего объекта";
                        break;
                }
            }

            if (currentProtocol == ModulesTypes.ClassifiersModule)
            {
                cell = row.Cells["OBJECTTYPE"];
                if (cell.Value != DBNull.Value && cell.Value != null)
                    val = Convert.ToInt32(cell.Value);
                switch (val)
                {
                    case 0:
                        cell.Appearance.Image = this.ilObjectType.Images[0];
                        cell.ToolTipText = "Тип объекта: 'Сопоставимый классификатор'";
                        break;
                    case 1:
                        cell.Appearance.Image = this.ilObjectType.Images[1];
                        cell.ToolTipText = "Тип объекта: 'Классификатор данных'";
                        break;
                    case 2:
                        cell.Appearance.Image = this.ilObjectType.Images[2];
                        cell.ToolTipText = "Тип объекта: 'Фиксированный классификатор'";
                        break;
                    case 3:
                        cell.Appearance.Image = this.ilObjectType.Images[3];
                        cell.ToolTipText = "Тип объекта: 'Таблица фактов'";
                        break;
                    case 4:
                        cell.Appearance.Image = this.ilObjectType.Images[4];
                        cell.ToolTipText = "Тип объекта: 'Системная таблица'";
                        break;
                }
            }
        }

        void ugeProtocolGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private Dictionary<string, GridColumnsStates> cashedColumnsSettings = new Dictionary<string, GridColumnsStates>();

        GridColumnsStates ugeProtocolGrid_OnGetGridColumnsState(object sender)
        {
            if (cashedColumnsSettings.ContainsKey(currentProtocol.ToString()))
                return cashedColumnsSettings[currentProtocol.ToString()];

            GridColumnsStates states = new GridColumnsStates();
            if (currentProtocol == ModulesTypes.AuditModule)
            {
                GridColumnState state = new GridColumnState();
                state.ColumnCaption = "ID";
                state.ColumnName = "ID";
                states.Add("ID", state);

                state = new GridColumnState();
                state.ColumnCaption = "Время операции";
                state.ColumnName = "CHANGETIME";
                state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
                states.Add("CHANGETIME", state);

                state = new GridColumnState();
                state.ColumnCaption = "Тип операции";
                state.ColumnName = "KINDOFOPERATION";
                states.Add("KINDOFOPERATION", state);

                state = new GridColumnState();
                state.ColumnCaption = "Объект БД";
                state.ColumnName = "OBJECTNAME";
                states.Add("OBJECTNAME", state);

                state = new GridColumnState();
                state.ColumnCaption = "Тип объекта БД";
                state.ColumnName = "OBJECTTYPE";
                states.Add("OBJECTTYPE", state);

                state = new GridColumnState();
                state.ColumnCaption = "Пользователь";
                state.ColumnName = "USERNAME";
                states.Add("USERNAME", state);

                state = new GridColumnState();
                state.ColumnCaption = "ID сессии";
                state.ColumnName = "SESSIONID";
                state.IsHiden = true;
                states.Add("SESSIONID", state);

                state = new GridColumnState();
                state.ColumnCaption = "ID записи объекта";
                state.ColumnName = "RECORDID";
                states.Add("RECORDID", state);

                state = new GridColumnState();
                state.ColumnCaption = "ID задачи";
                state.ColumnName = "TASKID";
                states.Add("TASKID", state);

                state = new GridColumnState();
                state.ColumnCaption = "ID операции закачки";
                state.ColumnName = "PUMPID";
                states.Add("PUMPID", state);
            }
            else
            {
                GridColumnState state = new GridColumnState();
                state.ColumnCaption = "ID записи протокола";
                state.ColumnName = "ID";
                states.Add("ID", state);

                state = new GridColumnState();
                state.ColumnCaption = "";
                state.ColumnName = "RefUsersOperations";
                state.IsHiden = true;
                states.Add("RefUsersOperations", state);

                state = new GridColumnState();
                state.ColumnCaption = "Операция закачки";
                state.ColumnName = "PumpHistoryID";
                states.Add("PumpHistoryID", state);

                state = new GridColumnState();
                state.ColumnCaption = "ID источника данных";
                state.ColumnName = "DataSourceID";
                states.Add("DataSourceID", state);

                state = new GridColumnState();
                state.ColumnCaption = string.Empty;
                state.ColumnName = "ObjectName";
                state.IsHiden = true;
                states.Add("ObjectName", state);

                state = new GridColumnState();
                state.ColumnCaption = String.Empty;
                state.ColumnName = "ActionName";
                state.IsHiden = true;
                states.Add("ActionName", state);

                state = new GridColumnState();
                state.ColumnCaption = "Дата/Время";
                state.ColumnName = "EventDateTime";
                //state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
                state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                state.ColumnWidth = 120;
                states.Add("EventDateTime", state);

                state = new GridColumnState();
                state.ColumnCaption = "Сообщение";
                state.ColumnName = "InfoMessage";
                state.ColumnWidth = 500;
                states.Add("InfoMessage", state);

                state = new GridColumnState();
                state.ColumnCaption = "Пользователь";
                state.ColumnName = "UserName";
                states.Add("UserName", state);

                state = new GridColumnState();
                state.ColumnCaption = String.Empty;
                state.ColumnName = "KindsOfEvents";
                state.IsHiden = true;
                states.Add("KindsOfEvents", state);

                state = new GridColumnState();
                state.ColumnCaption = "Модуль";
                state.IsHiden = false;

                state.ColumnName = "MODULE";
                state.ColumnWidth = 150;
                states.Add("Module", state);

                state = new GridColumnState();
                state.ColumnCaption = "Имя объекта";
                state.ColumnName = "MDObjectName";
                states.Add("MDObjectName", state);

                state = new GridColumnState();
                state.ColumnCaption = "Объект БД";
                state.ColumnName = "Classifier";
                states.Add("Classifier", state);

                state = new GridColumnState();
                state.ColumnCaption = "Сопоставляемый";
                state.ColumnName = "BridgeRoleA";
                states.Add("BridgeRoleA", state);

                state = new GridColumnState();
                state.ColumnCaption = "Сопоставимый";
                state.ColumnName = "BridgeRoleB";
                states.Add("BridgeRoleB", state);

                state = new GridColumnState();
                state.ColumnCaption = "Машина пользователя";
                state.ColumnName = "USERHOST";
                states.Add("USERHOST", state);

                //SessionID
                state = new GridColumnState();
                state.ColumnCaption = "ID сессии";
                state.ColumnName = "SessionID";
                state.IsHiden = true;
                states.Add("SessionID", state);

                state = new GridColumnState();
                state.ColumnCaption = "";
                state.ColumnName = "ObjectFullName";
                state.IsHiden = true;
                states.Add("ObjectFullName", state);

                state = new GridColumnState();
                state.ColumnCaption = "Имя объекта";
                state.ColumnName = "ObjectFullCaption";
                states.Add("ObjectFullCaption", state);

                state = new GridColumnState();
                state.ColumnCaption = "Тип модификации";
                state.ColumnName = "ModificationType";
                states.Add("ModificationType", state);

                state = new GridColumnState();
                state.ColumnCaption = "Тип объекта БД";
                state.ColumnName = "OBJECTTYPE";
                states.Add("OBJECTTYPE", state);

                //

                state = new GridColumnState();
                state.ColumnCaption = "Тип объекта";
                state.ColumnName = "OlapObjectType";
                states.Add("OlapObjectType", state);

                state = new GridColumnState();
                state.ColumnCaption = "Идентификатор объекта";
                state.ColumnName = "ObjectIdentifier";
                states.Add("ObjectIdentifier", state);

                state = new GridColumnState();
                state.ColumnCaption = "Идентификатор пакета";
                state.ColumnName = "BatchId";
                states.Add("BatchId", state);
            }
            cashedColumnsSettings.Add(currentProtocol.ToString(), states);

            return states;
        }

        #endregion

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtocolsObject));
            this.ugeProtocolGrid = new Krista.FM.Client.Components.UltraGridEx();
            this.ilColumns = new System.Windows.Forms.ImageList(this.components);
            this.ilObjectType = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // ugeProtocolGrid
            // 
            this.ugeProtocolGrid.AllowAddNewRecords = true;
            this.ugeProtocolGrid.AllowClearTable = true;
            this.ugeProtocolGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeProtocolGrid.InDebugMode = false;
            this.ugeProtocolGrid.LoadMenuVisible = false;
            this.ugeProtocolGrid.Location = new System.Drawing.Point(0, 0);
            this.ugeProtocolGrid.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeProtocolGrid.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeProtocolGrid.Name = "ugeProtocolGrid";
            this.ugeProtocolGrid.SaveLoadFileName = "";
            this.ugeProtocolGrid.SaveMenuVisible = false;
            this.ugeProtocolGrid.ServerFilterEnabled = false;
            this.ugeProtocolGrid.SingleBandLevelName = "Добавить запись...";
            this.ugeProtocolGrid.Size = new System.Drawing.Size(516, 365);
            this.ugeProtocolGrid.sortColumnName = "";
            this.ugeProtocolGrid.StateRowEnable = false;
            this.ugeProtocolGrid.TabIndex = 0;
            // 
            // ilColumns
            // 
            this.ilColumns.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilColumns.ImageStream")));
            this.ilColumns.TransparentColor = System.Drawing.Color.Magenta;
            this.ilColumns.Images.SetKeyName(0, "BeginWork.bmp");
            this.ilColumns.Images.SetKeyName(1, "Message.bmp");
            this.ilColumns.Images.SetKeyName(2, "Warning.bmp");
            this.ilColumns.Images.SetKeyName(3, "SuccefullFinish.bmp");
            this.ilColumns.Images.SetKeyName(4, "Error.bmp");
            this.ilColumns.Images.SetKeyName(5, "CriticalError.bmp");
            this.ilColumns.Images.SetKeyName(6, "UserBeginWork.bmp");
            this.ilColumns.Images.SetKeyName(7, "UserEndWork.bmp");
            this.ilColumns.Images.SetKeyName(8, "FilePumpBegin.bmp");
            this.ilColumns.Images.SetKeyName(9, "FilePumpSucces.bmp");
            this.ilColumns.Images.SetKeyName(10, "FilePumpError.bmp");
            this.ilColumns.Images.SetKeyName(11, "GoTo.bmp");
            this.ilColumns.Images.SetKeyName(12, "User.bmp");
            this.ilColumns.Images.SetKeyName(13, "users.bmp");
            this.ilColumns.Images.SetKeyName(14, "systemObjects.bmp");
            this.ilColumns.Images.SetKeyName(15, "reference.bmp");
            this.ilColumns.Images.SetKeyName(16, "changeUsersInGroup.bmp");
            this.ilColumns.Images.SetKeyName(17, "Copy.bmp");
            this.ilColumns.Images.SetKeyName(18, "Check.bmp");
            this.ilColumns.Images.SetKeyName(19, "ProtectForm.bmp");
            this.ilColumns.Images.SetKeyName(20, "deleteRows.bmp");
            this.ilColumns.Images.SetKeyName(21, "SchemeUpdate.bmp");
            // 
            // ilObjectType
            // 
            this.ilObjectType.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilObjectType.ImageStream")));
            this.ilObjectType.TransparentColor = System.Drawing.Color.Magenta;
            this.ilObjectType.Images.SetKeyName(0, "cls_Bridge_16.gif");
            this.ilObjectType.Images.SetKeyName(1, "cls_Data_16.gif");
            this.ilObjectType.Images.SetKeyName(2, "cls_Fixed_16.gif");
            this.ilObjectType.Images.SetKeyName(3, "cls_FactTable_16.gif");
            this.ilObjectType.Images.SetKeyName(4, "Table.bmp");
            // 
            // ProtocolsObject
            // 
            this.Controls.Add(this.ugeProtocolGrid);
            this.Name = "ProtocolsObject";
            this.Size = new System.Drawing.Size(516, 365);
            this.ResumeLayout(false);

        }

        #region внутренние объекты хранения параметров фильтра протокола

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
            private Dictionary<int, CashedDBParam> parameters = new Dictionary<int, CashedDBParam>();

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

            public IDbDataParameter[] ToParamsArray(IDatabase db)
            {
                if (parameters.Count == 0)
                    return null;

                IDbDataParameter[] res = new IDbDataParameter[parameters.Count];
                int i = 0;
                foreach (CashedDBParam prm in parameters.Values)
                {
                    res[i] = db.CreateParameter(prm.ParamName, prm.ParamValue, prm.ParamType);
                    i++;
                }
                return res;
            }
        }
        #endregion
    }
}
