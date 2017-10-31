using System;
using System.Windows.Forms;
using Krista.FM.Client.ProcessManager;
using Krista.FM.Client.ViewObjects.BaseViewObject;

using Krista.FM.Server.ProcessorLibrary;


namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class MDObjectsManagementUI : BaseViewObj
    {
        /// <summary>
        /// Обработчик перемещения по модулям
        /// </summary>
        private UserControl GetViewObjectControl()
        {
            switch (moduleName)
            {
                case "Partitions":
                    return InitPartitionsViewer();
                case "DimensionsNew":
                    return InitDimensionsViewer();
                case "ProcessManager":
                    return InitProcessManagerView();
                case "Others":
                    return InitOthersViewer();
				case "DatabaseErrors":
					return InitDatabaseErrorsViewer();
                case "ProcessOption":
                    return InitProcessOptionViewer();
				default:
                    throw new ArgumentException();
            }                
        }

        private UserControl InitPartitionsViewer()
        {
            ProcessManager.OlapObjectsView partitionsView = new Krista.FM.Client.ProcessManager.OlapObjectsView();
            partitionsView.ProcessManagerViewEvent += new Krista.FM.Client.ProcessManager.OlapObjectsView.ProcessManagerDelegateHandler(partitionsView_ProcessManagerViewEvent);
            partitionsView.Connect(Workplace.ActiveScheme.Processor, Workplace.OperationObj, OlapObjectType.Partition);
            partitionsView.Visible = true;
            partitionsView.Text = "Разделы кубов";
            return partitionsView;
        }

        private void partitionsView_ProcessManagerViewEvent(object sender, Krista.FM.Client.ProcessManager.ProceccManagerEventArgs args)
        {
            MDObjectsManagementNavigation.Instance.OpenProcessManagerView(args, false);
        }

        
        private UserControl InitDimensionsViewer()
        {
            ProcessManager.OlapObjectsView dimensionsView = new Krista.FM.Client.ProcessManager.OlapObjectsView();
            dimensionsView.ProcessManagerViewEvent += new Krista.FM.Client.ProcessManager.OlapObjectsView.ProcessManagerDelegateHandler(partitionsView_ProcessManagerViewEvent);
            dimensionsView.Connect(Workplace.ActiveScheme.Processor, Workplace.OperationObj, OlapObjectType.Dimension);
            dimensionsView.Visible = true;
            dimensionsView.Text = "Измерения";
            return dimensionsView;
        }

        private UserControl InitProcessManagerView()
        {
            ProcessManager.ProcessManagerView processManagerView = new Krista.FM.Client.ProcessManager.ProcessManagerView();
            processManagerView.Connect(Workplace.ActiveScheme, Workplace.OperationObj);
            processManagerView.Visible = true;
            processManagerView.Text = "Менеджер расчетов";
            return processManagerView;
        }

		private UserControl InitDatabaseErrorsViewer()
		{
			ProcessManager.DatabaseErrorsView databaseErrorsView = new ProcessManager.DatabaseErrorsView();
			databaseErrorsView.Connect(Workplace.ActiveScheme.Processor.OlapDBWrapper.GetDatabaseErrors());
			databaseErrorsView.Visible = true;
			databaseErrorsView.Text = "Ошибки многомерной базы";
			return databaseErrorsView;
		}

		private UserControl InitOthersViewer()
        {
            OthersViewer othersViewer = new OthersViewer();
            othersViewer.Connect(Workplace);
            othersViewer.Visible = true;
            othersViewer.Text = "Прочее";
            return othersViewer;
        }

        private UserControl InitProcessOptionViewer()
        {
            ProcessManager.ProcessOptionControl optionViewer = new ProcessOptionControl();
            optionViewer.Connect(Workplace.ActiveScheme.Processor.ProcessManager);
            optionViewer.Visible = true;
            optionViewer.Text = "Опции расчета";
            return optionViewer;
        }        

        public override void InternalFinalize()
        {
            foreach (Control item in _view.Controls)
            {
                item.Dispose();
            }
            base.InternalFinalize();
        }
    }
}