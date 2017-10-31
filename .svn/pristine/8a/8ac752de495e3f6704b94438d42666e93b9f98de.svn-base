using System;
using System.Collections.ObjectModel;
using System.Web;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Core
{
    public class CustomParam
    {
        public static CustomParam CustomParamFactory(string paramName)
        {
            if (HttpContext.Current.Session[paramName] == null ||
                !(HttpContext.Current.Session[paramName] is CustomParam))
            {
                CustomParam param = new CustomParam(paramName);
                HttpContext.Current.Session[paramName] = param;
            }
            return (CustomParam)HttpContext.Current.Session[paramName];
        }

        private string name;
        private string value;
        private bool locked;

        private CustomParam(string paramName)
        {
            name = paramName;
            locked = false;
            value = string.Empty;
        }

        internal bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
            }
        }

        public string Value
        {
            get
            {
                return CustomParamFactory(name).value;
            }
            set
            {
                if (!CustomParamFactory(name).Locked)
                {
                    CustomParamFactory(name).value = value;
                }
            }
        }

        /// <summary>
        /// Равно ли значение кастом-параметра указанному
        /// </summary>
        public bool ValueIs(string paramValue)
        {
            string oldValue = CustomParamFactory(name).value;
            return (oldValue == paramValue);
        }
    }

    public class CustomParams
    {
        public CustomParams()
        {
            currentReportParams = new Collection<string>();
        }

        /// <summary>
        /// Создает параметр с произвольным именем.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CustomParam CustomParam(string name)
        {
            return CustomParam(name, false);
        }

        /// <summary>
        /// Создает параметр с произвольным именем.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rememberName"></param>
        /// <returns></returns>
        public CustomParam CustomParam(string name, bool rememberName)
        {
            if (rememberName && !currentReportParams.Contains(name))
            {
                currentReportParams.Add(name);
            }
            return Core.CustomParam.CustomParamFactory(name);
        }

        /// <summary>
        /// Коллекция имен используемых параметров текущего отчета.
        /// </summary>
        private Collection<string> currentReportParams;

        /// <summary>
        ///  Возвращает список параметров текущего отчета.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentReportParamList()
        {
            string paramList = string.Empty;
            foreach (string item in currentReportParams)
            {
                string safeValue = Core.CustomParam.CustomParamFactory(item).Value.ToString();
                if (!String.IsNullOrEmpty(safeValue))
                {
                    safeValue = HttpContext.Current.Server.UrlEncode(safeValue);
                    paramList += String.Format("{0}={1};", item, safeValue);
                }
            }
            paramList = paramList.Trim(';');
            return HttpContext.Current.Request.Url.AbsoluteUri + "?paramlist=" + paramList;
        }

        public void UnlockParams()
        {
            foreach (string item in currentReportParams)
            {
                Core.CustomParam.CustomParamFactory(item).Locked = false;
            }
        }

        /// <summary>
        /// Инициализирует параметры из списка.
        /// </summary>
        /// <param name="paramsString"></param>
        public static void InitializeParamsFromList(string paramsString)
        {
            string[] paramsList = paramsString.Split(';');
            for (int i = 0; i < paramsList.Length; i++)
            {
                string[] parameter = paramsList[i].Split('=');
                // Если что-то намудрили
                if (parameter.Length != 2)
                {
                    throw new Exception(string.Format(
                            "Некорректный синтаксис списка параметров отчета.<br/>Параметр: {0}.<br/>Строка запроса:{1}",
                            paramsList[i], paramsString));
                }
                if (parameter[0].ToLower() == "subjectid")
                {
                    MakeRegionParams(parameter[1], "id");
                }
                else if (parameter[0].ToLower() == "moid")
                {
                    MakeMoParams(parameter[1], "id");
                }
                else
                {
                    Core.CustomParam.CustomParamFactory(parameter[0]).Value = parameter[1];
                    Core.CustomParam.CustomParamFactory(parameter[0]).Locked = true;
                }
            }
        }

        public static void MakeRegionParams(string subjectID, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/Regions.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "RegionsList")
                {
                    foreach (XmlNode regionNode in rootNode.ChildNodes)
                    {
                        foreach (XmlNode subjectNode in regionNode.ChildNodes)
                        {
                            if (subjectNode.Attributes[compareField].Value == subjectID)
                            {
                                HttpContext.Current.Session["CurrentSiteName"] = subjectNode.Attributes["siteName"].Value;
                                HttpContext.Current.Session["CurrentSubjectID"] = subjectNode.Attributes["id"].Value;
                                HttpContext.Current.Session["CurrentSiteRef"] = subjectNode.Attributes["siteRef"].Value;

                                HttpContext.Current.Session["CurrentSgmID"] = subjectNode.Attributes["sgmId"].Value;

                                Core.CustomParam.CustomParamFactory("region").Value = RegionsNamingHelper.GetFoBySubject(subjectNode.Attributes["name"].Value);
                                Core.CustomParam.CustomParamFactory("state_area").Value = subjectNode.Attributes["name"].Value;

                                Core.CustomParam.CustomParamFactory("short_region").Value = RegionsNamingHelper.ShortName(Core.CustomParam.CustomParamFactory("region").Value);
                                Core.CustomParam.CustomParamFactory("short_state_area").Value = RegionsNamingHelper.ShortName(subjectNode.Attributes["name"].Value);

                                return;
                            }
                        }
                    }
                }
            }
            HttpContext.Current.Session["CurrentSiteName"] = null;
            HttpContext.Current.Session["CurrentSubjectID"] = null;
            HttpContext.Current.Session["CurrentSiteRef"] = null;

            Core.CustomParam.CustomParamFactory("region").Value = null;
            Core.CustomParam.CustomParamFactory("state_area").Value = null;

            Core.CustomParam.CustomParamFactory("short_region").Value = null;
            Core.CustomParam.CustomParamFactory("short_state_area").Value = null;
        }

        #warning Перенести в RegionNamingHelper
        public static string GetSubjectIdByName(string name)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/Regions.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "RegionsList")
                {
                    foreach (XmlNode regionNode in rootNode.ChildNodes)
                    {
                        foreach (XmlNode subjectNode in regionNode.ChildNodes)
                        {
                            if (subjectNode.Attributes["name"].Value == name)
                            {
                                return subjectNode.Attributes["id"].Value;
                            }
                        }
                    }
                }
            }
            return String.Empty;
        }

        public static string GetFOIdByName(string name)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/Regions.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "RegionsList")
                {
                    foreach (XmlNode regionNode in rootNode.ChildNodes)
                    {

                        if (regionNode.Attributes["name"].Value == name)
                            {
                                return regionNode.Attributes["id"].Value;
                            }
                        
                    }
                }
            }
            return String.Empty;
        }

        public static void MakeFoParams(string subjectID, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/Regions.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "RegionsList")
                {
                    foreach (XmlNode regionNode in rootNode.ChildNodes)
                    {
                        if (regionNode.Attributes[compareField].Value == subjectID)
                        {
                            Core.CustomParam.CustomParamFactory("region").Value = regionNode.Attributes["shortName"].Value;
                            Core.CustomParam.CustomParamFactory("fullRegionName").Value = regionNode.Attributes["name"].Value;
                            HttpContext.Current.Session["CurrentFOID"] = regionNode.Attributes["id"].Value;
                            return;
                        }
                    }
                }
            }
        }

        public static void MakeControlsParams(string controlID, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/ControlIndicators.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "ControlIndicators")
                {
                    foreach (XmlNode controlNode in rootNode.ChildNodes)
                    {
                        if (controlNode.Attributes[compareField].Value == controlID)
                        {
                            Core.CustomParam.CustomParamFactory("control").Value = controlNode.Attributes["name"].Value;
                            HttpContext.Current.Session["CurrentСontrolID"] = controlID;
                            return;
                        }
                    }
                }
            }
        }

        public static void MakeBanksParams(string controlID, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/Banks.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                foreach (XmlNode bankNode in rootNode.ChildNodes)
                {
                    if (bankNode.Attributes[compareField].Value == controlID)
                    {
                        Core.CustomParam.CustomParamFactory("terbank").Value = bankNode.Attributes["name"].Value;
                        HttpContext.Current.Session["CurrentTerbankID"] = controlID;
                        return;
                    }
                }                
            }
        }

        public static void MakeBudgetParams(string controlID, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/BudgetLevel.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                foreach (XmlNode bankNode in rootNode.ChildNodes)
                {
                    if (bankNode.Attributes[compareField].Value == controlID)
                    {
                        Core.CustomParam.CustomParamFactory("budget_level").Value = bankNode.Attributes["name"].Value;
                        return;
                    }
                }
            }
        }

        public static void MakeSettlementParams(string controlID, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/Settlement.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                foreach (XmlNode settlementNode in rootNode.ChildNodes)
                {
                    if (settlementNode.Attributes[compareField].Value == controlID)
                    {
                        Core.CustomParam.CustomParamFactory("settlement").Value = settlementNode.Attributes["name"].Value;
                        return;
                    }
                }
            }
        }

        public static string GetFoIdByName(string foName)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/Regions.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "RegionsList")
                {
                    foreach (XmlNode regionNode in rootNode.ChildNodes)
                    {
                        if (regionNode.Attributes["name"].Value.Replace("&quot;", "\"").ToLower() == foName.ToLower())
                        {
                            return regionNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            return String.Empty;
        }

        public static void MakeMoParams(string moId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/MO.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "MOList")
                {
                    foreach (XmlNode subjectNode in rootNode.ChildNodes)
                    {
                        foreach (XmlNode moNode in subjectNode.ChildNodes)
                        {
                            if (moNode.Attributes != null && moNode.Attributes[compareField] != null && moNode.Attributes[compareField].Value == moId)
                            {
                                Core.CustomParam.CustomParamFactory("Mo").Value = moNode.Attributes["name"].Value;
                                HttpContext.Current.Session["CurrentMoID"] = moNode.Attributes["id"].Value;
                                HttpContext.Current.Session["CurrentMoSiteRef"] = moNode.Attributes["moSiteRef"].Value;
                                
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static string GetMONameByLogin(string login)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/MO.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "MOList")
                {
                    foreach (XmlNode subjectNode in rootNode.ChildNodes)
                    {
                        foreach (XmlNode moNode in subjectNode.ChildNodes)
                        {
                            if (moNode.Attributes != null && moNode.Attributes["moLogin"] != null && moNode.Attributes["moLogin"].Value == login)
                            {
                                return moNode.Attributes["name"].Value;
                            }
                        }
                    }
                }
            }
            return String.Empty;
        }

        public static void MakeGrbsParams(string grbsId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/Grbs.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "grbs")
                {
                    foreach (XmlNode grbsNode in rootNode.ChildNodes)
                    {
                        if (grbsNode.Attributes[compareField].Value == grbsId)
                        {
                            Core.CustomParam.CustomParamFactory("grbs").Value = grbsNode.Attributes["name"].Value;
                            HttpContext.Current.Session["CurrentGrbsID"] = grbsNode.Attributes["id"].Value;
                            return;
                        }
                    }
                }
            }
        }

        public static void MakeOutcomesParams(string outcomesId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/Outcomes.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "outcomes")
                {
                    foreach (XmlNode grbsNode in rootNode.ChildNodes)
                    {
                        if (grbsNode.Attributes[compareField].Value == outcomesId)
                        {
                            Core.CustomParam.CustomParamFactory("outcomes").Value = grbsNode.Attributes["name"].Value;
                            HttpContext.Current.Session["CurrentOutcomesID"] = grbsNode.Attributes["id"].Value;
                            return;
                        }
                    }
                }
            }
        }

        public static void MakeItParams(string ITId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/IT.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "ITList")
                {
                    foreach (XmlNode ITNode in rootNode.ChildNodes)
                    {
                        if (ITNode.Attributes[compareField].Value == ITId)
                        {
                            HttpContext.Current.Session["CurrentITID"] = ITNode.Attributes["id"].Value;
                            return;
                        }
                    }
                }
            }
        }

        public static void MakeBuildingCustomerParams(string customerId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/BuildingCustomers.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "customers")
                {
                    foreach (XmlNode customerNode in rootNode.ChildNodes)
                    {
                        if (customerNode.Attributes[compareField].Value == customerId)
                        {
                            HttpContext.Current.Session["CurrentBuilderCustomerID"] = customerNode.Attributes["id"].Value;
                            Core.CustomParam.CustomParamFactory("current_builder_customer").Value = customerNode.Attributes["name"].Value.Replace("&quot;", "\"");
                            return;
                        }
                    }
                }
            }
        }

        public static void MakeOilParams(string oilId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/Oil.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "OilList")
                {
                    foreach (XmlNode oilNode in rootNode.ChildNodes)
                    {
                        if (oilNode.Attributes[compareField].Value == oilId)
                        {
                            Core.CustomParam.CustomParamFactory("oil").Value = oilNode.Attributes["name"].Value;
                            Core.CustomParam.CustomParamFactory("oil_federal").Value = oilNode.Attributes["nameFederal"].Value;
                            Core.CustomParam.CustomParamFactory("oil_shortName").Value = oilNode.Attributes["shortName"].Value;
                            HttpContext.Current.Session["CurrentOilID"] = oilNode.Attributes["id"].Value;
                            return;
                        }
                    }
                }
            }
        }

		public static void MakeFoodParams(string foodId, string compareField)
		{
			// загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/Food.xml");
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFile);
			foreach (XmlNode rootNode in doc.ChildNodes)
			{
				if (rootNode.Name == "food")
				{
					foreach (XmlNode foodNode in rootNode.ChildNodes)
					{
						if (foodNode.Attributes[compareField].Value == foodId)
						{
							Core.CustomParam.CustomParamFactory("food").Value = foodNode.Attributes["name"].Value;
							HttpContext.Current.Session["CurrentFoodID"] = foodNode.Attributes["id"].Value;
							return;
						}
					}
				}
			}
		}

        public static void MakeMotypeParams(string moTypeId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/MoType.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "motype")
                {
                    foreach (XmlNode typeNode in rootNode.ChildNodes)
                    {
                        if (typeNode.Attributes[compareField].Value == moTypeId)
                        {
                            Core.CustomParam.CustomParamFactory("motype").Value = typeNode.Attributes["name"].Value;
                            HttpContext.Current.Session["CurrentMotypeID"] = typeNode.Attributes["id"].Value;
                            return;
                        }
                    }
                }
            }
        }

        public static void MakeProgParams(string progId, string compareField)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/Prog.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "prog")
                {
                    foreach (XmlNode progNode in rootNode.ChildNodes)
                    {
                        if (progNode.Attributes[compareField].Value == progId)
                        {
                            Core.CustomParam.CustomParamFactory("prog").Value = progNode.Attributes["name"].Value;
                            HttpContext.Current.Session["CurrentProgID"] = progNode.Attributes["id"].Value;
                            return;
                        }
                    }
                }
            }
        }

        public static string GetGrbsIdByName(string grbsName)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/Grbs.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)

            {
                if (rootNode.Name == "grbs")
                {
                    foreach (XmlNode grbsNode in rootNode.ChildNodes)
                    {
                        if (grbsNode.Attributes["name"].Value.Replace("&quot;", "\"").ToLower() == grbsName.ToLower())
                        {
                            return grbsNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            return String.Empty;
        }

		public static string GetMOIdByName(string moName)
		{
			string xmlFile = HttpContext.Current.Server.MapPath("~/MO.xml");
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFile);
			// Ищем узел регионов
			foreach (XmlNode rootNode in doc.ChildNodes)
			{
				if (rootNode.Name.ToLower() == "molist")
				{
					foreach (XmlNode stateNode in rootNode.ChildNodes)
					{
						foreach (XmlNode moNode in stateNode.ChildNodes)
						{
							if (moNode.Attributes["name"].Value.Replace("&quot;", "\"").ToLower() == moName.ToLower())
							{
								return moNode.Attributes["id"].Value;
							}
						}
					}
				}
			}
			return String.Empty;
		}

        public static string GetOutcomeIdByName(string outcomesName)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/Outcomes.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "outcomes")
                {
                    foreach (XmlNode outcomesNode in rootNode.ChildNodes)
                    {
                        if (outcomesNode.Attributes["name"].Value.Replace("&quot;", "\"") == outcomesName)
                        {
                            return outcomesNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            return String.Empty;
        }



        public static string GetProgIdByName(string progName)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/Prog.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "prog")
                {
                    foreach (XmlNode progNode in rootNode.ChildNodes)
                    {
                        if (progNode.Attributes["name"].Value.Replace("&quot;", "\"") == progName)
                        {
                            return progNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            return String.Empty;
        }

		public static string GetFoodIdByName(string foodName)
		{
			string xmlFile = HttpContext.Current.Server.MapPath("~/Food.xml");
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFile);
			foreach (XmlNode rootNode in doc.ChildNodes)
			{
				if (rootNode.Name == "food")
				{
					foreach (XmlNode foodNode in rootNode.ChildNodes)
					{
						if (foodNode.Attributes["name"].Value.Replace("&quot;", "\"") == foodName)
						{
							return foodNode.Attributes["id"].Value;
						}
					}
				}
			}
			return String.Empty;
		}

        public static string GetSettlementIdByName(string SettlementName)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/Settlement.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел поселений
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "settlementList")
                {
                    foreach (XmlNode settlementNode in rootNode.ChildNodes)
                    {                      
                        if (settlementNode.Attributes["name"].Value.Replace("&quot;", "\"") == SettlementName)
                        {
                            return settlementNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            return String.Empty;
        }
        public static string GetMotypeIdByName(string moType)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/MoType.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "motype")
                {
                    foreach (XmlNode typeNode in rootNode.ChildNodes)
                    {
                        if (typeNode.Attributes["name"].Value.Replace("&quot;", "\"") == moType)
                        {
                            return typeNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            return String.Empty;
        }

        public static string GetBuilderCustomerIdByName(string customerName)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/iPadParamsSettings/BuildingCustomers.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "customers")
                {
                    foreach (XmlNode typeNode in rootNode.ChildNodes)
                    {
                        if (typeNode.Attributes["name"].Value.Replace("&quot;", "\"") == customerName)
                        {
                            return typeNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            return String.Empty;
        }

        #region Жестко зашитые параметры
        public CustomParam PeriodCurrentDate
        {
            get { return CustomParam("period_cur_date", true); }
        }

        public CustomParam PeriodLastDate
        {
            get { return CustomParam("period_last_date"); }
        }

        public CustomParam PeriodMonthFO
        {
            get { return CustomParam("period_month_fo"); }
        }

        public CustomParam PeriodDayFO
        {
            get { return CustomParam("period_day_fo"); }
        }

        public CustomParam PeriodDayFK
        {
            get { return this.CustomParam("period_day_fk"); }
        }

        public CustomParam PeriodYQM_Quarter
        {
            get { return this.CustomParam("period_yqm_quarter"); }
        }

        public CustomParam PeriodMonth
        {
            get { return this.CustomParam("period_month", true); }
        }

        public CustomParam PeriodYear
        {
            get { return this.CustomParam("period_year", true); }
        }

        public CustomParam PeriodLastYear
        {
            get { return this.CustomParam("period_last_year"); }
        }

        public CustomParam PeriodYearQuater
        {
            get { return this.CustomParam("period_year_quater"); }
        }

        public CustomParam PeriodQuater
        {
            get { return this.CustomParam("period_quater"); }
        }

        public CustomParam PeriodHalfYear
        {
            get { return this.CustomParam("period_half_year"); }
        }

        public CustomParam Region
        {
            get { return this.CustomParam("region"); }
        }

        public CustomParam Organization
        {
            get { return this.CustomParam("organization"); }
        }

        public CustomParam KDLevel
        {
            get { return this.CustomParam("kd_level_un"); }
        }

        public CustomParam KDGroup
        {
            get { return this.CustomParam("kd_group", true); }
        }

        public CustomParam KDTotal
        {
            get { return this.CustomParam("kd_total"); }
        }

        public CustomParam PeriodFirstYear
        {
            get { return this.CustomParam("period_first_year"); }
        }

        public CustomParam PeriodEndYear
        {
            get { return this.CustomParam("period_end_year"); }
        }

        public CustomParam Subject
        {
            get { return this.CustomParam("subject"); }
        }

        public CustomParam SubjectFO
        {
            get { return this.CustomParam("subject_FO"); }
        }

        public CustomParam PeriodDimension
        {
            get { return this.CustomParam("period_dimension"); }
        }

        public CustomParam RegionDimension
        {
            get { return this.CustomParam("region_dimension"); }
        }

        public CustomParam IncomesKDRootName
        {
            get { return this.CustomParam("incomes_kd_root_name"); }
        }

        public CustomParam IncomesKDSocialNeedsTax
        {
            get { return this.CustomParam("incomes_kd_social_needs_tax"); }
        }

        public CustomParam IncomesKDReturnOfRemains
        {
            get { return this.CustomParam("incomes_kd_return_of_remains"); }
        }

        public CustomParam IncomesKD11800000000000000
        {
            get { return this.CustomParam("incomes_kd_11800000000000000"); }
        }

        public CustomParam IncomesKD11402000000000000
        {
            get { return this.CustomParam("incomes_kd_11402000000000000"); }
        }

        public CustomParam IncomesKD11403000000000410
        {
            get { return this.CustomParam("incomes_kd_11403000000000410"); }
        }

        public CustomParam IncomesKD11403000000000440
        {
            get { return this.CustomParam("incomes_kd_11403000000000440"); }
        }

        public CustomParam IncomesKD11406000000000430
        {
            get { return this.CustomParam("incomes_kd_11406000000000430"); }
        }

        /// <summary>
        /// ПРОЧИЕ НЕНАЛОГОВЫЕ ДОХОДЫ
        /// </summary>
        public CustomParam IncomesKD11700000000000000
        {
            get { return this.CustomParam("incomes_kd_11700000000000000"); }
        }

        /// <summary>
        /// ГОСУДАРСТВЕННАЯ ПОШЛИНА
        /// </summary>
        public CustomParam IncomesKD10800000000000000
        {
            get { return this.CustomParam("incomes_kd_10800000000000000"); }
        }

        /// <summary>
        /// ДОХОДЫ ОТ ПРЕДПРИНИМАТЕЛЬСКОЙ И ИНОЙ ПРИНОСЯЩЕЙ ДОХОД ДЕЯТЕЛЬНОСТИ
        /// </summary>
        public CustomParam IncomesKD30000000000000000
        {
            get { return this.CustomParam("incomes_kd_30000000000000000"); }
        }

        public CustomParam IncomesKDGroupLevel
        {
            get { return this.CustomParam("incomes_kd_group_level"); }
        }

        public CustomParam IncomesKDAllLevel
        {
            get { return this.CustomParam("incomes_kd_all_level"); }
        }

        public CustomParam IncomesKDSectionLevel
        {
            get { return this.CustomParam("incomes_kd_section_level"); }
        }

        public CustomParam FNSOKVEDGovernment
        {
            get { return this.CustomParam("fns_okved_government"); }
        }

        public CustomParam FNSOKVEDHousehold
        {
            get { return this.CustomParam("fns_okved_household"); }
        }

        public CustomParam IncomesKDSubSectionLevel
        {
            get { return this.CustomParam("incomes_kd_subsection_level"); }
        }

        public CustomParam IncomesKDDimension
        {
            get { return this.CustomParam("incomes_kd_dimension"); }
        }
        
        public CustomParam FKRDimension
        {
            get { return this.CustomParam("fkr_dimension"); }
        }

        public CustomParam FKRAllLevel
        {
            get { return this.CustomParam("fkr_all_level"); }
        }

        public CustomParam FKRSectionLevel
        {
            get { return this.CustomParam("fkr_section_level"); }
        }

        public CustomParam FKRSubSectionLevel
        {
            get { return this.CustomParam("fkr_subsection_level"); }
        }

        public CustomParam EKRDimension
        {
            get { return this.CustomParam("ekr_dimension"); }
        }

        public CustomParam FOFKRCulture
        {
            get { return this.CustomParam("fo_fkr_culture"); }
        }

        public CustomParam FOFKRHelthCare
        {
            get { return this.CustomParam("fo_fkr_helthcare"); }
        }


        public CustomParam OwnSubjectBudgetName
        {
            get { return this.CustomParam("own_subject_budget_name"); }
        }

        public CustomParam KDInternalCircualtionExtruding
        {
            get { return this.CustomParam("kd_internal_circulation_extruding"); }
        }

        public CustomParam RzPrInternalCircualtionExtruding
        {
            get { return this.CustomParam("rzpr_internal_circulation_extruding"); }
        }

        public CustomParam PopulationCube
        {
            get { return this.CustomParam("population_cube"); }
        }

        public CustomParam PopulationFilter
        {
            get { return this.CustomParam("population_filter"); }
        }

        public CustomParam PopulationPeriodDimension
        {
            get { return this.CustomParam("population_period_dimension"); }
        }

        public CustomParam PopulationValueDivider
        {
            get { return this.CustomParam("population_value_divider"); }
        }

        public CustomParam RegionsLocalBudgetLevel
        {
            get { return this.CustomParam("regions_local_budget_level"); }
        }

        /// <summary>
        /// Федеральный округ
        /// </summary>
        public CustomParam StateArea
        {
            get { return this.CustomParam("state_area", true); }
        }

        /// <summary>
        /// Уровень СКИФ
        /// </summary>
        public CustomParam SKIFLevel
        {
            get { return this.CustomParam("skif_level"); }
        }

        /// <summary>
        /// Элемент измерения Вариант.МесОтч
        /// </summary>
        public CustomParam VariantMesOtch
        {
            get { return this.CustomParam("variant_mes_otch"); }
        }

        /// <summary>
        /// Уровни бюджета. Перечисление унэймов элементов
        /// </summary>
        public CustomParam BudgetLevelEnum
        {
            get { return this.CustomParam("budget_level_enum"); }
        }

        public CustomParam DataCount
        {
            get { return this.CustomParam("data_count"); }
        }

        public CustomParam DayCount
        {
            get { return this.CustomParam("day_count"); }
        }

        public CustomParam ShortStateArea
        {
            get { return this.CustomParam("short_state_area"); }
        }

        public CustomParam ShortRegion
        {
            get { return this.CustomParam("short_region"); }
        }

        public CustomParam Filter
        {
            get { return this.CustomParam("filter", true); }
        }

        public CustomParam FKRFilter
        {
            get { return this.CustomParam("fkr_filter"); }
        }

        public CustomParam SelectItem
        {
            get { return this.CustomParam("selectedItem", true); }
        }

        public CustomParam SelectedMap
        {
            get { return this.CustomParam("selectedMap"); }
        }

        public CustomParam CubeName
        {
            get { return this.CustomParam("cube_name"); }
        }

        public CustomParam FODeficitCount
        {
            get { return this.CustomParam("fo_deficit_count"); }
        }

        public CustomParam FODeficitBudgetLevel
        {
            get { return this.CustomParam("fo_deficit_budgetLevel"); }
        }

        public CustomParam FODeficitDocumentType
        {
            get { return this.CustomParam("fo_deficit_documentType"); }
        }

        public CustomParam Mo
        {
            get { return this.CustomParam("Mo"); }
        }

        public CustomParam Grbs
        {
            get { return this.CustomParam("grbs"); }
        }

        public CustomParam Outcomes
        {
            get { return this.CustomParam("outcomes"); }
        }

		public CustomParam Food
		{
			get { return this.CustomParam("food"); }
		}

        public CustomParam MoType
        {
            get { return this.CustomParam("motype"); }
        }

        public CustomParam Prog
        {
            get { return this.CustomParam("prog"); }
        }

        public CustomParam Oil
        {
            get { return this.CustomParam("oil"); }
        }

        public CustomParam OilFederal
        {
            get { return this.CustomParam("oil_federal"); }
        }

        public CustomParam Terbank
        {
            get { return this.CustomParam("terbank"); }
        }

        public CustomParam BudgetLevel
        {
            get { return this.CustomParam("budget_level"); }
        }

        public CustomParam Settlement
        {
            get { return this.CustomParam("settlement"); }
        }

		public CustomParam FullRegionName
        {
            get { return this.CustomParam("fullRegionName"); }
        }

        #endregion
    }
}
