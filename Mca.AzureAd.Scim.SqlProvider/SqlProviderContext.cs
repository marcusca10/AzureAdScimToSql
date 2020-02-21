using Microsoft.EntityFrameworkCore;
using Microsoft.SCIM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mca.AzureAd.Scim.SqlProvider
{
    public class SqlProviderContext : DbContext
    {
        //public DbSet<Core2Group> Groups { get; set; }
        public DbSet<Core2EnterpriseUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=App_Data\\scimresources.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            // 1:Many
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new EmailConfiguration());
            modelBuilder.ApplyConfiguration(new ImConfiguration());
            modelBuilder.ApplyConfiguration(new PhoneConfiguration());
        }
    }
}
