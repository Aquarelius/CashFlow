using System;
using System.Collections.Generic;
using System.Web;
using CashFlow.Domain.Models;
using CashFlow.Storage;
using Microsoft.AspNet.Identity;

namespace CashFlow.BusinessLogic.Helpers
{
    public class UsersHelper
    {
        private readonly IUsersStorage _datastorage;

        public UsersHelper(IUsersStorage datastorage)
        {
            _datastorage = datastorage;
        }

        public Guid GetCurrentUserId()
        {
            return Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
        }

        public CfUser GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var user = _datastorage.GetUser(userId);
            return user;
        }

        public List<CfUser> GetUsersForLogin()
        {
            return _datastorage.ListUsers();
        }
    }
}
