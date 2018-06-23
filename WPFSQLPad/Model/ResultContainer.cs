using System.Collections.Generic;
using System.Data;
using System.Data.Common;

//https://github.com/JaCraig/SQLParser/blob/master/LICENSE

namespace Model
{
    /// <summary>
    /// Structure of data returned by
    /// SELECT-like statement.
    /// </summary>
    ///
    /// todo add transform(int row) returning column
    public struct ResultContainer
    {
        public List<DbColumn> Columns;
        public List<string[]> Data;

        public string Name { get; }
        public string SourceDatabase { get; }

        //shorthands
        public string[] FirstRow => Data[0];
        public string FirstResult => Data[0][0];

        public ResultContainer(string name, string sourceDatabase, List<DbColumn> columns, List<string[]> data)
        {
            Name = name;
            SourceDatabase = sourceDatabase;
            Columns = columns;
            Data = data;
        }

        //fill DataTable
        public DataTable ToDataTable()
        {
            //the Name and SourceDatabase are here for XML (latter serves as namespace)
            DataTable data = new DataTable(Name, SourceDatabase);

            //add columns
            DataColumn[] columns = GetDataColumns();
            data.Columns.AddRange(columns);

            //add rows
            var row = new object[Columns.Count];
            foreach (var rowData in Data)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    row[j] = rowData[j];
                }
                data.Rows.Add(row);
            }
            return data;
        }

        //get data columns for DataTable
        private DataColumn[] GetDataColumns()
        {
            DataColumn[] columns = new DataColumn[Columns.Count];

            //protect from duplicated headers - DataTable doesn't like that
            Dictionary<string, int> names = new Dictionary<string, int>();

            for (int i = 0; i < Columns.Count; i++)
            {
                string name = Columns[i].ColumnName;

                if (names.ContainsKey(name))
                {
                    names[name]++;
                    columns[i] = new DataColumn($"{Columns[i].ColumnName}_{names[name]}");
                }
                else
                {
                    columns[i] = new DataColumn(Columns[i].ColumnName);
                    names.Add(name, 1);
                }
            }

            return columns;
        }

    }

}
