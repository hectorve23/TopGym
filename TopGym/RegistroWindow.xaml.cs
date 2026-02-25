using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace TopGym
{
    public partial class RegistroWindow : Window
    {
        public RegistroWindow()
        {
            InitializeComponent();
        }

        private void CrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string contrasena = txtPassword.Password;
            string confirmar = txtConfirmar.Password;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contrasena) || string.IsNullOrEmpty(confirmar))
            {
                MessageBox.Show("Por favor, rellena todos los campos", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (contrasena != confirmar)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (contrasena.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var u in MainWindow.usuarios)
            {
                if (u.Nombre.ToLower() == nombre.ToLower())
                {
                    MessageBox.Show("Ese nombre de usuario ya está en uso. Elige otro", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Contrasena = contrasena,
                Rol = RolUsuario.Usuario
            };

            MainWindow.usuarios.Add(nuevoUsuario);

            MessageBox.Show("Cuenta creada con éxito. Ya puedes iniciar sesion", "Registro completado", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
