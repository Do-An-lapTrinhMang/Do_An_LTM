using ClientApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Configurations
{
    public class HistoryConfiguration : EntityTypeConfiguration<History>
    {
        public HistoryConfiguration()
        {
            HasKey(h => h.HistoryID);

            // FK Sender
            HasRequired(h => h.Sender)
                .WithMany(u => u.SentHistories)
                .HasForeignKey(h => h.SenderID)
                .WillCascadeOnDelete(false);

            // FK Receiver
            HasRequired(h => h.Receiver)
                .WithMany(u => u.ReceivedHistories)
                .HasForeignKey(h => h.ReceiverID)
                .WillCascadeOnDelete(false);

            Property(h => h.FileName)
                .IsRequired()
                .HasMaxLength(255);

            Property(h => h.FileSize)
                .IsRequired();

            Property(h => h.Timestamp)
                .IsRequired();

            Property(h => h.Status)
                .HasMaxLength(50);
        }
    }
}
