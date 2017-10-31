using System;
using System.Security.Cryptography;
using System.Text;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    public static class ScriptingUtils
    {
        public static string GetSectionTableName(string formCode, string sectionCode, int version, string suffix)
        {
            return new StringBuilder("x")
                .Append(formCode.PadRight(12, '_'))
                .Append(sectionCode.PadRight(12, '_'))
                .Append("{0,-3:X}".FormatWith(version).Replace(' ', '_'))
                .Append(suffix)
                .ToString();
        }

        public static string GetReportTableName(string formCode, int version, string suffix)
        {
            return new StringBuilder("x")
                .Append(formCode.PadRight(12, '_'))
                .Append("{0,-3:X}".FormatWith(version).Replace(' ', '_'))
                .Append(suffix)
                .ToString();
        }
    }
}
