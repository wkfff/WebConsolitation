using System;
using System.Collections.Generic;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public sealed class OfficeAddInSubProgram : BaseSubProgram
    {
        public OfficeAddInSubProgram(string path) : base(path)
        {
        }

        protected override string GetBaseUrl(string appVersion, string installerVersion)
        {
            return String.Format(@"OfficeAddIn\{0}\", appVersion);
        }

        public override List<IUpdateCondition> DependentConditions
        {
            get { return dependentConditions; }
        }

        /// <summary>
        /// Зависимости
        /// </summary>
        public override List<Type> SubProgramDependentTypes
        {
            get { return new List<Type> { typeof(ServerSubProgram) }; }
        }

        public override bool IsHandle
        {
            get { return true; }
        }

        public override string GetFeedName()
        {
            return "OfficeAddIn.Feed.xml";
        }
    }
}
