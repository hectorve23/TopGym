using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.IO;
using System.Windows;

namespace TopGym
{
    public partial class InformeWindow : Window
    {
        private ReportDocument reporte;

        public InformeWindow()
        {
            InitializeComponent();
            CargarInforme();
        }

        private void CargarInforme()
        {
            DsInformeActividades ds = new DsInformeActividades();

            foreach (var act in UsuarioWindow.Actividades)
            {
                DsInformeActividades.TablaActividadesRow fila = ds.TablaActividades.NewTablaActividadesRow();
                fila.Nombre = act.Nombre;
                fila.Descripcion = act.Descripcion;
                fila.Horario = act.Horario;
                fila.PlazasTotal = act.PlazasTotal;
                fila.PlazasLibres = act.PlazasLibres;
                fila.TotalInscritos = act.inscritos.Count;
                ds.TablaActividades.Rows.Add(fila);
            }

            reporte = new ReportDocument();
            reporte.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InformeActividades.rpt"));
            reporte.SetDataSource(ds);
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rutaTemporal = Path.Combine(Path.GetTempPath(), "InformeActividades_temp.pdf");
                reporte.ExportToDisk(ExportFormatType.PortableDocFormat, rutaTemporal);
                System.Diagnostics.Process.Start(rutaTemporal);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}