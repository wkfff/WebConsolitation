using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

using Krista.FM.Client.SMO;
using Krista.FM.Client.SMO.Design;

namespace Krista.FM.Client.Design.Editors
{
    public class DataKindConvertor : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
        {
            if (t == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, t);
        }
        
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is SmoDataKindDesign)
            {
                SmoDataKindDesign p = (SmoDataKindDesign)value;
                return p.Code + ", " + p.Name + " (" + p.Description + ")";
            }
            return base.ConvertTo(context, culture, value, destType);
        }   
        
         public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo info, object value)
         {
            if (value is string)
            {
                try 
                {
                    string s = (string) value;

                    int comma = s.IndexOf(',');
                    if (comma != -1) 
                    {
                        // теперь, когда мы нашли запятую,

                        string last = s.Substring(0, comma);
                        int paren = s.LastIndexOf('(');
                        if (paren != -1 && s.LastIndexOf(')') == s.Length - 1)
                        {
                            string first = 
                            s.Substring(comma + 1, paren - comma - 1);
                        }
                    }
                } 
                catch {}
                // если мы попали сюда, сообщим,
                // что мы не можем провести разбор строки
                //
                throw new ArgumentException(
                "Can not convert '" + (string)value + "' to type SMODataKind");
            }
            return base.ConvertFrom(context, info, value);
        }
    }
}
