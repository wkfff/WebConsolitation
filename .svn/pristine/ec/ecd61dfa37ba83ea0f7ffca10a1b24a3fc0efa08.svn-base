using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Configuration.Install;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert;
using Krista.FM.Common.RegistryUtils;


namespace InstallTool
{
    [RunInstaller(true)]
    public class ExpertSetupAction : Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string targetDirectory = Context.Parameters["targetdir"];
            string sourceDirectory = Context.Parameters["sourcedir"];
            string mapsDirectory = sourceDirectory + Consts.mapsFolderName;
            
            if(mapsDirectory.Length > 0)
            {
                if ((mapsDirectory[0] == '\\') && (!mapsDirectory.StartsWith("\\\\")))
                    mapsDirectory = "\\" + mapsDirectory;
            }


            if (Directory.Exists(mapsDirectory))
            {
                CommonUtils.CopyDirectory(mapsDirectory, targetDirectory + Consts.mapsFolderName);
            }
            else
                MessageBox.Show("Каталог с шаблонами карт не найден",
                                "MDX Expert", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!HasMapRepositoryPath())
            {
                //прописываем в реестре путь к каталогу с картами
                Utils regUtils = new Utils(typeof (MapReportElement), true);
                regUtils.SetKeyValue(Consts.mapRepositoryPathRegKey, targetDirectory + Consts.mapsFolderName);
            }
        }

        /// <summary>
        /// Имеется ли уже в реестре путь к шаблонам карт
        /// </summary>
        /// <returns></returns>
        private bool HasMapRepositoryPath()
        {
            Utils regUtils = new Utils(typeof (MapReportElement), true);
            string repositoryPath = regUtils.GetKeyValue(Consts.mapRepositoryPathRegKey);
            return Directory.Exists(repositoryPath);
        }

        public override void Uninstall(System.Collections.IDictionary stateSaver)
        {
            base.Uninstall(stateSaver);
            string targetDirectory = Context.Parameters["targetdir"];
            string mapsDirectory = targetDirectory + Consts.mapsFolderName;

            if (!Directory.Exists(mapsDirectory))
            {
                return;
            }

            if (MessageBox.Show("Удалить каталог с шаблонами карт из папки приложения?",
                 "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Directory.Delete(mapsDirectory, true);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

    }

}
