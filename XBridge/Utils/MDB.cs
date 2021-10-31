using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;

namespace XBridge.Utils
{
    public class MDB
    {
        private static OleDbConnection dbConnection;
        private static OleDbCommand command;
        public MDB(string dbpath) {
            if (File.Exists(dbpath) == false)
                CreateMDBDataBase(dbpath);
            dbConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbpath + ";");
            dbConnection.Open();
        }
        public static bool CreateMDBDataBase(string mdbPath)
        {
            try
            {
                ADOX.CatalogClass cat = new ADOX.CatalogClass();
                ADOX.Table table = new ADOX.Table();
                table.Name = "PlayerData";
                cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";");
                return true;
            }
            catch { return false; }
        }
        public static DataTable Execute(string sql) {
            DataTable dt = new DataTable();
            DataRow dr;
            command = dbConnection.CreateCommand();
            command.CommandText = sql;
            OleDbDataReader reader = command.ExecuteReader();
            for (int i = 0; i < reader.FieldCount; i++) {
                DataColumn dc  = new DataColumn(reader.GetName(i));
                dt.Columns.Add(dc);
            }
            while (reader.Read()) {
                dr = dt.NewRow();
                for (int i = 0; i < reader.FieldCount; i++) {
                    dr[reader.GetName(i)] = reader[reader.GetName(i)].ToString();
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
