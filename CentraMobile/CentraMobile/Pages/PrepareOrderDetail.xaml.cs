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
    public partial class PrepareOrderDetail : TabbedPage
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
        public PrepareOrderDetail (DeSellOrder order)
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

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            await OrderSave();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_customerCode))
            {
                AcrToast.Warning("Cliente es requerido", 3);
                return;
            }
            if (string.IsNullOrEmpty(_priceListCode))
            {
                AcrToast.Warning("Lista de Precio es requerido", 3);
                return;
            }

            if (string.IsNullOrEmpty(txtBarcode.Text) || string.IsNullOrEmpty(txtQuantity.Text))
                AcrToast.Info("Por favor completar los campos", 2);
            else
                await AddItemDetail();
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            var res = await DisplayAlert("Cerrar orden", "¿Seguro que desea cerrar esta orden?", "Si", "No");
            if (res)
            {
                _order.IsClosed = true;
                await OrderSave();
            }
        }

        private async void MyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (DeSellOrderDetail)e.Item;
            var res = await DisplayAlert("Elimiar articulo", "¿Desea eliminar este articulo?", "Si", "No");
            if (res)
            {
                var obj = _orderDetail.FirstOrDefault(x=>x.ItemCode == item.ItemCode);
                _orderDetail.Remove(obj);

                MyListView.ItemsSource = _orderDetail;
            }

            MyListView.SelectedItem = null;
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

        private async Task AddItemDetail()
        {
            var items = (await new DlItem().ReadAll()).Where(x => x.ItemCode == txtBarcode.Text || x.Barcode == txtBarcode.Text || x.ItemDescription.ToUpper().Contains(txtBarcode.Text.ToUpper()));

            if (items.Count() > 1)
            {
                var strings = items.Select(x => x.ItemDescription).ToArray();
                var res = await DisplayActionSheet("Seleccione articulo", "Cancel", null, strings);

                if (res != "Cancel")
                {
                    var item = items.FirstOrDefault(x => x.ItemDescription == res);
                    var price = await new DlPrice().ReadByCode(item.ItemCode, _priceListCode);
                    var priceVal = price?.SellPrice ?? 0;

                    var obj = _orderDetail.FirstOrDefault(x => x.ItemCode == item.ItemCode);
                    if (obj == null)
                    {
                        _orderDetail.Add(new DeSellOrderDetail
                        {
                            ItemCode = item.ItemCode,
                            ItemDescription = item.ItemDescription,
                            Quantity = int.Parse(txtQuantity.Text),
                            Price = priceVal,
                            VatPercent = item.TaxValue,
                            VatValue = (priceVal * (item.TaxValue / 100)),
                            PriceAftVat = priceVal + (priceVal * (item.TaxValue / 100)),
                            PriceBefDiscounts = priceVal + (priceVal * (item.TaxValue / 100)),
                        });
                        AcrToast.Success("Agregado " + res, 4);

                        txtQuantity.Text = "1";
                        txtBarcode.Text = "";
                    }
                    else
                    {
                        await DisplayAlert("", "Este articulo ya existe", "Ok");
                        //foreach (var detail in _orderDetail)
                        //{
                        //    if (detail.ItemCode == item.ItemCode)
                        //    {
                        //        detail.Quantity += 1;
                        //    }
                        //}
                    }
                }
            }
            else
            {
                var item = items.FirstOrDefault();

                var price = await new DlPrice().ReadByCode(item.ItemCode, _priceListCode);

                var obj = _orderDetail.FirstOrDefault(x => x.ItemCode == item.ItemCode);
                if (obj == null)
                {
                    _orderDetail.Add(new DeSellOrderDetail
                    {
                        ItemCode = item.ItemCode,
                        ItemDescription = item.ItemDescription,
                        Quantity = int.Parse(txtQuantity.Text),
                        Price = price?.SellPrice ?? 0,
                        VatPercent = item.TaxValue,
                        VatValue = ((price?.SellPrice ?? 0) * (item.TaxValue / 100)),
                        PriceAftVat = (price?.SellPrice ?? 0) + ((price?.SellPrice ?? 0) * (item.TaxValue / 100)),
                        PriceBefDiscounts = (price?.SellPrice ?? 0) + ((price?.SellPrice ?? 0) * (item.TaxValue / 100)),
                    });
                    AcrToast.Success("Agregado " + item.ItemDescription, 2);

                    txtQuantity.Text = "1";
                    txtBarcode.Text = "";
                }
                else
                {
                    await DisplayAlert("", "Este articulo ya existe", "Ok");
                    //foreach (var detail in _orderDetail)
                    //{
                    //    if (detail.ItemCode == item.ItemCode)
                    //    {
                    //        detail.Quantity += 1;
                    //    }
                    //}
                }
            }

            MyListView.ItemsSource = _orderDetail;
        }
        #endregion
    }
}