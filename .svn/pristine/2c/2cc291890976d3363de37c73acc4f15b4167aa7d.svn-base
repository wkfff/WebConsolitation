
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.FO41
{
    public class UserRoleEstimateValueProvider : IParameterValueProvider
    {
        private readonly IFO41Extension extension;

        public UserRoleEstimateValueProvider(IFO41Extension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            switch (extension.UserGroup)
            {
                case FO41Extension.GroupDF:
                    {
                        return "ДФ";
                    }

                case FO41Extension.GroupOGV:
                    {
                        return "ОГВ";
                    }

                case FO41Extension.GroupTaxpayer:
                    {
                        return "Налогоплательщик";
                    }
            }

            return "Пользователь";
        }
    }
}
