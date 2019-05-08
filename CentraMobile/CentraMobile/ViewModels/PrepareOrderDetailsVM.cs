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
using Xamarin.Forms;

namespace CentraMobile.ViewModels
{
    public class PrepareOrderDetailsVM : INotifyPropertyChanged
    {
        public ObservableCollection<DeSellOrderDetail> OrderDetail { get; set; }
        private DlSellOrderDetail _dlSellOrderDetail { get; set; }

        public Command AddItemCommand { get; private set; }

        public PrepareOrderDetailsVM()
        {
            OrderDetail = new ObservableCollection<DeSellOrderDetail>();
            _dlSellOrderDetail = new DlSellOrderDetail();
            AddItemCommand = new Command<string>(CalculateSquareRoot);

            Task.Run(async () =>
            {
                var list = await _dlSellOrderDetail.ReadAll();
                foreach (var order in list)
                {
                    OrderDetail.Add(order);
                }
            });
        }

        public PrepareOrderDetailsVM(DeSellOrder _order)
        {
            OrderDetail = new ObservableCollection<DeSellOrderDetail>();
            _dlSellOrderDetail = new DlSellOrderDetail();
            AddItemCommand = new Command<string>(CalculateSquareRoot);

            Task.Run(async () =>
            {
                var list = (await _dlSellOrderDetail.ReadAll()).Where(x => x.SellOrderId == _order.SellOrderId).ToList();
                
                foreach (var order in list)
                {
                    OrderDetail.Add(order);
                }
            });
        }

        async void CalculateSquareRoot(string value)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
