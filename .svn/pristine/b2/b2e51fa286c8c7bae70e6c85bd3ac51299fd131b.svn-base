using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.E86N.Services.OGSService
{
    public interface IOGSService
    {
        /// <summary>
        /// Импорт учреждений
        /// </summary>
        void ImportFile(XmlTextReader xmlFile);

        /// <summary>
        /// добавление учреждения
        /// </summary>
        D_Org_Structure AddOrg(ref D_Org_Structure record);

        /// <summary>
        /// Закачка ППО и ОКАТО
        /// </summary>
        string ImportFilePPO(XDocument xmlFile);

        /// <summary>
        /// Закачка учреждений без кэширования
        /// </summary>
        void ImportFileOGSNew(XmlTextReader xmlFile);

        /// <summary>
        /// Закачка Вида учреждения
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
        /// Закачка ОКВЕД
        /// </summary>
        void ImportOkved(XmlTextReader xmlFile);

        /// <summary>
        /// Закачка OKTMO
        /// </summary>
        void ImportOKTMO(XmlTextReader xmlFile);

        /// <summary>
        /// Закачка OKATO
        /// </summary>
        void ImportOKATO(XmlTextReader xmlFile);

        /// <summary>
        /// Закачка NsiOGS
        /// </summary>
        void ImportNsiOGS(XmlTextReader xmlFile);

        /// <summary>
        /// Закачка NsiBudget
        /// </summary>
        void ImportNsiBudget(XmlTextReader xmlFile);
    }
}