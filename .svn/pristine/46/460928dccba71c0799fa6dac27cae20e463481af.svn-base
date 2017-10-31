using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Класс для конвертирования отчета от одной версии к другой. Единственный можер работать напрямую
    /// с xml отчета.
    /// </summary>
    public class Convertor
    {
        private XmlNode reportNode;

        /// <summary>
        /// Обновить мета данные до актуального состояния
        /// </summary>
        /// <param name="reportNode">xml узел с метаданными отчета</param>
        /// <param name="reportVersion">версия отчета</param>
        /// <returns>признак успешности обновления</returns>
        public bool Update(ref XmlNode reportNode, string reportVersion)
        {
            if (reportVersion == string.Empty)
                reportVersion = "0.0.0.0";

            if ((reportVersion == Consts.applicationVersion))
                return true;

            string[] versionParts = reportVersion.Split('.');
            int major = int.Parse(versionParts[0]);
            int minor = int.Parse(versionParts[1]);
            int build = int.Parse(versionParts[2]);
            int revision = int.Parse(versionParts[3]);
            this.reportNode = reportNode;

            //Обновляем до 3.4.0.0
            if ((major == 3) && (minor < 4))
            {
                if (!this.UpdateTo_3_4_0_0())
                    return false;
                major = 3;
                minor = 4;
                build = 0;
                revision = 0;
            }

            return true;
        }

        /// <summary>
        /// В данной версии появилась поддержка скрытия пустых с помощью функции NonEmpty доступной
        /// лишь в MASS2005, поэтому если отчет подключен к 2005 серверу, поменяем устаревшие режимы скрытия 
        /// пустых такие как: "Filters" на новый.
        /// </summary>
        /// <returns></returns>
        private bool UpdateTo_3_4_0_0()
        {
            if (this.reportNode == null)
                return true;

            try
            {
                this.ConverHideEmptyMode();
            }
            catch
            {
                return false;
            }
            return true;
        }

        #region приватные вспомогательные методы конвертора

        private void ConverHideEmptyMode()
        {
            if ((PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2005) ||
                (PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2008))
            {
                //находим коллекцию измерений, именно у нее выставляется данный признак
                XmlNodeList fieldSetsNode = this.reportNode.SelectNodes("//fieldsets");
                if (fieldSetsNode != null)
                {
                    foreach (XmlNode fieldSetNode in fieldSetsNode)
                    {
                        string sHideEmptyMode = XmlHelper.GetStringAttrValue(fieldSetNode, Consts.hideEmptyMode,
                            HideEmptyMode.NonEmpty2005.ToString());
                        HideEmptyMode hideEmptyNode = (HideEmptyMode)Enum.Parse(typeof(HideEmptyMode), sHideEmptyMode);
                        //Если стоят устаревшие режимы, исправляем их
                        if (hideEmptyNode == HideEmptyMode.UsingFilter)
                        {
                            XmlHelper.SetAttribute(fieldSetNode, Consts.hideEmptyMode, 
                                HideEmptyMode.NonEmpty2005.ToString());
                        }
                    }
                }
            }
        }

        #endregion
    }
}
