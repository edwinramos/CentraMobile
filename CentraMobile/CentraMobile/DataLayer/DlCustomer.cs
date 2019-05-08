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
    public class DlCustomer
    {
        private static readonly object Locker = new object();
        private readonly SQLiteAsyncConnection _database;
        private readonly string _path =
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.LocalApplicationData), "myLocalDatabase.db3");
        public enum CustomerStatus
        {
            Valid = 0,
            Invalid = 1
        }

        public DlCustomer()
        {
            _database = new SQLiteAsyncConnection(_path);
            _database.CreateTableAsync<DeCustomer>().Wait();
        }

        public async Task<DeCustomer> ReadByCode(string businessPartnerCode)
        {
            var l = await _database.QueryAsync<DeCustomer>("Select * from srCustomer");
            return l.FirstOrDefault(x => x.BusinessPartnerCode == businessPartnerCode);
        }

        public async Task Save(DeCustomer value)
        {
            var list = await _database.QueryAsync<DeCustomer>("Select * from srCustomer");
            var obj = list.FirstOrDefault(x => x.BusinessPartnerCode == value.BusinessPartnerCode);

            if (obj != null)
            {
                await _database.DeleteAsync(obj);
                await _database.InsertAsync(value);
            }
            else
                await _database.InsertAsync(value);
        }

        public async Task<ObservableCollection<DeCustomer>> Read(DeCustomer value)
        {
            var list = await _database.QueryAsync<DeCustomer>("Select * from srCustomer");
            var query = from cd in list select cd;
            if (!string.IsNullOrEmpty(value.BusinessPartnerCode))
            {
                query = query.Where(x => x.BusinessPartnerCode == value.BusinessPartnerCode);
            }

            var oList = new ObservableCollection<DeCustomer>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task<ObservableCollection<DeCustomer>> ReadAll()
        {
            var list = await _database.QueryAsync<DeCustomer>("Select * from srCustomer");
            var query = from cd in list select cd;
            var oList = new ObservableCollection<DeCustomer>();
            foreach (var obj in query)
            {
                oList.Add(obj);
            }

            return oList;
        }

        public async Task Delete(string businessPartnerCode)
        {
            var Customer = ReadByCode(businessPartnerCode);
            if (Customer != null)
            {
                await _database.DeleteAsync(Customer);
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
