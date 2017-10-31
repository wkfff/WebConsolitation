using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    ///   Показатели объема и качества услуг\работ справочник
    /// </summary>
    public class IndicatorsServiceView : View
    {
        private readonly IndicatorsServiceModel model = new IndicatorsServiceModel();

        public override List<Component> Build(ViewPage page)
        {
            var gp = new ModelGridView
            {
                AutoLoad = true,
                Model = model,
                ActionController = typeof(IndicatorsServiceController),
                PageSize = 100,
                UpdeteAction = "Save"
            };

            gp.Grid.AddRefreshButton();
            gp.Grid.AddNewRecordNoEditButton();
            gp.Grid.AddDeleteRecordWithConfirmButton();
            gp.Grid.AddSaveButton();

            var grid = (GridPanel)gp.Build(page)[0];

            var mapping = new Dictionary<string, string[]>
                {
                    { model.NameOf(() => model.RefCharacteristicType), new[] { "ID" } },
                    { model.NameOf(() => model.RefCharacteristicTypeName), new[] { "Name", "Code" } }
                };

            var col = grid.ColumnModel.GetColumnById(model.NameOf(() => model.RefCharacteristicTypeName));
            col.Editor.Clear();
            col.SetComboBoxEditor(FX_FX_CharacteristicType.Key, page, mapping);

            mapping = new Dictionary<string, string[]>
                {
                    { model.NameOf(() => model.RefOKEI), new[] { "ID" } },
                    { model.NameOf(() => model.RefOKEIName), new[] { "Name", "Code" } }
                };

            col = grid.ColumnModel.GetColumnById(model.NameOf(() => model.RefOKEIName));
            col.Editor.Clear();
            col.SetComboBoxEditor(D_Org_OKEI.Key, page, mapping, UiBuilders.GetUrl<CommonDataController>("GetOkei"));

            return new List<Component> { new Viewport { Items = { new FitLayout { Items = { grid } } } } };
        }
    }
}
