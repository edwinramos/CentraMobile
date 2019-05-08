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
    public class DlTable
    {
        private static readonly object Locker = new object();
        private readonly SQLiteAsyncConnection _database;
        private readonly string _path =
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.LocalApplicationData), "myLocalDatabase.db3");
        public enum TableStatus
        {
            Valid = 0,
            Invalid = 1
        }

        public DlTable()
        {
            _database = new SQLiteAsyncConnection(_path);
            _database.CreateTableAsync<DeTable>().Wait();
        }

        public async Task<DeTable> ReadByCode(string keyFixed, string keyVariable)
        {
            var l = await _database.QueryAsync<DeTable>("Select * from srTable");
            return l.FirstOrDefault(x => x.KeyFixed == keyFixed && x.KeyVariable == keyVariable);
        }

        public async Task Save(DeTable value)
        {
            var list = await _database.QueryAsync<DeTable>("Select * from srTable");
            var obj = list.FirstOrDefault(x => x.KeyFixed == value.KeyFixed && x.KeyVariable == value.KeyVariable);
            if (obj != null)
                await _database.DeleteAsync(obj);

            await _database.InsertAsync(value);
        }

        public async Task<ObservableCollection<DeTable>> Read(DeTable value)
        {
                var list = await _database.QueryAsync<DeTable>("Select * from srTable");
                var query = from cd in list select cd;
                if (!string.IsNullOrEmpty(value.KeyFixed))
                {
                    query = query.Where(q => q.KeyFixed == value.KeyFixed);
                }

                var oList = new ObservableCollection<DeTable>();
                foreach (var obj in query)
                {
                    oList.Add(obj);
                }

                return oList;
        }

        public async Task<ObservableCollection<DeTable>> ReadAll()
        {
                var list = await _database.QueryAsync<DeTable>("Select * from srTable");
                var query = from cd in list select cd;
                var oList = new ObservableCollection<DeTable>();
                foreach (var obj in query)
                {
                    oList.Add(obj);
                }

                return oList;
        }

        public async void Delete(string keyFixed, string keyVariable)
        {
                var Table = ReadByCode(keyFixed, keyVariable);
                if (Table != null)
                {
                    await _database.DeleteAsync(Table);
                }
        }
    }
}
