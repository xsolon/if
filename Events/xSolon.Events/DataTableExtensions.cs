using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace xSolon.Events
{
    public static partial class SerializationExtensions
    {
        public static DataTable ListToTable(this List<Dictionary<string, string>> dic)
        {
            var dt = new DataTable("Results");

            dic.ForEach(props =>
            {
                props.ToList().ForEach(i =>
                {
                    if (!dt.Columns.Contains(i.Key))
                    {
                        dt.Columns.Add(new DataColumn(i.Key));
                    }
                });

                var dr = dt.NewRow();

                props.ToList().ForEach(i => dr[i.Key] = i.Value);

                dt.Rows.Add(dr);
            });

            dt.AcceptChanges();

            return dt;
        }

        public static string ToCSV(this DataTable table)
        {
            var result = new StringBuilder();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(string.Concat("\"", table.Columns[i].ColumnName, "\""));

                result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    var value = "";

                    if (row[i] != null)
                        value = row[i].ToString().Replace("\"", "\"\"");

                    result.Append(string.Concat("\"", value, "\""));

                    result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
                }
            }

            return result.ToString();
        }

        public static List<Dictionary<string, string>> TableToList(this DataTable table)
        {
            var dic = new List<Dictionary<string, string>>();

            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    var list1 = new Dictionary<string, string>();

                    foreach (DataColumn col in table.Columns)
                    {
                        var value = string.Empty;

                        if (!row.IsNull(col))
                        {
                            value = row[col].ToString();
                        }

                        list1.Add(col.ColumnName, value);
                    }

                    dic.Add(list1);
                }
            }

            return dic;
        }

        public static DataTable RemoveDuplicates(this DataTable table, string columnName)
        {
            var urls = new List<string>();

            var rowsToRemove = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                var url = row[columnName].ToString();

                if (urls.Contains(url))
                {
                    rowsToRemove.Add(row);
                }
                else
                {
                    urls.Add(url);
                }
            }

            rowsToRemove.ForEach(i => table.Rows.Remove(i));

            table.AcceptChanges();

            return table;
        }

        public static DataTable Sort(this DataTable table, string dir, DataColumn sortRow)
        {
            var newTable = table.Clone();

            EnumerableRowCollection<DataRow> query = null;

            #region Get Query

            if (sortRow.DataType == typeof(String))
            {
                query = from order in table.AsEnumerable()
                        orderby order.Field<String>(sortRow.ColumnName)
                        select order;
            }
            else if (sortRow.DataType == typeof(int))
            {
                query = from order in table.AsEnumerable()
                        orderby order.Field<int>(sortRow.ColumnName)
                        select order;
            }
            else if (sortRow.DataType == typeof(DateTime))
            {
                query = from order in table.AsEnumerable()
                        orderby order.Field<DateTime>(sortRow.ColumnName)
                        select order;
            }

            #endregion

            query.ToList().ForEach(i =>
            {
                newTable.ImportRow(i);
            });

            newTable.AcceptChanges();

            return newTable;
        }

        public static DataTable DefineDataTable(this object o, string[] headers)
        {
            var dt = new System.Data.DataTable("row");

            dt.RemotingFormat = SerializationFormat.Xml;

            bool cont = true;

            int index = 1;

            foreach (string header in headers)
            {
                var dc = new DataColumn(header);

                dc.ColumnMapping = MappingType.Attribute;

                dt.Columns.Add(dc);
            }

            dt.AcceptChanges();

            return dt;
        }
    }
}