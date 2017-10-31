using System;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamTypesPersons : ParamInfo
    {
        public ParamTypesPersons()
        {
            Description = "Виды.Лица";
            NotUncheckBeforeSelect = true;
            BookInfo = new ParamBookInfo
            {
                MultiSelect = true,
                FullScreen = false,
                EntityKey = fx_Types_Persons.InternalKey,
                DefaultSort = fx_Types_Persons.Code,
                ItemTemplate = String.Format("{0}", fx_Types_Persons.Name),
                WriteFullText = true
            };
        }
    }
}
