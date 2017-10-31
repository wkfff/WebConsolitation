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
    /// ����� ��� ��������������� ������ �� ����� ������ � ������. ������������ ����� �������� ��������
    /// � xml ������.
    /// </summary>
    public class Convertor
    {
        private XmlNode reportNode;

        /// <summary>
        /// �������� ���� ������ �� ����������� ���������
        /// </summary>
        /// <param name="reportNode">xml ���� � ����������� ������</param>
        /// <param name="reportVersion">������ ������</param>
        /// <returns>������� ���������� ����������</returns>
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

            //��������� �� 3.4.0.0
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
        /// � ������ ������ ��������� ��������� ������� ������ � ������� ������� NonEmpty ���������
        /// ���� � MASS2005, ������� ���� ����� ��������� � 2005 �������, �������� ���������� ������ ������� 
        /// ������ ����� ���: "Filters" �� �����.
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

        #region ��������� ��������������� ������ ����������

        private void ConverHideEmptyMode()
        {
            if ((PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2005) ||
                (PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2008))
            {
                //������� ��������� ���������, ������ � ��� ������������ ������ �������
                XmlNodeList fieldSetsNode = this.reportNode.SelectNodes("//fieldsets");
                if (fieldSetsNode != null)
                {
                    foreach (XmlNode fieldSetNode in fieldSetsNode)
                    {
                        string sHideEmptyMode = XmlHelper.GetStringAttrValue(fieldSetNode, Consts.hideEmptyMode,
                            HideEmptyMode.NonEmpty2005.ToString());
                        HideEmptyMode hideEmptyNode = (HideEmptyMode)Enum.Parse(typeof(HideEmptyMode), sHideEmptyMode);
                        //���� ����� ���������� ������, ���������� ��
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
