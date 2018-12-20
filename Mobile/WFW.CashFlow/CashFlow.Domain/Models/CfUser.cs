using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashFlow.Domain.Models
{
    public class CfUser : IdentityUser
    {

        public string NickName { get; set; }
        public string Avatar { get; set; }
        public bool ShowOnLogon { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CfUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
