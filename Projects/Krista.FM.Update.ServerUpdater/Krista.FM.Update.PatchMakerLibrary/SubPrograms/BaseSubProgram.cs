using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Krista.FM.Update.Framework;
using Krista.FM.Update.Framework.Utils;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public abstract class BaseSubProgram : IUpdetableSubProgram
    {
        /// <summary>
        /// Путь к папке с обновлением. 
        /// </summary>
        protected string path;

        protected List<IUpdateCondition> dependentConditions = new List<IUpdateCondition>();

        public BaseSubProgram(string path)
        {
            this.path = path;
            SubProgramDependences = new List<IUpdetableSubProgram>();
            this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Имя обновляемой подпрограммы
        /// </summary>
        public string Name { get; set; }

        public List<IUpdetableSubProgram> SubProgramDependences { get; set; }

        public virtual List<IUpdateCondition> GetConditions()
        {
            var conditions = new List<IUpdateCondition>();

            foreach (var subProgramDependence in SubProgramDependences)
            {
                conditions.AddRange(subProgramDependence.DependentConditions);
            }

            return conditions;
        }

        public virtual List<IUpdateTask> GetFileTasks(IUpdatePatch patch, string patchFolder)
        {
            var tasks = new List<IUpdateTask>();

            List<FileInfo> files = GetFilesForPatch(patchFolder);

            foreach (var fileInfo in files)
            {
                string apply = GetApplyName(fileInfo);

                string version = string.Empty;
                string localCondName = String.Empty;
                bool needVersionCondition = true;

                if (fileInfo.Extension == ".dll" || fileInfo.Extension == ".exe")
                {
                    version = String.Format("{0}.{1}.{2}.{3}",
                      FileVersionInfo.GetVersionInfo(fileInfo.FullName).FileMajorPart,
                      FileVersionInfo.GetVersionInfo(fileInfo.FullName).FileMinorPart,
                      FileVersionInfo.GetVersionInfo(fileInfo.FullName).FileBuildPart,
                      FileVersionInfo.GetVersionInfo(fileInfo.FullName).FilePrivatePart);
                }
                else if (fileInfo.Extension == ".pdb")
                {
                    FileInfo info;

                    try
                    {
                        if (File.Exists(System.IO.Path.Combine(fileInfo.DirectoryName, string.Format("{0}.dll",
                                                               System.IO.Path.GetFileNameWithoutExtension(
                                                                   fileInfo.FullName)))))
                        {
                            info = new FileInfo(System.IO.Path.Combine(fileInfo.DirectoryName, string.Format("{0}.dll",
                                                               System.IO.Path.GetFileNameWithoutExtension(
                                                                   fileInfo.FullName))));
                        }
                        else if (File.Exists(System.IO.Path.Combine(fileInfo.DirectoryName, string.Format("{0}.exe",
                                                               System.IO.Path.GetFileNameWithoutExtension(
                                                                   fileInfo.FullName)))))
                        {
                            info = new FileInfo(System.IO.Path.Combine(fileInfo.DirectoryName, string.Format("{0}.exe",
                                                               System.IO.Path.GetFileNameWithoutExtension(
                                                                   fileInfo.FullName))));
                        }
                        else
                        {
                            throw new FileNotFoundException();
                        }

                        version = FileVersionInfo.GetVersionInfo(info.FullName).FileVersion.Replace(", ", ".");
                        localCondName = info.FullName.Substring(patchFolder.Length + 1);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(String.Format("Файл {0}.dll не найден.", fileInfo.Name));
                        continue;
                    }
                }
                else
                {
                    needVersionCondition = false;
                }

                tasks.Add(AddFileUpdateTask(patchFolder, fileInfo, patch, apply, needVersionCondition, version,
                                             localCondName));
                if (fileInfo.Extension == ".cmd")
                {
                    tasks.Add(AddExecuteFileTask(patchFolder, fileInfo, patch));
                }
            }

            return tasks;
        }

        private static IUpdateTask AddExecuteFileTask(string patchFolder, FileInfo fileInfo, IUpdatePatch patch)
        {
            IUpdateTask task = PatchMaker.Instance.AddExecuteFileTask(patch, fileInfo.FullName.Substring(
                patchFolder.Length + 1));

            task.UpdateConditions.AddCondition(PatchMaker.Instance.AddFileChecksumCondition(
                fileInfo.FullName.Substring(patchFolder.Length + 1),
                FileChecksum.GetSHA256Checksum(fileInfo.FullName)));
            task.UpdateConditions.AddCondition(
                PatchMaker.Instance.AddFileDateCondition(fileInfo.FullName.Substring(patchFolder.Length + 1),
                                                            File.GetLastWriteTime(fileInfo.FullName).ToString(),
                                                            "older"));

            return task;
        }

        private static IUpdateTask AddFileUpdateTask(string patchFolder, FileInfo fileInfo, IUpdatePatch patch, string apply, bool needVersionCondition, string version, string localCondName)
        {
            IUpdateTask task = PatchMaker.Instance.AddFileUpdateTask(patch,
                                                                     fileInfo.FullName.Substring(patchFolder.Length + 1),
                                                                     fileInfo.FullName.Substring(patchFolder.Length + 1),
                                                                     apply,
                                                                     fileInfo.FullName.Substring(patchFolder.Length + 1));
            if (needVersionCondition)
            {
                task.UpdateConditions.AddCondition(PatchMaker.Instance.AddFileVersionCondition(version,
                                                                                               localCondName,
                                                                                               "below"));
            }
            else
            {
                task.UpdateConditions.AddCondition(PatchMaker.Instance.AddFileChecksumCondition(
                    fileInfo.FullName.Substring(patchFolder.Length + 1),
                    FileChecksum.GetSHA256Checksum(fileInfo.FullName)));
                task.UpdateConditions.AddCondition(
                    PatchMaker.Instance.AddFileDateCondition(fileInfo.FullName.Substring(patchFolder.Length + 1),
                                                             File.GetLastWriteTime(fileInfo.FullName).ToString(),
                                                             "older"));
            }
            return task;
        }

        private static string GetApplyName(FileInfo fileInfo)
        {
            if (fileInfo.Extension != ".dll" 
                && fileInfo.Extension != ".exe"
                && fileInfo.Extension != ".pdb")
            {
                return "hot-swap";
            }

            // Если название содержит каталог App, то считаем эти задачи не требующими перезапуска
            if (fileInfo.FullName.Contains("\\App\\"))
            {
                return "hot-swap";
            }
            
            return "app-restart";
        }

        protected static List<FileInfo> GetFilesForPatch(string patchFolder)
        {
            var files = new List<string>();
            GetFiles(patchFolder, files);
            List<FileInfo> fi = files.Where(f => !f.Contains("info.xml")).Select(file => new FileInfo(file)).ToList();
            return fi;
        }

        private static void GetFiles(string patchFolder, List<string> files)
        {
            string[] locfiles = Directory.GetFileSystemEntries(patchFolder);
            foreach (string element in locfiles)
            {
                if (Directory.Exists(element))
                {
                    GetFiles(element, files);
                }
                else
                {
                    files.Add(element);
                }
            }
        }

        protected abstract string GetBaseUrl(string appVersion, string installerVersion);

        public IUpdatePatch GetPatch(string patchName, string patchDescription, string patchDetailDescription, Use use, string baseUrl, string version, string displayName, string displayVersion, string installerVersion)
        {
            baseUrl = GetBaseUrl(version, installerVersion);

            IUpdatePatch patch = PatchMaker.Instance.AddUpdatePatch(patchName, patchDescription, patchDetailDescription,
                                                                    use, baseUrl);
            patch.Tasks = GetFileTasks(patch, path);

            if (!String.IsNullOrEmpty(displayName))
            {
                patch.Tasks.Add(AddUpdateVersionTask(patch, displayName, displayVersion));
            }

            foreach (var updateCondition in GetConditions())
            {
                patch.UpdateConditions.AddCondition(updateCondition);
            }

            return patch;
        }

        private static IUpdateTask AddUpdateVersionTask(IUpdatePatch patch, string displayName, string displayVersion)
        {
            return PatchMaker.Instance.AddVersionUpdateTask(patch, displayName, displayVersion);
        }

        public abstract List<IUpdateCondition> DependentConditions { get; }

        public abstract List<Type> SubProgramDependentTypes { get; }

        public abstract bool IsHandle { get; }

        public abstract string GetFeedName();
    }
}
