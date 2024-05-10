using CodeGeneratorBusiness;
using CodeGeneratorDataAccess;
using System.Collections.Generic;

namespace WindowsFormsApp1.Layers.Data_Access_Layer
{
    public class clsGenerateDataAccess : IDataAccess
    {
        public string AddNew(List<clsRow> RowOfTable, string TableName, string PrimaryKey)
        {
            return $@"
        public static int AddNewPerson({clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithDataType, clsUtil.enVariablesMode.Parameters)})
        {{
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            int {PrimaryKey} = -1;

            string Query = @""INSERT INTO {TableName}(
                                            {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithoutDataType, clsUtil.enVariablesMode.Parameters, PrimaryKey, true, " \n\t\t\t\t\t\t")}
                                        )
                                        VALUES
                                        (
                                                {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.Query, clsUtil.enVariablesMode.Parameters, PrimaryKey, true, "", " \n\t\t\t\t\t\t")}
                                        );
                                        SELECT_SCOPE_IDENTITY();
                                        "";
            
            SqlCommand Command = new SqlCommand(Query, Connection);

            {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithoutDataType, clsUtil.enVariablesMode.AddWithValue, PrimaryKey, true)}


            /*
             
                ExecuteScalar() method in C# executes a SQL query against the database and returns the value of the first column 
                of the first row in the result set. It's commonly used for queries that return a single value, such as 
                aggregate functions (COUNT, MAX, MIN, SUM, AVG, etc.) or queries that return a single value.

                For example, you can use a query like SELECT COUNT(*) FROM TableName to get the total number of rows in a table, 
                and then use the ExecuteScalar() method to execute this query and retrieve the result. In this case, 
                the returned value will be the total row count.

            */

            try
            {{
                Connection.Open();

                object Result = Command.ExecuteScalar();

                if (Result != null && int.TryParse(Result.ToString(), out int InsertedID))
                {{
                    {PrimaryKey} = InsertedID;
                }}
            }}
            catch (Exception ex)
            {{

            }}
            finally
            {{
                Connection.Close();
            }}
            return {PrimaryKey};
        }}
        ";
        }

        public string Delete(string TableName, string DeleteBy, string DataType)
        {
            return $@"

    public static bool Delete{TableName}({DataType} {DeleteBy})
    {{
        // Connection: It is used to establish a connection to a specific data source.
        SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            
        string Query = @""DELETE FROM {TableName} Where {DeleteBy} = @{DeleteBy}"";
            
        // Command: It is used to execute queries to perform database operations.
        SqlCommand Command = new SqlCommand(Query, Connection);

        Command.Parameters.AddWithValue(""@{DeleteBy}"", {DeleteBy});

        int AffectedRows = -1;

        try
        {{
            Connection.Open();
            AffectedRows = Command.ExecuteNonQuery();
        }}
        catch (Exception ex) 
        {{

        }}
        finally
        {{
            Connection.Close();
        }}
        return (AffectedRows > 0);
    }}

            ";
        }

        public string Update(List<clsRow> RowOfTable, string TableName, string PrimaryKey)
        {
            return $@"
        public static bool Update{TableName}({clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithDataType, clsUtil.enVariablesMode.Parameters)})
        {{
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @""UPDATE {TableName} 
                             SET
                                {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.Query, clsUtil.enVariablesMode.UpdateQueryVar, PrimaryKey, true, "", "\n\t\t\t\t")}
                             WHERE
                             {PrimaryKey} = @{PrimaryKey};
                            "";

            SqlCommand Command = new SqlCommand(Query, Connection);
            
            {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithoutDataType, clsUtil.enVariablesMode.AddWithValue)}

            int AffectedRows = -1;

            try
            {{
                Connection.Open();
                AffectedRows = Command.ExecuteNonQuery();
            }}
            catch (Exception ex) 
            {{

            }}
            finally
            {{
                Connection.Close();
            }}
            return (AffectedRows > 0);
        }}";
        }

        public string GetAll(string TableName)
        {
            return $@"

    public static DataTable GetAll{TableName}()
    {{
        DataTable dataTable = new DataTable();

        // Connection: It is used to establish a connection to a specific data source.
        SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

        string Query = @""SELECT * FROM {TableName};"";

        // Command: It is used to execute queries to perform database operations.
        SqlCommand Command = new SqlCommand(Query, Connection);

        try
        {{
            Connection.Open();

            // DataReader: It is used to read data from data source.
            // The DbDataReader is a base class for all DataReader objects.

            SqlDataReader Reader = Command.ExecuteReader();

            if (Reader.HasRows)
            {{
                dataTable.Load(Reader);
            }}

            Reader.Close();
        }}
        catch (Exception ex)
        {{

        }}
        finally
        {{
            Connection.Close();
        }}

        return dataTable;
    }}

                    ";
        }

        public string Find(List<clsRow> RowOfTable, string TableName, string FindBy, string DataType)
        {
            return $@"
    public static bool Get{TableName}InfoBy{FindBy}({DataType} {FindBy}, {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByRefWithDataType, clsUtil.enVariablesMode.Parameters, FindBy, true)})
    {{
        // Connection: It is used to establish a connection to a specific data source.
        SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

        string Query = @""SELECT * FROM {TableName} WHERE {FindBy} = @{FindBy}"";

        // Command: It is used to execute queries to perform database operations
        SqlCommand Command = new SqlCommand(Query, connection);

        Command.Parameters.AddWithValue(""@{FindBy}"", {FindBy});

        bool isFound = false;
        try
        {{
            connection.Open();

            SqlDataReader Reader = Command.ExecuteReader();

            if (Reader.Read())
            {{
                isFound = true;

                {clsUtil.GenerateSequenceOfVariables(RowOfTable, clsUtil.enGenerateMode.ByValueWithoutDataType, clsUtil.enVariablesMode.SettingData, FindBy, true)}

                Reader.Close();
            }}
        }}
        catch (Exception ex)
        {{
        }}
        finally
        {{
            connection.Close();
        }}

        return isFound;
    }}
                    ";
        }

        public string IsExist(string TableName, string ExistBy, string DataType)
        {
            //ExistBy.Substring(0, ExistBy.Length - 2) 
            return $@"
    public bool Is{TableName}ExistBy{ExistBy}({DataType} {ExistBy})
    {{
        // Connection: It is used to establish a connection to a specific data source.
        SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

        string Query = ""SELECT Found = 1 FROM {TableName} WHERE {ExistBy} = @{ExistBy}"";

        // Command: It is used to execute queries to perform database operations
        SqlCommand Command = new SqlCommand(Query, Connection);

        Command.Parameters.AddWithValue(""@{ExistBy}"", {ExistBy});

        bool IsExist = false;

        try
        {{
            Connection.Open();

            SqlDataReader Reader = Command.ExecuteReader();

            IsExist = Reader.HasRows;

            Reader.Close();

        }}
        catch (Exception ex)
        {{

        }}
        finally
        {{
            Connection.Close();
        }}

        return IsExist;
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
        public string BulidBodyOfclsDataAccess(List<clsRow> RowOfTable, string TableName)
        {
            string PrimaryKey = clsCodeGenerator.FindPrimaryKey(RowOfTable).ColumnName;

            return $@"
using System;
using System.Data;

public class cls{TableName}
{{
                                        
        {AddNew(RowOfTable, TableName, PrimaryKey)}
        {_PrintRepeatedFunctions(RowOfTable, TableName)}
        {Update(RowOfTable, TableName, PrimaryKey)}
        {GetAll(TableName)}
}}
                ";
        }

    }
}
