using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Krista.FM.Utils
{
    public class MSBuildPublishSiteTask : Task
    {
        private ITaskItem solutionName;
        private ITaskItem[] publishingProjectsNames;
                
        [Required]
        public ITaskItem SolutionName
        {
            get { return solutionName; }
            set { solutionName = value; }
        }

        [Required, Output]
        public ITaskItem[] PublishingProjectsNames
        {
            get { return publishingProjectsNames; }
            set { publishingProjectsNames = value; }
        }

        public override bool Execute()
        {
            if (SolutionName != null)
            {
                string sln = ReadSolutionFile();
                MatchCollection projects = GetProjectsList(sln);
                FillPublishingProjects(projects);
            }

            // Сообщаем, что задание отработало успешно.
            return true;
        }

        private void FillPublishingProjects(MatchCollection projects)
        {
            PublishingProjectsNames = new ITaskItem[projects.Count];
            for (int i = 0; i < projects.Count; i++)
            {
                PublishingProjectsNames[i] = new TaskItem(projects[i].Captures[0].Value);
            }
        }

        private static MatchCollection GetProjectsList(string sln)
        {
            Regex regex = new Regex("Dashboards\\\\Krista.FM.Server.Dashboards\\\\[^(\")]*?.csproj");
            return regex.Matches(sln);
        }

        private string ReadSolutionFile()
        {
            FileStream fs = new FileStream(SolutionName.ItemSpec, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string sln = sr.ReadToEnd();
            sr.Close();
            return sln;
        }
    }
}
