using CentraMobile.Services;
using System;
using Acr.UserDialogs;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CentraMobile.Dialogs;
using CentraMobile.DataLayer;
using CentraMobile.DataEntities;
using Newtonsoft.Json;
using CentraMobile.Utils;
using Plugin.LocalNotifications;

namespace CentraMobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogIn : ContentPage
	{
		public LogIn ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override async void OnAppearing()
        {
            //var users = await new DlUser().ReadAll();
            //if (users.Any())
            //{
            //    var usr = users.FirstOrDefault();
            //    StaticHelper.User = usr;
            //    switch (usr.MobileProfileType)
            //    {
            //        case MobileProfileType.NULO:
            //            await DisplayAlert("Usuario no autorizado", "Este usuario no posee autorizacion en esta aplicacion. Solicite autorizacion antes de continuar.", "Ok");
            //            break;
            //        case MobileProfileType.PREVENTA:
            //            AcrToast.Success($"¡Bienvenido {usr.Name}!", 2);
            //            await new DlUser().Save(usr);
            //            await Navigation.PushAsync(new TabbedMainMenu());
            //            break;
            //        case MobileProfileType.TRANSPORTISTA:
            //            AcrToast.Success($"¡Bienvenido {usr.Name}!", 2);
            //            await new DlUser().Save(usr);
            //            await Navigation.PushAsync(new Transportist.TabbedMainMenuTransportist());
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private async void LogIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                //CrossLocalNotifications.Current.Show("Title", "Body");
                if(string.IsNullOrEmpty(UserCode.Text) || string.IsNullOrEmpty(Password.Text))
                {
                    AcrToast.Error("Usuario o contraseña incorrectos", 2);
                    return;
                }

                if (await new DlTable().ReadByCode("CONFIG", "SERVERADDRESS") == null)
                {
                    if (await DisplayAlert("No existe servidor", "Configure la coneccion al servidor antes de continuar", "Ok", "Cancelar"))
                    {
                        await Navigation.PushAsync(new ServerSettings());
                        return;
                    }
                }

                using (var dlg = UserDialogs.Instance.Loading("Verificando usuario..."))
                {
                    var restService = new RestService(StaticHelper.ServerAddress);
                    var result = await restService.GetResponse<Response>(
                                $"mob/authenticateUser?userCode={UserCode.Text}&userPassword={Password.Text}");
                    var dlUser = new DlUser();
                    if (result.IsSuccess)
                    {
                        var usr = JsonConvert.DeserializeObject<DeUser>(result.ResponseData);
                        
                        switch (usr.MobileProfileType)
                        {
                            case MobileProfileType.NULO:
                                await DisplayAlert("Usuario no autorizado", "Este usuario no posee autorizacion en esta aplicacion. Solicite autorizacion antes de continuar.", "Ok");
                                break;
                            case MobileProfileType.PREVENTA:
                                AcrToast.Success($"¡Bienvenido {usr.Name}!", 2);
                                
                                foreach (var itm in await dlUser.ReadAll())
                                {
                                    await dlUser.Delete(itm.UserCode);
                                }
                                await dlUser.Save(usr);
                                await Navigation.PushAsync(new TabbedMainMenu());
                                break;
                            case MobileProfileType.TRANSPORTISTA:
                                AcrToast.Success($"¡Bienvenido {usr.Name}!", 2);
                                foreach (var itm in await dlUser.ReadAll())
                                {
                                    await dlUser.Delete(itm.UserCode);
                                }
                                await dlUser.Save(usr);
                                await Navigation.PushAsync(new Transportist.TabbedMainMenuTransportist());
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        AcrToast.Error(result.Message, 2);           
                }
            }
            catch (Exception ex) {
                try
                {
                    if (await new DlUser().ReadByCode(UserCode.Text) != null)
                    {
                        AcrToast.Success($"¡Bienvenido {UserCode.Text}!", 2);
                        await Navigation.PushAsync(new Transportist.TabbedMainMenuTransportist());
                    }
                    else
                    {
                        AcrToast.Warning("Usuario no registrado", 2);
                        btnServer.IsVisible = true;
                    }
                }
                catch(Exception exe)
                {
                    AcrToast.Error("Error de conección", 2);
                }
                
            }
        }

        private async void btnServer_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ServerSettings());
        }
    }
}