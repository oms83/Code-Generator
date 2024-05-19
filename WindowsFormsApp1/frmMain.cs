using CodeGeneratorBusiness;
using CodeGeneratorDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsApp1.Layers.Business_Layer;
using WindowsFormsApp1.Layers.Data_Access_Layer;
using WindowsFormsApp1.Layers.Files;

namespace WindowsFormsApp1
{
    public partial class frmMain : Form
    {
        Dictionary<string, List<clsRow>> _AllDataOfDB = new Dictionary<string, List<clsRow>>();
        List<clsRow> _AllDataOfTable = new List<clsRow>();
        public frmMain()
        {
            InitializeComponent();
        }

        private void main_Load(object sender, EventArgs e)
        {
            _GetAllDatabasesName();
            _Reset();
        }

        private void _Reset()
        {
            guna2CircleProgressBar1.Visible = false;
            guna2CircleProgressBar1.Value = 0;
            lblMessage.Visible = false;

            if (cmbDatabases.Text == "None")
            {
                btnGenerate.Enabled = false;
            }

            cmbDatabases.SelectedIndex = 0;
            cmbTables.Visible = false;
        }
        private void _RefreshData()
        {
            DataTable dtColumns = clsCodeGenerator.ConvertListToDataTable(_AllDataOfTable);
            dgvColumns.DataSource = dtColumns;

        }
        private void _GetAllDatabasesName()
        {
            DataTable dtDatabases = new DataTable();
            dtDatabases = clsCodeGenerator.GetAllDatabases();

            foreach(DataRow dr in dtDatabases.Rows)
            {
                cmbDatabases.Items.Add(dr["name"]);
            }
        }
        private void _GetAllTables(string DBName)
        {
            cmbTables.Items.Clear();
            DataTable dtTables = new DataTable();
            dtTables = clsCodeGenerator.GetAllTables(DBName);
            foreach (DataRow dr in dtTables.Rows)
            {
                cmbTables.Items.Add(dr["TABLE_NAME"]);
            }
        }

        private void _GetAllData()
        {
            _AllDataOfDB = clsCodeGenerator.GetSplittedRowsOfAllTablesByDictionary(cmbDatabases.Text.Trim());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvColumns.DataSource = null;
            
            _GetAllData();

            if (cmbDatabases.Text == "None")
            {
                cmbTables.Visible = false;
                btnGenerate.Enabled = false;
            }
            else
            {
                cmbTables.Visible = true;
                _GetAllTables(cmbDatabases.Text);
            }
        }

        private void cmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGenerate.Enabled = true;
            _AllDataOfTable = _AllDataOfDB[cmbTables.Text.Trim()];
            _RefreshData();
        }

        private void _ChangeKeyType(bool KeyType)
        {
            string UniqueKey = dgvColumns.CurrentRow.Cells[0].Value.ToString();
            string TableName = cmbTables.Text.Trim();
            clsCodeGenerator.ChangeKeyType(ref _AllDataOfDB, TableName, UniqueKey, KeyType);
            _AllDataOfTable = _AllDataOfDB[TableName];
            _RefreshData();
        }
        private void setAsUniqueKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ChangeKeyType(true);
        }

        private void removePrimarilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ChangeKeyType(false);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            clsGenerateDataAccess DataAccess = new clsGenerateDataAccess();
            clsGenerateBusiness Business = new clsGenerateBusiness();
            string DataAccessLayer = string.Empty;
            string DataBusinessLayer = string.Empty;
            string FolderPath = "C:\\Users\\omerm\\Desktop\\Code Generator\\";

            guna2CircleProgressBar1.Visible = true;
            int Counter = _AllDataOfTable.Count;
            int NumberOfClasses = 0;
            
            foreach (KeyValuePair<string, List<clsRow>> row in _AllDataOfDB)
            {
                DataAccessLayer = DataAccess.BulidBodyOfclsDataAccess(row.Value, row.Key.Trim());
                DataBusinessLayer = Business.BulidBodyOfclsBusiness(row.Value, row.Key.Trim());

                Thread.Sleep(500);
                guna2CircleProgressBar1.Value += (100 / Counter);

                clsSaveData.SaveToFile(DataBusinessLayer, row.Key.Trim(), $"{FolderPath}Business");
                clsSaveData.SaveToFile(DataAccessLayer, row.Key.Trim(), $"{FolderPath}DataAccess");

                NumberOfClasses++;

                guna2CircleProgressBar1.Refresh();
            }

            guna2CircleProgressBar1.Visible = false;
            lblMessage.Visible = true;
            lblMessage.Text = $"{NumberOfClasses * 2} classes have been generated for business and data access layers";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _Reset();
            _AllDataOfDB.Clear();
            _AllDataOfTable.Clear();
        }
    }
}

