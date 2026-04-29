
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace TopGym
{
    public class DatabaseConnection
    {
        private static string connectionString = "Server=localhost;Database=TopGymDB;Uid=root;Pwd=root;";

        private static MySqlConnection GetConnection()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}",
                    "Error de Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static bool ProbarConexion()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn != null)
                    {
                        conn.Open();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al probar la conexión: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        /// Inserta un nuevo usuario en la base de datos
        public static bool InsertarUsuario(string nombre, string contrasena, string rol)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();
                    string query = "INSERT INTO Usuario (Nombre, Contrasena, Rol) VALUES (@nombre, @contrasena, @rol)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);
                        cmd.Parameters.AddWithValue("@rol", rol);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate entry
                {
                    MessageBox.Show("El nombre de usuario ya existe.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Error al insertar usuario: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }
        }

        /// Obtiene todos los usuarios de la base de datos
        public static List<Usuario> ObtenerTodosUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return usuarios;

                    conn.Open();
                    string query = "SELECT IdUsuario, Nombre, Contrasena, Rol FROM Usuario";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                Nombre = reader.GetString("Nombre"),
                                Contrasena = reader.GetString("Contrasena"),
                                Rol = reader.GetString("Rol") == "administrador" ? RolUsuario.Administrador : RolUsuario.Usuario
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener usuarios: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return usuarios;
        }

        /// Valida las credenciales de un usuario
        public static Usuario ValidarUsuario(string nombre, string contrasena)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return null;

                    conn.Open();
                    string query = "SELECT IdUsuario, Nombre, Contrasena, Rol FROM Usuario WHERE Nombre = @nombre AND Contrasena = @contrasena";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Usuario
                                {
                                    IdUsuario = reader.GetInt32("IdUsuario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Contrasena = reader.GetString("Contrasena"),
                                    Rol = reader.GetString("Rol") == "administrador" ? RolUsuario.Administrador : RolUsuario.Usuario
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al validar usuario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        /// Actualiza un usuario existente
        public static bool ActualizarUsuario(int idUsuario, string nombre, string contrasena, string rol)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();
                    string query = "UPDATE Usuario SET Nombre = @nombre, Contrasena = @contrasena, Rol = @rol WHERE IdUsuario = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);
                        cmd.Parameters.AddWithValue("@rol", rol);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar usuario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// Elimina un usuario de la base de datos
        public static bool EliminarUsuario(int idUsuario)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();
                    string query = "DELETE FROM Usuario WHERE IdUsuario = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idUsuario);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar usuario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }





        /// Inserta una nueva actividad en la base de datos
        public static bool InsertarActividad(string nombre, string descripcion, string horario, int plazasTotal)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();
                    string query = "INSERT INTO Actividad (Nombre, Descripcion, Horario, PlazasTotal) VALUES (@nombre, @descripcion, @horario, @plazas)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@horario", horario);
                        cmd.Parameters.AddWithValue("@plazas", plazasTotal);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al insertar actividad: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// Obtiene todas las actividades de la base de datos
        public static List<Actividad> ObtenerTodasActividades()
        {
            List<Actividad> actividades = new List<Actividad>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return actividades;

                    conn.Open();
                    string query = "SELECT IdActividad, Nombre, Descripcion, Horario, PlazasTotal FROM Actividad";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Actividad actividad = new Actividad
                            {
                                IdActividad = reader.GetInt32("IdActividad"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? "" : reader.GetString("Descripcion"),
                                Horario = reader.GetString("Horario"),
                                PlazasTotal = reader.GetInt32("PlazasTotal")
                            };
                            actividades.Add(actividad);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener actividades: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return actividades;
        }

        /// Actualiza una actividad existente
        public static bool ActualizarActividad(int idActividad, string nombre, string descripcion, string horario, int plazasTotal)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();
                    string query = "UPDATE Actividad SET Nombre = @nombre, Descripcion = @descripcion, Horario = @horario, PlazasTotal = @plazas WHERE IdActividad = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idActividad);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@horario", horario);
                        cmd.Parameters.AddWithValue("@plazas", plazasTotal);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar actividad: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// Elimina una actividad de la base de datos
        public static bool EliminarActividad(int idActividad)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();
                    string query = "DELETE FROM Actividad WHERE IdActividad = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idActividad);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar actividad: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }



      

        /// Inscribe un usuario en una actividad
        public static bool InscribirUsuarioEnActividad(int idUsuario, int idActividad)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();

                    // Verificar si hay plazas disponibles
                    string queryPlazas = @"SELECT a.PlazasTotal, COUNT(ua.IdUsuario) as Inscritos 
                                          FROM Actividad a 
                                          LEFT JOIN UsuarioActividad ua ON a.IdActividad = ua.IdActividad 
                                          WHERE a.IdActividad = @idActividad 
                                          GROUP BY a.IdActividad, a.PlazasTotal";

                    using (MySqlCommand cmdPlazas = new MySqlCommand(queryPlazas, conn))
                    {
                        cmdPlazas.Parameters.AddWithValue("@idActividad", idActividad);

                        using (MySqlDataReader reader = cmdPlazas.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int plazasTotal = reader.GetInt32("PlazasTotal");
                                int inscritos = reader.GetInt32("Inscritos");

                                if (inscritos >= plazasTotal)
                                {
                                    MessageBox.Show("No hay plazas disponibles en esta actividad.",
                                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return false;
                                }
                            }
                        }
                    }

                    // Insertar la inscripcion
                    string query = "INSERT INTO UsuarioActividad (IdUsuario, IdActividad) VALUES (@idUsuario, @idActividad)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@idActividad", idActividad);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) 
                {
                    MessageBox.Show("El usuario ya esta inscrito en esta actividad.",
                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Error al inscribir usuario: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }
        }

        /// Desinscribe un usuario de una actividad
        public static bool DesinscribirUsuarioDeActividad(int idUsuario, int idActividad)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return false;

                    conn.Open();
                    string query = "DELETE FROM UsuarioActividad WHERE IdUsuario = @idUsuario AND IdActividad = @idActividad";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@idActividad", idActividad);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al desinscribir usuario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// Obtiene todas las actividades en las que esta inscrito un usuario
        public static List<Actividad> ObtenerActividadesDeUsuario(int idUsuario)
        {
            List<Actividad> actividades = new List<Actividad>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return actividades;

                    conn.Open();
                    string query = @"SELECT a.IdActividad, a.Nombre, a.Descripcion, a.Horario, a.PlazasTotal 
                                    FROM Actividad a 
                                    INNER JOIN UsuarioActividad ua ON a.IdActividad = ua.IdActividad 
                                    WHERE ua.IdUsuario = @idUsuario";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Actividad actividad = new Actividad
                                {
                                    IdActividad = reader.GetInt32("IdActividad"),
                                    Nombre = reader.GetString("Nombre"),
                                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? "" : reader.GetString("Descripcion"),
                                    Horario = reader.GetString("Horario"),
                                    PlazasTotal = reader.GetInt32("PlazasTotal")
                                };
                                actividades.Add(actividad);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener actividades del usuario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return actividades;
        }

        /// Obtiene todos los usuarios inscritos en una actividad
        public static List<Usuario> ObtenerUsuariosDeActividad(int idActividad)
        {
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    if (conn == null) return usuarios;

                    conn.Open();
                    string query = @"SELECT u.IdUsuario, u.Nombre, u.Contrasena, u.Rol 
                                    FROM Usuario u 
                                    INNER JOIN UsuarioActividad ua ON u.IdUsuario = ua.IdUsuario 
                                    WHERE ua.IdActividad = @idActividad";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idActividad", idActividad);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Usuario usuario = new Usuario
                                {
                                    IdUsuario = reader.GetInt32("IdUsuario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Contrasena = reader.GetString("Contrasena"),
                                    Rol = reader.GetString("Rol") == "administrador" ? RolUsuario.Administrador : RolUsuario.Usuario
                                };
                                usuarios.Add(usuario);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener usuarios de la actividad: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return usuarios;
        }
    }
}

