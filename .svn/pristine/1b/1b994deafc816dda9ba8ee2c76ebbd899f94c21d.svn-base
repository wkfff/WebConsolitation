using System;
using bus.gov.ru.types.Item1;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using PostSharp.Aspects;
//using PostSharp.Laos;
//using OnMethodBoundaryAspect = PostSharp.Aspects.OnMethodBoundaryAspect;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public class Attributes
    {
        [Serializable]
        public class FieldAccessAttribute : LocationInterceptionAspect
        {
            public static string id;
            public static string positionId;

            //public override bool CompileTimeValidate(PostSharp.Reflection.LocationInfo locationInfo)
            //{
            //    if (locationInfo.LocationKind != LocationKind.Property)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            public override void OnSetValue(LocationInterceptionArgs args) 
            {
                try
                {
                    if (args.Location.Name == "positionId")
                    {
                       
                        positionId = args.Value.ToString();
                    }
                    args.ProceedSetValue();
                }
                catch (Exception e)
                {
                    int index = e.Message.IndexOf("Possible");
                    violationType violation = new violationType
                                    {
                                        code = "IDE",
                                        level = "error",
                                        name = e.Message.Substring(0, index - 1),
                                        description = e.Message.Substring(index, e.Message.Length - index),
                                    };
                    ErrorsCollector.addViolation(positionId, violation);
                    //string type =  args.Location.DeclaringType.FullName + "." + args.Location.Name;
                    switch (args.Location.DeclaringType.FullName + "." + args.Location.Name)
                    {
                        case "bus.gov.ru.types.Item1.refNsiInstitutionTypeType.code":
                            args.Value = "1111111";
                            break;
                        case "bus.gov.ru.types.Item1.refNsiPpoType.code":
                            args.Value = "11111111111";
                            break;
                        case "bus.gov.ru.types.Item1.refNsiOktmoType.code":
                            args.Value = "11111111";
                            break;
                        case "bus.gov.ru.types.Item1.institutionInfoType+additionalLocalType.phone":
                            args.Value = "9-999-9999999";
                            break;
                        case "bus.gov.ru.types.Item1.institutionInfoType+additionalLocalType.eMail":
                            args.Value = "a@mail.ru";
                            break;
                        case "bus.gov.ru.types.Item1.institutionInfoType+additionalLocalType.www":
                            args.Value = "a.com";
                            break;
                    }
                    args.ProceedSetValue();
                }
	        
	        }

        }
    }
}
