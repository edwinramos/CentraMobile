using CentraMobile.DataEntities;
using CentraMobile.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CentraMobile.DataLayer
{
    public class DlItem
    {
        private static readonly object Locker = new object();
        private readonly SQLiteAsyncConnection _database;
        private readonly string _path =
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.LocalApplicationData), "myLocalDatabase.db3");
        public enum PriceListStatus
        {
            Valid = 0,
            Invalid = 1
        }

        public DlItem()
        {
            _database = new SQLiteAsyncConnection(_path);
            _database.CreateTableAsync<DeItem>().Wait();
        }

        public async Task<DeItem> ReadByCode(string itemCode)
        {
            var l = await _database.QueryAsync<DeItem>("Select * from srItem");
            return l.FirstOrDefault(x => x.ItemCode == itemCode);
        }

        public async Task Save(DeItem value)
        {
            var list = await _database.QueryAsync<DeItem>("Select * from srItem");
            if (list.Any(x => x.ItemCode == value.ItemCode))
                await _database.UpdateAsync(value);
            else
                await _database.InsertAsync(value);
        }

        public async Task<ObservableCollection<DeItem>> Read(DeItem value)
        {
            var list = await _database.QueryAsync<DeItem>("Select * from srItem");
            var query = from cd in list select cd;
            if (!string.IsNullOrEmpty(value.ItemCode))
            {
                query = query.Where(x => x.ItemCode == value.ItemCode);
            }
            if (!string.IsNullOrEmpty(value.Barcode))
            {
                query = query.Where(x => x.Barcode == value.Barcode);
            }

            var oList = new ObservableCollection<DeItem>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task<ObservableCollection<DeItem>> ReadAll()
        {
            var list = await _database.QueryAsync<DeItem>("Select * from srItem");
            var query = from cd in list select cd;
            var oList = new ObservableCollection<DeItem>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task Delete(string itemCode)
        {
            var price = ReadByCode(itemCode);
            if (price != null)
            {
                await _database.DeleteAsync(price);
            }
        }

        public async Task DeleteAll()
        {
            foreach (var obj in await ReadAll())
            {
                await _database.DeleteAsync(obj);
            }
        }
    }
}
