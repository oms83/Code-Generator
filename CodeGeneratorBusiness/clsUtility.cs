using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorDataAccess
{
    public partial class clsUtility
    {
        public static string ConvertDbTypeToCSharpType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "nvarchar":
                case "varchar":
                case "char":
                case "text":
                case "ntext":
                case "longtext":
                case "mediumtext":
                case "tinytext":
                    return "string";

                case "decimal":
                case "money":
                case "smallmoney":
                    return "decimal";

                case "int":
                case "integer":
                case "mediumint":
                    return "int";

                case "tinyint":
                    return "byte";

                case "smallint":
                    return "short";

                case "bit":
                case "boolean":
                    return "bool";

                case "float":
                case "real":
                    return "float";

                case "double":
                case "double precision":
                case "numeric":
                    return "double";

                case "date":
                case "datetime":
                case "timestamp":
                    return "DateTime";

                case "time":
                    return "TimeSpan";

                case "year":
                    return "int"; // Year in four-digit format

                case "binary":
                case "varbinary":
                case "blob":
                case "longblob":
                case "mediumblob":
                case "tinyblob":
                    return "byte[]";

                default:
                    return "object";
            }
        }
    }
}
