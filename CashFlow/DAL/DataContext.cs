using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CashFlow.DAL
{
    public class DataContext:DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
            
        }

        public DbSet<Models.Transaction> Transactions { get; set; } 
    }
}