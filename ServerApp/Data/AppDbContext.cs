using ClientApp.Models;
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
    }
}
