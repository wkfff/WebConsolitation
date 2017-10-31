using System;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class D_S_TitleReportGridView : GridView
    {
        private readonly IDebtBookExtension extension;

        public D_S_TitleReportGridView(IDebtBookExtension extension)
        {
            this.extension = extension;
            if (extension.UserRegionType == UserRegionType.Region || extension.UserRegionType == UserRegionType.Town)
            {
                BookFilter = "(ID = {0})".FormatWith(extension.UserRegionId);
            }
        }

        protected override string GetAddButtonHandler(string gridPanelId)
        {
            var repository = new NHibernateRepository<D_Regions_Analysis>();
            var region = repository.Get(
                Convert.ToInt32(new Params.UserRegionIdValueProvider(extension).GetValue()));

            return @"
function(){{
    var r = {{ REFREGION: {1}, LP_REFREGION: '{2}' }};
    {0}.stopEditing();
    {0}.insertRecord(0, r);
    {0}.getView().focusRow(0);
    {0}.startEditing(0, 0);
}}".FormatWith(gridPanelId, region.ID, region.Name);
        }
    }
}
