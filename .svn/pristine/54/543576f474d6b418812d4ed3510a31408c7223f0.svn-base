using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Krista.FM.Update.Framework.Conditions;
using Krista.FM.Update.Framework.UpdateObjects.Tasks;

namespace Krista.FM.Update.Framework.UpdateObjects
{
    [Serializable]
    public class UpdatePatch : IUpdatePatch
    {
        #region Fields

        private string tempArchiveName;

        #endregion

        #region Constuctor

        public UpdatePatch(string objectKey, string name, string description, string descriptionDetail, Use use, string baseUrl, IUpdateFeed feed)
            : this(objectKey, name, description, descriptionDetail, use)
        {
            BaseUrl = baseUrl;
            Feed = feed;
        }

        public UpdatePatch(string objectKey, string name, string description, string descriptionDetail, IList<IUpdateTask> tasks, Use use)
            : this(objectKey, name, description, descriptionDetail, use)
        {
            Tasks = tasks;
        }

        public UpdatePatch(string objectKey, string name, string description, string descriptionDetail, Use use)
        {
            ObjectKey = objectKey;
            Name = name;
            Description = description;
            DescriptionDetail = descriptionDetail;
            Use = use;
            UpdateConditions = new BooleanCondition();
            Tasks = new List<IUpdateTask>();
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string DescriptionDetail { get; set; }

        public IList<IUpdateTask> Tasks { get; set; }

        public Use Use { get; set; }

        public string BaseUrl { get; set; }

        public string ObjectKey { get; set; }

        public IBooleanCondition UpdateConditions { get; set; }

        public IUpdateFeed Feed { get; set; }

        public PrepareState IsPrepared { get; set; }

        #endregion

        #region Implement IUpdatePatch 

        public void CheckForUpdates(IUpdateFeed updateFeed)
        {
            if (UpdateManager.Instance.IsServerMode && !UpdateManager.InstallerFeed.Equals(Feed.Name))
            {
                if (UpdateConditions == null || UpdateConditions.IsMet(null))
                {
                    Trace.TraceInformation(string.Format("Проверка наличия патча {0}", Name));
                    if (!Directory.Exists(Path.Combine(UpdateManager.Instance.DestBaseUrl, Path.Combine(BaseUrl, Name))))
                    {
                        Trace.TraceInformation(string.Format("Найдено новое обновление: {0} - {1}", Name, Description));
                        updateFeed.UpdatesToApply.Add(this);
                    }
                }
            }
            else
            {
                Trace.TraceInformation(string.Format("Проверка условии патча {0}", Name));

                if (UpdateConditions == null || UpdateConditions.IsMet(null))
                {
                    Trace.TraceVerbose(String.Format("Услови патча выполнены"));
                    var p = new UpdatePatch(ObjectKey, Name, Description, DescriptionDetail, Use, BaseUrl,
                                            updateFeed);

                    var tasks = new List<IUpdateTask>();

                    // для обновления на сервере учитываем только задачи FileTask и FileExecuteTask
                    foreach (IUpdateTask t in Tasks.Where
                        (t => !UpdateManager.Instance.IsServerMode 
                        || (t is FileTask || t is FileExecuteTask || t is VersionUpdateTask)))
                    {
                        if (t.UpdateConditions == null || t.UpdateConditions.IsMet(t))
                            // Только если все условия удовлетворены
                            tasks.Add(t);
                    }
                    // если в патче есть хоть одна задача, удовлетворяющая условиям, добавляем ее в список патчей
                    if (tasks.Count != 0)
                    {
                        p.Tasks = tasks;
                        updateFeed.UpdatesToApply.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// Подготовка патча целиком 
        /// </summary>
        /// <returns>true-успешная подготовка патча к обновлению</returns>
        public bool Prepare(IUpdateSource source)
        {
            if (UpdateManager.Instance.IsServerMode && !UpdateManager.InstallerFeed.Equals(Feed.Name))
            {
                try
                {
                    tempArchiveName = Path.Combine(UpdateManager.Instance.TempFolder, Guid.NewGuid().ToString());
                    string basePath = Path.Combine(UpdateManager.Instance.SourceBaseUri, "Global") + "\\";
                    if (!source.GetData(
                            string.Format("{0}.zip", Name),
                            basePath,
                            ref tempArchiveName, false))
                    {
                        return false;
                    }

                    Trace.TraceVerbose(string.Format(
                        "Файл {0} скопирован во временную папку {1}",
                        string.Format("{0}{1}.zip", basePath, Name),
                        tempArchiveName));
                    IsPrepared = PrepareState.PrepareSuccess;
                    return true;
                }
                catch (PreparetaskException ex)
                {
                    IsPrepared = PrepareState.PrepareWithError;
                    Trace.TraceError(String.Format("Ошибка при подготовке задачи к выполнению : {0}", ex.Message));
                    return false;
                }
            }

            bool success = true;
            foreach (var updateTask in Tasks.Where(updateTask => !updateTask.Prepare(source)))
            {
                success = false;
            }

            return success;
        }

        public ExecuteState Execute()
        {
            if (UpdateManager.Instance.IsServerMode && !UpdateManager.InstallerFeed.Equals(Feed.Name))
            {
                try
                {
                    if (IsPrepared == PrepareState.PrepareWithError)
                    {
                        Trace.TraceError(
                            String.Format(
                                "Патч {0} не выполнен, потому что для него операция подготовки завершилась неудачно",
                                Name));
                        return ExecuteState.ExecuteWithError;
                    }

                    if (IsPrepared == PrepareState.PrepareWithWarning)
                    {
                        Trace.TraceError(
                            String.Format(
                                "Патча {0} не выполнен, потому что для него операция подготовки завершилась с предупреждениями",
                                Name));
                        return ExecuteState.ExecuteWithWarning;
                    }

                    Trace.TraceInformation("Извлечение архива {0} в директорию {1}", tempArchiveName, UpdateManager.Instance.DestBaseUrl);

                    ArchiveHelper.ExtractToDir(
                        Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "ExternalApp\\7za.exe"),
                        tempArchiveName,
                        UpdateManager.Instance.DestBaseUrl);

                    return ExecuteState.ExecuteSuccess;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        String.Format("Не удалось извлечь патч из временной директории : {0}", ex.Message));
                    return ExecuteState.ExecuteWithError;
                }
            }

            ExecuteState success = ExecuteState.ExecuteSuccess;
            foreach (ExecuteState state in
                Tasks.OrderBy(t => t.OrderByFactor).Select(updateTask => updateTask.Execute()).Where(state => state == ExecuteState.ExecuteWithError || state == ExecuteState.ExecuteWithWarning))
            {
                // Если уж были ошибки, то обновление должно закончиться с ошибками
                if (success != ExecuteState.ExecuteWithError)
                    success = state;
            }

            return success;
        }

        public bool RollbackAsync(Action<bool> callback)
        {
            Trace.TraceVerbose("Асинхронный откат обновлений");
            
            Thread _updatesThread = new Thread(delegate()
            {
                try
                {
                    Rollback();
                }
                catch (Exception e)
                {
                    Trace.TraceError(String.Format("Ошибка при подготовке обновлений: {0}", e));
                    UpdateManager.Instance.State = UpdateProcessState.Error;
                    if (callback != null)
                        callback.BeginInvoke(false, null, null); /* TODO: Handle error */
                }
            }) { IsBackground = true };
            _updatesThread.Start();
            
            return true;
        }
        
        private bool Rollback()
        {
            UpdateManager.Instance.InitializeDiagnostics();

            if (!Directory.Exists(Path.Combine(Path.Combine(UpdateManager.Instance.BackupFolder,
                                             UpdateManager.Instance.GetApplicationVersion()), Name)))
            {
                Trace.TraceError(String.Format("Не найден каталог для отката обновлений: {0}",
                                               Path.Combine(Path.Combine(UpdateManager.Instance.BackupFolder,
                                                                         UpdateManager.Instance.GetApplicationVersion()),
                                                            Name)));
                throw new Exception(String.Format("Не найден каталог для отката обновлений: {0}",
                                               Path.Combine(Path.Combine(UpdateManager.Instance.BackupFolder,
                                                                         UpdateManager.Instance.GetApplicationVersion()),
                                                            Name)));
            }

            if (!Directory.Exists(UpdateManager.Instance.TempFolder))
            {
                Directory.CreateDirectory(UpdateManager.Instance.TempFolder);
            }
            UpdateManager.Instance.ExecuteOnAppRestart.Clear();

            Trace.TraceInformation(String.Format("Rollback патча {0}", Name));
            Trace.TraceVerbose(String.Format("Количество задач -  {0}", Tasks.Count));
            foreach (IUpdateTask task in Tasks)
            {
                try
                {
                    task.Rollback();
                }
                catch (RollbackException e)
                {
                    Trace.TraceError(String.Format("При откате изменений возникло исключение - {0}", e.Message));
                    UpdateManager.Instance.State = UpdateProcessState.Error;
                }
            }

            DeleteFromInstalledDocument();

            if (UpdateManager.Instance.ExecuteOnAppRestart.Count > 0)
            {
                MessageBox.Show(UpdateManager.Instance.GetMessageBoxText(true), "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                Trace.TraceInformation("Есть задачи для \"холодной\" замены.");

                Trace.TraceVerbose("Задачи для холодной замены");
                foreach (var o in UpdateManager.Instance.ExecuteOnAppRestart)
                {
                    Trace.TraceVerbose(String.Format("{0} - {1}", o.Key, o.Value));
                }

                UpdateManager.Instance.ExecuteOnAppRestart["ENV:AppPath"] =
                    Process.GetCurrentProcess().MainModule.FileName;
                UpdateManager.Instance.ExecuteOnAppRestart["ENV:OfficeAddInPath"] =
                    Path.GetDirectoryName(UpdateManager.GetProcessModule().FileName);
                UpdateManager.Instance.ExecuteOnAppRestart["ENV:TempFolder"] = UpdateManager.Instance.TempFolder;
                UpdateManager.Instance.ExecuteOnAppRestart[String.Format("ENV:BackupFolder{0}", Name)] =
                    Path.Combine(UpdateManager.Instance.BackupFolder, Name);
                UpdateManager.Instance.ExecuteOnAppRestart["ENV:RelaunchApplication"] = true;
                UpdateManager.Instance.ExecuteOnAppRestart["ENV:Rollback"] = true;
                UpdateManager.Instance.ExecuteOnAppRestart["ENV:ProcessName"] = Process.GetCurrentProcess().ProcessName;

                UpdateStarter updStarter =
                    new UpdateStarter(
                        Path.Combine(UpdateManager.Instance.TempFolder, "updaterfm.exe"),
                        UpdateManager.Instance.ExecuteOnAppRestart,
                        UpdateManager.Instance.UpdateProcessName);
                Trace.TraceVerbose("Создаем updaterfm.exe");
                updStarter.Start();

                UpdateManager.Instance.State = UpdateProcessState.WaitRestart;
                Trace.TraceVerbose("Updaterfm.exe запущен");

                UpdateManager.Instance.ClearTraceSource();
            }

            return true;
        }

        public XElement ToXml()
        {
            var patchElement = new XElement("Patch");

            patchElement.SetAttributeValue("objectKey", ObjectKey);
            patchElement.SetAttributeValue("name", Name);
            if (!String.IsNullOrEmpty(BaseUrl) && !BaseUrl.StartsWith("http"))
            {
                patchElement.SetAttributeValue(
                    "baseUrl",
                    BaseUrl);
            }
            else
            {
                if (String.IsNullOrEmpty(UpdateManager.Instance.DestBaseUrl))
                {
                    UpdateManager.Instance.DestBaseUrl = Path.GetDirectoryName(UpdateManager.Instance.ApplicationPath);
                }

                Uri path;
                if (Uri.TryCreate(
                    Path.Combine(UpdateManager.Instance.DestBaseUrl, string.Format("{0}/", BaseUrl)),
                    UriKind.Absolute,
                    out path))
                {
                    patchElement.SetAttributeValue(
                        "baseUrl",
                        path.AbsoluteUri);
                }
                else
                {
                    Trace.TraceError(
                        String.Format("Путь {0}, по которому пытаемся сохранить патч не является абсолютным Uri", path));
                    return null;
                }
            }

            patchElement.SetAttributeValue("description", Description);
            patchElement.SetAttributeValue("descriptionDetail", DescriptionDetail);
            patchElement.SetAttributeValue("use", Use.ToString().ToLower());

            if (UpdateConditions.ChildConditionsCount != 0)
            {
                patchElement.Add(UpdateConditions.ToXml());
            }

            XElement tasksElement = new XElement("Tasks");
            foreach (var updateTask in Tasks)
            {
                tasksElement.Add(updateTask.ToXml());
            }

            patchElement.Add(tasksElement);

            return patchElement;
        }

        #endregion

        #region Helper methods

        private void DeleteFromInstalledDocument()
        {
            XDocument installedUpdatesDocument =
                UpdateFeed.GetUpdatesDocument(Path.Combine(
                    UpdateManager.Instance.GetInstalledUpdateFolder(), UpdateManager.InstalledUpdatesFileName));
            DeletePatchFromFeed(installedUpdatesDocument);
            installedUpdatesDocument.Save(
                Path.Combine(UpdateManager.Instance.GetInstalledUpdateFolder(), UpdateManager.InstalledUpdatesFileNameTemp));
        }

        private void DeletePatchFromFeed(XDocument installedUpdatesDocument)
        {
            if (installedUpdatesDocument == null)
            {
                return;
            }

            XElement root = installedUpdatesDocument.Descendants("Patches").First();
            if (root != null)
            {
                if (
                    root.Descendants("Patch").Where(el => el.Attribute("objectKey").Value == ObjectKey).
                        Count() == 1)
                {
                    root.Descendants("Patch").Where(el => el.Attribute("objectKey").Value == ObjectKey).Remove();
                }
            }
        }

        #endregion
    }
}
