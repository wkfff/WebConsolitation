using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    /// Представление - селектор ПФХД
    /// </summary>
    internal class PfhdCommonView : View
    {
        public int DocId { get; set; }

        [Dependency]
        public INewRestService NewRestService { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            DocId = Convert.ToInt32(Params["docId"]);

            var year = NewRestService.GetItem<F_F_ParameterDoc>(this.DocId).RefYearForm.ID;

            Type cntType;

            if (year < 2017)
            {
                cntType = typeof(PfhdView);
            }
            else
            {
                cntType = typeof(Pfhd2017View);
            }

            View view = (View)Core.Gui.ControlBuilder.Current.GetControllerFactory().CreateControl(cntType);

            Params.Each(x =>
                        {
                            view.Params.Add(x.Key, x.Value);
                        });

            return view.Build(page);
        }
    }
}
