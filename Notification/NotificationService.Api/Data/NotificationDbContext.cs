using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Model;
using System;

namespace NotificationService.Api.Data
{
    public sealed class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
    {
        public DbSet<NotificationHistory> NotificationHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }

}
