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
    public class DlSellOrderDetail
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

        public DlSellOrderDetail()
        {
            _database = new SQLiteAsyncConnection(_path);
            _database.CreateTableAsync<DeSellOrderDetail>().Wait();
        }

        public async Task<DeSellOrderDetail> ReadByCode(int sellOrderId, double lineNumber)
        {
            var l = await _database.QueryAsync<DeSellOrderDetail>("Select * from srSellOrderDetail");
            return l.FirstOrDefault(x => x.SellOrderId == sellOrderId && x.LineNumber == lineNumber);
        }

        public async Task Save(DeSellOrderDetail value)
        {
            var list = await _database.QueryAsync<DeSellOrderDetail>("Select * from srSellOrderDetail");
            var obj = list.FirstOrDefault(x => x.SellOrderId == value.SellOrderId && x.LineNumber == value.LineNumber);
            if (obj != null)
            {
                await _database.DeleteAsync(obj);
                await _database.InsertAsync(value);
            }
            else
                await _database.InsertAsync(value);
        }

        public async Task<ObservableCollection<DeSellOrderDetail>> ReadAll()
        {
            var list = await _database.QueryAsync<DeSellOrderDetail>("Select * from srSellOrderDetail");
            var query = from cd in list select cd;
            var oList = new ObservableCollection<DeSellOrderDetail>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task<int> GetNextLineNumber(int sellOrderId)
        {
            var list = await _database.QueryAsync<DeSellOrderDetail>("Select * from srSellOrderDetail where SellOrderId = " + sellOrderId);
            return list.Count + 1;
        }

        public async Task Delete(int sellOrderId, double lineNumber)
        {
            var SellOrder = ReadByCode(sellOrderId, lineNumber);
            if (SellOrder != null)
            {
                await _database.DeleteAsync(SellOrder);
            }
        }

        public async Task DeleteByOrder(int sellOrderId)
        {
            var list = await ReadAll();
            foreach (var obj in list.Where(x => x.SellOrderId == sellOrderId))
            {
                await _database.DeleteAsync(obj);
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
