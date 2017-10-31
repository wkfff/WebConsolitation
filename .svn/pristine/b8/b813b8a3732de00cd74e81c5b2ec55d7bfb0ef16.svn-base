using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.EGRUL
{
    class ParamOKATO : ParamINN
    {
        class LocalOKATOBookInfo : LocalINNBookInfo
        {
            public LocalOKATOBookInfo(ReportDBHelper dbHelper)
                : base(dbHelper)
            {
            }

            public override string GetTextName()
            {
                return b_Org_EGRUL.OKATO;
            }
        }

        public ParamOKATO()
        {
            Description = "ОКАТО";
            BookInfo = new LocalOKATOBookInfo(dbHelper)
            {
                FullScreen = true,
                MultiSelect = true
            };
        }
    }
}
