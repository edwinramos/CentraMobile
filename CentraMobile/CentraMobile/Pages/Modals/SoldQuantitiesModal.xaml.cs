using CentraMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CentraMobile.Pages.Modals
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SoldQuantitiesModal : ContentPage
	{
		public SoldQuantitiesModal (List<SellOrderDetailModel> list)
		{
			InitializeComponent ();
            var finalList = new List<SellOrderDetailModel>();
            var groups = list.GroupBy(x => x.ItemDescription).Select(x=> new { Items = x.ToList() }).ToList();
            foreach (var group in groups)
            {
                var obj = new SellOrderDetailModel();
                foreach (var item in group.Items)
                {
                    obj.Quantity += item.Quantity;
                    obj.ItemDescription = item.ItemDescription;
                }
                finalList.Add(obj);
            }
            lvOrders.ItemsSource = finalList;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}