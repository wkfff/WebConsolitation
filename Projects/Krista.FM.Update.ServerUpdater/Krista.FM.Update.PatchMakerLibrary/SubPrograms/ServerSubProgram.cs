using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public sealed class ServerSubProgram : BaseSubProgram
    {
        public ServerSubProgram(string path)
            : base(path)
        {
        }

        public List<IUpdateTask> GetTasks()
        {
            throw new NotImplementedException();
        }

        protected override string GetBaseUrl(string appVersion, string installerVersion)
        {
            throw new NotImplementedException();
        }

        public override List<IUpdateCondition> DependentConditions
        {
            get
            {
                dependentConditions.Clear();

                List<FileInfo> files = GetFilesForPatch(path);
                foreach (var fileInfo in files)
                {
                    if (fileInfo.Extension == ".dll")
                    {
                        string version = FileVersionInfo.GetVersionInfo(fileInfo.FullName).FileVersion.Replace(", ", ".");
                        dependentConditions.Add(PatchMaker.Instance.AddServerModuleVersionCondition("is-above", version, fileInfo.Name));
                    }
                }
                return dependentConditions;
            }
        }

        public override List<Type> SubProgramDependentTypes
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsHandle
        {
            get { return false; }
        }

        public override string GetFeedName()
        {
            throw new NotImplementedException();
        }
    }
}
