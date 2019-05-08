using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Models;
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
    public class PrepareOrdersVM : INotifyPropertyChanged
    {
        public ObservableCollection<DeSellOrder> _orders;
        public ObservableCollection<DeSellOrder> Orders {
            get { return _orders; }
            set { _orders = value; OnPropertyChanged("Orders");
            }
        }

        private DlSellOrder _dlSellOrder { get; set; }
        public PrepareOrdersVM(bool isClosed)
        {
            Orders = new ObservableCollection<DeSellOrder>();
            _dlSellOrder = new DlSellOrder();

            LoadData(isClosed);
        }

        public void LoadData(bool isClosed)
        {
            Task.Run(async () =>
            {
                Orders.Clear();
                var list = await _dlSellOrder.ReadAll();
                foreach (var order in list.Where(x => x.IsClosed == isClosed))
                {
                    Orders.Add(order);
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
