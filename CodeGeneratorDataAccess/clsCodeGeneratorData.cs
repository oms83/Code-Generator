using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGeneratorDataAccess
{
    public partial class clsCodeGeneratorData
    {
        public static DataTable GetAllDatabases()
        {
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT name 
                             FROM master.sys.databases 
                             WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')";

            SqlCommand Command = new SqlCommand(Query, Connection);

            DataTable dtDatabases = new DataTable();

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();

                if (Reader != null)
                {
                    dtDatabases.Load(Reader);
                }

                Reader.Close();
            }
            catch (Exception ex)
            {
                Connection.Close();
            }
            finally
            {
                Connection.Close();
            }

            return dtDatabases;
        }

        public static DataTable GetAllTables(string DBName)
        {
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = $@"SELECT TABLE_NAME FROM {@DBName}.information_schema.tables 
                              WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME != 'sysdiagrams'";

            SqlCommand Command = new SqlCommand(Query, Connection);

            Command.Parameters.AddWithValue("@DBName", DBName);

            DataTable dataTable = new DataTable();

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();

                if (Reader != null)
                {
                    dataTable.Load(Reader);
                }

                Reader.Close();
            }
            catch (Exception ex)
            {
                Connection.Close();
            }
            finally
            {
                Connection.Close();
            }

            return dataTable;
        }

        public static DataTable GetAllColumns(string DBName, string TableName)
        {
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = $@"
                             DECLARE @DatabaseName NVARCHAR(128) 
                             SET @DatabaseName = '' + @DBName + ''
                             
                             DECLARE @Sql NVARCHAR(MAX) 
                             SET @Sql = '
                                 SELECT 
                                     COLUMN_NAME, 
                                     DATA_TYPE, 
                                     CASE 
                                         WHEN EXISTS (
                                             SELECT 1 
                                             FROM ' + @DatabaseName + '.INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
                                             INNER JOIN ' + @DatabaseName + '.INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU 
                                                 ON TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
                                             WHERE TC.TABLE_NAME = ''{@TableName}''
                                                 AND KCU.COLUMN_NAME = C.COLUMN_NAME 
                                                 AND TC.CONSTRAINT_TYPE = ''PRIMARY KEY''
                                         ) THEN ''PK'' 
                                         ELSE '''' 
                                     END AS ''KeyType'', 
                                     CASE 
                                         WHEN IS_NULLABLE = ''YES'' THEN ''Null'' 
                                         ELSE ''Not Null'' 
                                     END AS ''Nullability''
                                 FROM ' + @DatabaseName + '.INFORMATION_SCHEMA.COLUMNS AS C 
                                 WHERE TABLE_NAME = ''{@TableName}'''
                             
                             EXEC(@Sql)
                     ";

            /*
                string Query = $@"
                             DECLARE @DatabaseName NVARCHAR(128) SET @DatabaseName = '{ @DBName }'
                             DECLARE @Sql NVARCHAR(MAX)
                             SET @Sql = 'SELECT COLUMN_NAME, DATA_TYPE
                                         FROM ' + @DatabaseName + '.INFORMATION_SCHEMA.COLUMNS
                                         WHERE TABLE_NAME = ''Countries'''
                             EXEC(@Sql)
                     "; 
            Command.Parameters.AddWithValue("@DBName", DBName); this required using the parameters

            string Query = $@"
                             DECLARE @DatabaseName NVARCHAR(128) SET @DatabaseName = '{ DBName }'
                             DECLARE @Sql NVARCHAR(MAX)
                             SET @Sql = 'SELECT COLUMN_NAME, DATA_TYPE
                                         FROM ' + @DatabaseName + '.INFORMATION_SCHEMA.COLUMNS
                                         WHERE TABLE_NAME = ''Countries'''
                             EXEC(@Sql)
                     ";
            */

            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@DBName", DBName);
            Command.Parameters.AddWithValue("@TableName", TableName);
            DataTable dataTable = new DataTable();

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();

                if (Reader != null)
                {
                    dataTable.Load(Reader);
                }

                Reader.Close();
            }
            catch (Exception ex)
            {
                
                Connection.Close();
            }
            finally
            {
                Connection.Close();
            }

            return dataTable;
        }

        
    }
}
