using ClientApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Configurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            HasKey(u => u.UserID);

            Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            Property(u => u.PasswordHash)
                .IsRequired();

            Property(u => u.FullName)
                .HasMaxLength(100);

            // Navigation properties có thể để mặc định, EF tự nhận
        }
    }
}
