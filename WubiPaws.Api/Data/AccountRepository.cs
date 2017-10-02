using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WubiPaws.Api.Models;

namespace WubiPaws.Api.Data
{
    public class AccountRepository
    {
        readonly IDataAccessRepository<AccountEntity> _dataAccess;
        public AccountRepository(IDataAccessRepository<AccountEntity> repo)
        {
            _dataAccess = repo;
            _dataAccess.CollectionId = "accounts";
            _dataAccess.DatabaseId = "accountdb";

        }

        public async Task<Tuple<Guid,bool>> IsAuthorized(string userName, string userPassword)
        {
           
            var userTokens = userName.Split("@".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Tuple<Guid, bool> dataObj = new Tuple<Guid, bool>(Guid.Empty,false);
            List<dynamic> coll = await _dataAccess.QueryAsync(@"Select * from 
                                                                accounts a 
                                                                where a.userName = @userName   
                                                                AND a.userEmail = @userEmail
                                                                ",
                             DocumentDbExtensions.ToSqlParams(new 
                             { 
                               userName = userTokens[0], 
                               userEmail = string.Format("{0}@{1}",userTokens[0],userTokens[1])
                             }));

            var account = coll.FirstOrDefault();

            if (account != null)
            {
                var validPass = PasswordHash.ValidatePassword(userPassword, account.userPassword.ToString());
                dataObj = new Tuple<Guid, bool>(account.id, validPass);
            }
                
            return dataObj;

        }

        public async Task<AccountEntity> RegisterNewAccount(AccountEntity account)
        {
            var hashedPass = PasswordHash.CreateHash(account.GetPropertyValue<string>("userPassword"));
            account.SetPropertyValue("userPassword", hashedPass);
            account.Id = Guid.NewGuid().ToString();
            return await Task.FromResult<AccountEntity>(account);
            //return await _dataAccess.Save(account, true);
        }

        public async Task UpdateAccount(AccountEntity account)
        {
            await _dataAccess.Save(account, false);
        }

        public async Task<List<AccountEntity>> QueryAllAccounts()
        {
            // List<dynamic> dataList = await _dataAccess.QueryAsync("Select * from accounts");
            var dataList = await _dataAccess.QueryWhereAsync();

            return dataList.ToList();
        }

        public async Task<AccountEntity> Find(string key)
        {
            var single = await _dataAccess.LookupAsync(key);
            return single;
        }
    }
}
