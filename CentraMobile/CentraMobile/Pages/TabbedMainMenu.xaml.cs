using Acr.UserDialogs;
using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Dialogs;
using CentraMobile.Models;
using CentraMobile.Services;
using CentraMobile.Utils;
using CentraMobile.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CentraMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedMainMenu : TabbedPage
    {
        private MenuVM vm = new MenuVM();
        public TabbedMainMenu ()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            using (var dlg = UserDialogs.Instance.Loading("Cargando..."))
            {
                vm.LoadData();
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var order = (DeSellOrder)e.Item;

            if (!order.IsClosed)
                await Navigation.PushAsync(new PrepareOrderDetail(order));
            else
                await Navigation.PushAsync(new ClosedOrderDetail(order));

            //Unselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async void Remove_Clicked(object sender, EventArgs e)
        {
            using (var dlg = UserDialogs.Instance.Loading("Cargando..."))
            {
                var menuItem = sender as MenuItem;

                if (menuItem != null)
                {
                    var item = menuItem.BindingContext as DeSellOrder;

                    var res = await DisplayAlert("Abir orden", "¿Seguro que desea eliminar esta orden?", "Si", "No");
                    if (res)
                    {
                        await new DlSellOrder().Delete(item.SellOrderId);

                        var list = (await new DlSellOrderDetail().ReadAll()).Where(x => x.SellOrderId == item.SellOrderId);

                        await new DlSellOrderDetail().DeleteByOrder(item.SellOrderId);

                        vm.LoadData();
                        AcrToast.Info("Orden Eliminada", 2);
                    }
                }
            }
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var item = menuItem.BindingContext as DeSellOrder;

                var res = await DisplayAlert("Abir orden", "¿Seguro que desea terminar esta orden?", "Si", "No");
                if (res)
                {
                    item.IsClosed = true;
                    await new DlSellOrder().Save(item);

                    vm.LoadData();
                }
            }
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrepareOrderDetail(new DeSellOrder { DocDateTime = DateTime.Today }));
        }

        private async void ReOpen_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var item = menuItem.BindingContext as DeSellOrder;

                var res = await DisplayAlert("Abir orden", "¿Seguro que desea abrir esta orden?", "Si", "No");
                if (res)
                {
                    item.IsClosed = false;
                    await new DlSellOrder().Save(item);

                    vm.LoadData();
                }
            }

        }

        private async void Send_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            using (var dlg = UserDialogs.Instance.Loading("Cargando..."))
            {
                if (menuItem != null)
                {
                    var item = menuItem.BindingContext as DeSellOrder;
                    var detail = (await new DlSellOrderDetail().ReadAll()).Where(x => x.SellOrderId == item.SellOrderId).ToList();
                    var restService = new RestService(StaticHelper.ServerAddress);

                    var detailList = new List<SellOrderDetailModel>();
                    var objHead = new SellOrderModel();
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
                    objHead.VatSum = item.VatSum;
                    objHead.WarehouseCode = item.WarehouseCode;
                    objHead.StorePosCode = item.StorePosCode;

                    foreach (var orderItem in detail)
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
                        var response = await restService.PostResponse<Tuple<SellOrderModel, List<SellOrderDetailModel>>>("mob/save_sell_order?password=" + StaticHelper.ServerPassword + "&userCode=" + StaticHelper.User.UserCode, jsonObj);

                        if (response)
                        {
                            AcrToast.Success("¡Orden Cargada!", 2);
                            await new DlSellOrder().Delete(item.SellOrderId);

                            var list = (await new DlSellOrderDetail().ReadAll()).Where(x => x.SellOrderId == item.SellOrderId);
                            await new DlSellOrderDetail().DeleteByOrder(item.SellOrderId);
                            vm.LoadData();
                        }
                        else
                            AcrToast.Error("Error al cargar la orden", 2);
                    }
                }
            }
        }

        #region Settings

        private async void lvSettings_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (ListElementModel)e.Item;

            if (item.Title == "Configuracion de servidor")
                await Navigation.PushAsync(new ServerSettings());

            if (item.Title == "Descargar datos")
            {
                if (await DisplayAlert("Advertencia", "Este proceso puede tardar varios minutos¿Desea continuar?", "Si", "No"))
                {
                    await StaticHelper.DownloadData();
                }
            }

            if (item.Title == "Salir")
                BackBtnPressed();
        }

        #endregion

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            BackBtnPressed();
            return true;
        }

        private async void BackBtnPressed()
        {
            if (await DisplayAlert("Cerrar sesión", "¿Seguro desea salir?", "Si", "No"))
            {
                var dlUser = new DlUser();
                foreach (var itm in await dlUser.ReadAll())
                {
                    await dlUser.Delete(itm.UserCode);
                }
                await Navigation.PopAsync();
            }
        }

        private void TabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            this.Title = this.CurrentPage.Title;
        }
    }
}