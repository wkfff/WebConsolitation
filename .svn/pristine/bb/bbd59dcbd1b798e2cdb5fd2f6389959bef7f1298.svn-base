using System;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO
{
    class ParamMarksReceipt : ParamInfo
    {
        public ParamMarksReceipt()
        {
            Description = "Показатели.МОФО_Поступления по арендной плате";
            BookInfo = new ParamBookInfo
                           {
                               FullScreen = true,
                               MultiSelect = true,
                               EntityKey = d_Marks_Receipt.InternalKey,
                               DefaultSort = d_Marks_Receipt.CodeInd,
                               ItemTemplate = String.Format("{0} {1}", d_Marks_Receipt.CodeInd, d_Marks_Receipt.Name)
                           };
        }
    }
}
