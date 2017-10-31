using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Data;
using System.Xml;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    /// <summary>
    /// Вспомогательный класс для работы с базой данной
    /// </summary>
    public class DatabaseHelper : IDisposable
    {
        private SqlCeConnection _connection;

        protected SqlCeConnection Connection
        {
            get { return _connection; }
        }

        public DatabaseHelper(string startupPath)
        {
            //Небольшая хитрость, надо что бы при обновлении программы, сохранялись настройки пользователя,
            //даты обновления отчетов, писать все в реестр, не вариант, т.к. много инфы, да и работает он 
            //медленней чем БД. При инсталяции программы имя бд пишется с подчеркиванием iMonDatabase_.sdf
            //при запуске программы оно переименовывается в такое же только без подчеривание, и все остальное
            //время работает уже с ней. Нужно для того что бы при удалении программы инсталятор не удалял БД,
            //а оставлял ее для следующей версии. При переходе от версии к версии, при необходимости 
            //добавления новых таблиц, полей, будем в апдейтере создавать их скриптами.
            string dataBasePath = System.IO.Path.Combine(startupPath, Consts.dbName + ".sdf");
            string tempDdataBasePath = System.IO.Path.Combine(startupPath, Consts.dbTempName + ".sdf");
            //переименуем временную БД в постоянную
            if (!System.IO.File.Exists(dataBasePath))
                System.IO.File.Move(tempDdataBasePath, dataBasePath);
            //удалим временную БД
            if (System.IO.File.Exists(tempDdataBasePath))
                System.IO.File.Delete(tempDdataBasePath);

            string connectionString = string.Format(@"Data Source = {0}", dataBasePath);
            this._connection = new SqlCeConnection(connectionString);
            this.Connection.Open();
        }

        /// <summary>
        /// Получить список субъектов РФ
        /// </summary>
        /// <returns>список субъектов</returns>
        public List<string> GetEntityList()
        {
            List<string> result = new List<string>();
            using (SqlCeCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT name FROM {0}", Consts.dbEntityList);
                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        result.Add(name);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Извлечь из базы данных настройки пользователя
        /// </summary>
        /// <param name="user">имя пользователя</param>
        /// <returns>настройки пользователя</returns>
        public UserSettings GetUserSettings(string user)
        {
            UserSettings result = null;
            if (!string.IsNullOrEmpty(user))
            {
                using (SqlCeCommand command = this.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM {0} WHERE name='{1}'",
                        Consts.dbUsersTable, user);
                    using (SqlCeDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            string password = reader["password"].ToString();
                            DateTime lastConnection = DateTime.Parse(reader["lastConnection"].ToString());
                            int entityIndex = int.Parse(reader["entityIndex"].ToString());

                            result = new UserSettings();
                            result.Name = name;
                            result.Password = password;
                            result.EntityIndex = entityIndex;
                            result.LastConnection = lastConnection;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Сохраним настройки пользователя
        /// </summary>
        /// <param name="userSettings"></param>
        public void SetUserSettings(UserSettings userSettings)
        {
            string sqlQuery = string.Format("UPDATE {0} SET entityIndex='{1}', password='{2}', lastConnection='{3}' WHERE name='{4}'",
                    Consts.dbUsersTable, userSettings.EntityIndex, userSettings.Password, 
                    GetFormatedDateString(userSettings.LastConnection), userSettings.Name);
            //Выполним запрос
            using (SqlCeCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Получить из базы данных выбранный пользователем субъект
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int GetUserSubjectIndex(string user)
        {
            UserSettings settings = this.GetUserSettings(user);
            return (settings == null) ? 1 : settings.EntityIndex;
        }

        protected void UpdateUsersTable(UserSettings userSettings, bool isUserExists)
        {
            string sqlQuery = string.Empty;

            userSettings.LastConnection = DateTime.Now;

            if (isUserExists)
            {
                //Пользователь уже есть, надо обновить информацию для него
                sqlQuery = string.Format("UPDATE {0} SET lastConnection='{1}', password='{2}' WHERE name='{3}'",
                    Consts.dbUsersTable, GetFormatedDateString(userSettings.LastConnection), userSettings.Password, userSettings.Name);
            }
            else
            {
                //Если такого пользователя еще нет, вставляем запись в таблицу
                string atributes = "name, password, entityIndex, lastConnection";
                string values = string.Format("'{0}', '{1}', '{2}', '{3}'", userSettings.Name, userSettings.Password,
                    userSettings.EntityIndex, GetFormatedDateString(userSettings.LastConnection));
                sqlQuery = string.Format("INSERT INTO {0}({1}) VALUES ({2})",
                    Consts.dbUsersTable, atributes, values);
            }

            //Выполним запрос
            using (SqlCeCommand command = this.Connection.CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.ExecuteNonQuery();
            }
        }

        #region Методы позволяющие сохранять и читать настройки приложения из БД
        /// <summary>
        /// Получить булевское значение из БД, по ключу
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="defaultValue">значение по умолчанию</param>
        /// <returns></returns>
        public bool GetBoolValue(string key, bool defaultValue)
        {
            string result = this.GetStrValue(key, defaultValue.ToString());
            return bool.Parse(result);
        }

        /// <summary>
        /// Получить значение из БД, по ключу
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="defaultValue">значение по умолчанию</param>
        /// <returns></returns>
        public string GetStrValue(string key, string defaultValue)
        {
            string result = defaultValue;
            if (!string.IsNullOrEmpty(key) && (this.Connection != null))
            {
                using (SqlCeCommand command = this.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT value FROM {0} WHERE id='{1}'", 
                        Consts.dbAppSettings, key);
                    using (SqlCeDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader["value"].ToString();
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Сохранить значение в БД
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="value">значение</param>
        public void SetStrValue(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && (this.Connection != null))
            {
                using (SqlCeCommand command = this.Connection.CreateCommand())
                {
                    //Сначала удалим старое значение
                    command.CommandText = string.Format("DELETE FROM {0} WHERE id='{1}'",
                        Consts.dbAppSettings, key);
                    command.ExecuteNonQuery();

                    //Запишем новое
                    command.CommandText = string.Format("INSERT INTO {0} (id, value) VALUES('{1}', '{2}')",
                        Consts.dbAppSettings, key, value);
                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        protected static string GetFormatedDateString(DateTime date)
        {
            return date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Connection.Close();
            this._connection = null;
        }

        #endregion
    }
}
