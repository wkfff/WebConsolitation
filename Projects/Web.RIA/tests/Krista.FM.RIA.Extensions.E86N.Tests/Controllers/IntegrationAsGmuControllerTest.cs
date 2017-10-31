using System.Collections.Generic;
using System.Web;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.E86N.Tests.Controllers
{
    [TestFixture]
    public class IntegrationAsGmuControllerTest
    {
        private MockRepository _mocks;
        private IDbContext _context;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _context = _mocks.DynamicMock<IDbContext>();
        }

        [Test]
        [Ignore]
        public void Upload()
        {
            //var httpPostedFileBase = MockRepository.GenerateMock<HttpPostedFileBase>();
            //httpPostedFileBase.Stub(@base => @base.InputStream).Return();
            //httpPostedFileBase.Expect(@base => @base.InputStream).Return();
            //httpPostedFileBase.VerifyAllExpectations();

            //var controller = new IntegrationAsGmuController();
            //controller.Upload(string.Empty, string.Empty, httpPostedFileBase);
        }
    }
}
