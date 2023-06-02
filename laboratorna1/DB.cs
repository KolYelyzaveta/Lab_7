using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laboratorna1
{
    public class DB
    {
        public static string strCon = @"Data Source=.;Initial Catalog=DB_4;Integrated Security=True";
        SqlConnection connection = new SqlConnection(strCon);

        public void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
                MessageBox.Show((connection.State).ToString(), "Соединение с БД", MessageBoxButtons.OK);
            }
        }

        public void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
                MessageBox.Show((connection.State).ToString(), "Соединение с БД", MessageBoxButtons.OK);
            }
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }
    }
}
