using CentraMobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CentraMobile.ViewModels
{
    public class SettingsVM : INotifyPropertyChanged
    {
        public ObservableCollection<ListElementModel> Items { get; set; }

        public SettingsVM() {
            Items = new ObservableCollection<ListElementModel>();
            Items.Add(new ListElementModel { Title = "Configuracion de servidor", Description = "Maneje los parametros de coneccion al servidor" });
            Items.Add(new ListElementModel { Title = "Descargar datos", Description = "Actualice los articulos y precios en su dispositivo" });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
