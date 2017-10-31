using System;

namespace Krista.FM.Server.DataPumps.FNS24Pump
{
    public partial class FNS24PumpModule : CorrectedPumpModuleBase
    {

        #region Константы

        #region Атрибуты

        private const string ATTR_ADRESIN = "ADRESIN";

        private const string ATTR_BIK = "BIK";

        private const string ATTR_CNT_IUL = "CNT_IUL";
        private const string ATTR_CNT_RUL = "CNT_RUL";
        private const string ATTR_CNT_UCHRFL = "CNT_UCHRFL";

        private const string ATTR_DOLGN = "DOLGN";
        private const string ATTR_DOM = "DOM";
        private const string ATTR_DT = "DT";
        private const string ATTR_DTEND = "DTEND";
        private const string ATTR_DTENDDOK = "DTENDDOK";
        private const string ATTR_DTOGRN = "DTOGRN";
        private const string ATTR_DTREG = "DTREG";
        private const string ATTR_DTRESH = "DTRESH";
        private const string ATTR_DTSTART = "DTSTART";
        private const string ATTR_DTSTARTNOV = "DTSTARTNOV";
        private const string ATTR_DTSTOP = "DTSTOP";
        private const string ATTR_DTZAP = "DTZAP";

        private const string ATTR_FAM_FL = "FAM_FL";
        private const string ATTR_FAMLAT = "FAMLAT";
        private const string ATTR_FAX = "FAX";

        private const string ATTR_ID = "ID";
        private const string ATTR_ID_TA = "ID_TA";
        private const string ATTR_ID_VA = "ID_VA";
        private const string ATTR_IDDOK = "IDDOK";
        private const string ATTR_IDREG = "IDREG";
        private const string ATTR_INDEKS = "INDEKS";
        private const string ATTR_INN = "INN";

        private const string ATTR_KOD = "KOD";
        private const string ATTR_KOD_KL = "KOD_KL";
        private const string ATTR_KOD_LANG = "KOD_LANG";
        private const string ATTR_KOD_OKVED = "KOD_OKVED";
        private const string ATTR_KOD_OPF = "KOD_OPF";
        private const string ATTR_KOD_ST = "KOD_ST";
        private const string ATTR_KODGOROD = "KODGOROD";
        private const string ATTR_KODORG = "KODORG";
        private const string ATTR_KORP = "KORP";
        private const string ATTR_KPP = "KPP";
        private const string ATTR_KVART = "KVART";

        private const string ATTR_MAIN = "MAIN";

        private const string ATTR_NAME = "NAME";
        private const string ATTR_NAME_FL = "NAME_FL";
        private const string ATTR_NAME_TA = "NAME_TA";
        private const string ATTR_NAME_VA = "NAME_VA";
        private const string ATTR_NAME_VLIC = "NAME_VLIC";
        private const string ATTR_NAMEF = "NAMEF";
        private const string ATTR_NAMEISPORG = "NAMEISPORG";
        private const string ATTR_NAMELAT = "NAMELAT";
        private const string ATTR_NAMEORG = "NAMEORG";
        private const string ATTR_NAMEP = "NAMEP";
        private const string ATTR_NAMES = "NAMES";
        private const string ATTR_NUM = "NUM";
        private const string ATTR_NUM_CONTRACT = "NUM_CONTRACT";
        private const string ATTR_NUM_SV = "NUM_SV";
        private const string ATTR_NUMLIC = "NUMLIC";
        private const string ATTR_NUMOLD = "NUMOLD";
        private const string ATTR_NUMST = "NUMST";

        private const string ATTR_OGRN = "OGRN";
        private const string ATTR_OGRNIP = "OGRNIP";
        private const string ATTR_OKATO = "OKATO";
        private const string ATTR_OTCH_FL = "OTCH_FL";
        private const string ATTR_OTCHLAT = "OTCHLAT";

        private const string ATTR_REGN_FOMS = "REGN_FOMS";
        private const string ATTR_REGN_FSS = "REGN_FSS";
        private const string ATTR_REGN_PF = "REGN_PF";
        private const string ATTR_REGNUM = "REGNUM";

        private const string ATTR_SER = "SER";
        private const string ATTR_SER_SV = "SER_SV";
        private const string ATTR_SEX = "SEX";
        private const string ATTR_SPR = "SPR";
        private const string ATTR_SUMMA = "SUMMA";

        private const string ATTR_TELEFON = "TELEFON";

        private const string ATTR_VID_OB = "VID_OB";

        #endregion Атрибуты

        #region Элементы

        private const string ELEM_ACCOUNT = "ACCOUNT";
        private const string ELEM_ADDRESS = "ADDRESS";

        private const string ELEM_CITIZEN = "CITIZEN";
        private const string ELEM_CONTACT = "CONTACT";

        private const string ELEM_DOCDSN = "DOCDSN";
        private const string ELEM_DOCFL = "DOCFL";
        private const string ELEM_DOCREZ = "DOCREZ";
        private const string ELEM_DOKDN = "DOKDN";
        private const string ELEM_DOLGNFL = "DOLGNFL";

        private const string ELEM_FL = "FL";
        private const string ELEM_FL_ADDR = "FL_ADDR";
        private const string ELEM_FOMS = "FOMS";
        private const string ELEM_FSS = "FSS";

        private const string ELEM_GOROD = "GOROD";

        private const string ELEM_IP = "IP";
        private const string ELEM_IUL = "IUL";

        private const string ELEM_LANG = "LANG";
        private const string ELEM_LICENZ = "LICENZ";
        private const string ELEM_LICORG = "LICORG";

        private const string ELEM_MNS = "MNS";

        private const string ELEM_NAMEI = "NAMEI";
        private const string ELEM_NAMEN = "NAMEN";
        private const string ELEM_NASPUNKT = "NASPUNKT";

        private const string ELEM_OKSM = "OKSM";
        private const string ELEM_OKVED = "OKVED";
        private const string ELEM_OPF = "OPF";
        private const string ELEM_ORGAN_FOMS = "ORGAN_FOMS";
        private const string ELEM_ORGAN_FSS = "ORGAN_FSS";
        private const string ELEM_ORGAN_MNS = "ORGAN_MNS";
        private const string ELEM_ORGAN_PF = "ORGAN_PF";
        private const string ELEM_OSNDN = "OSNDN";

        private const string ELEM_PF = "PF";
        private const string ELEM_PREDSH = "PREDSH";
        private const string ELEM_PREEM = "PREEM";

        private const string ELEM_RAION = "RAION";
        private const string ELEM_REESTR_AO = "REESTR_AO";
        private const string ELEM_REGEGRIP = "REGEGRIP";
        private const string ELEM_REGEGRUL = "REGEGRUL";
        private const string ELEM_REGION = "REGION";
        private const string ELEM_REGOLD = "REGOLD";
        private const string ELEM_REGORG = "REGORG";
        private const string ELEM_RUL = "RUL";

        private const string ELEM_SOSTLIC = "SOSTLIC";
        private const string ELEM_SOSTZAP = "SOSTZAP";
        private const string ELEM_STATUS = "STATUS";
        private const string ELEM_STREET = "STREET";
        private const string ELEM_SVSV = "SVSV";

        private const string ELEM_UCHR = "UCHR";
        private const string ELEM_UCHRFL = "UCHRFL";
        private const string ELEM_UL = "UL";
        private const string ELEM_UL_ADDRESS = "UL_ADDRESS";
        private const string ELEM_UL_CAPITAL = "UL_CAPITAL";
        private const string ELEM_UL_FINISH = "UL_FINISH";
        private const string ELEM_UL_NAME = "UL_NAME";
        private const string ELEM_UL_OB = "UL_OB";
        private const string ELEM_UL_START = "UL_START";
        private const string ELEM_UL_UPR = "UL_UPR";

        private const string ELEM_VIDADR = "VIDADR";
        private const string ELEM_VIDCAP = "VIDCAP";
        private const string ELEM_VIDCITIZEN = "VIDCITIZEN";
        private const string ELEM_VIDDOC = "VIDDOC";
        private const string ELEM_VIDDOK = "VIDDOK";
        private const string ELEM_VIDDOKREZ = "VIDDOKREZ";
        private const string ELEM_VIDIP = "VIDIP";
        private const string ELEM_VIDLIC = "VIDLIC";
        private const string ELEM_VIDREG = "VIDREG";

        #endregion Элементы

        #endregion Константы

    }
}
