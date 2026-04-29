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
    public partial class AdminWindow : Window
    {
        private Usuario adminActual;
        private Actividad actividadSeleccionada;

        public AdminWindow(Usuario admin)
        {
            InitializeComponent();
            adminActual = admin;
            ActualizarVista();
        }

        private void VerActividades_Click(object sender, RoutedEventArgs e)
        {
            panelActividades.Visibility = Visibility.Visible;
            panelInscritos.Visibility = Visibility.Collapsed;
            panelInformes.Visibility = Visibility.Collapsed;
            ActualizarVista();
        }

        private void VerInscritos_Click(object sender, RoutedEventArgs e)
        {
            panelActividades.Visibility = Visibility.Collapsed;
            panelInscritos.Visibility = Visibility.Visible;
            panelInformes.Visibility = Visibility.Collapsed;
            CargarComboActividades();
        }

        private void VerInformes_Click(object sender, RoutedEventArgs e)
        {
            InformeWindow informeWin = new InformeWindow();
            informeWin.ShowDialog();
        }

        /// <summary>
        /// Actualiza la vista del DataGrid con los datos de la base de datos
        /// </summary>
        private void ActualizarVista()
        {
            // Recargar actividades desde la base de datos para tener datos frescos
            RecargarActividadesDesdeBaseDatos();

            dgActividades.ItemsSource = null;
            dgActividades.ItemsSource = UsuarioWindow.Actividades;
        }

        /// <summary>
        /// Recarga todas las actividades desde la base de datos
        /// </summary>
        private void RecargarActividadesDesdeBaseDatos()
        {
            try
            {
                UsuarioWindow.Actividades.Clear();
                List<Actividad> actividadesBD = DatabaseConnection.ObtenerTodasActividades();

                foreach (var actividad in actividadesBD)
                {
                    // Cargar los usuarios inscritos en cada actividad
                    List<Usuario> inscritos = DatabaseConnection.ObtenerUsuariosDeActividad(actividad.IdActividad);
                    actividad.inscritos = new System.Collections.ObjectModel.ObservableCollection<Usuario>(inscritos);

                    UsuarioWindow.Actividades.Add(actividad);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al recargar actividades: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgActividades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            actividadSeleccionada = dgActividades.SelectedItem as Actividad;

            if (actividadSeleccionada != null)
            {
                txtNombre.Text = actividadSeleccionada.Nombre;
                txtDescripcion.Text = actividadSeleccionada.Descripcion;
                txtHorario.Text = actividadSeleccionada.Horario;
                txtPlazas.Text = actividadSeleccionada.PlazasTotal.ToString();
            }
        }

        private bool ValidarFormulario(out string nombre, out string descripcion, out string horario, out int plazas)
        {
            nombre = txtNombre.Text.Trim();
            descripcion = txtDescripcion.Text.Trim();
            horario = txtHorario.Text.Trim();
            plazas = 0;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(descripcion) ||
                string.IsNullOrEmpty(horario) || string.IsNullOrEmpty(txtPlazas.Text))
            {
                MessageBox.Show("Por favor, rellena todos los campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtPlazas.Text, out plazas) || plazas <= 0)
            {
                MessageBox.Show("Las plazas deben ser un número entero mayor que 0.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtHorario.Text = "";
            txtPlazas.Text = "";
            actividadSeleccionada = null;
            dgActividades.SelectedItem = null;
        }

        private void Añadir_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario(out string nombre, out string descripcion, out string horario, out int plazas))
                return;

            // Insertar en la base de datos
            bool resultado = DatabaseConnection.InsertarActividad(nombre, descripcion, horario, plazas);

            if (resultado)
            {
                LimpiarFormulario();
                ActualizarVista();
                MessageBox.Show($"Actividad '{nombre}' añadida correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Si falla, DatabaseConnection ya muestra el mensaje de error
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (actividadSeleccionada == null)
            {
                MessageBox.Show("Selecciona una actividad del listado para editarla.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidarFormulario(out string nombre, out string descripcion, out string horario, out int plazas))
                return;

            // Validar que no se reduzcan las plazas por debajo del número de inscritos
            int numeroInscritos = actividadSeleccionada.inscritos.Count;
            if (plazas < numeroInscritos)
            {
                MessageBox.Show($"No puedes reducir las plazas por debajo del número de inscritos ({numeroInscritos}).",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Actualizar en la base de datos
            bool resultado = DatabaseConnection.ActualizarActividad(
                actividadSeleccionada.IdActividad,
                nombre,
                descripcion,
                horario,
                plazas);

            if (resultado)
            {
                LimpiarFormulario();
                ActualizarVista();
                MessageBox.Show("Actividad actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (actividadSeleccionada == null)
            {
                MessageBox.Show("Selecciona una actividad del listado para eliminarla.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show(
                $"¿Seguro que quieres eliminar la actividad '{actividadSeleccionada.Nombre}'?\nSe dará de baja a todos los inscritos automáticamente.",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                // Eliminar de la base de datos (las inscripciones se eliminan automáticamente por CASCADE)
                bool exito = DatabaseConnection.EliminarActividad(actividadSeleccionada.IdActividad);

                if (exito)
                {
                    LimpiarFormulario();
                    ActualizarVista();
                    MessageBox.Show("Actividad eliminada.", "Hecho", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CargarComboActividades()
        {
            // Recargar actividades antes de llenar el combo
            RecargarActividadesDesdeBaseDatos();

            cmbActividades.Items.Clear();
            foreach (var act in UsuarioWindow.Actividades)
                cmbActividades.Items.Add(act.Nombre);

            if (cmbActividades.Items.Count > 0)
                cmbActividades.SelectedIndex = 0;
        }

        private void CmbActividades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbActividades.SelectedIndex < 0) return;

            var actividadSeleccionada = UsuarioWindow.Actividades[cmbActividades.SelectedIndex];

            // Recargar los inscritos desde la base de datos para tener datos actualizados
            List<Usuario> inscritos = DatabaseConnection.ObtenerUsuariosDeActividad(actividadSeleccionada.IdActividad);
            actividadSeleccionada.inscritos = new System.Collections.ObjectModel.ObservableCollection<Usuario>(inscritos);

            dgInscritos.ItemsSource = actividadSeleccionada.inscritos;
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("¿Seguro que quieres cerrar sesión?",
                "Cerrar sesión", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                MainWindow login = new MainWindow();
                login.Show();
                this.Close();
            }
        }
    }
}
