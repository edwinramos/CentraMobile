using Acr.UserDialogs;
using CentraMobile.DataEntities;
using CentraMobile.DataLayer;
using CentraMobile.Dialogs;
using CentraMobile.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentraMobile.Utils
{
    public class StaticHelper
    {
        public static string ServerTimeOut { get; set; }
        public static string ServerAddress { get; set; }
        public static string ServerPassword { get; set; }

        public static async Task DownloadData()
        {
            var restService = new RestService(StaticHelper.ServerAddress);
            var dlCustomer = new DlCustomer();
            var dlPriceList = new DlPriceList();
            var dlPrice = new DlPrice();
            var dlItem = new DlItem();

            try
            {
                using (var dlg = UserDialogs.Instance.Progress("Preparando..."))
                {
                    var result = await restService.GetResponse<Response>(
                            $"mob/customers?");

                    int acc = 0;
                    double progress = 0;
                    if (result.IsSuccess)
                    {
                        dlg.Title = "Actualizando Clientes...";

                        await dlCustomer.DeleteAll();

                        var list = JsonConvert.DeserializeObject<List<DeCustomer>>(result.ResponseData);

                        progress = list.Count() / 100;

                        foreach (var obj in list)
                        {
                            await dlCustomer.Save(obj);
                            acc += (int)progress;
                            dlg.PercentComplete = acc;
                        }
                    }

                    result = await restService.GetResponse<Response>(
                            $"mob/pricelists?");

                    acc = 0;
                    progress = 0;
                    if (result.IsSuccess)
                    {
                        dlg.Title = "Actualizando Listas de Precios...";
                        await dlPriceList.DeleteAll();
                        
                        var list = JsonConvert.DeserializeObject<List<DePriceList>>(result.ResponseData);

                        progress = list.Count() / 100;

                        foreach (var obj in list)
                        {
                            await dlPriceList.Save(obj);
                            acc += (int)progress;
                            dlg.PercentComplete = acc;
                        }
                    }

                    result = await restService.GetResponse<Response>(
                            $"mob/prices?");

                    acc = 0;
                    progress = 0;
                    if (result.IsSuccess)
                    {
                        dlg.Title = "Actualizando Precios...";
                        await dlPrice.DeleteAll();
                        
                        var list = JsonConvert.DeserializeObject<List<DePrice>>(result.ResponseData);

                        progress = list.Count() / 100;

                        foreach (var obj in list)
                        {
                            await dlPrice.Save(obj);
                            acc += (int)progress;
                            dlg.PercentComplete = acc;
                        }
                    }

                    result = await restService.GetResponse<Response>(
                            $"mob/items?");

                    acc = 0;
                    progress = 0;
                    if (result.IsSuccess)
                    {
                        dlg.Title = "Actualizando Articulos...";
                        await dlItem.DeleteAll();

                        var list = JsonConvert.DeserializeObject<List<DeItem>>(result.ResponseData);

                        progress = list.Count() / 100;

                        foreach (var obj in list)
                        {
                            await dlItem.Save(obj);
                            acc += (int)progress;
                            dlg.PercentComplete = acc;
                        }
                    }
                }
                AcrToast.Success("¡Descarga completada!", 2);
            }
            catch (Exception ex)
            {
                AcrToast.Error("Error de conección", 2);
            }
        }
    }
}
