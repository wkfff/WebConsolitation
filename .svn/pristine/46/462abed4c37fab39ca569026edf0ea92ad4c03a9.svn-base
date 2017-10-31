using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


using Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.TranslationsTables
{
    public partial class TranslationsTablesUI : BaseViewObj
    {
        private IInplaceProtocolView ipvClassifiersProtocol;


        void utcDataCls_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            if (e.Tab.Index == 1)
                FillProtocols();
        }
        

        private void FillProtocols()
        {
            // протокол классификаторов
            string filter = "Classifier = ?";
            IDbDataParameter param = new System.Data.OleDb.OleDbParameter("Classifier", currentObjName);//db.CreateParameter("Classifier", currentObjName);

            if (ipvClassifiersProtocol == null)
            {
                ipvClassifiersProtocol = Workplace.ProtocolsInplacer;
                ipvClassifiersProtocol.AttachViewObject(ModulesTypes.ClassifiersModule, TranslationsView.pClassifiers, String.Empty,
                    filter, param); 
            }
            else
            {
                ipvClassifiersProtocol.RefreshAttachData(String.Empty, filter, param);
            }

            ((StateButtonTool)ipvClassifiersProtocol.GridComponent.utmMain.Tools["CLASSIFIER"]).Checked = false;
            ((StateButtonTool)ipvClassifiersProtocol.GridComponent.utmMain.Tools["OBJECTTYPE"]).Checked = false;
        }
    }
}
