using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using NUnit.Framework;

namespace Krista.FM.Server.Dashboards.Tests.Core
{
	[TestFixture]
	public class MdxQueryRenameServiceTest
	{
		[SetUp]
		public void Setup()
		{
			Singleton<MdxQueryRenameService>.Instance.Init("Core\\RenameList_2000_to_2005.xml");
		}

		[Test]
		public void CanRename()
		{
			Assert.AreEqual("[Администратор__Анализ].[Администратор__Анализ]", Singleton<MdxQueryRenameService>.Instance.Rename("[Администратор].[Анализ]"));
			Assert.AreEqual("foo [Администратор__Анализ].[Администратор__Анализ] foo", Singleton<MdxQueryRenameService>.Instance.Rename("foo [Администратор].[Анализ] foo"));
			Assert.AreEqual("foo [Электронные Вручную].[Электронные Вручную] foo", Singleton<MdxQueryRenameService>.Instance.Rename("foo [Электронные Вручную] foo"));
		}
	}
}
