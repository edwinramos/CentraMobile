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
    public class DlPrice
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

        public DlPrice()
        {
            _database = new SQLiteAsyncConnection(_path);
            _database.CreateTableAsync<DePrice>().Wait();
        }

        public async Task<DePrice> ReadByCode(string itemCode, string priceListCode)
        {
            var l = await _database.QueryAsync<DePrice>("Select * from srPrice");
            return l.FirstOrDefault(x => x.PriceListCode == priceListCode && x.ItemCode == itemCode);
        }

        public async Task Save(DePrice value)
        {
            var list = await _database.QueryAsync<DePrice>("Select * from srPrice");
            var obj = list.FirstOrDefault(x => x.ItemCode == value.ItemCode && x.PriceListCode == value.PriceListCode);
            if (obj != null)
            {
                await _database.DeleteAsync(obj);
                await _database.InsertAsync(value);
            }
            else
                await _database.InsertAsync(value);
        }

        public async Task<ObservableCollection<DePrice>> Read(DePrice value)
        {
            var list = await _database.QueryAsync<DePrice>("Select * from srPrice");
            var query = from cd in list select cd;
            if (!string.IsNullOrEmpty(value.ItemCode))
            {
                query = query.Where(x => x.ItemCode == value.ItemCode);
            }
            if (!string.IsNullOrEmpty(value.PriceListCode))
            {
                query = query.Where(x => x.PriceListCode == value.PriceListCode);
            }

            var oList = new ObservableCollection<DePrice>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task<ObservableCollection<DePrice>> ReadAll()
        {
            var list = await _database.QueryAsync<DePrice>("Select * from srPrice");
            var query = from cd in list select cd;
            var oList = new ObservableCollection<DePrice>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task Delete(string itemCode, string priceListCode)
        {
            var price = ReadByCode(itemCode, priceListCode);
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
