using System;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.DebtBook.Services.ControlRelationships;
using Krista.FM.RIA.Extensions.DebtBook.Services.DAL;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class ControlRelationshipsTests
    {
        [Test]
        [Ignore]
        public void CheckRelationshipsTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
                "Oracle",
                "10");

            IObjectRepository repository = new ObjectRepository();

            ////decimal staleDebt = new Decimal(10.2);
            ////decimal attract = new decimal(50);
            ////var factTableRow = new F_S_SchBCapital { StaleDebt = staleDebt, Attract = attract };

            ////string expression = "StaleDebt <= Attract";
            
            ////expression = "if(FactServiceSum!=PREV(FactServiceSum),TotalSum!=PREV(TotalSum),true)";
            ////factTableRow.FactServiceSum = 100;
            ////factTableRow.TotalSum = 10;
            ////factTableRow.SourceKey = 132;

            ////var relationship = new ControlRelationship(expression, "Текст ошибки!");
            ////relationship.Check(factTableRow, repository);

            ////var row = repository.GetRow("f_S_SchBCreditincome", 68);
            ////string expression = "if([Attract]!=0 and DATE([PaymentDate]) < DATE([RefVariant.ReportDate]), [StaleDebt] > 0, true)";
            ////var relationship = new ControlRelationship(expression, "Текст ошибки!");
            ////relationship.Check(row, repository);
            
            ////var row = repository.GetRow("f_S_SchBCreditincome", 25);
            ////string expression = "[StaleDebt] <= [Sum]";
            ////var relationship = new ControlRelationship(expression, "Текст ошибки!");
            ////relationship.Check(row, repository);

            /*
            var row = repository.GetRow("f_S_SchBGuarantissued", 687);
                string expression = "if([RemnsEndMnthDbt] != 0 and DATE([PrincipalEndDate]) < DATE([RefVariant.ReportDate]), [StalePrincipalDebt] > 0, true)";
                var relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "if(DAY(DATE([RefVariant.ReportDate]))!=1 or MONTH(DATE([RefVariant.ReportDate]))!=2, if([RemnsEndMnthDbt] < PREV([RemnsEndMnthDbt]), [DownDebt] > PREV([DownDebt]), true), true)";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "if(DAY(DATE([RefVariant.ReportDate]))!=1 or MONTH(DATE([RefVariant.ReportDate]))!=2, if([DownDebt] > PREV([DownDebt]), [RemnsEndMnthDbt] < PREV([RemnsEndMnthDbt]), true), true)";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "if(DAY(DATE([RefVariant.ReportDate]))!=1 or MONTH(DATE([RefVariant.ReportDate]))!=2, if([DownGarant] > PREV([DownGarant]), [RemnsEndMnthDbt] < PREV([RemnsEndMnthDbt]), true), true)";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "[StalePrincipalDebt] <= [TotalDebt]";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "[CapitalDebt] <= [TotalDebt]";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "[DownGarant] <= [DownDebt]";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "[BgnYearDebt] <= [RemnsEndMnthDbt]";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());
            */
            /*
            var row = repository.GetRow("f_S_SchBCreditincome", 25);
                string expression = "if([Attract] > 0 and DATE([PaymentDate]) < DATE([RefVariant.ReportDate]), [StaleDebt] > 0, true)";
                var relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

                expression = "[StaleDebt] <= [Attract]";
                relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());
            */

            var row = repository.GetRow("f_S_SchBCreditincome", 25);
                string expression = "[StaleDebt] <= [CapitalDebt]";
                var relationship = new ControlRelationship(expression, "Текст ошибки!");
                relationship.Check(row, repository);
                Console.WriteLine(relationship.GetResultMessage());

            Assert.IsTrue(true);
        }
    }
}
