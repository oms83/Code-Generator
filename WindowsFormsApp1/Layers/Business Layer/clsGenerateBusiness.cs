using CodeGeneratorDataAccess;
using System.Collections.Generic;

namespace WindowsFormsApp1.Layers.Business_Layer
{
    internal class clsGenerateBusiness : IBusiness
    {
        public clsGenerateBusiness() { }
        public string GetAll(string TableName)

        {
            TableName = TableName.Trim();

            return $@"
        public static DataTable GetAll{TableName}()
        {{
            return cls{TableName}Data.GetAll{TableName}();
        }}
        ";
        }
        public string AddNew(List<clsRow> RowOfTable, string TableName, string PrimaryKey)
        {
            return $@"
        private bool _AddNew{TableName}()
        {{
            this.{PrimaryKey} = cls{TableName}Data.AddNew{TableName}({clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.WithThisObj, clsUtil.enVariablesMode.Parameters, PrimaryKey, true)});

            return (this.{PrimaryKey} > -1);
        }}

        ";
        }

        public string Update(List<clsRow> RowOfTable, string TableName)
        {
            return $@"
            private bool _Update{TableName}()
            {{
                return cls{TableName}Data.Update{TableName}({clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.WithThisObj, clsUtil.enVariablesMode.Parameters)});
            }}
            ";

        }

        public string Delete(string TableName, string DeleteBy, string DataType)
        {
            return $@"
        public static bool Delete{TableName}({DataType} {DeleteBy})
        {{
            return cls{TableName}Data.Delete{TableName}({DeleteBy});
        }}
        ";

        }

        public string IsExist(string TableName, string ExistBy, string DataType)
        {
            return $@"
        
        public static bool Is{TableName}ExistBy{ExistBy}({DataType} {ExistBy})
        {{
            return cls{TableName}Data.Is{TableName}ExistBy{ExistBy}({ExistBy});
        }}
        
            ";

        }

        public string Find(List<clsRow> RowOfTable, string TableName, string FindBy, string DataType)
        {
            return $@"
        
        public static cls{TableName} Get{TableName}InfoBy{FindBy}({DataType} {FindBy})
        {{
            
            {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithDataType, clsUtil.enVariablesMode.InitializeLocalVarialbes, FindBy, true)}

            bool IsFound = cls{TableName}Data.Get{TableName}Info{FindBy}({FindBy}, {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByRefWithoutDataType, clsUtil.enVariablesMode.Parameters, FindBy, true)});

            if (IsFound)
            {{
                return new cls{TableName}({clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithoutDataType, clsUtil.enVariablesMode.Parameters)});
            }}
            else
            {{
                return null;
            }}
        }}
        
        ";
        }

        public string Properties(List<clsRow> RowOfTable)
        {
            return clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithoutDataType, clsUtil.enVariablesMode.Properties);

        }

        public string Save(string TableName)
        {
            return $@"
        public bool Save()
        {{
            switch (_Mode)
            {{
                case en{TableName}Mode.AddNew:

                    if (_AddNew{TableName}())
                    {{
                        _Mode = en{TableName}Mode.Update;
                        return true;
                    }}
                    else
                    {{
                        return false;
                    }}


                case en{TableName}Mode.Update:

                    return _Update{TableName}();
            }}

            return false;
        }}
        ";

        }

        public string PrivateConstructor(List<clsRow> RowOfTable, string TableName)
        {
            return $@"
        private cls{TableName}({clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithDataType, clsUtil.enVariablesMode.Parameters)})
        {{
            {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.WithThisObj, clsUtil.enVariablesMode.InitializeVariablesByParameters)}
            _Mode = en{TableName}Mode.Update;
        }}
        ";

        }

        public string PublicConstructor(List<clsRow> RowOfTable, string TableName)
        {
            return $@"
        public cls{TableName}()
        {{
                            
            {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.WithThisObj, clsUtil.enVariablesMode.InitializeVariables)}

            _Mode = en{TableName}Mode.AddNew;
        }}
        ";
        }

        private string _PrintRepeatedFunctions(List<clsRow> RowOfTable, string TableName)
        {
            string str = string.Empty;

            foreach (var Row in RowOfTable)
            {
                if (Row.IsPrimaryKey)
                {
                    str = str + Find(RowOfTable, TableName, Row.ColumnName, Row.Type);
                    str = str + IsExist(TableName, Row.ColumnName, Row.Type);
                }
            }

            return str;
        }
        public string BulidBodyOfclsBusiness(List<clsRow> RowOfTable, string TableName)
        {
            return $@"
using System;
using System.Data;

public class cls{TableName}
{{
        public enum en{TableName}Mode {{AddNew = 0, Update = 1}};
        private en{TableName}Mode _Mode;
                                        
        {Properties(RowOfTable)}
        {PublicConstructor(RowOfTable, TableName)}
        {PrivateConstructor(RowOfTable, TableName)}
        {_PrintRepeatedFunctions(RowOfTable, TableName)}
        {Update(RowOfTable, TableName)}
        {GetAll(TableName)}
        {Save(TableName)}
}}
                ";
        }
    }
}


