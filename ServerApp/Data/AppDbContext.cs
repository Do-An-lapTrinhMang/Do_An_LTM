using ClientApp.Models;
using ServerApp.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("MyDB")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Đăng ký các configuration
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new HistoryConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
