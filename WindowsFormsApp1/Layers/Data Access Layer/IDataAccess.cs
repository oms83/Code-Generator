using CodeGeneratorDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Layers.Data_Access_Layer
{
    public interface IDataAccess 
    {
        string GetAll(string TableName);

        string AddNew(List<clsRow> RowOfTable, string TableName, string PrimaryKey);

        string Update(List<clsRow> RowOfTable, string TableName, string PrimaryKey);

        string Delete(string TableName, string DeleteBy, string DataType);

        string IsExist(string TableName, string ExistBy, string DataType);

        string Find(List<clsRow> RowOfTable, string TableName, string FindBy, string DataType);
    }
}
