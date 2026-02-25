using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopGym
{
    public enum RolUsuario
    {
        Administrador,
        Usuario
    }

    public class Usuario
    {
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public RolUsuario Rol { get; set; }
        public ObservableCollection<Actividad> ActividadesInscritas { get; set; } = new ObservableCollection<Actividad>();
    }
}
