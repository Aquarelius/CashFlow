using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CashFlow.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashFlow.DAL
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext() : base("DefaultConnection")
        {
            
        }

        public DbSet<Models.Transaction> Transactions { get; set; } 
    }
}