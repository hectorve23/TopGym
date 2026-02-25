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
            CargarUsuariosPrueba();
        }

        private void CargarUsuariosPrueba()
        {
            usuarios.Add(new Usuario { Nombre = "admin", Contrasena = "1234", Rol = RolUsuario.Administrador });
            usuarios.Add(new Usuario { Nombre = "juan", Contrasena = "abcd", Rol = RolUsuario.Usuario });
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

            Usuario usuarioEncontrado = null;
            foreach (var u in usuarios)
            {
                if (u.Nombre == nombreIntroducido && u.Contrasena == passwordIntroducida)
                {
                    usuarioEncontrado = u;
                    break;
                }
            }

            if (usuarioEncontrado == null)
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
        }
    }
}
