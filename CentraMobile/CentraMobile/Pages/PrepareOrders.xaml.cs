using Acr.UserDialogs;
using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CentraMobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrepareOrders : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        private PrepareOrdersVM vm = new PrepareOrdersVM(false);
        public PrepareOrders()
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            using (var dlg = UserDialogs.Instance.Loading("Cargando..."))
            {
                vm.LoadData(false);
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var order = (DeSellOrder)e.Item;

            await Navigation.PushAsync(new PrepareOrderDetail(order));

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var item = menuItem.BindingContext as DeSellOrder;

                var res = await DisplayAlert("Abir orden", "¿Seguro que desea cerrar esta orden?", "Si", "No");
                if (res)
                {
                    item.IsClosed = true;
                    await new DlSellOrder().Save(item);

                    vm.LoadData(false);
                }
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrepareOrderDetail(new DeSellOrder { DocDateTime = DateTime.Today }));
        }
    }
}
