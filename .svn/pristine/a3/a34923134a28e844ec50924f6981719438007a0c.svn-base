using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Krista.FM.Update.Framework.UpdateObjects
{
    /// <summary>
    /// Канал обновления
    /// </summary>
    public class UpdateFeed : IUpdateFeed
    {
        public UpdateFeed(string name, string url)
        {
            UpdatesToApply = new List<IUpdatePatch>();
            Name = name;
            Url = url;
            ObjectKey = Guid.NewGuid().ToString();

            IsBase = false;
        }

        /// <summary>
        /// Уникальный идентификатор канала
        /// </summary>
        public string ObjectKey { get; set; }
        /// <summary>
        /// Каталог канала обновления
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Имя канала обновления
        /// </summary>
        public string Name { get; set; }
        public IList<IUpdatePatch> Patches { get; set; }
        public IList<IUpdatePatch> UpdatesToApply { get; set; }
        public bool IsBase { get; set; }

        public void Save()
        {
            if (IsBase)
            {
                SaveBaseFeed();
                return;
            }

            string feedName = Path.Combine(UpdateManager.Instance.DestBaseUrl, Path.Combine(Url, Name));

            // Создание каталога
            CreateDirectory(feedName);

            XDocument doc = GetUpdatesDocument(feedName);

            try
            {
                foreach (var updatePatch in Patches)
                {
                    SavePatchToFeed(doc, updatePatch);
                }

                doc.Save(feedName);
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("При сохранении канала возникло исключение - {0}", e.Message));
            }
        }

        private void SaveBaseFeed()
        {
            try
            {
                string feedPath = Path.Combine(UpdateManager.Instance.SourceBaseUri, Path.Combine(Url, Name));

                // Создание каталога
                CreateDirectory(feedPath);

                XDocument doc = XDocument.Parse(UpdateManager.Instance.UpdateSource.GetUpdatesFeed(feedPath));
                doc.Save(Path.Combine(UpdateManager.Instance.DestBaseUrl, Name));
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("При сохранении канала возникло исключиние - {0}", e.Message));
            }
        }

        private static void CreateDirectory(string feedPath)
        {
            string folderName = Path.GetDirectoryName(feedPath);
            var directoryInfo = new DirectoryInfo(folderName);
            directoryInfo.CreateDirectory();
        }

        public static void SavePatchToFeed(XDocument feedUpdatesDocument, IUpdatePatch updatePatch)
        {
            try
            {
                if (feedUpdatesDocument == null)
                {
                    return;
                }

                XElement root = feedUpdatesDocument.Descendants("Patches").First();
                if (root != null)
                {
                    if (
                        root.Descendants("Patch").Where(el => el.Attribute("objectKey").Value == updatePatch.ObjectKey).
                            Count() == 0)
                    {
                        XElement patch = updatePatch.ToXml();
                        root.Add(patch);
                    }
                    else
                    {
                        UpdatePatch(root, updatePatch);
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                Trace.TraceError(String.Format("При сохранении патча {0} возникло исключение - {1}", updatePatch.Name, e.Message));
            }
            catch (InvalidOperationException e)
            {
                Trace.TraceError(String.Format("При сохранении патча {0} возникло исключение - {1}", updatePatch.Name, e.Message));
            }
            catch (OverflowException e)
            {
                Trace.TraceError(String.Format("При сохранении патча {0} возникло исключение - {1}", updatePatch.Name, e.Message));
            }
        }

        /// <summary>
        /// Обновление патча, пока разрешено изменить только обязательность патча
        /// </summary>
        /// <param name="root"></param>
        /// <param name="updatePatch"></param>
        public static void UpdatePatch(XElement root, IUpdatePatch updatePatch)
        {
            XElement patchNode =
                root.Descendants("Patch").Where(el => el.Attribute("objectKey").Value == updatePatch.ObjectKey).First();
            if (patchNode != null)
            {
                if (patchNode.Attribute("use").Value.ToLower() != updatePatch.Use.ToString().ToLower())
                {
                    Trace.TraceVerbose(String.Format("Обязательность патча сменилась c {0} на {1}",
                                               patchNode.Attribute("use").Value, updatePatch.Use));
                    patchNode.Attribute("use").Value = updatePatch.Use.ToString();
                }
            }
        }

        public static XDocument GetUpdatesDocument(string path)
        {
            XDocument document = !File.Exists(path)
                                     ? XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?><Patches></Patches>")
                                     : XDocument.Load(path);

            return document;
        }
    }
}
