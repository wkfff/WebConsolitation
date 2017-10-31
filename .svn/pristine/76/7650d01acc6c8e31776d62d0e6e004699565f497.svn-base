using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class FormEntityWithDetailsViewTests
    {
        [Test]
        public void CanBuild()
        {
            MockRepository mocks = new MockRepository();
            IEntity entity = mocks.DynamicMock<IEntity>();
            IPresentation presentation = mocks.DynamicMock<IPresentation>();
            IViewService viewService = mocks.DynamicMock<IViewService>();

            FormEntityWithDetailsView form = new FormEntityWithDetailsView();
            form.Entity = entity;
            form.Presentation = presentation;
            form.ViewService = viewService;
            form.Id = "form1";
            form.Params.Add("", "");
            form.StoreListeners.Add("", "");
            form.Title = "Название формы";

            mocks.ReplayAll();

            List<Ext.Net.Component> list = form.Build(new ViewPage());

            mocks.VerifyAll();
        }
    }
}
