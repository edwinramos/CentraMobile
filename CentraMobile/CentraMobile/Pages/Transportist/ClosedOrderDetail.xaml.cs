using Acr.UserDialogs;
using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Dialogs;
using CentraMobile.Models;
using CentraMobile.Services;
using CentraMobile.Utils;
using Newtonsoft.Json;
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

namespace CentraMobile.Pages.Transportist
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

            if (_order.SellOrderId > 0)
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
            var selectedCustomer = _customers.FirstOrDefault(x => x.BusinessPartnerDescription == pkrCustomer.SelectedItem.ToString());
            _priceListCode = selectedCustomer.PriceListCode;
            _customerCode = selectedCustomer.BusinessPartnerCode;

            pkrPriceList.ItemsSource = _priceLists.Where(x => x.PriceListCode == selectedCustomer.PriceListCode).Select(x => x.PriceListDescription).ToList();
            pkrPriceList.SelectedIndex = 0;
        }

        private async void btnUpload_Clicked(object sender, EventArgs e)
        {
            using (var dlg = UserDialogs.Instance.Loading("Cargando..."))
            {
                var item = _order;
                var restService = new RestService(StaticHelper.ServerAddress);

                var detailList = new List<SellOrderDetailModel>();
                var objHead = new SellOrderModel();
                objHead.SellOrderId = item.SellOrderId;
                objHead.PriceListCode = item.PriceListCode;
                objHead.PaymentTypeCode = item.PaymentTypeCode;
                objHead.IsClosed = false;
                objHead.ExternalReference = item.ExternalReference;
                objHead.ClientCode = item.ClientCode;
                objHead.ClientDescription = item.ClientDescription;
                objHead.ClosedDateTime = item.ClosedDateTime;
                objHead.Comments = item.Comments;
                objHead.DocDateTime = item.DocDateTime;
                objHead.DocNetTotal = item.DocNetTotal;
                objHead.DocTotal = item.DocTotal;
                objHead.StoreCode = item.StoreCode;
                objHead.TotalDiscount = item.TotalDiscount;
                objHead.VatSum = _orderDetail.Sum(x => x.VatValue);
                objHead.WarehouseCode = item.WarehouseCode;
                objHead.StorePosCode = item.StorePosCode;

                foreach (var orderItem in _orderDetail)
                {
                    detailList.Add(new SellOrderDetailModel
                    {
                        SellOrderId = orderItem.SellOrderId,
                        ItemCode = orderItem.ItemCode,
                        ItemDescription = orderItem.ItemDescription,
                        Barcode = orderItem.Barcode,
                        IsClosed = orderItem.IsClosed,
                        LineNumber = orderItem.LineNumber,
                        DiscountValue = orderItem.DiscountValue,
                        ExternalCode = orderItem.ExternalCode,
                        Price = orderItem.Price,
                        PriceAftVat = orderItem.PriceAftVat,
                        PriceBefDiscounts = orderItem.PriceBefDiscounts,
                        Quantity = orderItem.Quantity,
                        TotalRowValue = orderItem.TotalRowValue,
                        VatCode = orderItem.VatCode,
                        VatPercent = orderItem.VatPercent,
                        VatValue = orderItem.VatValue,
                        WarehouseCode = orderItem.WarehouseCode
                    });
                }

                var res = await DisplayAlert("Abir orden", "¿Seguro que desea enviar esta orden?", "Si", "No");
                if (res)
                {
                    var tuple = new Tuple<SellOrderModel, List<SellOrderDetailModel>>(objHead, detailList);
                    var jsonObj = JsonConvert.SerializeObject(tuple);
                    var response = await restService.PostResponse<Tuple<SellOrderModel, List<SellOrderDetailModel>>>("mob/update_sell_order?password=" + StaticHelper.ServerPassword + "&userCode=" + StaticHelper.User.UserCode, jsonObj);

                    if (response)
                    {
                        AcrToast.Success("¡Orden Cargada!", 2);
                        await new DlSellOrder().Delete(item.SellOrderId);

                        var list = (await new DlSellOrderDetail().ReadAll()).Where(x => x.SellOrderId == item.SellOrderId);
                        await new DlSellOrderDetail().DeleteByOrder(item.SellOrderId);
                        await Navigation.PopAsync();
                    }
                    else
                        AcrToast.Error("Error al cargar la orden", 2);
                }
            }
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