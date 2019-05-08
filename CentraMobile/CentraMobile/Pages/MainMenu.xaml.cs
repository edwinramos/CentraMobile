using Acr.UserDialogs;
using CentraMobile.DataLayer;
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
	public partial class MainMenu : ContentPage
	{
		public MainMenu ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override async void OnAppearing()
        {
            lblDate.Text = DateTime.Now.ToString("dddd d, MMMM");
        }

        private async void Button1_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrepareOrders());
        }

        private async void Button2_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClosedOrders());
        }

        private async void Button3_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }

        private void Button4_Clicked(object sender, EventArgs e)
        {
            BackBtnPressed();
        }

        protected override bool OnBackButtonPressed()
        {
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
    }
}