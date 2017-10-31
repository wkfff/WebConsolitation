using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI
{
    public partial class ConsBudgetForecastUI
    {

        private IInplaceProtocolView ipvClassifiersProtocol;
        private IInplaceProtocolView ipvAssociateProtocol;

        private void FillProtocols()
        {

            IDatabase db = this.Workplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                // формируем имя файла для сохранения
                string ProtocolSaveFileName = string.Empty;
                
                // протокол классификаторов
                string filter = "Classifier = ? and DATASOURCEID = ?";
                db.CreateParameter("Classifier", "ФО_Результат доходов с расщеплением");

                if (ipvClassifiersProtocol == null)
                {
                    ipvClassifiersProtocol = this.Workplace.ProtocolsInplacer;
                    if (ipvClassifiersProtocol == null)
                        return;
                    ipvClassifiersProtocol.AttachViewObject(ModulesTypes.ClassifiersModule, vo.protocols, ProtocolSaveFileName,
                        filter, new DbParameterDescriptor("p0", "ФО_Результат доходов с расщеплением"),
                        new DbParameterDescriptor("p1", NosplitVariant));
                }
                else
                {
                    ipvClassifiersProtocol.RefreshAttachData(ProtocolSaveFileName, filter, 
                        new DbParameterDescriptor("p0", "ФО_Результат доходов с расщеплением"),
                        new DbParameterDescriptor("p1", NosplitVariant));
                }

                ((StateButtonTool)ipvClassifiersProtocol.GridComponent.utmMain.Tools["CLASSIFIER"]).Checked = false;
                ((StateButtonTool)ipvClassifiersProtocol.GridComponent.utmMain.Tools["OBJECTTYPE"]).Checked = false;
            }
            finally
            {
                db.Dispose();
            }
        }
    }
}
