using Krista.FM.Common;
using Krista.FM.ServerLibrary;

using NUnit.Framework;

namespace Krista.FM.Server.Scheme.Tests
{
	[TestFixture]
	public class SchemeXmlUmlObjectsTest
	{
		[Test(Description = "Корректность проверки типов ассоциаций")]
		public void CheckRolesClassesBridgeAssociationTest()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Bridge, ClassTypes.clsDataClassifier, ClassTypes.clsBridgeClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Bridge, ClassTypes.clsBridgeClassifier, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		public void CheckRolesClassesMasterDetailAssociationTest()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.Table, ClassTypes.clsFixedClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.Table, ClassTypes.clsBridgeClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.Table, ClassTypes.clsDataClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.Table, ClassTypes.clsFactData,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.Table, ClassTypes.Table,
										  "name");
		}

		[Test]
		public void CheckRolesClassesLinkAssociationTest()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFactData, ClassTypes.clsFixedClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFactData, ClassTypes.clsBridgeClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFactData, ClassTypes.clsDataClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFactData, ClassTypes.clsFactData,
										  "name");

			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.DocumentEntity, ClassTypes.DocumentEntity,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.DocumentEntity, ClassTypes.clsDataClassifier,
										  "name");
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.DocumentEntity, ClassTypes.clsFixedClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest1()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.Table, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.MasterDetail, ClassTypes.clsBridgeClassifier
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest2()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsBridgeClassifier, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest3()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsBridgeClassifier, ClassTypes.clsDataClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest4()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsBridgeClassifier, ClassTypes.clsFixedClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest5()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsBridgeClassifier, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest6()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsBridgeClassifier, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest7()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsBridgeClassifier, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.MasterDetail, ClassTypes.clsFixedClassifier
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest12()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFixedClassifier, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest13()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFixedClassifier, ClassTypes.clsDataClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest14()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFixedClassifier, ClassTypes.clsFixedClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest15()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFixedClassifier, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest16()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFixedClassifier, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest17()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFixedClassifier, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.MasterDetail, ClassTypes.clsDataClassifier
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest22()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsDataClassifier, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest23()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsDataClassifier, ClassTypes.clsDataClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest24()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsDataClassifier, ClassTypes.clsFixedClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest25()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsDataClassifier, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest26()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsDataClassifier, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest27()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsDataClassifier, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.MasterDetail, ClassTypes.clsFactData
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest32()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFactData, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest33()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFactData, ClassTypes.clsDataClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest34()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFactData, ClassTypes.clsFixedClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest35()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFactData, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest36()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFactData, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest37()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.clsFactData, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.MasterDetail, ClassTypes.DocumentEntity
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest42()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.DocumentEntity, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest43()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.DocumentEntity, ClassTypes.clsDataClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest44()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.DocumentEntity, ClassTypes.clsFixedClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest45()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.DocumentEntity, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest46()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.DocumentEntity, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesMasterDetailAssociationATest47()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.MasterDetail, ClassTypes.DocumentEntity, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.Link, ClassTypes.clsFixedClassifier
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest10()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFixedClassifier, ClassTypes.clsFixedClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest11()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFixedClassifier, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest12()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFixedClassifier, ClassTypes.clsDataClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest13()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFixedClassifier, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest14()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFixedClassifier, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest15()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFixedClassifier, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.Link, ClassTypes.clsBridgeClassifier
		//[Test]
		//[ExpectedException(typeof(ServerException))]
		//public void CheckRolesClassesLinkAssociationATest21()
		//{
		//    SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsBridgeClassifier, ClassTypes.clsBridgeClassifier,
		//                                  "name");
		//}

		//[Test]
		//[ExpectedException(typeof(ServerException))]
		//public void CheckRolesClassesLinkAssociationATest22()
		//{
		//    SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsBridgeClassifier, ClassTypes.clsDataClassifier,
		//                                  "name");
		//}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest23()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsBridgeClassifier, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest24()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsBridgeClassifier, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest25()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsBridgeClassifier, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.Link, ClassTypes.clsDataClassifier
		//[Test]
		//[ExpectedException(typeof(ServerException))]
		//public void CheckRolesClassesLinkAssociationATest31()
		//{
		//    SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsDataClassifier, ClassTypes.clsBridgeClassifier,
		//                                  "name");
		//}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest33()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsDataClassifier, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest34()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsDataClassifier, ClassTypes.Table,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest35()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsDataClassifier, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.Link, ClassTypes.clsFactData
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest44()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.clsFactData, ClassTypes.Table,
										  "name");
		}

		// ---------- AssociationClassTypes.Link, ClassTypes.Table
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest55()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.Table, ClassTypes.DocumentEntity,
										  "name");
		}

		// ---------- AssociationClassTypes.Link, ClassTypes.DocumentEntity
		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest61()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.DocumentEntity, ClassTypes.clsBridgeClassifier,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest63()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.DocumentEntity, ClassTypes.clsFactData,
										  "name");
		}

		[Test]
		[ExpectedException(typeof(ServerException))]
		public void CheckRolesClassesLinkAssociationATest64()
		{
			SchemeClass.CheckRolesClasses(AssociationClassTypes.Link, ClassTypes.DocumentEntity, ClassTypes.Table,
										  "name");
		}
	}
}
