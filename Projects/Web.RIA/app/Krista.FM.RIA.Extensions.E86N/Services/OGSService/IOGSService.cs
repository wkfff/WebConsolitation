using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.E86N.Services.OGSService
{
    public interface IOGSService
    {
        /// <summary>
        /// ������ ����������
        /// </summary>
        void ImportFile(XmlTextReader xmlFile);

        /// <summary>
        /// ���������� ����������
        /// </summary>
        D_Org_Structure AddOrg(ref D_Org_Structure record);

        /// <summary>
        /// ������� ��� � �����
        /// </summary>
        string ImportFilePPO(XDocument xmlFile);

        /// <summary>
        /// ������� ���������� ��� �����������
        /// </summary>
        void ImportFileOGSNew(XmlTextReader xmlFile);

        /// <summary>
        /// ������� ���� ����������
        /// </summary>
        void ImportInstitutionType(XmlTextReader xmlFile);

        void CopyPassportContent(
            F_F_ParameterDoc parameterDoc,
            out F_F_ParameterDoc oldParameterDoc,
            out F_Org_Passport oldPassport,
            out List<F_F_Founder> oldFounders, 
            out List<F_F_OKVEDY> oldActivities,
            out List<F_F_Filial> oldFilials);

        /// <summary>
        /// ������� �����
        /// </summary>
        void ImportOkved(XmlTextReader xmlFile);

        /// <summary>
        /// ������� OKTMO
        /// </summary>
        void ImportOKTMO(XmlTextReader xmlFile);

        /// <summary>
        /// ������� OKATO
        /// </summary>
        void ImportOKATO(XmlTextReader xmlFile);

        /// <summary>
        /// ������� NsiOGS
        /// </summary>
        void ImportNsiOGS(XmlTextReader xmlFile);

        /// <summary>
        /// ������� NsiBudget
        /// </summary>
        void ImportNsiBudget(XmlTextReader xmlFile);
    }
}