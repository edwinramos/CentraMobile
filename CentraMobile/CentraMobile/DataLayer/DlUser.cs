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
    public class DlUser
    {
        private static readonly object Locker = new object();
        private readonly SQLiteAsyncConnection _database;
        private readonly string _path =
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.LocalApplicationData), "myLocalDatabase.db3");
        public enum UserStatus
        {
            Valid = 0,
            Invalid = 1
        }

        public DlUser()
        {
            _database = new SQLiteAsyncConnection(_path);
            _database.CreateTableAsync<DeUser>().Wait();
        }

        public async Task<DeUser> ReadByCode(string userCode)
        {
            var l = await _database.QueryAsync<DeUser>("Select * from srUsers");
            return l.FirstOrDefault(x => x.UserCode == userCode);
        }

        public async Task Save(DeUser value)
        {
            var list = await _database.QueryAsync<DeUser>("Select * from srUsers");
            if (list.All(x => x.UserCode != value.UserCode))
                await _database.InsertAsync(value);
            else
                await _database.UpdateAsync(value);
        }

        public async Task<ObservableCollection<DeUser>> Read(DeUser value)
        {
                var list = await _database.QueryAsync<DeUser>("Select * from srUsers");
                var query = from cd in list select cd;
                if (!string.IsNullOrEmpty(value.UserCode))
                {
                    query = query.Where(q => q.UserCode == value.UserCode);
                }

                if (!string.IsNullOrEmpty(value.Name))
                {
                    query = query.Where(q => q.Name == value.Name);
                }

                if (!string.IsNullOrEmpty(value.LastName))
                {
                    query = query.Where(q => q.LastName == value.LastName);
                }

                if (!string.IsNullOrEmpty(value.Password))
                {
                    query = query.Where(q => q.Password == value.Password);
                }

                var oList = new ObservableCollection<DeUser>();
                foreach (var obj in query)
                {
                    oList.Add(obj);
                }

                return oList;
        }

        public async Task<ObservableCollection<DeUser>> ReadAll()
        {
                var list = await _database.QueryAsync<DeUser>("Select * from srUsers");
                var query = from cd in list select cd;
                var oList = new ObservableCollection<DeUser>();
                foreach (var obj in query)
                {
                    oList.Add(obj);
                }

                return oList;
        }

        public async Task Delete(string userCode)
        {
                var user = await ReadByCode(userCode);
                if (user != null)
                {
                    await _database.DeleteAsync(user);
                }
        }

        public UserStatus CheckUser(string userCode, string userPassword)
        {
            var list = Read(new DeUser { UserCode = userCode, Password = userPassword }).Result;
            var user = list.FirstOrDefault();
            if (user == null)
            {
                return UserStatus.Invalid;
            }

            return EncryptionService.Decrypt(user.UserCode, user.Password) != userPassword ? UserStatus.Invalid : UserStatus.Valid;
        }
    }
}
