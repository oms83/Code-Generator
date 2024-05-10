using CodeGeneratorDataAccess;
using System.Collections.Generic;
using System.Data;

namespace CodeGeneratorBusiness
{
    public partial class clsCodeGenerator
    {
        public static DataTable GetAllDatabases()
        {
            return clsCodeGeneratorData.GetAllDatabases();
        }

        public static DataTable GetAllColumns(string DBName, string TableName)
        {
            return clsCodeGeneratorData.GetAllColumns(DBName, TableName);
        }

        public static DataTable GetAllTables(string DBName)
        {
            return clsCodeGeneratorData.GetAllTables(DBName);
        }

        public static List<clsRow> GetSplittedRowsByList(string DBName, string TableName)
        {
            return clsRow.GetAllRows(clsCodeGeneratorData.GetAllColumns(DBName, TableName));
        }

        public static Dictionary<string, List<clsRow>> GetSplittedRowsOfAllTablesByDictionary(string DBName)
        {
            DataTable dtTables = GetAllTables(DBName);

            Dictionary<string, List<clsRow>> TablesRows = new Dictionary<string, List<clsRow>>();
            
            foreach (DataRow table in dtTables.Rows)
            {
                TablesRows.Add((string)table["TABLE_NAME"], GetSplittedRowsByList(DBName, (string)table["TABLE_NAME"]));
            }

            return TablesRows;
        }

        public static bool ChangeKeyType(ref Dictionary<string, List<clsRow>> AllTables, string TableName, string ColumnName, bool IsPrimary)
        {
            List<clsRow> Rows = AllTables[TableName];

            foreach (clsRow Obj in Rows)
            {
                if (Obj.ColumnName == ColumnName)
                {
                    Obj.IsPrimaryKey = IsPrimary;
                    return true;
                }
            }

            return false;
        }

        public static clsRow FindPrimaryKey(Dictionary<string, List<clsRow>> TablesRows, string TableName)
        {
            List<clsRow> RowsOfTable = TablesRows[TableName];

            foreach (clsRow Obj in RowsOfTable)
            {
                if (Obj.IsPrimaryKey)
                {
                    return Obj;
                }
            }

            return null;
        }

        public static clsRow FindPrimaryKey(List<clsRow> RowsOfTable)
        {
            foreach (clsRow Obj in RowsOfTable)
            {
                if (Obj.IsPrimaryKey)
                {
                    return Obj;
                }
            }

            return null;
        }

        public static DataTable ConvertListToDataTable(List<clsRow> tableInfo)
        {
            DataTable Table = new DataTable();

            Table.Columns.Add("Column_Name", typeof(string));
            Table.Columns.Add("Data_Type", typeof(string));

            Table.Columns.Add("Primary_Key", typeof(bool));
            Table.Columns.Add("Nullability", typeof(bool));

            for (int i = 0; i < tableInfo.Count; i++)
            {
                Table.Rows.Add(tableInfo[i].ColumnName, tableInfo[i].Type, tableInfo[i].IsPrimaryKey, tableInfo[i].AllowNull);
            }
            return Table;
        }

        public static DataTable GetTableInfoByDataTable(string DBName, string TableName)
        {
            return ConvertListToDataTable(GetSplittedRowsByList(DBName, TableName));
        }
    }
}
