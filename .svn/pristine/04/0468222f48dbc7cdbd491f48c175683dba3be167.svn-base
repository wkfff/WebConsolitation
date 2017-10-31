using System;
using System.Collections.Generic;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public sealed class MdxSubProgram : BaseSubProgram
    {
        public MdxSubProgram(string path) : base(path)
        {
        }

        protected override string GetBaseUrl(string appVersion, string installerVersion)
        {
            return String.Format(@"MDXExpert\{0}\", appVersion);
        }

        public override List<IUpdateCondition> DependentConditions
        {
            get { return dependentConditions; }
        }

        public override List<Type> SubProgramDependentTypes
        {
            get { return new List<Type>(); }
        }

        public override bool IsHandle
        {
            get { return true; }
        }

        public override string GetFeedName()
        {
            return "MDXExpert.Feed.xml";
        }
    }
}
