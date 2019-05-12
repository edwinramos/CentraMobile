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
    public class DlSellOrder
    {
        private static readonly object Locker = new object();
        private readonly SQLiteAsyncConnection _database;
        private readonly string _path =
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.LocalApplicationData), "myLocalDatabase.db3");
        public enum SellOrderStatus
        {
            Valid = 0,
            Invalid = 1
        }

        public DlSellOrder()
        {
            _database = new SQLiteAsyncConnection(_path);
            _database.CreateTableAsync<DeSellOrder>().Wait();
        }

        public async Task<DeSellOrder> ReadByCode(int sellOrderId)
        {
            var l = await _database.QueryAsync<DeSellOrder>("Select * from srSellOrder");
            return l.FirstOrDefault(x => x.SellOrderId == sellOrderId);
        }

        public async Task Save(DeSellOrder value)
        {
            var list = await _database.QueryAsync<DeSellOrder>("Select * from srSellOrder");
            var obj = list.FirstOrDefault(x => x.SellOrderId == value.SellOrderId);
            if (obj != null)
            {
                await _database.DeleteAsync(obj);
                await _database.InsertAsync(value);
            }
            else
                await _database.InsertAsync(value);
        }

        public async Task<ObservableCollection<DeSellOrder>> ReadAll()
        {
            var list = await _database.QueryAsync<DeSellOrder>("Select * from srSellOrder");
            var query = from cd in list select cd;
            var oList = new ObservableCollection<DeSellOrder>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task Delete(int sellOrderId)
        {
            var SellOrder = await ReadByCode(sellOrderId);
            if (SellOrder != null)
            {
                await _database.DeleteAsync(SellOrder);
            }
        }

        public async Task<int> GetNextOrderId()
        {
            var list = await ReadAll();
            return list.Count + 1;
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
