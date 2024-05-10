using CodeGeneratorDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Layers.Business_Layer
{
    public interface IBusiness 
    {
        string GetAll(string TableName);

        string AddNew(List<clsRow> RowOfTable, string TableName, string PrimaryKey);

        string Update(List<clsRow> RowOfTable, string TableName);

        string Delete(string TableName, string DeleteBy, string DataType);

        string IsExist(string TableName, string ExistBy, string DataType);

        string Find(List<clsRow> RowOfTable, string TableName, string FindBy, string DataType);
        string Properties(List<clsRow> RowOfTable);

        string Save(string TableName);

        string PrivateConstructor(List<clsRow> RowOfTable, string TableName);

        string PublicConstructor(List<clsRow> RowOfTable, string TableName);
    }
}
