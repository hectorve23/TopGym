using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _TopGym.ado
{
    internal class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        bool openConnection = false;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "world";
            uid = "root";
            password = "root";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);

        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                if (!openConnection)
                {
                    openConnection = true;
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        public bool Close()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        //Insert statement
        public int executeQuery(MySqlCommand mySqlCommand)
        {
            int registros = 0;
            //open connection
            if (this.OpenConnection() == true)
            {

                //Execute command
                mySqlCommand.Connection = this.connection;
                registros = mySqlCommand.ExecuteNonQuery();
            }
            return registros;
        }


        //Select statement
        public MySqlDataReader Select(MySqlCommand mySqlCommand)
        {
            MySqlDataReader mySqlDataReader = null;
            if (this.OpenConnection() == true)
            {

                //Execute command
                mySqlCommand.Connection = this.connection;
                mySqlDataReader = mySqlCommand.ExecuteReader();

            }
            return mySqlDataReader;
        }
        public DataTable getDataTable(MySqlCommand mySqlCommand)
        {
            DataTable dt = new DataTable();
            mySqlCommand.Connection = this.connection;
            MySqlDataAdapter myAdapter = new MySqlDataAdapter(mySqlCommand);
            myAdapter.Fill(dt);
            return dt;
        }

    }
}
