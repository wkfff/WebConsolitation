using System;
using System.Data;
using System.Collections.Generic;

using Infragistics.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        private IInplaceProtocolView ipvClassifiersProtocol;
        private IInplaceProtocolView ipvAssociateProtocol;

        private void FillProtocols()
        {
            IDatabase db = this.Workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                IDbDataParameter param = null;
                
                // формируем имя файла для сохранения
                string ProtocolSaveFileName = string.Empty;
                ComboBoxTool cbTool = (ComboBoxTool)vo.utbToolbarManager.Tools["cbDataSources"];
                string currentFilterCaption = cbTool.Text;
                if (currentFilterCaption != string.Empty && HasDataSources())
                    ProtocolSaveFileName = GetDataObjSemanticRus(ActiveDataObj) + '_' + ActiveDataObj.Caption + '_' + currentFilterCaption;
                else
                    ProtocolSaveFileName = GetDataObjSemanticRus(ActiveDataObj) + '_' + ActiveDataObj.Caption;

               
                // протокол классификаторов
                string filter = "Classifier = ?";
                param = db.CreateParameter("Classifier", this.ActiveDataObj.OlapName);

                if (ipvClassifiersProtocol == null)
                {
                    ipvClassifiersProtocol = this.Workplace.ProtocolsInplacer;
                    if (ipvClassifiersProtocol == null)
                        return;
                    ipvClassifiersProtocol.AttachViewObject(ModulesTypes.ClassifiersModule, vo.pClassifiers, ProtocolSaveFileName,
                        filter, param);
                }
                else
                {
                    ipvClassifiersProtocol.RefreshAttachData(ProtocolSaveFileName, filter, param);
                }

                ((StateButtonTool)ipvClassifiersProtocol.GridComponent.utmMain.Tools["CLASSIFIER"]).Checked = false;
                ((StateButtonTool)ipvClassifiersProtocol.GridComponent.utmMain.Tools["OBJECTTYPE"]).Checked = false;

                if (vo.utcLogSwitcher.Tabs[1].Visible)
                {

                    // протокол сопоставлений
                    filter = "(BridgeRoleA = ? or BridgeRoleB = ?)";
                    IDbDataParameter param1 = new System.Data.OleDb.OleDbParameter("BridgeRoleA", ActiveDataObj.OlapName);// db.CreateParameter("BridgeRoleA", this.ActiveDataObj.OlapName);
                    IDbDataParameter param2 = new System.Data.OleDb.OleDbParameter("BridgeRoleB", ActiveDataObj.OlapName);// db.CreateParameter("BridgeRoleB", this.ActiveDataObj.OlapName);

                    if (ipvAssociateProtocol == null)
                    {
                        ipvAssociateProtocol = this.Workplace.ProtocolsInplacer;
                        ipvAssociateProtocol.AttachViewObject(ModulesTypes.BridgeOperationsModule, vo.pAssociate, ProtocolSaveFileName,
                            filter, new IDbDataParameter[] { param1, param2 });
                    }
                    else
                    {
                        ipvAssociateProtocol.RefreshAttachData(ProtocolSaveFileName, filter, new IDbDataParameter[] { param1, param2 });
                    }
                    // скрываем колонку с названием объекта
                }
            }
            finally
            {
                if (!InInplaceMode)
                    this.SetViewObjectCaption();
                db.Dispose();
            }
        }
    }
}
