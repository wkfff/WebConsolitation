using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.Test.HashCode
{
    internal class ClassifierHashCode
    {
        private static int CalcHashCode(params object[] prms)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object value in prms)
            {
                sb.Append(value);
            }
            return sb.ToString().GetHashCode();
        }

        public static DataTable GetHashTable(IClassifier classifier, List<string> attributes, out DataTable collisionsTable)
        {
            collisionsTable = null;
            try
            {
                SchemeEditor.Instance.Operation.StartOperation();
                SchemeEditor.Instance.Operation.Text = "Загрузка данных...";

                List<string> attributeNames = new List<string>();
                //attributeNames.Add("RowType");
                attributeNames.AddRange(attributes);

                bool isDivided = classifier.IsDivided;

                IDataUpdater du = classifier.GetDataUpdater();
                Dictionary<int, string> dataSources = null;

                if (isDivided)
                    dataSources = classifier.GetDataSourcesNames();

                DataTable dt = new DataTable();
                du.Fill(ref dt);


                Dictionary<String, DataRow> hash = new Dictionary<String, DataRow>(dt.Rows.Count);

                DataTable hashTable = new DataTable();
                hashTable.Columns.Add(new DataColumn("HashCode", typeof(String)));
                hashTable.Columns.Add(new DataColumn("Key", typeof(string)));
                hashTable.Columns.Add(new DataColumn("Collisions", typeof(int)));

                collisionsTable = new DataTable();
                collisionsTable.Columns.Add(new DataColumn("HashCode", typeof(String)));
                collisionsTable.Columns.Add(new DataColumn("Key", typeof(string)));

                int total = dt.Rows.Count;
                int current = 0;
                int duplicates = 0;
                int collisions = 0;
                int missingSources = 0;

                foreach (DataRow row in dt.Rows)
                {
                    StringBuilder sb = new StringBuilder();
                    int dsHashCode = 0;
                    if (isDivided)
                    {
                        int id = Convert.ToInt32(row["SourceID"]);
                        if (dataSources.ContainsKey(id))
                        {
                            dsHashCode = dataSources[id].GetHashCode();
                            //sb.Append(dataSources[id]);
                        }
                        else
                        {
                            continue;
                            sb.Append(id);
                            missingSources++;
                        }
                    }
                    foreach (string attributeName in attributeNames)
                    {
                        sb.Append(row[attributeName]);
                    }
                    string key = sb.ToString();

                    int hashCode = key.GetHashCode();

                    String fullHashCode = String.Format("[{0}][{1}]", dsHashCode, hashCode);

                    DataRow r = hashTable.NewRow();
                    r[0] = fullHashCode; // hashCode;
                    r[1] = key;
                    r[2] = 0;

                    //DataRow[] findRow = hashTable.Select(String.Format("HashCode = {0}", hashCode));
                    //if (findRow.GetLength(0) > 0)
                    if (hash.ContainsKey(fullHashCode))
                    {
                        //int cl = findRow.GetLength(0);
                        //foreach (DataRow item in findRow)
                        //{
                        //    item[2] = cl;
                        //}
                        // ФО\0001 АС Бюджет - Калмыкия 2006022209000Большеболдинский
                        // ФО\0001 АС Бюджет - Калмыкия 2006036212812Гвардейский
                        // -------------------------------------------------------
                        // ФО\0001 АС Бюджет - Надым 2004053203832Николькинский
                        // ФО\0001 АС Бюджет - Калмыкия 200608255816Елабужская
                        // -------------------------------------------------------
                        // ФО\0001 АС Бюджет - Надым 2004053241843Спасский
                        // ФО\0001 АС Бюджет - Калмыкия 200604226802Балахтонский
                        // -------------------------------------------------------
                        // ФО\0001 АС Бюджет - Тверь 200700ОАО Банк "Петрокоммерц"
                        // ФО\0001 АС Бюджет - Тверь 200700ОАО Банк "Петрокоммерц"
                        if (Convert.ToString(hash[fullHashCode][1]) != key)
                        {
                            int collisionsCount = Convert.ToInt32(hash[fullHashCode][2]);
                            if (collisionsCount == 0)
                            {
                                DataRow firstCR = collisionsTable.NewRow();
                                firstCR[0] = fullHashCode;
                                firstCR[1] = hash[fullHashCode][1];
                                collisionsTable.Rows.Add(firstCR);
                            }
                            hash[fullHashCode][2] = collisionsCount + 1;
                            collisions++;
                            
                            DataRow cr = collisionsTable.NewRow();
                            cr[0] = fullHashCode;
                            cr[1] = key;
                            collisionsTable.Rows.Add(cr);
                        }
                        else
                            duplicates++;
                    }
                    else
                        hash.Add(fullHashCode, r);

                    //hashTable.Rows.Add(r);

                    current++;

                    if ((current % 10) == 0)
                    {
                        SchemeEditor.Instance.Operation.Text =
                            String.Format("Обработано {0} из {1} ({2}%) Коллизий {3}({4}) ", current, total, Convert.ToInt32(((float)current / (float)total) * 100), collisions, duplicates);
                    }
                }

                SchemeEditor.Instance.Operation.Text = "Сохранение данных...";
                foreach (DataRow row in hash.Values)
                {
                    hashTable.Rows.Add(row);
                }

                if (collisions > 0 || missingSources > 0 || duplicates > 0)
                    Services.MessageService.ShowWarning(String.Format("Обнаружено {0} коллизий,\n{1} неизвестных источников,\n {2} неуникальных записей.", collisions, missingSources, duplicates));

                return hashTable;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                SchemeEditor.Instance.Operation.StopOperation();
            }
        }

        public static void ShowHashTable(IClassifier classifier)
        {
            List<string> attributes = new List<string>();
            if (classifier.Attributes.ContainsKey("Code"))
                attributes.Add("Code");
            if (classifier.Attributes.ContainsKey("CodeStr"))
                attributes.Add("CodeStr");
            if (classifier.Attributes.ContainsKey("Name"))
                attributes.Add("Name");

            DataTable collisionsTable;
            DataTable dt = GetHashTable(classifier, attributes, out collisionsTable);

            HashCodeRowsGridForm form = new HashCodeRowsGridForm();
            form.Table = dt;
            form.CollisionsTable = collisionsTable;
            SchemeEditor.Instance.ShowDialog(form);
        }
    }
}
