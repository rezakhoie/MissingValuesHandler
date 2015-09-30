using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace HCAHPS_Cleaning
{
    public class MySqlDB
    {
        private static string settings = "server=127.0.0.1; user id=root; password=`123456; database=hcahps; pooling=false; Convert Zero Datetime=True";


        public static void NonQuery(string sql)
        {
            using (var myConnection = new MySqlConnection(settings))
            {
                MySqlCommand command = new MySqlCommand(sql, myConnection);
                command.CommandTimeout = 0;
                myConnection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static object ScalarQuery(string sql)
        {
            using (var myConnection = new MySqlConnection(settings))
            {
                MySqlCommand command = new MySqlCommand(sql, myConnection);
                myConnection.Open();
                var result = command.ExecuteScalar();
                return result;
            }
        }

        public static DataTable Query(string sql, string table)
        {
            using (var myConnection = new MySqlConnection(settings))
            {
                myConnection.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, myConnection);
                DataSet ds = new DataSet();
                da.Fill(ds, table);
                DataTable dt = ds.Tables[table];
                return dt;
            }
        }
    }
}