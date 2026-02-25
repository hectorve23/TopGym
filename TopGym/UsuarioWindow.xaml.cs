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

            CargarActividadesPrueba();
            CargarPlanes();
            ActualizarVista();
        }

        //DATOS DE PRUEBA

        private void CargarActividadesPrueba()
        {
            if (Actividades.Count == 0)
            {
                Actividades.Add(new Actividad { Nombre = "Yoga", Descripcion = "Relax y flexibilidad", Horario = "Lunes 10:00", PlazasTotal = 15 });
                Actividades.Add(new Actividad { Nombre = "Spinning", Descripcion = "Cardio intenso en bici", Horario = "Martes 18:00", PlazasTotal = 20 });
                Actividades.Add(new Actividad { Nombre = "Zumba", Descripcion = "Baile y diversión", Horario = "Miercoles 19:00", PlazasTotal = 25 });
                Actividades.Add(new Actividad { Nombre = "Pilates", Descripcion = "Core y postura", Horario = "Viernes 11:00", PlazasTotal = 12 });
            }
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

        private void ActualizarVista()
        {
            dgActividades.ItemsSource = null;
            dgActividades.ItemsSource = Actividades;

            lstMisActividades.Items.Clear();
            foreach (var act in usuarioActual.ActividadesInscritas)
                lstMisActividades.Items.Add($"{act.Nombre} — {act.Horario}");
        }

        private void Apuntarme_Click(object sender, RoutedEventArgs e)
        {
            var seleccionada = dgActividades.SelectedItem as Actividad;
            if (seleccionada == null)
            {
                MessageBox.Show("Selecciona una actividad primero", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (usuarioActual.ActividadesInscritas.Contains(seleccionada))
            {
                MessageBox.Show("Ya estas apuntado a esta actividad", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (seleccionada.PlazasLibres <= 0)
            {
                MessageBox.Show("No quedan plazas disponibles", "Sin plazas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            seleccionada.inscritos.Add(usuarioActual);
            usuarioActual.ActividadesInscritas.Add(seleccionada);
            ActualizarVista();
            MessageBox.Show($"Te has apuntado a {seleccionada.Nombre}", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DarmeDeBaja_Click(object sender, RoutedEventArgs e)
        {
            var seleccionada = dgActividades.SelectedItem as Actividad;
            if (seleccionada == null)
            {
                MessageBox.Show("Selecciona una actividad primero", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!usuarioActual.ActividadesInscritas.Contains(seleccionada))
            {
                MessageBox.Show("No estas apuntado a esta actividad.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show($"¿Seguro que quieres darte de baja de {seleccionada.Nombre}?",
                "Confirmar baja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                seleccionada.inscritos.Remove(usuarioActual);
                usuarioActual.ActividadesInscritas.Remove(seleccionada);
                ActualizarVista();
            }
        }

        private void CmbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstPlanes.Items.Clear();

            switch (cmbCategoria.SelectedIndex)
            {
                case 0: 
                    lstPlanes.Items.Add("Día 1 — Pecho y triceps: Press banca 4x10, Fondos 3x12, Extensiones 3x15");
                    lstPlanes.Items.Add("Día 2 — Espalda y biceps: Dominadas 4x8, Remo 4x10, Curl barra 3x12");
                    lstPlanes.Items.Add("Día 3 — Piernas: Sentadilla 4x10, Prensa 3x12, Extensiones 3x15");
                    lstPlanes.Items.Add("Día 4 — Hombros: Press militar 4x10, Elevaciones laterales 3x15");
                    break;
                case 1: 
                    lstPlanes.Items.Add("Día 1 — Cinta: 30 min ritmo moderado (zona 2)");
                    lstPlanes.Items.Add("Día 2 — Bicicleta: 20 min HIIT (1 min fuerte / 1 min suave)");
                    lstPlanes.Items.Add("Día 3 — Eliptica: 40 min ritmo constante");
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
            var resultado = MessageBox.Show("¿Seguro que quieres cerrar sesion?",
                "Cerrar sesion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                MainWindow login = new MainWindow();
                login.Show();
                this.Close();
            }
        }
    }
}
