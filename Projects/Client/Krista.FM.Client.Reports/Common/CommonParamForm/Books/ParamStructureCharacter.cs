using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamStructureCharacter : ParamInfo
    {
        public ParamStructureCharacter()
        {
            Description = "Признак структуры";
            BookInfo = new ParamBookInfo
                           {
                               MultiSelect = true, 
                               EntityKey = fx_FX_StructureCharacter.InternalKey
                           };
        }
    }
}
