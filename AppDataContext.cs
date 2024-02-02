using Microsoft.EntityFrameworkCore;
using SampleApp.Models;

namespace SampleApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<ContactInformation> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactInformation>().ToTable("ContactInformations");
        }

        public void EnsureDatabaseCreated()
        {
            Database.EnsureCreated();
            Database.Migrate();
        }
    }
}
