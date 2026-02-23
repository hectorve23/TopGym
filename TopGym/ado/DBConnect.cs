using MySql.Data.MySqlClient;
using System.Data;
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

        // Constructor
        public DBConnect()
        {
            Initialize();
        }

        // Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "TopGym";   // ← base de datos creada
            uid = "root";     // ← tu usuario de MySQL
            password = "root";     // ← tu contraseña de MySQL

            string connectionString = "SERVER=" + server + ";" +
                                      "DATABASE=" + database + ";" +
                                      "UID=" + uid + ";" +
                                      "PASSWORD=" + password + ";" +
                                      "CharSet=utf8;";  // ← evita problemas con tildes y ñ

            connection = new MySqlConnection(connectionString);
        }

        // Abrir conexión
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
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("No se puede conectar al servidor. Contacta con el administrador.",
                                        "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case 1045:
                        MessageBox.Show("Usuario o contraseña incorrectos.",
                                        "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    default:
                        MessageBox.Show("Error de base de datos: " + ex.Message,
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
                openConnection = false;
                return false;
            }
        }

        // Cerrar conexión
        public bool Close()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    openConnection = false;
                }
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cerrar la conexión: " + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // INSERT, UPDATE, DELETE → devuelve filas afectadas
        public int ExecuteQuery(MySqlCommand mySqlCommand)
        {
            int registros = 0;
            if (this.OpenConnection())
            {
                mySqlCommand.Connection = this.connection;
                registros = mySqlCommand.ExecuteNonQuery();
                this.Close();
            }
            return registros;
        }

        // SELECT → devuelve DataReader (recuerda cerrarlo después de usarlo)
        public MySqlDataReader Select(MySqlCommand mySqlCommand)
        {
            MySqlDataReader mySqlDataReader = null;
            if (this.OpenConnection())
            {
                mySqlCommand.Connection = this.connection;
                // CommandBehavior.CloseConnection cierra la conexión al cerrar el reader
                mySqlDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                openConnection = false;
            }
            return mySqlDataReader;
        }

        // SELECT → devuelve DataTable (más cómodo para DataGrids)
        public DataTable GetDataTable(MySqlCommand mySqlCommand)
        {
            DataTable dt = new DataTable();
            if (this.OpenConnection())
            {
                mySqlCommand.Connection = this.connection;
                MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                adapter.Fill(dt);
                this.Close();
            }
            return dt;
        }

        // Verificar si la conexión está activa
        public bool IsConnected()
        {
            return connection != null && connection.State == ConnectionState.Open;
        }
    }
}
