using System;
using Krista.FM.Update.Framework;
using Krista.FM.Update.Framework.Conditions;
using Krista.FM.Update.Framework.Conditions.SchemeConditions;
using Krista.FM.Update.Framework.UpdateObjects;
using Krista.FM.Update.Framework.UpdateObjects.Tasks;

namespace Krista.FM.Update.PatchMakerLibrary
{
    public class PatchMaker
    {
        private static readonly PatchMaker instance = new PatchMaker();

        private PatchMaker()
        {
            // нужные для создания патча поля
        }

        public static PatchMaker Instance
        {
            get { return instance; }
        }

        public IUpdatePatch AddUpdatePatch(string name, string description, string detailDescription, Use use, string baseUrl)
        {
            var updatePatch = new UpdatePatch(Guid.NewGuid().ToString(), name, description, detailDescription, use)
                                          {BaseUrl = baseUrl};
            return updatePatch;
        }

        #region Методы добавления задач в патч

        public IUpdateTask AddFileUpdateTask(IUpdatePatch patch, string updateTo, string localpath, string applay, string description)
        {
            FileUpdateTask task = new FileUpdateTask(patch);
            task.Attributes["updateTo"] = updateTo;
            task.Attributes["localPath"] = localpath;
            task.Attributes["apply"] = applay;
            task.Description = description;

            return task;
        }

        public IUpdateTask AddVersionUpdateTask(IUpdatePatch patch, string displayName, string displayVersion)
        {
            VersionUpdateTask versionUpdateTask = new VersionUpdateTask(patch);
            versionUpdateTask.Attributes["DisplayName"] = displayName;
            versionUpdateTask.Attributes["DisplayVersion"] = displayVersion;

            return versionUpdateTask;
        }

        public IUpdateTask AddExecuteFileTask(IUpdatePatch patch, string updateTo)
        {
            FileExecuteTask fileExecuteTask = new FileExecuteTask(patch);
            fileExecuteTask.Attributes["updateTo"] = updateTo;

            return fileExecuteTask;
        }

        #endregion

        #region Методы добавления ограничений

        public IUpdateCondition AddFileVersionCondition(string version, string localName, string what)
        {
            var fvc = new FileVersionCondition();
            fvc.Attributes["what"] = what;
            fvc.Attributes["version"] = version;
            if (!String.IsNullOrEmpty(localName)) fvc.Attributes["localPath"] = localName;

            return fvc;
        }

        public IUpdateCondition AddFileExistsCondition(string localPath)
        {
            var fec = new FileExistsCondition();
            fec.Attributes["localPath"] = localPath;
            return fec;
        }

        public IUpdateCondition AddServerModuleVersionCondition(string what, string version, string moduleName)
        {
            var smvc = new ServerModuleVersionCondition(true);
            smvc.Attributes["what"] = what;
            smvc.Attributes["version"] = version;
            smvc.Attributes["moduleName"] = moduleName;
            return smvc;
        }

        public IUpdateCondition AddOKTMOCondition(string oktmo)
        {
            var oktmoCondition = new OKTMOCondition(true);
            oktmoCondition.Attributes["OKTMO"] = oktmo;
            return oktmoCondition;
        }

        public IUpdateCondition AddFileChecksumCondition(string localPath, string sha256checksum)
        {
            var fcsc = new FileChecksumCondition();
            fcsc.Attributes["localPath"] = localPath;
            fcsc.Attributes["sha256-checksum"] = sha256checksum;
            return fcsc;
        }

        public IUpdateCondition AddFileDateCondition(string localPath, string timestamp, string what)
        {
            var fdc = new FileDateCondition();
            fdc.Attributes["localPath"] = localPath;
            fdc.Attributes["what"] = what;
            fdc.Attributes["timestamp"] = timestamp;
            return fdc;
        }

        #endregion

    }
}
