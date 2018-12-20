using System.Data.Entity;
using CashFlow.Domain.Models;
using CashFlow.Storage.Properties;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashFlow.Storage.SQLStorage
{
    public class StorageContext : IdentityDbContext<CfUser>
    {
        public StorageContext() : base(Settings.Default.DBConnectionString)
        {
    
        }

        public static StorageContext Create()
        {
           return new StorageContext();
        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<SettingItem> SettingItems { get; set; }
        public DbSet<TokenModel> Tokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasRequired<Currency>(s => s.Currency)
                .WithMany(s => s.Transactions);
            base.OnModelCreating(modelBuilder);
        }
    }

//    public class UsersContext : IdentityDbContext<CfUser>
//    {
//        public UsersContext() : base(Settings.Default.DBConnectionString)
//        {

//        }

//        public static UsersContext Create()
//        {
//            return new UsersContext();
//        }
//    }
}
