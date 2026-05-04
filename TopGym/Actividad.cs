using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopGym
{
    public class Actividad : INotifyPropertyChanged
    {
        public int IdActividad { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Horario { get; set; }
        public int PlazasTotal { get; set; }
        public ObservableCollection<Usuario> inscritos { get; set; } = new ObservableCollection<Usuario>();

        public int PlazasLibres => PlazasTotal - inscritos.Count;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
