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
using System.Windows.Shapes;

namespace TopGym
{
    public partial class UsuarioWindow : Window
    {
        private Usuario usuarioActual;

        // Lista de actividades compartida con el admin
        public static ObservableCollection<Actividad> Actividades = new ObservableCollection<Actividad>();

        public UsuarioWindow(Usuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;
            txtBienvenida.Text = $"Hola, {usuario.Nombre}";

            CargarPlanes();
            ActualizarVista();
        }

        private void CargarPlanes()
        {
            cmbCategoria.Items.Add("Fuerza");
            cmbCategoria.Items.Add("Cardio");
            cmbCategoria.Items.Add("Flexibilidad");
            cmbCategoria.SelectedIndex = 0;
        }

        private void VerActividades_Click(object sender, RoutedEventArgs e)
        {
            panelActividades.Visibility = Visibility.Visible;
            panelPlanes.Visibility = Visibility.Collapsed;
            ActualizarVista();
        }

        private void VerPlanes_Click(object sender, RoutedEventArgs e)
        {
            panelActividades.Visibility = Visibility.Collapsed;
            panelPlanes.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Actualiza la vista con datos frescos de la base de datos
        /// </summary>
        private void ActualizarVista()
        {
            // Recargar actividades desde la base de datos
            RecargarActividadesDesdeBaseDatos();

            // Recargar las actividades del usuario actual
            RecargarActividadesUsuario();

            // Actualizar el DataGrid
            dgActividades.ItemsSource = null;
            dgActividades.ItemsSource = Actividades;

            // Actualizar la lista de "Mis Actividades"
            lstMisActividades.Items.Clear();
            foreach (var act in usuarioActual.ActividadesInscritas)
                lstMisActividades.Items.Add($"{act.Nombre} — {act.Horario}");
        }

        /// <summary>
        /// Recarga todas las actividades desde la base de datos
        /// </summary>
        private void RecargarActividadesDesdeBaseDatos()
        {
            try
            {
                Actividades.Clear();
                List<Actividad> actividadesBD = DatabaseConnection.ObtenerTodasActividades();

                foreach (var actividad in actividadesBD)
                {
                    // Cargar los usuarios inscritos en cada actividad
                    List<Usuario> inscritos = DatabaseConnection.ObtenerUsuariosDeActividad(actividad.IdActividad);
                    actividad.inscritos = new ObservableCollection<Usuario>(inscritos);

                    Actividades.Add(actividad);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al recargar actividades: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Recarga las actividades inscritas del usuario actual
        /// </summary>
        private void RecargarActividadesUsuario()
        {
            try
            {
                usuarioActual.ActividadesInscritas.Clear();
                List<Actividad> actividadesUsuario = DatabaseConnection.ObtenerActividadesDeUsuario(usuarioActual.IdUsuario);

                foreach (var act in actividadesUsuario)
                {
                    usuarioActual.ActividadesInscritas.Add(act);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al recargar tus actividades: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Apuntarme_Click(object sender, RoutedEventArgs e)
        {
            var seleccionada = dgActividades.SelectedItem as Actividad;
            if (seleccionada == null)
            {
                MessageBox.Show("Selecciona una actividad primero", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verificar si ya está inscrito (verificar en la base de datos)
            if (usuarioActual.ActividadesInscritas.Any(a => a.IdActividad == seleccionada.IdActividad))
            {
                MessageBox.Show("Ya estás apuntado a esta actividad", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verificar plazas disponibles
            if (seleccionada.PlazasLibres <= 0)
            {
                MessageBox.Show("No quedan plazas disponibles", "Sin plazas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Inscribir en la base de datos
            bool resultado = DatabaseConnection.InscribirUsuarioEnActividad(
                usuarioActual.IdUsuario,
                seleccionada.IdActividad);

            if (resultado)
            {
                ActualizarVista(); // Recargar datos
                MessageBox.Show($"Te has apuntado a {seleccionada.Nombre}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Si falla, DatabaseConnection ya muestra el mensaje de error
        }

        private void DarmeDeBaja_Click(object sender, RoutedEventArgs e)
        {
            var seleccionada = dgActividades.SelectedItem as Actividad;
            if (seleccionada == null)
            {
                MessageBox.Show("Selecciona una actividad primero", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verificar si está inscrito
            if (!usuarioActual.ActividadesInscritas.Any(a => a.IdActividad == seleccionada.IdActividad))
            {
                MessageBox.Show("No estás apuntado a esta actividad.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show($"¿Seguro que quieres darte de baja de {seleccionada.Nombre}?",
                "Confirmar baja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                // Desinscribir de la base de datos
                bool exito = DatabaseConnection.DesinscribirUsuarioDeActividad(
                    usuarioActual.IdUsuario,
                    seleccionada.IdActividad);

                if (exito)
                {
                    ActualizarVista(); // Recargar datos
                    MessageBox.Show("Te has dado de baja correctamente", "Hecho", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CmbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstPlanes.Items.Clear();

            switch (cmbCategoria.SelectedIndex)
            {
                case 0:
                    lstPlanes.Items.Add("Día 1 — Pecho y tríceps: Press banca 4x10, Fondos 3x12, Extensiones 3x15");
                    lstPlanes.Items.Add("Día 2 — Espalda y bíceps: Dominadas 4x8, Remo 4x10, Curl barra 3x12");
                    lstPlanes.Items.Add("Día 3 — Piernas: Sentadilla 4x10, Prensa 3x12, Extensiones 3x15");
                    lstPlanes.Items.Add("Día 4 — Hombros: Press militar 4x10, Elevaciones laterales 3x15");
                    break;
                case 1:
                    lstPlanes.Items.Add("Día 1 — Cinta: 30 min ritmo moderado (zona 2)");
                    lstPlanes.Items.Add("Día 2 — Bicicleta: 20 min HIIT (1 min fuerte / 1 min suave)");
                    lstPlanes.Items.Add("Día 3 — Elíptica: 40 min ritmo constante");
                    lstPlanes.Items.Add("Día 4 — Remo: 15 min + saltar a la comba 10 min");
                    break;
                case 2:
                    lstPlanes.Items.Add("Día 1 — Estiramientos globales: 45 min yoga básico");
                    lstPlanes.Items.Add("Día 2 — Movilidad cadera y columna: 30 min");
                    lstPlanes.Items.Add("Día 3 — Pilates suelo: Core y postura, 40 min");
                    lstPlanes.Items.Add("Día 4 — Foam roller + estiramientos profundos: 30 min");
                    break;
            }
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
