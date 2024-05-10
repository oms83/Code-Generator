using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorDataAccess
{
    public partial class clsRow
    {
        public string ColumnName { get; set; }

        public string Type { get; set; }

        public bool AllowNull { get; set; }

        public bool IsPrimaryKey { get; set; }

        private clsRow(string ColumnName, string Type, bool AllowNull, bool IsPrimaryKey)
        {
            this.ColumnName = ColumnName;
            this.Type = Type;
            this.AllowNull = AllowNull;
            this.IsPrimaryKey = IsPrimaryKey;
        }

        public clsRow()
        {

            this.ColumnName = string.Empty;
            this.Type = "";
            this.AllowNull = false;
            this.IsPrimaryKey = false;
        }

        private static clsRow _GetRowInfo(DataRow row)
        {
            clsRow ObjOfRow = new clsRow();
            ObjOfRow.ColumnName = (string)row["COLUMN_NAME"];
            ObjOfRow.Type = clsUtility.ConvertDbTypeToCSharpType((string)row["DATA_TYPE"]);
            ObjOfRow.AllowNull = ((string)row["Nullability"] == "Null" ? true : false);
            ObjOfRow.IsPrimaryKey = ((string)row["KeyType"] == "PK" ? true : false);
            return ObjOfRow;
        }

        public static List<clsRow> GetAllRows(DataTable dtRows)
        {
            List<clsRow> AllRows = new List<clsRow>();

            foreach (DataRow row in dtRows.Rows)
            {
                AllRows.Add(_GetRowInfo((DataRow)row));
            }
            
            return AllRows;
        }

    }
}
