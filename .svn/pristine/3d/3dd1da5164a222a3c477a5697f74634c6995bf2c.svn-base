using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Krista.FM.Update.Framework;
using Krista.FM.Update.Framework.Utils;
using Krista.FM.Update.PatchMakerLibrary;
using Krista.FM.Update.PatchMakerLibrary.SubPrograms;
using NConsoler;

namespace Krista.FM.Update.PatchMakerConsole
{
    /// <summary>
    /// Консоль создания патчей обновления
    /// </summary>
    public class PatchMakerConsole
    {
        static void Main(string[] args)
        {
            Consolery.Run(typeof(PatchMakerConsole), args);
            Console.ReadLine();
        }

        [Action]
        public static void CreatePatch(
            [Required(Description = "Путь к каталогу с патчем на FS")]
            string remotePatchFolder, 
            [Required(Description = "Путь к патчу для системы обновления")]
            string destPatchFolder)
        {
            var customConditions = new List<IUpdateCondition>();

            string patchDescription = String.Empty;
            string patchDetailDescription = String.Empty;
            Use use = Use.Optional;
            string baseUrl = string.Empty;
            string OKTMO = string.Empty;
            string appVersion = String.Empty;
            string installerVersion = String.Empty;
            List<VersionUpdateClass> versionUpdateClasses = new List<VersionUpdateClass>();

            string parametersPath = Path.Combine(remotePatchFolder, "info.xml");
            if (!File.Exists(parametersPath))
            {
                throw new FileNotFoundException(String.Format("В каталоге {0} не найден файл с параметрами {1}", remotePatchFolder,
                                                  "info.xml"));
            }

            ReadFileParameters(
                XDocument.Load(parametersPath),
                ref patchDescription,
                ref patchDetailDescription,
                ref use,
                ref OKTMO,
                ref appVersion,
                ref installerVersion,
                ref versionUpdateClasses);

            CheckOKTMOCondition(OKTMO, customConditions);

            DirectoryInfo dirInfo = new DirectoryInfo(remotePatchFolder);
            string patchName = dirInfo.Name;

            List<IUpdetableSubProgram> updetableSubPrograms = GetUpdatableSubPrograms(remotePatchFolder);
            GetSupprogramDependence(updetableSubPrograms);

            string tempFolder = Path.Combine(destPatchFolder, "Temp");
            CreateTempFolder(tempFolder);

            foreach (var updetableSubProgram in updetableSubPrograms.Where(usp => usp.IsHandle))
            {
                if (Directory.GetFiles(((BaseSubProgram)updetableSubProgram).Path, "*.*", SearchOption.AllDirectories).Length > 0)
                {
                    string displayName = String.Empty;
                    string displayVersion = String.Empty;
                    if (versionUpdateClasses.Count != 0)
                    {
                        foreach (var versionUpdateClass in
                            versionUpdateClasses.Where(versionUpdateClass => versionUpdateClass.Names.ToLower().Contains(updetableSubProgram.Name.ToLower())))
                        {
                            displayName = versionUpdateClass.DisplayName;
                            displayVersion = versionUpdateClass.DisplayVersion;
                        }    
                    }

                    IUpdatePatch patch = updetableSubProgram.GetPatch(
                        patchName,
                        patchDescription,
                        patchDetailDescription,
                        use,
                        baseUrl,
                        appVersion,
                        displayName,
                        displayVersion,
                        installerVersion);

                    AddUserCondition(patch, customConditions);
                    SavePatch(updetableSubProgram.GetFeedName(), patch, ((BaseSubProgram) updetableSubProgram).Path,
                              destPatchFolder);
                }
            }

            try
            {
                if (IsInstallerPatch(remotePatchFolder))
                {
                    Console.WriteLine("Создание патча для службы автоматического обновления");
                    Console.WriteLine("Копирование файлов из временной директории.");

                    CopyDirectory(
                          Path.Combine(tempFolder, "Installer")
                        , Path.Combine(destPatchFolder, "Installer")
                        , false);
                    Directory.Delete(Path.Combine(tempFolder, "Installer"), true);
                }

                if (!IsOnlyInstallerPatch(remotePatchFolder))
                {
                    Console.WriteLine("Создание архива с патчем");
                    ArchiveHelper.CompressDir(
                        Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                                     "ExternalApp\\7za.exe"), tempFolder, patchName,
                        string.Format("{0}\\Global", destPatchFolder));
                }
                
                Console.WriteLine("Удаление временной папки");
                DropTempFolder(tempFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"В процессе создания архива произошла ошибка: {0}", e);
            }

            Console.WriteLine("Патч создан!");
        }

        /// <summary>
        /// В патче содержиться часть обновления для службы
        /// </summary>
        /// <param name="remotePatchFolder"></param>
        /// <returns></returns>
        private static bool IsInstallerPatch(string remotePatchFolder)
        {
            string[] subdirectories = Directory.GetDirectories(remotePatchFolder, "*", SearchOption.AllDirectories);
            return subdirectories.Any(subdirectory => subdirectory.ToLower().Contains("installer"));
        }

        /// <summary>
        /// В патче находятся ТОЛЬКО обновления для службы
        /// </summary>
        /// <param name="remotePatchFolder"></param>
        /// <returns></returns>
        private static bool IsOnlyInstallerPatch(string remotePatchFolder)
        {
            string[] subdirectories = Directory.GetDirectories(remotePatchFolder, "*", SearchOption.AllDirectories);
            return subdirectories.Any(subdirectory => subdirectory.ToLower().Contains("installer")) 
                && subdirectories.Count() == 1;
        }

        private static void DropTempFolder(string tempFolder)
        {
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }

        private static void CreateTempFolder(string tempFolderName)
        {
            if (Directory.Exists(tempFolderName))
            {
                Directory.Delete(tempFolderName, true);
            }

            Directory.CreateDirectory(tempFolderName);
        }

        private static void CheckOKTMOCondition(string oktmo, List<IUpdateCondition> customConditions)
        {
            if (!String.IsNullOrEmpty(oktmo))
                customConditions.Add(PatchMaker.Instance.AddOKTMOCondition(oktmo));
        }

        /// <summary>
        /// Добавление пользовательских условий
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="conditions"></param>
        private static void AddUserCondition(IUpdatePatch patch, List<IUpdateCondition> conditions)
        {
            foreach (var updateCondition in conditions)
            {
                patch.UpdateConditions.AddCondition(updateCondition);
            }
        }

        private static List<IUpdetableSubProgram> GetUpdatableSubPrograms(string patchFolder)
        {
            var updetableSubPrograms = new List<IUpdetableSubProgram>();
            string[] subdirectories = Directory.GetDirectories(patchFolder, "*", SearchOption.TopDirectoryOnly);

            foreach (var subdirectory in subdirectories)
            {
                switch (subdirectory.Split('\\')[subdirectory.Split('\\').Length - 1].ToLower())
                {
                    case "server":
                        updetableSubPrograms.Add(new ServerSubProgram(subdirectory));
                        break;
                    case "common":
                        updetableSubPrograms.Add(new ClientCommonSubProgram(subdirectory));
                        break;
                    case "mdxexpert":
                        updetableSubPrograms.Add(new MdxSubProgram(subdirectory));
                        break;
                    case "officeaddin":
                        updetableSubPrograms.Add(new OfficeAddInSubProgram(subdirectory));
                        break;
                    case "installer":
                        updetableSubPrograms.Add(new InstallerSubProgram(subdirectory));
                        break;
                    case "client":
                        string[] subdir = Directory.GetDirectories(subdirectory, "*", SearchOption.TopDirectoryOnly);
                        foreach (var s in subdir)
                        {
                            switch (s.Split('\\')[s.Split('\\').Length - 1].ToLower())
                            {
                                case "olapadmin":
                                    updetableSubPrograms.Add(new ClientOlapAdminSubProgram(Path.Combine(subdirectory, s.Split('\\')[s.Split('\\').Length - 1])));
                                    break;
                                case "schemedesigner":
                                    updetableSubPrograms.Add(new ClientSchemeDesignerSubProgram(Path.Combine(subdirectory, s.Split('\\')[s.Split('\\').Length - 1])));
                                    break;
                                case "workplace":
                                    updetableSubPrograms.Add(new ClientWorkplaceSubProgram(Path.Combine(subdirectory, s.Split('\\')[s.Split('\\').Length - 1])));
                                    break;
                            }
                        }

                        break;
                }
            }

            return updetableSubPrograms;
        }

        /// <summary>
        /// Определяем наличие зависимостей в патче
        /// </summary>
        /// <param name="updetableSubPrograms"></param>
        private static void GetSupprogramDependence(List<IUpdetableSubProgram> updetableSubPrograms)
        {
            foreach (var updetableSubProgram in updetableSubPrograms)
            {
                try
                {
                    foreach (var subProgram in updetableSubPrograms)
                    {
                        if (updetableSubProgram.SubProgramDependentTypes.Contains(subProgram.GetType()))
                            ((BaseSubProgram)updetableSubProgram).SubProgramDependences.Add(subProgram);
                    }
                }
                catch (NotImplementedException ex)
                {
                    // глушим ошибку для пока нериальзованных зависимостей
                }
            }
        }

        /// <summary>
        /// Копирование файлов патча в каталог с каналами обн6овлеия, обновление каналов
        /// </summary>
        /// <param name="feedName"></param>
        /// <param name="patch"></param>
        private static void SavePatch(string feedName, IUpdatePatch patch, string remotePatchFolder, string destPatchFolder)
        {
            // 1. Копирование файлов
            try
            {
                CreateFolderAndCopyFiles(patch, remotePatchFolder, destPatchFolder);
            }
            catch (CreatePatchFolderException e)
            {
                Console.WriteLine(String.Format("ERROR: Ошибка при создании каталога с патчем и копированием файлов"));
            }
            // 2. Обновление канала
            UpdateFeed(Path.Combine(destPatchFolder, Path.Combine(patch.BaseUrl, feedName)), patch);
        }

        private static void CreateFolderAndCopyFiles(IUpdatePatch patch, string remotePatchFolder, string destPatchFolder)
        {

            string patchPath = Path.Combine(Path.Combine(Path.Combine(destPatchFolder, "Temp"), patch.BaseUrl), patch.Name);

            if (!PermissionsCheck.HaveWritePermissionsForFolder(destPatchFolder))
                throw new CreatePatchFolderException(String.Format("Нет прав на папку {0}", destPatchFolder));

            try
            {
                if (!Directory.Exists(patchPath))
                    Directory.CreateDirectory(patchPath);
            }
            catch (IOException e)
            {
                throw new CreatePatchFolderException(e.Message);
            }
            catch(ArgumentException e)
            {
                throw new CreatePatchFolderException(e.Message);
            }

            Console.WriteLine(String.Format("Каталог для патча {0} успешно создан!", patchPath));

            CopyDirectory(remotePatchFolder, patchPath, true);
        }

        public static void CopyDirectory(string sourceDir, string destDir, bool useDeploy)
        {
            if (destDir[destDir.Length - 1] != Path.DirectorySeparatorChar)
                destDir += Path.DirectorySeparatorChar;

            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            string[] files = Directory.GetFileSystemEntries(sourceDir);
            foreach (string element in files)
            {
                if (Directory.Exists(element))
                {
                    CopyDirectory(element, destDir + Path.GetFileName(element), useDeploy);
                }
                else
                {
                    File.Copy(element,
                              string.Format("{0}{1}{2}", destDir, Path.GetFileName(element),
                                            useDeploy ? ".deploy" : String.Empty), true);
                }
            }
        }

        private static void UpdateFeed(string feedName, IUpdatePatch patch)
        {
            XElement patchEl = patch.ToXml();
            Console.WriteLine(patchEl.ToString());

            XDocument feedDocument = XDocument.Load(feedName);

            if (feedDocument.Descendants("Patch").Attributes().Where(a => a.Name == "name").Any(xAttribute => xAttribute.Value == patch.Name))
            {
                String ans;
                do
                {
                    Console.WriteLine("Канал обновления {0} уже содержит патч с именем {1}. Заменить его?(Y/N)",
                                      feedName, patch.Name);
                    ans = Console.ReadLine();
                } 
                while (ans != null && (ans.ToLower() != "y" && ans.ToLower() != "n"));

                if (ans != null && ans.ToLower() == "y")
                {
                    ReplacePatch(feedDocument, feedName, patchEl, patch.Name);
                }
                
                return;
            }
            
            CreatePatch(feedDocument, feedName, patchEl);
        }

        private static void ReplacePatch(XDocument feedDocument, string feedName, XElement patchEl, string  patchName)
        {
            XElement root = feedDocument.Descendants("Patches").First();

            XElement replacePatch = feedDocument.Descendants("Patch").Where(p => p.Attribute("name").Value == patchName).First();

            if (root != null)
            {
                replacePatch.Remove();
                CreatePatch(feedDocument, feedName, patchEl);
                Console.WriteLine(String.Format("В канале обновления {0} заменен патч {1}!", feedName, patchName));
            }
        }

        private static void CreatePatch(XDocument feedDocument, string feedName, XElement patchEl)
        {
            XElement root = feedDocument.Descendants("Patches").First();

            if (root != null)
            {
                root.Add(patchEl);
                feedDocument.Save(feedName);
                Console.WriteLine(String.Format("В канал обновления {0} добавлен патч!", feedName));
            }
        }

        internal static void ReadFileParameters(
            XDocument document,
            ref string patchDescription,
            ref string patchDetailDescription,
            ref Use use,
            ref string OKTMO,
            ref string appVersion,
            ref string installerVersion,
            ref List<VersionUpdateClass> versionUpdateClasses)
        {
            XElement root = document.Descendants("Patch").First();
            if (root != null)
            {
                patchDescription = root.Descendants("Description").First().Value;
                use = GetPatchUse(root.Descendants("Use").First().Value);
                OKTMO = root.Descendants("OKTMO").First().Value;
                patchDetailDescription = (root.Descendants("DescriptionDetail").Count() != 0 &&
                                          !String.IsNullOrEmpty(root.Descendants("DescriptionDetail").First().Value))
                                             ? root.Descendants("DescriptionDetail").First().Value
                                             : patchDescription;

                // Базовая версия обновляемого клиентского приложения
                if (!root.Descendants("AppVersion").Any())
                {
                    Console.WriteLine("В файле info.xml отсутствует обязательный параметр AppVersion");
                    return;
                }
                appVersion = root.Descendants("AppVersion").First().Value;

                // Базовая версия службы автоматического обновления. Указываем ее, если обновление на сам инсталлятор
                if (root.Descendants("InstallerVersion").Any())
                {
                    installerVersion = root.Descendants("InstallerVersion").First().Value;
                }
                
                // Работа с версией
                versionUpdateClasses.AddRange(from versionUpdate in root.Descendants("VersionUpdate")
                                              where versionUpdate != null
                                              select
                                                  new VersionUpdateClass(
                                                  versionUpdate.Descendants("Names").First().Value,
                                                  versionUpdate.Descendants("DisplayName").First().Value,
                                                  versionUpdate.Descendants("DisplayVersion").First().Value));
            }
        }

        private static Use GetPatchUse(string value)
        {
            switch (value.ToLower())
            {
                case "optional":
                    return Use.Optional;
                case "required":
                    return Use.Required;
                case "prohibited":
                    return Use.Prohibited;
                default:
                    throw new Exception(String.Format("Не обработанный тип обязательности патча - {0}", value));
            }
        }

        internal static void ReadFileParameters()
        {
            throw new NotImplementedException();
        }
    }
}
