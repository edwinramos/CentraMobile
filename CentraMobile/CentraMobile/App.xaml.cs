using Acr.UserDialogs;
using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Dialogs;
using CentraMobile.Services;
using CentraMobile.Utils;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CentraMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SetConnectionSettings();
            MainPage = new NavigationPage(new Pages.LogIn());
        }

        protected async override void OnStart()
        {
            AppCenter.Start("a28508a2-622e-4e34-877c-1f03b9c86c81", typeof(Push));

            var users = await new DlUser().ReadAll();
            if (users.Any())
            {
                var usr = users.FirstOrDefault();
                StaticHelper.User = usr;
                switch (usr.MobileProfileType)
                {
                    case MobileProfileType.NULO:
                        //await DisplayAlert("Usuario no autorizado", "Este usuario no posee autorizacion en esta aplicacion. Solicite autorizacion antes de continuar.", "Ok");
                        break;
                    case MobileProfileType.PREVENTA:
                        AcrToast.Success($"¡Bienvenido {usr.Name}!", 2);
                        await new DlUser().Save(usr);
                        MainPage = new NavigationPage(new Pages.TabbedMainMenu());
                        break;
                    case MobileProfileType.TRANSPORTISTA:
                        AcrToast.Success($"¡Bienvenido {usr.Name}!", 2);
                        await new DlUser().Save(usr);
                        MainPage = new NavigationPage(new Pages.Transportist.TabbedMainMenuTransportist());
                        break;
                    default:
                        break;
                }
                
            }
            else
            {
                MainPage = new NavigationPage(new Pages.LogIn());
            }
        }
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #region Methods

        public void SetConnectionSettings()
        {
            Task.Run(async () => {
                StaticHelper.ServerAddress = (await new DlTable().ReadByCode("CONFIG", "SERVERADDRESS"))?.Value ?? "";
                StaticHelper.ServerPassword = (await new DlTable().ReadByCode("CONFIG", "SERVERPASSWORD"))?.Value ?? "";
            });

            var res = StaticHelper.ServerAddress;
        }
        #endregion
    }
}
