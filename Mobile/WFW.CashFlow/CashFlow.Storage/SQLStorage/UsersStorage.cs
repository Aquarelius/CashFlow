using System;
using System.Collections.Generic;
using System.Linq;
using CashFlow.Domain.Models;

namespace CashFlow.Storage.SQLStorage
{
    public class UsersStorage:IUsersStorage
    {
        private readonly StorageContext _context;

        public UsersStorage(StorageContext context)
        {
            _context = context;
        }

        public CfUser GetUser(Guid id)
        {
            return _context.Users.FirstOrDefault(z => z.Id.ToLower() == id.ToString().ToLower());
        }

        public List<CfUser> ListUsers(bool onlyVisible = true)
        {
            return _context.Users.Where(z => z.ShowOnLogon || !onlyVisible).ToList();
        }
    }
}
