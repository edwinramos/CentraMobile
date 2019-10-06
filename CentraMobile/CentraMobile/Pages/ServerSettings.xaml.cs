using Acr.UserDialogs;
using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Dialogs;
using CentraMobile.Services;
using CentraMobile.Utils;
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
    public partial class ServerSettings : ContentPage
    {
        private string _keyFixed = "CONFIG";
        private DlTable _dlTable { get; set; }
        public ServerSettings()
        {
            InitializeComponent();
            _dlTable = new DlTable();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var obj = await _dlTable.ReadByCode(_keyFixed, "TIMEOUT");
            txtTimeOut.Text = obj != null ? obj.Value : "3000";

            obj = await _dlTable.ReadByCode(_keyFixed, "SERVERADDRESS");
            txtServerAddress.Text = obj != null ? obj.Value : "";

            obj = await _dlTable.ReadByCode(_keyFixed, "SERVERPASSWORD");
            txtPassword.Text = obj != null ? obj.Value : "";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = UserDialogs.Instance.Loading("Conectando con servidor..."))
                {
                    var restService = new RestService(txtServerAddress.Text);
                    var result = await restService.GetResponse<Response>(
                                        $"mob/test?password={txtPassword.Text}");

                    if (result.IsSuccess)
                    {
                        AcrToast.Success($"¡Conección Exitosa!", 2);
                        var obj = new DeTable { KeyFixed = _keyFixed, KeyVariable = "TIMEOUT", Value = txtTimeOut.Text };

                        await _dlTable.Save(new DeTable { KeyFixed = _keyFixed, KeyVariable = "TIMEOUT", Value = txtTimeOut.Text });
                        await _dlTable.Save(new DeTable { KeyFixed = _keyFixed, KeyVariable = "SERVERADDRESS", Value = txtServerAddress.Text });
                        await _dlTable.Save(new DeTable { KeyFixed = _keyFixed, KeyVariable = "SERVERPASSWORD", Value = txtPassword.Text });

                        StaticHelper.ServerTimeOut = txtTimeOut.Text;
                        StaticHelper.ServerAddress = txtServerAddress.Text;
                        StaticHelper.ServerPassword = txtPassword.Text;

                        await Navigation.PopAsync();
                    }
                    else
                        AcrToast.Error("Contraseña incorrecta", 2);
                }
            }
            catch (Exception ex)
            {
                AcrToast.Error("No es posible conectar con el servidor", 2);
            }
        }
    }
}