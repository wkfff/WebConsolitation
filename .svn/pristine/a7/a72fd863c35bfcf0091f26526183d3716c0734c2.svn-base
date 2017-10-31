using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using Image = Ext.Net.Image;
using Label = Ext.Net.Label;
using Parameter = Ext.Net.Parameter;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningFormulaCoeffControl : Control
    {
        private const string StoreFormulaId = "stFormulaData";

        private readonly string Key; 
        
        private IForecastExtension extension;
        
        public PlanningFormulaCoeffControl(IForecastExtension extension, string key)
        {
            this.extension = extension;
            Key = key;
        }

        public Store StoreFormulaCoeffs { get; private set; }
        
        public override List<Component> Build(ViewPage page)
        {
            StoreFormulaCoeffs = CreateCoeffStore(StoreFormulaId);
            page.Controls.Add(StoreFormulaCoeffs);

            /*Panel panFormula = new Panel
            {
                ID = "pFormula",
                Title = "Формула",
                Layout = "fit"
            };*/

            TableLayout layout = new TableLayout
            {
                Columns = 2
            };

            Cell formulaImg = new Cell { };
            Cell formulaText = new Cell { RowSpan = 2 };
            Cell formulaCoeffs = new Cell { };

            Image img = new Image
            {
                ID = "imgFormula",
                AutoWidth = true,
                AutoHeight = true,
                ImageUrl = "/Krista.FM.RIA.Extensions.Forecast.MathStat/Presentation/Content/Formulas/EmptyImage.png/extention.axd",
                StyleSpec = "margin: 10px;"
            };

            formulaImg.Items.Add(img);
            
            TextArea txtArea = new TextArea
            {
                ID = "textArea",
                Text = String.Empty,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                BorderWidth = 0,
                StyleSpec = "border : none;"
            };

            formulaText.Items.Add(txtArea);

            GridPanel gridFormulaCoeff = new GridPanel
            {
                ID = "grFormula",
                StoreID = StoreFormulaId,
                AutoHeight = true
            };

            formulaCoeffs.Items.Add(gridFormulaCoeff);
            
            ColumnModel fcolModel = gridFormulaCoeff.ColumnModel;
            fcolModel.AddColumn("Key", "Key", "Коэффициент", DataAttributeTypes.dtString, Mandatory.NotNull);
            fcolModel.AddColumn("Value", "Value", "Значение", DataAttributeTypes.dtDouble, Mandatory.NotNull);
            
            ////panFormula.Items.Add(new List<Component> { img, gridFormulaCoef });

            layout.Cells.Add(formulaImg);
            layout.Cells.Add(formulaText);
            layout.Cells.Add(formulaCoeffs);
            
            if (!Key.Contains("_-1"))
            {
                var formulaScript = @"Ext.net.DirectEvent.confirmRequest(
{
    cleanRequest: true,
    isUpload: false,
    url: '/PlanningFormulaCoeff/ChangeFormula?group='+cbGroupMethod.getSelectedItem().value+'&method='+cbMethod.getSelectedItem().value,
    control: this      
});";
                gridFormulaCoeff.Listeners.AfterRender.AddAfter(String.Format("{0}.load();", StoreFormulaId));

                var rm = page.Controls.OfType<HtmlForm>().First().Controls.OfType<ResourceManager>().First();
                rm.RegisterOnReadyScript(formulaScript);
            }

            string sizeScript = @"textArea.setWidth(formulaCoeffContainer.getWidth()-Math.max(imgFormula.getWidth(),grFormula.getWidth()));
textArea.setHeight(imgFormula.getHeight()+grFormula.getHeight());";
            
            layout.AddScript(sizeScript);
            
            return new List<Component> { layout };
        }

        public Store CreateCoeffStore(string storeId)
        {
            Store store = new Store { ID = storeId, AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };
            store.Reader.Add(reader);
            reader.Fields.Add("Key");
            reader.Fields.Add("Value");

            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", Key), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningFormulaCoeff/LoadProgCoeffs",  ////progchartdata
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
