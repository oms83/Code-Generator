using CodeGeneratorDataAccess;
using System.Collections.Generic;
using static WindowsFormsApp1.Layers.clsUtil;

namespace WindowsFormsApp1.Layers
{
    public class clsUtil
    {
        public enum enGenerateMode
        {
            ByValueWithoutDataType, 
            ByRefWithoutDataType, 
            ByValueWithDataType, 
            ByRefWithDataType,
            WithThisObj, 
            Query,
        };

        public enum enVariablesMode
        {
            InitializeVariablesByParameters,
            InitializeVariables,
            Properties,
            Parameters,
            InitializeLocalVarialbes,
            UpdateQueryVar,
            SettingData,
            AddWithValue

        }

        private static string _SetPrefix(enGenerateMode GenerateMode)
        {
            switch (GenerateMode)
            {
                case enGenerateMode.ByValueWithDataType:
                case enGenerateMode.ByValueWithoutDataType:
                    return "";

                case enGenerateMode.ByRefWithoutDataType:
                case enGenerateMode.ByRefWithDataType:
                    return "ref ";

                case enGenerateMode.WithThisObj:
                    return "this.";

                case enGenerateMode.Query:
                    return "@";
            }

            return "";
        }

        private static void _GenerateParameters(clsRow Row, ref string SequenseOfVar, bool IsLastRow, enGenerateMode GenerateMode, string Space = " ", string Postfix = "")
        {
            string Prefix = _SetPrefix(GenerateMode);

            if (GenerateMode == enGenerateMode.ByValueWithDataType || GenerateMode == enGenerateMode.ByRefWithDataType)
            {
                Prefix = _SetPrefix(GenerateMode) + " " + Row.Type;
            }

            SequenseOfVar = SequenseOfVar + Prefix + Space + Row.ColumnName + (!IsLastRow ? ", " : "") + Postfix;
        }

        private static void _GenerateProperties(clsRow Row, ref string SequenseOfVar, enGenerateMode GenerateMode)
        {
            string Prefix = _SetPrefix(GenerateMode);

            SequenseOfVar = SequenseOfVar + $"public {Prefix} {Row.ColumnName}" + " { get; set; }\n\t";
        }

        private static void _GenerateInitializedVariablesByParameters(clsRow Row, ref string SequenseOfVar, enGenerateMode GenerateMode)
        {
            string Prefix = _SetPrefix(GenerateMode);

            SequenseOfVar = SequenseOfVar + Prefix + Row.ColumnName + " = " + Row.ColumnName + "; \n\t    ";
        }

        private static string _InitialzeVariable(clsRow Row)
        {
            switch (Row.Type)
            {
                case "int":
                    return "-1";
                case "short":
                case "double":
                case "float":
                case "decimal":
                case "byte":
                    return "0";
                case "string":
                    return "string.Empty";
                case "char":
                    return "''";
                case "bool":
                    return "true";
                case "DateTime":
                    return "DateTime.Now";
                case "TimeSpan":
                    return "TimeSpan.FromDays(1)";
                default:
                    return "null";
            }
        }
        
        private static void _GenerateInitializedVariables(clsRow Row, ref string SequenseOfVar, enGenerateMode GenerateMode)
        {
            string Prefix = _SetPrefix(GenerateMode);

            SequenseOfVar = SequenseOfVar + Prefix + Row.ColumnName + " = " + (Row.ColumnName.ToLower().EndsWith("id") ? "-1" : _InitialzeVariable(Row)) + "; \n\t    ";
        }

        private static void _GenerateInitializedLocalVariables(clsRow Row, ref string SequenseOfVar, enGenerateMode GenerateMode)
        {
            string Prefix = _SetPrefix(GenerateMode);

            SequenseOfVar = SequenseOfVar + $"{Prefix} {Row.Type} {Row.ColumnName}" + " = " + _InitialzeVariable(Row) + "; \n\t    ";
        }
        
        private static void _GenerateSequenceOfVariables(clsRow Row, ref string SequenseOfVar, bool IsLastRow, enGenerateMode GenerateMode, enVariablesMode VariablesMode, string Space = " ", string Postfix = "")
        {
            switch (VariablesMode)
            {
                    // func(int a, ref int a, a, ref a)
                case enVariablesMode.Parameters:
                    _GenerateParameters(Row, ref SequenseOfVar, IsLastRow, GenerateMode, Space, Postfix);
                    break;

                    // public string a {get; set;};
                case enVariablesMode.Properties:
                    _GenerateProperties(Row, ref SequenseOfVar, GenerateMode);
                    break;

                case enVariablesMode.InitializeVariablesByParameters:
                    _GenerateInitializedVariablesByParameters(Row, ref SequenseOfVar, GenerateMode);
                    break;

                    // a = "";
                case enVariablesMode.InitializeVariables:
                    _GenerateInitializedVariables(Row, ref SequenseOfVar, GenerateMode);
                    break;

                    // string a = "";
                case enVariablesMode.InitializeLocalVarialbes:
                    _GenerateInitializedLocalVariables(Row, ref SequenseOfVar, GenerateMode);
                    break;

                    // a = @a
                case enVariablesMode.UpdateQueryVar:
                    _PartOfUpdateQuery(Row, ref SequenseOfVar, GenerateMode, IsLastRow, Postfix);
                    break;

                    // a = (int)Reader["a"];
                case enVariablesMode.SettingData:
                    _SettingDataOperation(Row, ref SequenseOfVar, GenerateMode);
                    break;
                case enVariablesMode.AddWithValue:
                    _SettingAddWithValue(Row, ref SequenseOfVar);
                    break;
            }
        }

        // Access Layer Functions
        private static void _PartOfUpdateQuery(clsRow Row, ref string SequenseOfVar, enGenerateMode GenerateMode, bool IsLastRow, string Postfix)
        {
            string Prefix = _SetPrefix(GenerateMode);

            SequenseOfVar = SequenseOfVar + Row.ColumnName + " = " + Prefix + Row.ColumnName + (IsLastRow ? "" : ",") + Postfix;
        }

        private static string _SettingNullField(clsRow Row, string SequenseOfVar)
        {
            if (Row.Type != "float")
            {
                return $@"
                
                if (reader[""{Row.ColumnName}""] != DBNull.Value)
                {{
                    {Row.ColumnName} = ({Row.Type})reader[""{Row.ColumnName}""];
                }}
                else
                {{
                    {Row.ColumnName} = {_InitialzeVariable(Row)};
                }}
                
                    ";

            }
            else
            {
                return $@"
                  
                if (reader[""{Row.ColumnName}""] != DBNull.Value)
                {{
                    {Row.ColumnName} = Convert.ToSingle(Reader[""{Row.ColumnName}""]);
                }}
                else
                {{
                    {Row.ColumnName} = {_InitialzeVariable(Row)};
                }}
                
                    ";

            }

        }

        private static void _SettingDataOperation(clsRow Row, ref string SequenseOfVar, enGenerateMode GenerateMode)
        {
            string Prefix = _SetPrefix(GenerateMode);
            
            if (Row.AllowNull)
            {
                SequenseOfVar = SequenseOfVar + _SettingNullField(Row, SequenseOfVar);
                return;
            }
            if (Row.Type != "float")
            { 
                SequenseOfVar = SequenseOfVar + Prefix + Row.ColumnName + " = " + $"({Row.Type})Reader[\"{Row.ColumnName}\"]; \n\t\t";
            }
            else
            {
                SequenseOfVar = SequenseOfVar + Prefix + Row.ColumnName + " = " + $"Convert.ToSingle(Reader[\"{Row.ColumnName}\"]); \n\t\t";
            }
        }

        private static void _SettingAddWithValue(clsRow Row, ref string SequenseOfVar)
        {
            if (!Row.AllowNull)
            {
            SequenseOfVar = SequenseOfVar + $@"
            Command.Parameters.AddWithValue(""@{Row.ColumnName}"", {Row.ColumnName});
                        
            ";
            }
            else
            {
            SequenseOfVar = SequenseOfVar + $@"
            if ({Row.ColumnName} != null || {Row.ColumnName} != {_InitialzeVariable(Row)})
                Command.Parameters.AddWithValue(""@{Row.ColumnName}"", {Row.ColumnName});
            else
                Command.Parameters.AddWithValue(""@{Row.ColumnName}"", System.DBNull.Value);
                    
            ";
            }
            
        }
        public static string GenerateSequenceOfVariables(List<clsRow> RowOfTable, enGenerateMode GenerateMode, 
                            enVariablesMode VariablesMode, string PrimaryKey = "##", bool IncludePrimaryKey = false, string Space = " ", string Postfix = "")
        {
            string Parameters = string.Empty;
            bool IsLastRow = false;

            for (int i = 0; i < RowOfTable.Count; i++)
            {
                if (IncludePrimaryKey && RowOfTable[i].ColumnName == PrimaryKey)
                {
                    continue;
                }

                if (i == RowOfTable.Count-1)
                {
                    IsLastRow = true;
                }

                _GenerateSequenceOfVariables(RowOfTable[i], ref Parameters, IsLastRow, GenerateMode, VariablesMode, Space, Postfix);
            }

            return Parameters;
        }

    }

}


