using System;
using System.Data;
using System.Xml;
//using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Validations;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CreditIncomes
{
    internal class CreditIncomeValidations
	{
		/// <summary>
		/// ¬ыполн€ет проверку всех кредитов.
		/// </summary>
		public static IValidatorMessageHolder Validate(XmlDocument configuration)
		{
			using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
			{
				// foreach( ѕеребираем все кредиты
				{
					//Validate(masterRow, db);
				}
			}
			return null;
		}

		/// <summary>
		/// ¬ыполн€ет проверку заданного кредита.
		/// </summary>
		public static IValidatorMessageHolder Validate(XmlDocument configuration, DataRow masterRow)
		{
			using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
			{
				return Validate(configuration, masterRow, db);
			}
		}

		/// <summary>
		/// ¬ыполн€ет проверку заданного кредита.
		/// </summary>
		private static IValidatorMessageHolder Validate(XmlDocument configuration, DataRow masterRow, IDatabase db)
		{
			ValidationMessages vmh = new ValidationMessages();
			// - ѕроверка мастер записи
            XmlNodeList mvNodeList = configuration.SelectNodes("/Validations/MasterCompositeValidator/MasterValidator");
            if (mvNodeList != null)
            {
                foreach(XmlNode mvNode in mvNodeList)
                {
                    vmh.Add(new FinSourceMasterValidator(mvNode).Validate(masterRow, mvNode.ParentNode.Attributes["key"].Value));
                }
            }
		    // дл€ каждой детали
			XmlNodeList mdvNodeList = configuration.SelectNodes("/Validations/MasterCompositeValidator/MasterDetailCompositeValidator");
			if (mdvNodeList != null)
			{
				foreach (XmlNode mdvNode in mdvNodeList)
				{
					DataTable detailTable = Utils.GetDetailTable(db, Convert.ToInt32(masterRow["ID"]),
						mdvNode.Attributes["associationKey"].Value);
                    // все провевки между мастером и деталью
                    XmlNodeList nodeList = mdvNode.SelectNodes("MasterDetailValidator");
                    if (nodeList != null)
                        foreach (XmlNode node in nodeList)
                        {
                            vmh.Add(new FinSourceMasterDetailValidator(node).Validate(masterRow, detailTable));
                        }

                    // - ѕроверка условий между детал€ми
                    nodeList = mdvNode.SelectNodes("MasterDetailDetailValidator");
                    if (nodeList != null)
                        foreach (XmlNode node in nodeList)
                        {
                            DataTable boundDetailTable = Utils.GetDetailTable(db, Convert.ToInt32(masterRow["ID"]),
                                node.Attributes["associationKey"].Value);
                            vmh.Add(new FinSourceMasterDetailDetailValdator(node).Validate(masterRow, detailTable, boundDetailTable));
                        }

                    // проверка условий внутри детали
                    nodeList = mdvNode.SelectNodes("DetailValidator");
                    if (nodeList != null)
                        foreach (XmlNode node in nodeList)
                        {
                            vmh.Add(new FinSourceDetailValidator(node).Validate(detailTable));
                        }
				}
			}

			
			return vmh;
		}
	}
}
