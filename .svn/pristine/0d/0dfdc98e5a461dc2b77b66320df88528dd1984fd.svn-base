using System;
using Krista.FM.Server.Scheme.Classes;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.Server.Scheme.Tests.Classes
{
    [TestFixture]
    public class BridgeAssociationReportTests
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }
        
        [Test]
        public void IsBridgeRepTest()
        {
            Assert.True(EntityAssociationFactoryBase.IsBridgeRep("2ba01724-4d97-4571-9375-44fdcd410bf4"));
            Assert.True(EntityAssociationFactoryBase.IsBridgeRep("1fc24a5e-7de9-4622-a227-a667a36f1c09"));
            Assert.True(EntityAssociationFactoryBase.IsBridgeRep("2b725aee-53ff-4b08-9318-b3cc00319e75"));
            Assert.True(EntityAssociationFactoryBase.IsBridgeRep("e765239d-c9d9-4eaf-85ae-4f0711de4bc9"));
            Assert.True(EntityAssociationFactoryBase.IsBridgeRep("4d1172a1-0753-4f4c-9b1f-c048a5028e92"));

            Assert.False(EntityAssociationFactoryBase.IsBridgeRep("265b1e88-bab2-488c-9a89-ba60b140a4dd"));
        }

    }
}
