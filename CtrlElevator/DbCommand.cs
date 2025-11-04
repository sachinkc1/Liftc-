using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace CtrlElevator
{
    class DbCommand
    {
        public void SaveLog(string act)
        {
            const string sql = "INSERT INTO `ActionLogs` (`DATE`, `TIME`, `ACTIONS`) VALUES (@DATE, @TIME, @ACTIONS)";

            GlobalConnection.Open();

            using (var cmd = new MySqlCommand(sql, GlobalConnection.Con))
            {
                cmd.Parameters.AddWithValue("@DATE", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@TIME", DateTime.Now.TimeOfDay);
                cmd.Parameters.AddWithValue("@ACTIONS", act ?? string.Empty);

                cmd.ExecuteNonQuery();
            }
        }

        public DataTable ViewActionLog()
        {
            const string sql = "SELECT * FROM `ActionLogs`";
            using (var da = new MySqlDataAdapter(sql, GlobalConnection.Con))
            {
                var ds = new DataSet();
                da.Fill(ds, "ActionLogs");
                return ds.Tables[0];
            }
        }
    }
}