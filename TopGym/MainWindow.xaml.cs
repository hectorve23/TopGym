using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TopGym
{
    public partial class MainWindow : Window
    {
        public static ObservableCollection<Usuario> usuarios = new ObservableCollection<Usuario>();

        public MainWindow()
        {
            InitializeComponent();
            InicializarAplicacion();
        }

        /// Inicializa la aplicacion: prueba conexion y carga datos desde la base de datos
        private void InicializarAplicacion()
        {
            if (!DatabaseConnection.ProbarConexion())
            {
                MessageBox.Show("No se pudo conectar a la base de datos",
                    "Error de Conexion", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            CargarDatosDesdeBaseDatos();
        }

        /// Carga todos los usuarios y actividades desde la base de datos al iniciar la aplicación
        private void CargarDatosDesdeBaseDatos()
        {
            try
            {
                // Cargar usuarios
                usuarios.Clear();
                List<Usuario> usuariosBD = DatabaseConnection.ObtenerTodosUsuarios();
                foreach (var usuario in usuariosBD)
                {
                    usuarios.Add(usuario);
                }

                // Cargar actividades en la coleccion estatica de UsuarioWindow
                UsuarioWindow.Actividades.Clear();
                List<Actividad> actividadesBD = DatabaseConnection.ObtenerTodasActividades();

                foreach (var actividad in actividadesBD)
                {
                    // Cargar los usuarios inscritos en cada actividad
                    List<Usuario> inscritos = DatabaseConnection.ObtenerUsuariosDeActividad(actividad.IdActividad);
                    actividad.inscritos = new ObservableCollection<Usuario>(inscritos);

                    UsuarioWindow.Actividades.Add(actividad);
                }

                // Cargar las actividades inscritas para cada usuario
                foreach (var usuario in usuarios)
                {
                    List<Actividad> actividadesUsuario = DatabaseConnection.ObtenerActividadesDeUsuario(usuario.IdUsuario);
                    usuario.ActividadesInscritas = new ObservableCollection<Actividad>(actividadesUsuario);
                }

                Console.WriteLine($"Datos cargados: {usuarios.Count} usuarios, {UsuarioWindow.Actividades.Count} actividades");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            string nombreIntroducido = txtUsuario.Text.Trim();
            string passwordIntroducida = txtPassword.Password;

            if (string.IsNullOrEmpty(nombreIntroducido) || string.IsNullOrEmpty(passwordIntroducida))
            {
                MessageBox.Show("Por favor, rellena todos los campos", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar usuario contra la base de datos
            Usuario usuarioEncontrado = DatabaseConnection.ValidarUsuario(nombreIntroducido, passwordIntroducida);

            if (usuarioEncontrado == null)
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Cargar las actividades en las que está inscrito el usuario
            List<Actividad> actividadesInscritas = DatabaseConnection.ObtenerActividadesDeUsuario(usuarioEncontrado.IdUsuario);
            usuarioEncontrado.ActividadesInscritas = new ObservableCollection<Actividad>(actividadesInscritas);

            // Abrir la ventana correspondiente segun el rol
            if (usuarioEncontrado.Rol == RolUsuario.Administrador)
            {
                AdminWindow adminWin = new AdminWindow(usuarioEncontrado);
                adminWin.Show();
                this.Close();
            }
            else
            {
                UsuarioWindow usuarioWin = new UsuarioWindow(usuarioEncontrado);
                usuarioWin.Show();
                this.Close();
            }
        }

        private void Registro_Click(object sender, RoutedEventArgs e)
        {
            RegistroWindow registro = new RegistroWindow();
            registro.ShowDialog();

            // Recargar usuarios despues del registro por si se creo uno nuevo
            CargarDatosDesdeBaseDatos();
        }
    }
}
