using CentraMobile.Models;
using CentraMobile.Utils;
using CentraMobile.ViewModels;
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
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            BindingContext = new SettingsVM();
        }

        private async void MyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (ListElementModel)e.Item;

            if (item.Title == "Configuracion de servidor")
                await Navigation.PushAsync(new ServerSettings());

            if (item.Title == "Descargar datos")
                await StaticHelper.DownloadData();
        }
    }
}