using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CentraMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClosedOrderDetail : TabbedPage
    {
        private DlPriceList _dlPriceList { get; set; }
        private DlCustomer _dlCustomer { get; set; }
        private DlSellOrder _dlSellOrder { get; set; }
        private DlSellOrderDetail _dlSellOrderDetail { get; set; }
        private ObservableCollection<DeCustomer> _customers { get; set; }
        private ObservableCollection<DePriceList> _priceLists { get; set; }
        private ObservableCollection<DeSellOrderDetail> _orderDetail { get; set; }
        private string _priceListCode { get; set; }
        private string _customerCode { get; set; }
        private DeSellOrder _order { get; set; }
        public ClosedOrderDetail(DeSellOrder order)
        {
            InitializeComponent();
            _order = order;

            _priceListCode = _order.PriceListCode;
            _customerCode = _order.ClientCode;
            dpkrDocDate.Date = _order.DocDateTime;
            txtComments.Text = _order.Comments;

            _dlPriceList = new DlPriceList();
            _dlSellOrder = new DlSellOrder();
            _dlSellOrderDetail = new DlSellOrderDetail();
            _dlCustomer = new DlCustomer();
            _orderDetail = new ObservableCollection<DeSellOrderDetail>();
        }

        protected override async void OnAppearing()
        {
            _customers = await _dlCustomer.ReadAll();
            _priceLists = await _dlPriceList.ReadAll();
            pkrCustomer.ItemsSource = _customers.Select(x => x.BusinessPartnerDescription).ToList();

            pkrCustomer.SelectedItem = _order.ClientDescription;

            if(_order.SellOrderId > 0)
            {
                var list = (await _dlSellOrderDetail.ReadAll());//.Where(x => x.SellOrderId == _order.SellOrderId);
                foreach (var item in list.Where(x => x.SellOrderId == _order.SellOrderId))
                {
                    _orderDetail.Add(item);
                }
            }

            MyListView.ItemsSource = _orderDetail;
        }

        private void pkrCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedCustomer = _customers.FirstOrDefault(x=>x.BusinessPartnerDescription == pkrCustomer.SelectedItem.ToString());
            _priceListCode = selectedCustomer.PriceListCode;
            _customerCode = selectedCustomer.BusinessPartnerCode;

            pkrPriceList.ItemsSource = _priceLists.Where(x => x.PriceListCode == selectedCustomer.PriceListCode).Select(x => x.PriceListDescription).ToList();
            pkrPriceList.SelectedIndex = 0;
        }

        private async void btnUpload_Clicked(object sender, EventArgs e)
        {
            //await OrderSave();
        }

        private void DetailPage_Appearing(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_customerCode))
            {
                AcrToast.Warning("Cliente es requerido", 3);
                this.SelectedItem = 0;
            }
            if (string.IsNullOrEmpty(_priceListCode))
            {
                AcrToast.Warning("Lista de Precio es requerido", 3);
                this.SelectedItem = 0;
            }
        }

        private async void btnOpen_Clicked(object sender, EventArgs e)
        {
            var res = await DisplayAlert("Abir orden", "¿Seguro que desea abrir esta orden?", "Si", "No");
            if (res)
            {
                _order.IsClosed = false;
                await _dlSellOrder.Save(_order);
                await Navigation.PopAsync();
            }
        }

        #region Methods
        private async Task OrderSave()
        {
            if (_order.SellOrderId == 0)
                _order.SellOrderId = await _dlSellOrder.GetNextOrderId();

            var sellOrder = new DeSellOrder
            {
                SellOrderId = _order.SellOrderId,
                DocTotal = _orderDetail.Sum(x => x.Price * x.Quantity),
                DocDateTime = dpkrDocDate.Date,
                ClosedDateTime = new DateTime(1900, 1, 1),
                PriceListCode = _priceListCode,
                ClientCode = _customerCode,
                ClientDescription = pkrCustomer.SelectedItem.ToString(),
                Comments = txtComments.Text,
                IsClosed = _order.IsClosed
            };
            
            
            await _dlSellOrder.Save(sellOrder);

            await _dlSellOrderDetail.DeleteByOrder(_order.SellOrderId);
            var lineNumber = 1;
            foreach (var item in _orderDetail)
            {
                item.SellOrderId = _order.SellOrderId;
                item.LineNumber = lineNumber;
                await _dlSellOrderDetail.Save(item);

                lineNumber++;
            }

            AcrToast.Success("Guardado", 1);
            await Navigation.PopAsync();
        }
        #endregion
    }
}