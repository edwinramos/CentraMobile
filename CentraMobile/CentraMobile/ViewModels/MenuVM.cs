using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Models;
using CentraMobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CentraMobile.ViewModels
{
    public class MenuVM : INotifyPropertyChanged
    {
        public ObservableCollection<DeSellOrder> _orders;
        public ObservableCollection<DeSellOrder> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value; OnPropertyChanged("Orders");
            }
        }
        public ObservableCollection<DeSellOrder> _closedOrders;
        public ObservableCollection<DeSellOrder> ClosedOrders
        {
            get { return _closedOrders; }
            set
            {
                _closedOrders = value; OnPropertyChanged("ClosedOrders");
            }
        }

        public ObservableCollection<ListElementModel> SettingsItems { get; set; }

        private DlSellOrder _dlSellOrder { get; set; }
        public MenuVM()
        {
            Orders = new ObservableCollection<DeSellOrder>();
            ClosedOrders = new ObservableCollection<DeSellOrder>();
            _dlSellOrder = new DlSellOrder();

            SettingsItems = new ObservableCollection<ListElementModel>();
            if (StaticHelper.User.MobileProfileType == MobileProfileType.PREVENTA)
                SettingsItems.Add(new ListElementModel { Title = "Logros de hoy", Description = "Visualiza las ordenes que haz completado en el dia" });

            SettingsItems.Add(new ListElementModel { Title = "Configuracion de servidor", Description = "Maneje los parametros de coneccion al servidor" });
            SettingsItems.Add(new ListElementModel { Title = "Descargar datos", Description = "Actualice los articulos y precios en su dispositivo" });
            SettingsItems.Add(new ListElementModel { Title = "Salir", Description = "Desconectarse de la aplicacion" });

            LoadData();
        }

        public void LoadData()
        {
            Task.Run(async () =>
            {
                Orders.Clear();
                ClosedOrders.Clear();

                var list = await _dlSellOrder.ReadAll();
                foreach (var order in list)
                {
                    if (!order.IsClosed)
                        Orders.Add(order);
                    else
                        ClosedOrders.Add(order);
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
