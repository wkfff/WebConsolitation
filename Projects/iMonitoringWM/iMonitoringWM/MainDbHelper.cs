using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Data;
using System.Xml;

using Krista.FM.Client.iMonitoringWM.Common;
using Krista.FM.Client.iMonitoringWM.Controls;

namespace iMonotoringWM
{
    /// <summary>
    /// Вспомогательный класс для работы с базой данной, 
    /// уже знающий что такое Report, и ReportsCollection
    /// </summary>
    public class MainDbHelper: DatabaseHelper
    {
        public MainDbHelper(string startupPath)
            : base(startupPath)
        {
        }

        /// <summary>
        /// Инициализировать коллекцию отчетов для пользователя
        /// </summary>
        /// <param name="user">имя пользователя</param>
        /// <returns></returns>
        public void InitUserReports(string userName, ReportCollection reports)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                using (SqlCeCommand command = base.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM {0} WHERE userName='{1}'",
                        Consts.dbUserReports, userName);
                    using (SqlCeDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader["id"].ToString();
                            string name = reader["caption"].ToString();
                            DateTime deployDate = DateTime.Parse(reader["updateDate"].ToString());
                            int position = int.Parse(reader["position"].ToString());

                            //булевского типа нет, поэтому храним бит, потом преобразуем в буль                      
                            bool subjectDepended = Convert.ToBoolean(reader["subjectDepended"].ToString());
                            bool visible = Convert.ToBoolean(reader["visible"].ToString());

                            reports.Add(new Report(id, name, deployDate, subjectDepended, visible,
                                position));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обновить пользовательские настройки в базе данных
        /// </summary>
        /// <param name="userSettings">настройки пользователя</param>
        /// <param name="serverData">данные с сервера (для пользователя)</param>
        public void UpdateUserSettings(UserSettings userSettings, XmlDocument serverData)
        {
            if ((serverData == null) || string.IsNullOrEmpty(userSettings.Name))
                return;
            UserSettings settigns = this.GetUserSettings(userSettings.Name);
            bool isUserExists = (settigns != null);

            //Сначала обновим данные пользователя
            this.UpdateUsersTable(userSettings, isUserExists);
            this.UpdateUserReportsList(userSettings.Name, isUserExists, serverData);
        }

        private void UpdateUserReportsList(string userName, bool isUserExists, XmlDocument serverData)
        {
            ReportCollection newReportList =  new ReportCollection(serverData.SelectNodes("iMonitoring/reports/report"));
            if (isUserExists)
            {
                //В новом списке отчетов проставим настройки старых
                ReportCollection oldReportList = new ReportCollection();
                this.InitUserReports(userName, oldReportList);
                foreach (Report newReport in newReportList.Items)
                {
                    foreach (Report oldReport in oldReportList.Items)
                    {
                        if (newReport.Id == oldReport.Id)
                        {
                            //возьмем позицию отчета в списке
                            newReport.Position = oldReport.Position;
                            //видимость отчета
                            newReport.Visible = oldReport.Visible;
                            break;
                        }
                    }
                }
            }
            //Отсортируем полученный список
            newReportList.SortReportsPosition();
            //Удалим старые и всатвим новые отчеты
            this.RefreshUserReports(userName, newReportList);
        }

        /// <summary>
        /// Удаляет старые и вставляет новые отчет для пользователя
        /// </summary>
        /// <param name="userName">имя пользователя</param>
        /// <param name="newReportList">новые отчеты</param>
        private void RefreshUserReports(string userName, ReportCollection newReportList)
        {
            using (SqlCeCommand command = base.Connection.CreateCommand())
            {
                SqlCeTransaction transaction = null;
                try
                {
                    //начинаем транзакцию
                    transaction = command.Connection.BeginTransaction();
                    command.Transaction = transaction;
                    
                    //удалим информацию о старых отчетах
                    command.CommandText = string.Format("DELETE FROM {0} WHERE userName='{1}'",
                        Consts.dbUserReports, userName);
                    command.ExecuteNonQuery();

                    //заносим информацию о новых
                    foreach (Report report in newReportList.Items)
                    {
                        WriteReportSettings(userName, command, report);
                    }

                    //применяем
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    //откатываем назад
                    string d = e.Message;
                    transaction.Rollback();
                }
            }
        }

        private void DeleteReportSettings(string userName, SqlCeCommand command, Report report)
        {
            //удалим информацию о старых отчетах
            command.CommandText = string.Format("DELETE FROM {0} WHERE userName='{1}' AND id='{2}'",
                Consts.dbUserReports, userName, report.Id);
            command.ExecuteNonQuery();
        }

        private void WriteReportSettings(string userName, SqlCeCommand command, Report report)
        {
            //Если такого пользователя еще нет, вставляем запись в таблицу
            string atributes = "id, caption, updateDate, position, subjectDepended, userName, visible";
            string values = string.Format("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}'",
                report.Id, report.Caption, GetFormatedDateString(report.DeployDate), report.Position,
                Convert.ToInt16(report.IsSubjectDependent), userName,
                Convert.ToInt16(report.Visible));

            string sqlQuery = string.Format("INSERT INTO {0}({1}) VALUES({2})",
                Consts.dbUserReports, atributes, values);
            command.CommandText = sqlQuery;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Сохранить в базе данных все настройки отчета, для данного пользователя
        /// </summary>
        /// <param name="userName">пользователь</param>
        /// <param name="report">отчет, настройки которого будем сохранять</param>
        public void WriteReportSettings(string userName, Report report)
        {
            using (SqlCeCommand command = base.Connection.CreateCommand())
            {
                SqlCeTransaction transaction = null;
                try
                {
                    //начинаем транзакцию
                    transaction = command.Connection.BeginTransaction();
                    command.Transaction = transaction;

                    this.DeleteReportSettings(userName, command, report);
                    this.WriteReportSettings(userName, command, report);

                    //применяем
                    transaction.Commit();
                }
                catch
                {
                    //откатываем назад
                    transaction.Rollback();
                }
            }
        }

        #region Дата загрузки отчета
        /// <summary>
        /// Записать в БД информацию о загрузке отчета в кэш
        /// </summary>
        /// <param name="reportView">отчет</param>
        public void SetReportDownloadDate(ReportView reportView)
        {
            using (SqlCeCommand command = base.Connection.CreateCommand())
            {
                string compositeId = reportView.GetCompositeId(true);
                DateTime downloadDate = reportView.DownloadDate;

                //сначала удалим старую запись
                command.CommandText = string.Format("DELETE FROM {0} WHERE id='{1}'",
                    Consts.dbReportsDownloadDate, compositeId);
                command.ExecuteNonQuery();

                //теперь вставляем текущую
                command.CommandText = string.Format("INSERT INTO {0} (id, date) VALUES('{1}', '{2}')",
                    Consts.dbReportsDownloadDate, compositeId, GetFormatedDateString(downloadDate));
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Вернет дату загрузки отчета
        /// </summary>
        /// <param name="reportView">отчет</param>
        /// <returns>дата загрузки</returns>
        public DateTime GetReportDownloadDate(ReportView reportView)
        {
            DateTime result = DateTime.MinValue;

            using (SqlCeCommand command = base.Connection.CreateCommand())
            {
                string compositeId = reportView.GetCompositeId(true);

                command.CommandText = string.Format("SELECT date FROM {0} WHERE id='{1}'",
                    Consts.dbReportsDownloadDate, compositeId);
                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = DateTime.Parse(reader["date"].ToString());
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
