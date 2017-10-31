using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    public static class MessageUIUtils
    {
        public static string Indent(Graphics g, int count, Font font)
        {
            float si = GetAvgCharSizeF(g, font).Width;
            int _f = (int) Math.Round(count / si);
            int countSpace = _f > 1 ? _f : 1;
            string s = string.Empty;
            countSpace = (countSpace > 1) ? --countSpace : countSpace;
            for (int i = 0; i < countSpace; i++)
            {
                s += " ";
            }

            return s;
        }

        public static int GetStringSize(string s, Font f, Graphics g)
        {
            SizeF textSize = g.MeasureString(s, f);
            return (int)textSize.Width;
        }

        public static SizeF GetAvgCharSizeF(Graphics g, Font font)
        {
            String s = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            SizeF textSize = g.MeasureString(s, font);

            float baseUnitX = (textSize.Width / s.Length);
            float baseUnitY = textSize.Height;

            return new SizeF(baseUnitX, baseUnitY);
        }

        public static DataTable GetDifferenceTable(DataTable firstTable, DataTable secondTable, List<string> excludeColumns)
        {
            if (firstTable == null || secondTable == null)
            {
                throw new GetDifferenceTableException("Одна из таблиц для сравнения null");
            }

            try
            {
                DataTable table = new DataTable("GetDifferenceTable");

                using (DataSet ds = new DataSet())
                {
                    ds.Tables.AddRange(new[] { firstTable.Copy(), secondTable.Copy() });

                    DataColumn[] firstcolumns = new DataColumn[ds.Tables[0].Columns.Count - excludeColumns.Count];
                    int j = 0;
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        if (!excludeColumns.Contains(ds.Tables[0].Columns[i].ColumnName))
                        {
                            firstcolumns[j] = ds.Tables[0].Columns[i];
                            j++;
                        }
                    }

                    DataColumn[] secondcolumns = new DataColumn[ds.Tables[1].Columns.Count - excludeColumns.Count];

                    j = 0;
                    for (int i = 0; i < ds.Tables[1].Columns.Count; i++)
                    {
                        if (!excludeColumns.Contains(ds.Tables[1].Columns[i].ColumnName))
                        {
                            secondcolumns[j] = ds.Tables[1].Columns[i];
                            j++;
                        }
                    }

                    DataRelation r1 = new DataRelation("r1", firstcolumns, secondcolumns, false);
                    ds.Relations.Add(r1);
                    DataRelation r2 = new DataRelation("r2", secondcolumns, firstcolumns, false);
                    ds.Relations.Add(r2);

                    for (int i = 0; i < firstTable.Columns.Count; i++)
                    {
                        table.Columns.Add(firstTable.Columns[i].ColumnName, firstTable.Columns[i].DataType);
                    }

                    table.BeginLoadData();

                    foreach (DataRow parentrow in ds.Tables[0].Rows)
                    {
                        DataRow[] childrows = parentrow.GetChildRows(r1);

                        if (childrows.Length == 0)
                        {
                            table.LoadDataRow(parentrow.ItemArray, true);
                        }
                    }

                    foreach (DataRow parentrow in ds.Tables[1].Rows)
                    {
                        DataRow[] childrows = parentrow.GetChildRows(r2);

                        if (childrows.Length == 0)
                        {
                            table.LoadDataRow(parentrow.ItemArray, true);
                        }
                    }

                    table.EndLoadData();
                }

                return table;
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("При получении различий у таблицы {0} и {1} возникло исключение - {2}",
                                               firstTable.TableName, secondTable.TableName, e.Message));
                throw new GetDifferenceTableException(e.Message);
            }
        }
    }
}
